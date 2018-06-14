using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRPAutobotti
{
    public struct CalcoloViaggiStruct
    {
        public int MioOrdineViaggio;
        public int TargheViaggi;
        public int[] IdM;
        public string[,] targa;
        -public int n_viaggio;
        public int[,] sequenza;
        public double giacenza;
        public double giacenzapeso;
        public int giacenzapeso_stored;
        public List<double> viaggio_temp;
        public double[] lun;
        public int[] da_servire;
        public Int16 tempo_temp;
        public int tempo;
        public int viaggio;
        public double[] ordiniD_ord;
        public double[] ordiniBD_ord;
        public double[] ordiniB95_ord;
        public double[] ordiniBS_ord;
        public double[] ordiniALpino_ord;
        public double[] ordiniBluAlpino_ord;
    }

    class CalcoloViaggiClass
    {
        CalcoloViaggiStruct cvStruct;

        public CalcoloViaggiClass()
        {
            cvStruct = new CalcoloViaggiStruct();
        }

        public CalcoloViaggiStruct CalcoloViaggi(int[] IdM, string[,] targa, DataTable od_pv_pv_completa, int n_ordini, double[] pv_ord, int baseCarico, double[] ordinipeso_ord,
            int CARICA, int SCARICA, double SCARICALITRO, double[] max_product_ord, double[] od_dep_pv_ord, double[] od_pv_dep_ord, double[,] od_pv_pv_ord, double MINxKM,
            int TEMPO_MAX, double[,] valore_ord, double[] ordini_ord, double[] ordiniD_ord, double[] ordiniB95_ord, double[] ordiniBD_ord, double[] ordiniBS_ord,
            double[] ordiniAlpino_ord, double[] ordiniBluAlpino_ord, double[] ordini_piumeno_ord, double[] MioOrdine_ord, int GIACENZA_MIN, int KM_MIN, double[] capmax, double[] capton,
            double[] dens_D, double[] dens_B95, double[] dens_BD, double[] dens_BS, int[] MAXDROP, double[] MENOMILLE, SqlConnection conn)
        {
            int nTarghe = IdM.Length;
            int[] da_servire = new int[n_ordini];
            for(int i=0;i<n_ordini;i++)
            {
                da_servire[i] = 1;
            }

            int[] TargheViaggi = new int[capton.Length];
            for (int i = 0; i < capton.Length; i++)
            {
                TargheViaggi[i] = -1;
            }
            int n_viaggio = 0;
            int scartato = 0;
            int c= TEMPO_MAX - 10;
            Int16[] TargheTempo = new Int16[nTarghe];
            Int16[] ContaTarghe = new Int16[nTarghe];
            string[] Targhe = new string[nTarghe];
            int l = 0;
            for (int i=0;i<nTarghe;i++)
            {
                TargheTempo[i] = Convert.ToInt16(c);
                Targhe[i] = targa[i, l];
                //se nTarghe > dim 2 di targa, cambio colonna di accesso
                if (i >= targa.GetLength(1))
                    l++;
            }

            for(int i=0;i<n_ordini;i++)
            {
                //quanto rimane in ogni mezzo dopo avegli assegnato il quantitativo in PV
                int nelWhile = 0;
                int NoMezzo = 0;
                double giacenza = 0;
                double giacenzapeso = 0;
                int fineviaggio = 0;
                //
                List<double> viaggio_temp = new List<double>();
                List<double> Mio_temp = new List<double>();
                Int16 tempo_temp;
                List<double> lun = new List<double>();
                List<int> j_temp = new List<int>();
                int drop = 1;
                Int16 tempoGuidaOld = 0;
                int tt = 0;
                int t = 0;
                if(da_servire[i]==1)
                {
                    n_viaggio++;
                    if (n_viaggio == 14)
                        t = 1;
                    viaggio_temp[drop] = pv_ord[i];
                    Mio_temp[drop] = MioOrdine_ord[i];
                    sequenza[n_viaggio, 1] = Convert.ToDouble(baseCarico);
                    sequenza[n_viaggio, 2] = pv_ord[i];
                    giacenza = ordini_ord[i];
                    giacenzapeso = ordinipeso_ord[i];
                    //----------------------------------------------
                    Int16 temposcarica = Convert.ToInt16(CARICA + SCARICA + (SCARICALITRO * max_product_ord[i]));
                    //lunghezza del viaggio fino a quel momento
                    lun[n_viaggio] = od_dep_pv_ord[i] + od_pv_dep_ord[i];
                    Int16 tempoguida = Convert.ToInt16(MINxKM * lun[n_viaggio]);
                    tempo_temp = Convert.ToInt16(temposcarica + tempoguida);
                    //----------------------------------------------
                    da_servire[i] = 0;
                    int[] maschera = da_servire;
                    fineviaggio = 0;
                    //in j ho il migliore che si accoppia e in M il suo valoreù
                    double[] temp = new double[maschera.Length];
                    int o = 0;
                    for (o=0;o<maschera.Length;o++)
                    {
                        temp[o] = valore_ord[i, o] * maschera[o];
                    }
                    double M = temp.Max();
                    int j = temp.ToList().IndexOf(M);
                    int trovato = 0;

                    if (n_viaggio == nTarghe || n_viaggio == 3 * nTarghe)
                        tt = nTarghe;
                    else if (n_viaggio == 2 * nTarghe)
                        tt = 1;
                    else if (n_viaggio > nTarghe && n_viaggio < 2 * nTarghe)
                    {
                        if (n_viaggio == nTarghe + 1)
                            tt = nTarghe;
                        else
                            tt = nTarghe - (n_viaggio % nTarghe) + 1;
                    }
                    else
                        tt = (n_viaggio % nTarghe);

                    //cerco secondo drop
                    if (n_viaggio <= capton.Length)
                    {
                        nelWhile = 0;
                        while(trovato!=0 && M>0)
                        {
                            nelWhile = 1;
                            Int16 temposcaricasenzaj = Convert.ToInt16(temposcarica);
                            Int16 tempoguidasenzaj = Convert.ToInt16(tempoguida);
                            Int16 temposcaricaj = Convert.ToInt16(SCARICA + (SCARICALITRO * max_product_ord[j]));
                            temposcarica = Convert.ToInt16(temposcarica + temposcaricaj);
                            // controllo che sia meglio andare da i a j piuttosto che da j a i
                            Int16 tempoguida1 = Convert.ToInt16(MINxKM * od_pv_pv_ord[i, j] + MINxKM * od_pv_dep_ord[j] - MINxKM * od_pv_dep_ord[i]);
                            Int16 tempoguida2 = Convert.ToInt16(MINxKM * od_pv_pv_ord[j, i] + MINxKM * od_pv_dep_ord[i] - MINxKM * od_pv_dep_ord[j]);
                            Int16 tempoguidaj;
                            if (tempoguida1 < tempoguida2)
                                tempoguidaj = tempoguida1;
                            else
                                tempoguidaj = tempoguida2;

                            tempoguida = Convert.ToInt16(tempoguida+tempoguida1);
                            tempo_temp = Convert.ToInt16(tempoguida + temposcarica);
                            //----------------------------------------------
                            //capton è la capacità in tonellate del mezzo i-esimo
                            if (da_servire[j]==1 && giacenzapeso+ordinipeso_ord[j]<=capton[n_viaggio] && 
                                giacenza+ordini_ord[j]<=capmax[n_viaggio] && ContaTarghe[tt]<=2 && Convert.ToInt16(TargheTempo[tt]-tempo_temp)>=0)
                            {
                                nelWhile = 2;
                                drop++;
                                viaggio_temp[drop] = pv_ord[j];
                                Mio_temp[drop] = MioOrdine_ord[j];
                                j_temp[drop - 1] = j;
                                da_servire[j] = 0;
                                maschera[j] = 0;
                                trovato = 1;
                                giacenza = giacenza + ordini_ord[j];
                                giacenzapeso = giacenzapeso + ordinipeso_ord[j];
                                TargheTempo[tt] = Convert.ToInt16(TargheTempo[tt] - tempo_temp);
                                TargheViaggi[n_viaggio] = targa[n_viaggio];
                                tempoGuidaOld = Convert.ToInt16(tempo_temp);
                                ContaTarghe[tt]++;
                            }
                            else //non ho trovato un pv che possa andare bene, prima di cambiare il pv provo a vedere il mezzo se può essere cambiato
                            {
                                int trovato1 = 0;
                                int p = 1;
                                int n_viaggio_temp = 1;
                                while(trovato1!=0 && p<=nTarghe)
                                {
                                    if (Convert.ToInt16(TargheTempo[p]-tempo_temp)>=0 && giacenzapeso+ordinipeso_ord[j]<=capton[n_viaggio_temp] &&
                                        giacenza+ordini_ord[j]<=capmax[n_viaggio_temp] && ContaTarghe[p]<=2)
                                    {
                                        trovato1 = 1;
                                        trovato = 1;
                                        nelWhile = 2;
                                        TargheTempo[p] = Convert.ToInt16(TargheTempo[p] - tempo_temp);
                                        TargheViaggi[n_viaggio]=targa(n_viaggio_temp);
                                        ContaTarghe[p]++;
                                        drop++;
                                        viaggio_temp[drop] = pv_ord[j];
                                        Mio_temp[drop] = MioOrdine_ord[j];
                                        j_temp[drop - 1] = j;
                                        da_servire[j] = 0;
                                        maschera[j] = 0;
                                        giacenza = giacenza + ordini_ord[j];
                                        giacenzapeso = giacenzapeso + ordinipeso_ord[j];
                                        tempoGuidaOld = Convert.ToInt16(tempo_temp);
                                        tt = p;
                                    }
                                    n_viaggio_temp++;
                                    p++;
                                }
                                if (trovato1==0)
                                {
                                    temposcarica = Convert.ToInt16(temposcaricasenzaj);
                                    tempoguida = Convert.ToInt16(tempoguidasenzaj);
                                    tempo_temp = Convert.ToInt16(tempoguida + temposcarica);
                                    maschera[j] = 0;
                                    for (o = 0; o < maschera.Length; o++)
                                    {
                                        temp[o] = valore_ord[i, o] * maschera[o];
                                    }
                                    M = temp.Max();
                                    j = temp.ToList().IndexOf(M);
                                }
                            }
                        }
                        if(nelWhile==0 || nelWhile==1) //non ho trovato altri pv dispoinibili x prenderlo
                        {
                            int NoMezzi = 1;
                            if (giacenzapeso <= capton[n_viaggio] &&  giacenza <= capmax[n_viaggio] && 
                                Convert.ToInt16(TargheTempo[tt] - tempo_temp) >= 0 && ContaTarghe[tt] <= 2)
                            {
                                trovato = 2;
                                TargheTempo[tt] = Convert.ToInt16(TargheTempo[tt] - tempo_temp);
                                ContaTarghe[tt]++;
                                TargheViaggi[n_viaggio] = targa[n_viaggio];
                                tempoGuidaOld = Convert.ToInt16(tempo_temp);
                                capton[n_viaggio] = giacenzapeso;
                                capmax[n_viaggio] = giacenza;
                            }
                            else
                            {
                                int trovato1 = 0;
                                int p = 1;
                                int n_viaggio_temp = 1;
                                while (trovato1 != 0 && p <= nTarghe)
                                {
                                    if (Convert.ToInt16(TargheTempo[p] - tempo_temp) >= 0 && giacenzapeso <= capton[n_viaggio_temp] &&
                                        giacenza <= capmax[n_viaggio_temp] && ContaTarghe[p] <= 2)
                                    {
                                        trovato1 = 1;
                                        trovato = 2;
                                        TargheTempo[p] = Convert.ToInt16(TargheTempo[p] - tempo_temp);
                                        TargheViaggi[n_viaggio] = targa(n_viaggio_temp);
                                        tempoGuidaOld = Convert.ToInt16(tempo_temp);
                                        tt = p;
                                    }
                                    n_viaggio_temp++;
                                    p++;
                                }
                            }
                            //180
                        }
                    }
                }

            }
        }
    }
}
