using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace IRPAutobotti
{
    public struct CalcoloViaggiStruct
    {
        public List<List<double>> MioOrdineViaggio;
        public double[] TargheViaggi;
        public int[] IdM;
        public double[] targa;
        public double n_viaggio;
        public double scartato;
        public List<List<double>> sequenza;
        public double giacenza;
        public double giacenzapeso;
        public List<double> giacenzapeso_stored;
        public List<double> viaggio_temp;
        public List<double> lun;
        public double[] da_servire;
        public Int16 tempo_temp;
        public List<short> tempo;
        public List<List<double>> viaggio;
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

        public CalcoloViaggiStruct CalcoloViaggi(int[] IdM, double[] targa, DataTable od_pv_pv_completa, int n_ordini, double[] pv_ord, int baseCarico, double[] ordinipeso_ord,
            double CARICA, double SCARICA, double SCARICALITRO, double[] max_product_ord, double[] od_dep_pv_ord, double[] od_pv_dep_ord, double[,] od_pv_pv_ord, double MINxKM,
            double TEMPO_MAX, double[,] valore_ord, double[] ordini_ord, double[] ordiniD_ord, double[] ordiniB95_ord, double[] ordiniBD_ord, double[] ordiniBS_ord,
            double[] ordiniAlpino_ord, double[] ordiniBluAlpino_ord, double[] ordini_piumeno_ord, double[] MioOrdine_ord, double GIACENZA_MIN, double KM_MIN, double[] capmax, double[] capton,
            double dens_D, double dens_B95, double dens_BD, double dens_BS, double MAXDROP, double MENOMILLE, SqlConnection conn)
        {
            int nTarghe = IdM.Length;
            double[] da_servire = new double[n_ordini];
            for(int i=0;i<n_ordini;i++)
            {
                da_servire[i] = 1;
            }

            double[] TargheViaggi = new double[capton.Length];
            for (int i = 0; i < capton.Length; i++)
            {
                TargheViaggi[i] = -1;
            }
            int n_viaggio = -1; // Inizializzato a -1 perchè la variabile viene incrementata all'inizio del loop
            double scartato = 0;
            double c= TEMPO_MAX - 10;
            Int16[] TargheTempo = new Int16[nTarghe];
            Int16[] ContaTarghe = new Int16[nTarghe];
            double[] Targhe = new double[nTarghe];
            for (int i=0;i<nTarghe;i++)
            {
                TargheTempo[i] = Convert.ToInt16(c);
                Targhe[i] = targa[i];
            }
            List<double> viaggio_temp = new List<double>();
            List<double> Mio_temp;
            Int16 tempo_temp=0;
            List<double> lun = new List<double>();
            List<int> j_temp;
            List<double> giacenza_stored = new List<double>();
            List<double> giacenzapeso_stored = new List<double>();
            List<string> Quantita = new List<string>();
            List<Int16> tempo = new List<short>();
            List<List<double>> viaggio = new List<List<double>>();
            List<List<double>> MioOrdineViaggio = new List<List<double>>();
            List<List<double>> sequenza = new List<List<double>>();
            double giacenza = 0;
            double giacenzapeso = 0;

            for (int i=0;i<n_ordini;i++)
            {
                //quanto rimane in ogni mezzo dopo avegli assegnato il quantitativo in PV
                int nelWhile = 0;
                int NoMezzo = 0;
                giacenza = 0;
                giacenzapeso = 0;
                int fineviaggio = 0;
                //
                viaggio_temp = new List<double>();
                Mio_temp = new List<double>();
                j_temp = new List<int>();
                tempo_temp = 0;
                int NoMezzi = 0;
                int drop = 0;
                Int16 tempoGuidaOld = 0;
                int tt = 0;
                int t = 0;
                if(da_servire[i]==1)
                {
                    n_viaggio++;
                    if (n_viaggio == 14)
                        t = 1;
                    viaggio_temp.Insert(drop, pv_ord[i]);
                    //viaggio_temp[drop] = pv_ord[i];
                    Mio_temp.Insert(drop, MioOrdine_ord[i]);
                    //Mio_temp[drop] = MioOrdine_ord[i];
                    List<double> tempList = new List<double>();
                    tempList.Insert(0, Convert.ToDouble(baseCarico));
                    tempList.Insert(1, pv_ord[i]);
                    sequenza.Insert(n_viaggio, tempList);

                    //sequenza[n_viaggio][0] = Convert.ToDouble(baseCarico);
                    //sequenza[n_viaggio][1] = pv_ord[i];

                    giacenza = ordini_ord[i];
                    giacenzapeso = ordinipeso_ord[i];
                    //----------------------------------------------
                    Int16 temposcarica = Convert.ToInt16(CARICA + SCARICA + (SCARICALITRO * max_product_ord[i]));
                    //lunghezza del viaggio fino a quel momento
                    lun.Insert(n_viaggio, od_dep_pv_ord[i] + od_pv_dep_ord[i]);

                    Int16 tempoguida = Convert.ToInt16(MINxKM * lun[n_viaggio]);
                    tempo_temp = Convert.ToInt16(temposcarica + tempoguida);
                    //----------------------------------------------
                    da_servire[i] = 0;
                    double[] maschera = da_servire;
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
                        while(trovato==0 && M>0)
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
                                viaggio_temp.Insert(drop, pv_ord[j]);
                                Mio_temp[drop] = MioOrdine_ord[j];
                                j_temp.Insert(drop - 1, j);
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
                                while(trovato1 == 0 && p<=nTarghe)     //PRIMA ERA trovato1 != 0
                                {
                                    if (Convert.ToInt16(TargheTempo[p]-tempo_temp)>=0 && giacenzapeso+ordinipeso_ord[j]<=capton[n_viaggio_temp] &&
                                        giacenza+ordini_ord[j]<=capmax[n_viaggio_temp] && ContaTarghe[p]<=2)
                                    {
                                        trovato1 = 1;
                                        trovato = 1;
                                        nelWhile = 2;
                                        TargheTempo[p] = Convert.ToInt16(TargheTempo[p] - tempo_temp);
                                        TargheViaggi[n_viaggio]=targa[n_viaggio_temp];
                                        ContaTarghe[p]++;
                                        drop++;
                                        viaggio_temp.Insert(drop, pv_ord[j]);
                                        Mio_temp[drop] = MioOrdine_ord[j];
                                        j_temp.Insert(drop - 1, j);
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
                                if (trovato1 == 0)
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
                            NoMezzi = 1;
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
                                while (trovato1 == 0 && p <= nTarghe)
                                {
                                    if (Convert.ToInt16(TargheTempo[p] - tempo_temp) >= 0 && giacenzapeso <= capton[n_viaggio_temp] &&
                                        giacenza <= capmax[n_viaggio_temp] && ContaTarghe[p] <= 2)
                                    {
                                        trovato1 = 1;
                                        trovato = 2;
                                        TargheTempo[p] = Convert.ToInt16(TargheTempo[p] - tempo_temp);
                                        TargheViaggi[n_viaggio] = targa[n_viaggio_temp];
                                        tempoGuidaOld = Convert.ToInt16(tempo_temp);
                                        tt = p;
                                    }
                                    n_viaggio_temp++;
                                    p++;
                                }
                            }
                            if (trovato == 2)
                            {
                                NoMezzo = 1;
                                int iii = 0;
                                int g = 0;
                                for (g = 0; g < viaggio_temp.Count; g++)
                                {
                                    //sommo di nuovo i meno fino a stare sotto o uguale ai 39
                                    if (g == 0)
                                        iii = i;
                                    else
                                        iii = j_temp[g - 1];
                                    //bool isEqual = Enumerable.SequenceEqual(target1, target2);
                                    if (ordiniD_ord[iii] > MENOMILLE && giacenzapeso + dens_D <= capton[n_viaggio] &&
                                        giacenza+1<=capmax[n_viaggio])
                                    {
                                        ordiniD_ord[iii]++;
                                        giacenza++;
                                        giacenzapeso += dens_D;
                                    }
                                    if (ordiniB95_ord[iii] > MENOMILLE && giacenzapeso + dens_B95 <= capton[n_viaggio] &&
                                        giacenza + 1 <= capmax[n_viaggio])
                                    {
                                        ordiniB95_ord[iii]++;
                                        giacenza++;
                                        giacenzapeso += dens_B95;
                                    }
                                    if (ordiniBD_ord[iii] > MENOMILLE && giacenzapeso + dens_BD <= capton[n_viaggio] &&
                                        giacenza + 1 <= capmax[n_viaggio])
                                    {
                                        ordiniBD_ord[iii]++;
                                        giacenza++;
                                        giacenzapeso += dens_BD;
                                    }
                                    if (ordiniBS_ord[iii] > MENOMILLE && giacenzapeso + dens_BS <= capton[n_viaggio] &&
                                        giacenza + 1 <= capmax[n_viaggio])
                                    {
                                        ordiniBS_ord[iii]++;
                                        giacenza++;
                                        giacenzapeso += dens_BS;
                                    }
                                    if (ordiniAlpino_ord[iii] > MENOMILLE && giacenzapeso + dens_D <= capton[n_viaggio] &&
                                        giacenza + 1 <= capmax[n_viaggio])
                                    {
                                        ordiniAlpino_ord[iii]++;
                                        giacenza++;
                                        giacenzapeso += dens_D;
                                    }
                                    if (ordiniBluAlpino_ord[iii] > MENOMILLE && giacenzapeso + dens_BD <= capton[n_viaggio] &&
                                       giacenza + 1 <= capmax[n_viaggio])
                                    {
                                        ordiniBluAlpino_ord[iii]++;
                                        giacenza++;
                                        giacenzapeso += dens_BD;
                                    }
                                }
                                //se sotto 39 provo a mettercene ancora
                                double Temp_D = 0;
                                double Temp_BD = 0;
                                double Temp_BS = 0;
                                double Temp_B = 0;
                                double Temp_BAlpino = 0;
                                double Temp_Alpino = 0;
                                for(g = 0; g < viaggio_temp.Count; g++)
                                {
                                    if (g == 0)
                                        iii = i;
                                    else
                                        iii = j_temp[g - 1];

                                    if (ordiniD_ord[iii] > 0 && ordini_piumeno_ord[iii] > 0 &&
                                        giacenzapeso + dens_D <= capton[n_viaggio] && giacenza + 1 <= capmax[n_viaggio])
                                    {
                                        ordiniD_ord[iii]++;
                                        giacenza++;
                                        giacenzapeso += dens_D;
                                    }
                                    if (ordiniB95_ord[iii] > 0 && ordini_piumeno_ord[iii] > 0 &&
                                        giacenzapeso + dens_B95 <= capton[n_viaggio] && giacenza + 1 <= capmax[n_viaggio])
                                    {
                                        ordiniB95_ord[iii]++;
                                        giacenza++;
                                        giacenzapeso += dens_B95;
                                    }
                                    if (ordiniBD_ord[iii] > 0 && ordini_piumeno_ord[iii] > 0 &&
                                        giacenzapeso + dens_BD <= capton[n_viaggio] && giacenza + 1 <= capmax[n_viaggio])
                                    {
                                        ordiniBD_ord[iii]++;
                                        giacenza++;
                                        giacenzapeso += dens_BD;
                                    }
                                    if (ordiniBS_ord[iii] > 0 && ordini_piumeno_ord[iii] > 0 &&
                                        giacenzapeso + dens_BS <= capton[n_viaggio] && giacenza + 1 <= capmax[n_viaggio])
                                    {
                                        ordiniBS_ord[iii]++;
                                        giacenza++;
                                        giacenzapeso += dens_BS;
                                    }
                                    if (ordiniAlpino_ord[iii] > 0 && ordini_piumeno_ord[iii] > 0 &&
                                        giacenzapeso + dens_D <= capton[n_viaggio] && giacenza + 1 <= capmax[n_viaggio])
                                    {
                                        ordiniAlpino_ord[iii]++;
                                        giacenza++;
                                        giacenzapeso += dens_D;
                                    }
                                    if (ordiniBluAlpino_ord[iii] > 0 && ordini_piumeno_ord[iii] > 0 &&
                                        giacenzapeso + dens_BD <= capton[n_viaggio] && giacenza + 1 <= capmax[n_viaggio])
                                    {
                                        ordiniBluAlpino_ord[iii]++;
                                        giacenza++;
                                        giacenzapeso += dens_BD;
                                    }
                                    Temp_D += ordiniD_ord[iii];
                                    Temp_BD += ordiniBD_ord[iii];
                                    Temp_BS += ordiniBS_ord[iii];
                                    Temp_B += ordiniB95_ord[iii];
                                    Temp_Alpino += ordiniAlpino_ord[iii];
                                    Temp_BAlpino += ordiniBluAlpino_ord[iii];
                                }

                                Quantita.Insert(n_viaggio, Temp_D.ToString() + "," + Temp_BD.ToString() + "," + Temp_BS.ToString() + "," +
                                    Temp_B.ToString() + "," + Temp_Alpino.ToString() + "," + Temp_BAlpino.ToString());
                                //Quantita[n_viaggio] = Temp_D.ToString() + "," + Temp_BD.ToString() + "," + Temp_BS.ToString() + "," +
                                    //Temp_B.ToString() + "," + Temp_Alpino.ToString() + "," + Temp_BAlpino.ToString();
                                g = 0;
                                ScompartaturaClass sc = new ScompartaturaClass();

                                while (sc.Scompartatura(IdM[(int)TargheViaggi[n_viaggio]],Quantita[n_viaggio],MENOMILLE,conn)!=1 && g<viaggio_temp.Count)
                                {
                                    t = 0;
                                    if (g == 0)
                                        iii = i;
                                    else
                                        iii = j_temp[g - 1];

                                    if (ordiniD_ord[iii] <= MENOMILLE && ordiniD_ord[iii]>0)
                                    {
                                        ordiniD_ord[iii]--;
                                        giacenza--;
                                        giacenzapeso -= dens_D;
                                        Temp_D--;
                                        t = 1;
                                    }
                                    if (ordiniB95_ord[iii] <= MENOMILLE && t!=1 && ordiniB95_ord[iii] > 0)
                                    {
                                        ordiniB95_ord[iii]--;
                                        giacenza--;
                                        giacenzapeso -= dens_B95;
                                        Temp_B--;
                                        t = 1;
                                    }
                                    if (ordiniBD_ord[iii] <= MENOMILLE && t != 1 && ordiniBD_ord[iii] > 0)
                                    {
                                        ordiniBD_ord[iii]--;
                                        giacenza--;
                                        giacenzapeso -= dens_BD;
                                        Temp_BD--;
                                        t = 1;
                                    }
                                    if (ordiniBS_ord[iii] <= MENOMILLE && t != 1 && ordiniBS_ord[iii] > 0)
                                    {
                                        ordiniBS_ord[iii]--;
                                        giacenza--;
                                        giacenzapeso -= dens_BS;
                                        Temp_BS--;
                                        t = 1;
                                    }
                                    if (ordiniAlpino_ord[iii] <= MENOMILLE && t != 1 && ordiniAlpino_ord[iii] > 0)
                                    {
                                        ordiniAlpino_ord[iii]--;
                                        giacenza++;
                                        giacenzapeso -= dens_D;
                                        Temp_Alpino--;
                                        t = 1;
                                    }
                                    if (ordiniBluAlpino_ord[iii] <= MENOMILLE && t != 1 && ordiniBluAlpino_ord[iii] > 0)
                                    {
                                        ordiniBluAlpino_ord[iii]--;
                                        giacenza++;
                                        giacenzapeso -= dens_BD;
                                        Temp_BAlpino--;
                                        t = 1;
                                    }
                                    g++;
                                    Quantita[n_viaggio] = Temp_D.ToString() + "," + Temp_BD.ToString() + "," + Temp_BS.ToString() + "," +
                                    Temp_B.ToString() + "," + Temp_Alpino.ToString() + "," + Temp_BAlpino.ToString();
                                }
                                double TempPrecD = Temp_D;
                                double TempPrecB = Temp_B;
                                double TempPrecBD = Temp_BD;
                                double TempPrecBS = Temp_BS;
                                double TempPrecAlpino = Temp_Alpino;
                                double TempPrecBAlpino = Temp_BAlpino;
                                Quantita[n_viaggio] = TempPrecD.ToString() + "," + TempPrecBD.ToString() + "," + TempPrecBS.ToString() + "," +
                                    TempPrecB.ToString() + "," + TempPrecAlpino.ToString() + "," + TempPrecBAlpino.ToString();
                                giacenza_stored.Insert(n_viaggio, giacenza);
                                giacenzapeso_stored.Insert(n_viaggio, giacenzapeso);
                                tempo.Insert(n_viaggio, Convert.ToInt16(tempo_temp));

                                for (int q = 0; q < viaggio_temp.Count; q++)
                                {
                                    viaggio.Insert(n_viaggio, viaggio_temp.ToList<double>());
                                    MioOrdineViaggio.Insert(n_viaggio, Mio_temp.ToList<double>());
                                    //MioOrdineViaggio[n_viaggio][q] = Mio_temp[q];
                                }
                                for (int q = 0; q < j_temp.Count; q++)
                                    da_servire[j_temp[q]] = 0;
                                trovato = 2;
                            }
                        }
                        //ABBIAMO UNA COPPIA ANDIAMO ALLA RICERCA DI UNA TERNA O MAGGIORE
                        //altri drop dal terzo in poi
                        if(trovato==1&&nelWhile==2)
                        {
                            trovato = 0;
                            int zz = 3;
                            temp = new double[maschera.Length];
                            for (o = 0; o < maschera.Length; o++)
                            {
                                temp[o] = valore_ord[i, o] * maschera[o];
                            }
                            M = temp.Max();
                            j = temp.ToList().IndexOf(M);
                            while(trovato == 0 && M > 0 && zz < MAXDROP)
                            {
                                for(int z = zz; z < MAXDROP; z++)
                                {
                                    for(int pv = 0; pv < j_temp.Count; pv++)
                                    {
                                        int k = j_temp[pv];

                                        temp = new double[maschera.Length];
                                        for (o = 0; o < maschera.Length; o++)
                                        {
                                            temp[o] = valore_ord[k, o] * maschera[o];
                                        }
                                        double Mtmp = temp.Max();
                                        int jtmp = temp.ToList().IndexOf(M);
                                        if(Mtmp>M)
                                        {
                                            M = Mtmp;
                                            j = jtmp;
                                        }
                                    }
                                    trovato = 0;
                                    while(trovato==0 && M>0)
                                    {
                                        if (da_servire[j] == 1 && giacenzapeso + ordinipeso_ord[j] <= capton[n_viaggio] &&
                                           giacenza + ordini_ord[j] <= capmax[n_viaggio])
                                        {
                                            Int16 temposcaricaj = Convert.ToInt16(SCARICA + (SCARICALITRO * max_product_ord[j]));
                                            temposcarica = Convert.ToInt16(temposcarica + temposcaricaj);
                                            Int16 temposcaricasenzaj = Convert.ToInt16(temposcarica);

                                            Calcola_percorsoNewClass cpn = new Calcola_percorsoNewClass();
                                            Calcola_percorsoNewStruct cpnStruct = cpn.calcola_percorsoNew(baseCarico, od_pv_pv_completa, viaggio_temp, sequenza, lun, n_viaggio);
                                            //prendo il tempoguida precedente???
                                            Int16 tempoguidasenzaj = Convert.ToInt16(tempoguida);
                                            tempoguida = Convert.ToInt16(cpnStruct.lun[n_viaggio] * MINxKM);
                                            //devo rifare il calcolo del viaggio considerando latripla / quaterna / ecc..e nn solo la coppia
                                            tempo_temp = Convert.ToInt16(tempoguida + temposcarica);
                                            maschera[j] = 0;
                                            //----------------------------------------
                                            Int16 temp1 = Convert.ToInt16(TargheTempo[tt] + Convert.ToInt16(tempoGuidaOld) - tempo_temp);
                                            if (temp1 >= 0)
                                            {
                                                drop++;
                                                viaggio_temp.Insert(drop, pv_ord[j]);
                                                Mio_temp[drop] = MioOrdine_ord[j];
                                                j_temp.Insert(z - 1, j);
                                                da_servire[j] = 0;
                                                maschera[j] = 0;
                                                trovato = 1;
                                                giacenza = giacenza + ordini_ord[j];
                                                giacenzapeso = giacenzapeso + ordinipeso_ord[j];
                                                TargheTempo[tt] = temp1;
                                                tempoGuidaOld = Convert.ToInt16(tempo_temp);
                                            }
                                            else
                                            {
                                                maschera[j] = 0;
                                                int k = i;

                                                temposcarica = Convert.ToInt16(temposcaricasenzaj);
                                                tempoguida = Convert.ToInt16(tempoguidasenzaj);
                                                tempo_temp = Convert.ToInt16(tempoguida + temposcarica);

                                                for (o = 0; o < maschera.Length; o++)
                                                {
                                                    temp[o] = valore_ord[k, o] * maschera[o];
                                                }
                                                M = temp.Max();
                                                j = temp.ToList().IndexOf(M);

                                                for (int pv = 0; pv < z - 2; pv++)
                                                {
                                                    k = j_temp[pv];

                                                    temp = new double[maschera.Length];
                                                    for (o = 0; o < maschera.Length; o++)
                                                    {
                                                        temp[o] = valore_ord[k, o] * maschera[o];
                                                    }
                                                    double Mtmp = temp.Max();
                                                    int jtmp = temp.ToList().IndexOf(M);
                                                    if (Mtmp > M)
                                                    {
                                                        M = Mtmp;
                                                        j = jtmp;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            maschera[j] = 0;
                                            int k = i;

                                            temp = new double[maschera.Length];
                                            for (o = 0; o < maschera.Length; o++)
                                            {
                                                temp[o] = valore_ord[k, o] * maschera[o];
                                            }
                                            M = temp.Max();
                                            j = temp.ToList().IndexOf(M);

                                            for (int pv = 0; pv < z - 2; pv++)
                                            {
                                                k = j_temp[pv];

                                                temp = new double[maschera.Length];
                                                for (o = 0; o < maschera.Length; o++)
                                                {
                                                    temp[o] = valore_ord[k, o] * maschera[o];
                                                }
                                                double Mtmp = temp.Max();
                                                int jtmp = temp.ToList().IndexOf(M);
                                                if (Mtmp > M)
                                                {
                                                    M = Mtmp;
                                                    j = jtmp;
                                                }
                                            }
                                        }
                                    }
                                }
                                zz++;
                            }
                            int iii = 0;
                            int g = 0;
                            for (g = 0; g < viaggio_temp.Count; g++)
                            {
                                //sommo di nuovo i meno fino a stare sotto o uguale ai 39
                                if (g == 0)
                                    iii = i;
                                else
                                    iii = j_temp[g - 1];
                                //bool isEqual = Enumerable.SequenceEqual(target1, target2);
                                if (ordiniD_ord[iii] > MENOMILLE && giacenzapeso + dens_D <= capton[n_viaggio] &&
                                    giacenza + 1 <= capmax[n_viaggio])
                                {
                                    ordiniD_ord[iii]++;
                                    giacenza++;
                                    giacenzapeso += dens_D;
                                }
                                if (ordiniB95_ord[iii] > MENOMILLE && giacenzapeso + dens_B95 <= capton[n_viaggio] &&
                                    giacenza + 1 <= capmax[n_viaggio])
                                {
                                    ordiniB95_ord[iii]++;
                                    giacenza++;
                                    giacenzapeso += dens_B95;
                                }
                                if (ordiniBD_ord[iii] > MENOMILLE && giacenzapeso + dens_BD <= capton[n_viaggio] &&
                                    giacenza + 1 <= capmax[n_viaggio])
                                {
                                    ordiniBD_ord[iii]++;
                                    giacenza++;
                                    giacenzapeso += dens_BD;
                                }
                                if (ordiniBS_ord[iii] > MENOMILLE && giacenzapeso + dens_BS <= capton[n_viaggio] &&
                                    giacenza + 1 <= capmax[n_viaggio])
                                {
                                    ordiniBS_ord[iii]++;
                                    giacenza++;
                                    giacenzapeso += dens_BS;
                                }
                                if (ordiniAlpino_ord[iii] > MENOMILLE && giacenzapeso + dens_D <= capton[n_viaggio] &&
                                    giacenza + 1 <= capmax[n_viaggio])
                                {
                                    ordiniAlpino_ord[iii]++;
                                    giacenza++;
                                    giacenzapeso += dens_D;
                                }
                                if (ordiniBluAlpino_ord[iii] > MENOMILLE && giacenzapeso + dens_BD <= capton[n_viaggio] &&
                                   giacenza + 1 <= capmax[n_viaggio])
                                {
                                    ordiniBluAlpino_ord[iii]++;
                                    giacenza++;
                                    giacenzapeso += dens_BD;
                                }
                            }
                            //se sotto 39 provo a mettercene ancora
                            double Temp_D = 0;
                            double Temp_BD = 0;
                            double Temp_BS = 0;
                            double Temp_B = 0;
                            double Temp_BAlpino = 0;
                            double Temp_Alpino = 0;
                            for (g = 0; g < viaggio_temp.Count; g++)
                            {
                                if (g == 0)
                                    iii = i;
                                else
                                    iii = j_temp[g - 1];

                                if (ordiniD_ord[iii] > 0 && ordini_piumeno_ord[iii] > 0 &&
                                    giacenzapeso + dens_D <= capton[n_viaggio] && giacenza + 1 <= capmax[n_viaggio])
                                {
                                    ordiniD_ord[iii]++;
                                    giacenza++;
                                    giacenzapeso += dens_D;
                                }
                                if (ordiniB95_ord[iii] > 0 && ordini_piumeno_ord[iii] > 0 &&
                                    giacenzapeso + dens_B95 <= capton[n_viaggio] && giacenza + 1 <= capmax[n_viaggio])
                                {
                                    ordiniB95_ord[iii]++;
                                    giacenza++;
                                    giacenzapeso += dens_B95;
                                }
                                if (ordiniBD_ord[iii] > 0 && ordini_piumeno_ord[iii] > 0 &&
                                    giacenzapeso + dens_BD <= capton[n_viaggio] && giacenza + 1 <= capmax[n_viaggio])
                                {
                                    ordiniBD_ord[iii]++;
                                    giacenza++;
                                    giacenzapeso += dens_BD;
                                }
                                if (ordiniBS_ord[iii] > 0 && ordini_piumeno_ord[iii] > 0 &&
                                    giacenzapeso + dens_BS <= capton[n_viaggio] && giacenza + 1 <= capmax[n_viaggio])
                                {
                                    ordiniBS_ord[iii]++;
                                    giacenza++;
                                    giacenzapeso += dens_BS;
                                }
                                if (ordiniAlpino_ord[iii] > 0 && ordini_piumeno_ord[iii] > 0 &&
                                    giacenzapeso + dens_D <= capton[n_viaggio] && giacenza + 1 <= capmax[n_viaggio])
                                {
                                    ordiniAlpino_ord[iii]++;
                                    giacenza++;
                                    giacenzapeso += dens_D;
                                }
                                if (ordiniBluAlpino_ord[iii] > 0 && ordini_piumeno_ord[iii] > 0 &&
                                    giacenzapeso + dens_BD <= capton[n_viaggio] && giacenza + 1 <= capmax[n_viaggio])
                                {
                                    ordiniBluAlpino_ord[iii]++;
                                    giacenza++;
                                    giacenzapeso += dens_BD;
                                }
                                Temp_D += ordiniD_ord[iii];
                                Temp_BD += ordiniBD_ord[iii];
                                Temp_BS += ordiniBS_ord[iii];
                                Temp_B += ordiniB95_ord[iii];
                                Temp_Alpino += ordiniAlpino_ord[iii];
                                Temp_BAlpino += ordiniBluAlpino_ord[iii];
                            }

                            Quantita[n_viaggio] = Temp_D.ToString() + "," + Temp_BD.ToString() + "," + Temp_BS.ToString() + "," +
                                Temp_B.ToString() + "," + Temp_Alpino.ToString() + "," + Temp_BAlpino.ToString();
                            g = 0;
                            ScompartaturaClass sc = new ScompartaturaClass();

                            while (sc.Scompartatura(IdM[(int)TargheViaggi[n_viaggio]], Quantita[n_viaggio], MENOMILLE, conn) != 1 && g < viaggio_temp.Count)
                            {
                                t = 0;
                                if (g == 0)
                                    iii = i;
                                else
                                    iii = j_temp[g - 1];

                                if (ordiniD_ord[iii] <= MENOMILLE && ordiniD_ord[iii] > 0)
                                {
                                    ordiniD_ord[iii]--;
                                    giacenza--;
                                    giacenzapeso -= dens_D;
                                    Temp_D--;
                                    t = 1;
                                }
                                if (ordiniB95_ord[iii] <= MENOMILLE && t != 1 && ordiniB95_ord[iii] > 0)
                                {
                                    ordiniB95_ord[iii]--;
                                    giacenza--;
                                    giacenzapeso -= dens_B95;
                                    Temp_B--;
                                    t = 1;
                                }
                                if (ordiniBD_ord[iii] <= MENOMILLE && t != 1 && ordiniBD_ord[iii] > 0)
                                {
                                    ordiniBD_ord[iii]--;
                                    giacenza--;
                                    giacenzapeso -= dens_BD;
                                    Temp_BD--;
                                    t = 1;
                                }
                                if (ordiniBS_ord[iii] <= MENOMILLE && t != 1 && ordiniBS_ord[iii] > 0)
                                {
                                    ordiniBS_ord[iii]--;
                                    giacenza--;
                                    giacenzapeso -= dens_BS;
                                    Temp_BS--;
                                    t = 1;
                                }
                                if (ordiniAlpino_ord[iii] <= MENOMILLE && t != 1 && ordiniAlpino_ord[iii] > 0)
                                {
                                    ordiniAlpino_ord[iii]--;
                                    giacenza++;
                                    giacenzapeso -= dens_D;
                                    Temp_Alpino--;
                                    t = 1;
                                }
                                if (ordiniBluAlpino_ord[iii] <= MENOMILLE && t != 1 && ordiniBluAlpino_ord[iii] > 0)
                                {
                                    ordiniBluAlpino_ord[iii]--;
                                    giacenza++;
                                    giacenzapeso -= dens_BD;
                                    Temp_BAlpino--;
                                    t = 1;
                                }
                                g++;
                                Quantita[n_viaggio] = Temp_D.ToString() + "," + Temp_BD.ToString() + "," + Temp_BS.ToString() + "," +
                                Temp_B.ToString() + "," + Temp_Alpino.ToString() + "," + Temp_BAlpino.ToString();
                            }
                            double TempPrecD = Temp_D;
                            double TempPrecB = Temp_B;
                            double TempPrecBD = Temp_BD;
                            double TempPrecBS = Temp_BS;
                            double TempPrecAlpino = Temp_Alpino;
                            double TempPrecBAlpino = Temp_BAlpino;
                            Quantita[n_viaggio] = TempPrecD.ToString() + "," + TempPrecBD.ToString() + "," + TempPrecBS.ToString() + "," +
                                TempPrecB.ToString() + "," + TempPrecAlpino.ToString() + "," + TempPrecBAlpino.ToString();
                            giacenza_stored[n_viaggio] = giacenza;
                            giacenzapeso_stored[n_viaggio] = giacenzapeso;
                            tempo[n_viaggio] = Convert.ToInt16(tempo_temp);
                            for (int q = 0; q < viaggio_temp.Count; q++)
                            {
                                viaggio.Insert(n_viaggio, viaggio_temp.ToList<double>());
                                MioOrdineViaggio.Insert(n_viaggio, Mio_temp.ToList<double>());
                                //MioOrdineViaggio[n_viaggio][q] = Mio_temp[q];
                            }
                            /*for (int q = 0; q < viaggio_temp.Count; q++)
                            {
                                viaggio[n_viaggio][q] = viaggio_temp[q];
                                MioOrdineViaggio[n_viaggio][q] = Mio_temp[q];
                            }*/
                            for (int q = 0; q < j_temp.Count; q++)
                                da_servire[j_temp[q]] = 0;
                            trovato = 1;
                        }
                    }
                    if (n_viaggio > capton.Length || trovato==0)
                    {
                        if (NoMezzi == 0)
                        {
                            trovato = 0;
                            while (trovato == 0 && M > 0)
                            {
                                if (da_servire[j] == 1 && giacenzapeso + ordinipeso_ord[j] <= 31.0)
                                {
                                    //-------------------------
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

                                    tempoguida = Convert.ToInt16(tempoguida + tempoguida1);
                                    tempo_temp = Convert.ToInt16(tempoguida + temposcarica);
                                    drop++;
                                    trovato = 1;
                                    viaggio_temp.Insert(drop, pv_ord[j]);
                                    Mio_temp[drop] = MioOrdine_ord[j];
                                    j_temp.Insert(drop - 1, j);
                                    da_servire[j] = 0;
                                    maschera[j] = 0;
                                    giacenza = giacenza + ordini_ord[j];
                                    giacenzapeso = giacenzapeso + ordinipeso_ord[j];
                                    tempoGuidaOld = Convert.ToInt16(tempo_temp);
                                }
                                else
                                {
                                    temp = new double[maschera.Length];
                                    for (o = 0; o < maschera.Length; o++)
                                    {
                                        temp[o] = valore_ord[i, o] * maschera[o];
                                    }
                                    M = temp.Max();
                                    j = temp.ToList().IndexOf(M);
                                }
                            }

                            trovato = 0;
                            int zz = 3;
                            temp = new double[maschera.Length];
                            for (o = 0; o < maschera.Length; o++)
                            {
                                temp[o] = valore_ord[i, o] * maschera[o];
                            }
                            M = temp.Max();
                            j = temp.ToList().IndexOf(M);
                            while (trovato == 0 && M > 0 && zz < MAXDROP)
                            {
                                for (int z = zz; z < MAXDROP; z++)
                                {
                                    for (int pv = 0; pv < j_temp.Count; pv++)
                                    {
                                        int k = j_temp[pv];

                                        temp = new double[maschera.Length];
                                        for (o = 0; o < maschera.Length; o++)
                                        {
                                            temp[o] = valore_ord[k, o] * maschera[o];
                                        }
                                        double Mtmp = temp.Max();
                                        int jtmp = temp.ToList().IndexOf(M);
                                        if (Mtmp > M)
                                        {
                                            M = Mtmp;
                                            j = jtmp;
                                        }
                                    }
                                    trovato = 0;
                                    while (trovato == 0 && M > 0)
                                    {
                                        if (da_servire[j] == 1 && giacenzapeso + ordinipeso_ord[j] <= capton[n_viaggio] &&
                                           giacenza + ordini_ord[j] <= capmax[n_viaggio])
                                        {
                                            Int16 temposcaricaj = Convert.ToInt16(SCARICA + (SCARICALITRO * max_product_ord[j]));
                                            temposcarica = Convert.ToInt16(temposcarica + temposcaricaj);
                                            Int16 temposcaricasenzaj = Convert.ToInt16(temposcarica);

                                            Calcola_percorsoNewClass cpn = new Calcola_percorsoNewClass();
                                            Calcola_percorsoNewStruct cpnStruct = cpn.calcola_percorsoNew(baseCarico, od_pv_pv_completa, viaggio_temp, sequenza, lun, n_viaggio);
                                            //prendo il tempoguida precedente???
                                            Int16 tempoguidasenzaj = Convert.ToInt16(tempoguida);
                                            tempoguida = Convert.ToInt16(cpnStruct.lun[n_viaggio] * MINxKM);
                                            //devo rifare il calcolo del viaggio considerando latripla / quaterna / ecc..e nn solo la coppia
                                            tempo_temp = Convert.ToInt16(tempoguida + temposcarica);
                                            maschera[j] = 0;
                                            //----------------------------------------
                                            Int16 temp1 = Convert.ToInt16(TargheTempo[tt] + Convert.ToInt16(tempoGuidaOld) - tempo_temp);
                                            if (temp1 >= 0)
                                            {
                                                drop++;
                                                viaggio_temp.Insert(drop, pv_ord[j]);
                                                Mio_temp[drop] = MioOrdine_ord[j];
                                                j_temp.Insert(z - 1, j);
                                                da_servire[j] = 0;
                                                maschera[j] = 0;
                                                trovato = 1;
                                                giacenza = giacenza + ordini_ord[j];
                                                giacenzapeso = giacenzapeso + ordinipeso_ord[j];
                                                TargheTempo[tt] = temp1;
                                                tempoGuidaOld = Convert.ToInt16(tempo_temp);
                                            }
                                            else
                                            {
                                                maschera[j] = 0;
                                                int k = i;

                                                temposcarica = Convert.ToInt16(temposcaricasenzaj);
                                                tempoguida = Convert.ToInt16(tempoguidasenzaj);
                                                tempo_temp = Convert.ToInt16(tempoguida + temposcarica);

                                                for (o = 0; o < maschera.Length; o++)
                                                {
                                                    temp[o] = valore_ord[k, o] * maschera[o];
                                                }
                                                M = temp.Max();
                                                j = temp.ToList().IndexOf(M);

                                                for (int pv = 0; pv < z - 2; pv++)
                                                {
                                                    k = j_temp[pv];

                                                    temp = new double[maschera.Length];
                                                    for (o = 0; o < maschera.Length; o++)
                                                    {
                                                        temp[o] = valore_ord[k, o] * maschera[o];
                                                    }
                                                    double Mtmp = temp.Max();
                                                    int jtmp = temp.ToList().IndexOf(M);
                                                    if (Mtmp > M)
                                                    {
                                                        M = Mtmp;
                                                        j = jtmp;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            maschera[j] = 0;
                                            int k = i;

                                            temp = new double[maschera.Length];
                                            for (o = 0; o < maschera.Length; o++)
                                            {
                                                temp[o] = valore_ord[k, o] * maschera[o];
                                            }
                                            M = temp.Max();
                                            j = temp.ToList().IndexOf(M);

                                            for (int pv = 0; pv < z - 2; pv++)
                                            {
                                                k = j_temp[pv];

                                                temp = new double[maschera.Length];
                                                for (o = 0; o < maschera.Length; o++)
                                                {
                                                    temp[o] = valore_ord[k, o] * maschera[o];
                                                }
                                                double Mtmp = temp.Max();
                                                int jtmp = temp.ToList().IndexOf(M);
                                                if (Mtmp > M)
                                                {
                                                    M = Mtmp;
                                                    j = jtmp;
                                                }
                                            }
                                        }
                                    }
                                }
                                zz++;
                            }
                        }
                        int iii = 0;
                        int g = 0;
                        for (g = 0; g < viaggio_temp.Count; g++)
                        {
                            //sommo di nuovo i meno fino a stare sotto o uguale ai 39
                            if (g == 0)
                                iii = i;
                            else
                                iii = j_temp[g - 1];
                            //bool isEqual = Enumerable.SequenceEqual(target1, target2);
                            if (ordiniD_ord[iii] > MENOMILLE && giacenzapeso + dens_D <= capton[n_viaggio] &&
                                giacenza + 1 <= capmax[n_viaggio])
                            {
                                ordiniD_ord[iii]++;
                                giacenza++;
                                giacenzapeso += dens_D;
                            }
                            if (ordiniB95_ord[iii] > MENOMILLE && giacenzapeso + dens_B95 <= capton[n_viaggio] &&
                                giacenza + 1 <= capmax[n_viaggio])
                            {
                                ordiniB95_ord[iii]++;
                                giacenza++;
                                giacenzapeso += dens_B95;
                            }
                            if (ordiniBD_ord[iii] > MENOMILLE && giacenzapeso + dens_BD <= capton[n_viaggio] &&
                                giacenza + 1 <= capmax[n_viaggio])
                            {
                                ordiniBD_ord[iii]++;
                                giacenza++;
                                giacenzapeso += dens_BD;
                            }
                            if (ordiniBS_ord[iii] > MENOMILLE && giacenzapeso + dens_BS <= capton[n_viaggio] &&
                                giacenza + 1 <= capmax[n_viaggio])
                            {
                                ordiniBS_ord[iii]++;
                                giacenza++;
                                giacenzapeso += dens_BS;
                            }
                            if (ordiniAlpino_ord[iii] > MENOMILLE && giacenzapeso + dens_D <= capton[n_viaggio] &&
                                giacenza + 1 <= capmax[n_viaggio])
                            {
                                ordiniAlpino_ord[iii]++;
                                giacenza++;
                                giacenzapeso += dens_D;
                            }
                            if (ordiniBluAlpino_ord[iii] > MENOMILLE && giacenzapeso + dens_BD <= capton[n_viaggio] &&
                               giacenza + 1 <= capmax[n_viaggio])
                            {
                                ordiniBluAlpino_ord[iii]++;
                                giacenza++;
                                giacenzapeso += dens_BD;
                            }
                        }
                        //se sotto 39 provo a mettercene ancora
                        double Temp_D = 0;
                        double Temp_BD = 0;
                        double Temp_BS = 0;
                        double Temp_B = 0;
                        double Temp_BAlpino = 0;
                        double Temp_Alpino = 0;
                        for (g = 0; g < viaggio_temp.Count; g++)
                        {
                            if (g == 0)
                                iii = i;
                            else
                                iii = j_temp[g - 1];

                            if (ordiniD_ord[iii] > 0 && ordini_piumeno_ord[iii] > 0 &&
                                giacenzapeso + dens_D <= 31.0 && giacenza + 1 <= 39)
                            {
                                ordiniD_ord[iii]++;
                                giacenza++;
                                giacenzapeso += dens_D;
                            }
                            if (ordiniB95_ord[iii] > 0 && ordini_piumeno_ord[iii] > 0 &&
                                giacenzapeso + dens_B95 <= 31.0 && giacenza + 1 <= 39)
                            {
                                ordiniB95_ord[iii]++;
                                giacenza++;
                                giacenzapeso += dens_B95;
                            }
                            if (ordiniBD_ord[iii] > 0 && ordini_piumeno_ord[iii] > 0 &&
                                giacenzapeso + dens_BD <= 31.0 && giacenza + 1 <= 39)
                            {
                                ordiniBD_ord[iii]++;
                                giacenza++;
                                giacenzapeso += dens_BD;
                            }
                            if (ordiniBS_ord[iii] > 0 && ordini_piumeno_ord[iii] > 0 &&
                                giacenzapeso + dens_BS <= 31.0 && giacenza + 1 <= 39)
                            {
                                ordiniBS_ord[iii]++;
                                giacenza++;
                                giacenzapeso += dens_BS;
                            }
                            if (ordiniAlpino_ord[iii] > 0 && ordini_piumeno_ord[iii] > 0 &&
                                giacenzapeso + dens_D <= 31.0 && giacenza + 1 <= 39)
                            {
                                ordiniAlpino_ord[iii]++;
                                giacenza++;
                                giacenzapeso += dens_D;
                            }
                            if (ordiniBluAlpino_ord[iii] > 0 && ordini_piumeno_ord[iii] > 0 &&
                                giacenzapeso + dens_BD <= 31.0 && giacenza + 1 <= 39)
                            {
                                ordiniBluAlpino_ord[iii]++;
                                giacenza++;
                                giacenzapeso += dens_BD;
                            }
                            Temp_D += ordiniD_ord[iii];
                            Temp_BD += ordiniBD_ord[iii];
                            Temp_BS += ordiniBS_ord[iii];
                            Temp_B += ordiniB95_ord[iii];
                            Temp_Alpino += ordiniAlpino_ord[iii];
                            Temp_BAlpino += ordiniBluAlpino_ord[iii];
                        }

                        Quantita[n_viaggio] = Temp_D.ToString() + "," + Temp_BD.ToString() + "," + Temp_BS.ToString() + "," +
                            Temp_B.ToString() + "," + Temp_Alpino.ToString() + "," + Temp_BAlpino.ToString();
                        g = 0;
                        ScompartaturaClass sc = new ScompartaturaClass();

                        while (sc.Scompartatura(1032, Quantita[n_viaggio], MENOMILLE, conn) != 1 && g < viaggio_temp.Count)
                        {
                            t = 0;
                            if (g == 0)
                                iii = i;
                            else
                                iii = j_temp[g - 1];

                            if (ordiniD_ord[iii] <= MENOMILLE && ordiniD_ord[iii] > 0)
                            {
                                ordiniD_ord[iii]--;
                                giacenza--;
                                giacenzapeso -= dens_D;
                                Temp_D--;
                                t = 1;
                            }
                            if (ordiniB95_ord[iii] <= MENOMILLE && t != 1 && ordiniB95_ord[iii] > 0)
                            {
                                ordiniB95_ord[iii]--;
                                giacenza--;
                                giacenzapeso -= dens_B95;
                                Temp_B--;
                                t = 1;
                            }
                            if (ordiniBD_ord[iii] <= MENOMILLE && t != 1 && ordiniBD_ord[iii] > 0)
                            {
                                ordiniBD_ord[iii]--;
                                giacenza--;
                                giacenzapeso -= dens_BD;
                                Temp_BD--;
                                t = 1;
                            }
                            if (ordiniBS_ord[iii] <= MENOMILLE && t != 1 && ordiniBS_ord[iii] > 0)
                            {
                                ordiniBS_ord[iii]--;
                                giacenza--;
                                giacenzapeso -= dens_BS;
                                Temp_BS--;
                                t = 1;
                            }
                            if (ordiniAlpino_ord[iii] <= MENOMILLE && t != 1 && ordiniAlpino_ord[iii] > 0)
                            {
                                ordiniAlpino_ord[iii]--;
                                giacenza++;
                                giacenzapeso -= dens_D;
                                Temp_Alpino--;
                                t = 1;
                            }
                            if (ordiniBluAlpino_ord[iii] <= MENOMILLE && t != 1 && ordiniBluAlpino_ord[iii] > 0)
                            {
                                ordiniBluAlpino_ord[iii]--;
                                giacenza++;
                                giacenzapeso -= dens_BD;
                                Temp_BAlpino--;
                                t = 1;
                            }
                            g++;
                            Quantita[n_viaggio] = Temp_D.ToString() + "," + Temp_BD.ToString() + "," + Temp_BS.ToString() + "," +
                            Temp_B.ToString() + "," + Temp_Alpino.ToString() + "," + Temp_BAlpino.ToString();
                        }
                        double TempPrecD = Temp_D;
                        double TempPrecB = Temp_B;
                        double TempPrecBD = Temp_BD;
                        double TempPrecBS = Temp_BS;
                        double TempPrecAlpino = Temp_Alpino;
                        double TempPrecBAlpino = Temp_BAlpino;
                        Quantita[n_viaggio] = TempPrecD.ToString() + "," + TempPrecBD.ToString() + "," + TempPrecBS.ToString() + "," +
                            TempPrecB.ToString() + "," + TempPrecAlpino.ToString() + "," + TempPrecBAlpino.ToString();
                        giacenza_stored[n_viaggio] = giacenza;
                        giacenzapeso_stored[n_viaggio] = giacenzapeso;
                        tempo[n_viaggio] = Convert.ToInt16(tempo_temp);
                        for (int q = 0; q < viaggio_temp.Count; q++)
                        {
                            viaggio.Insert(n_viaggio, viaggio_temp.ToList<double>());
                            MioOrdineViaggio.Insert(n_viaggio, Mio_temp.ToList<double>());
                            //MioOrdineViaggio[n_viaggio][q] = Mio_temp[q];
                        }
                        /*for (int q = 0; q < viaggio_temp.Count; q++)
                        {
                            viaggio[n_viaggio][q] = viaggio_temp[q];
                            MioOrdineViaggio[n_viaggio][q] = Mio_temp[q];
                        }*/
                        for (int q = 0; q < j_temp.Count; q++)
                            da_servire[j_temp[q]] = 0;
                    }
                }
            }
            //assegno i valori alla struct
            cvStruct.MioOrdineViaggio = MioOrdineViaggio;
            cvStruct.TargheViaggi = TargheViaggi;
            cvStruct.IdM = IdM;
            cvStruct.targa = targa;
            cvStruct.n_viaggio = n_viaggio;
            cvStruct.scartato = scartato;
            cvStruct.sequenza = sequenza;
            cvStruct.giacenza = giacenza;
            cvStruct.giacenzapeso = giacenzapeso;
            cvStruct.giacenzapeso_stored = giacenzapeso_stored;
            cvStruct.viaggio_temp = viaggio_temp;
            cvStruct.lun = lun;
            cvStruct.da_servire = da_servire;
            cvStruct.tempo_temp = tempo_temp;
            cvStruct.tempo = tempo;
            cvStruct.viaggio = viaggio;
            cvStruct.ordiniD_ord = ordiniD_ord;
            cvStruct.ordiniBD_ord = ordiniBD_ord;
            cvStruct.ordiniB95_ord = ordiniB95_ord;
            cvStruct.ordiniBS_ord = ordiniBS_ord;
            cvStruct.ordiniALpino_ord = ordiniAlpino_ord;
            cvStruct.ordiniBluAlpino_ord = ordiniBluAlpino_ord;
            //e la ritorno
            return cvStruct;
        }
    }
}