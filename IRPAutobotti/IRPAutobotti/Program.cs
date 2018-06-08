using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRPAutobotti
{
    class Program
    {
        static void Main(string[] args)
        {
            int attivo = 1;
            int baseCarico = 17;
            String data = "2018-05-01";
            String formatOut = "mm-dd-yyyy";

            //Carico i settings dal database
            //TODO insert databse connection
            SqlConnection conn = ;

            CaricaSettingsClass caricaSettingsClass = new CaricaSettingsClass();
            // [GIACENZA_MIN,dens_BS,dens_D,dens_B95,dens_GA,dens_BD,CARICA,SCARICA,SCARICALITRO,V_MEDIA,MINxKM,TEMPO_MAX,MAXDROP,KM_MIN,DISTANZA_MAX_PVPV,ELLISSE,beta,esponente]
            Object[] settings = caricaSettingsClass.CaricaSettings(baseCarico, conn);
            
            IdSettingsVariabiliClass idSettingsVariabiliClass = new IdSettingsVariabiliClass();
            // [IdSettings,MENOMILLE,RIEMPIMENTOMAX]
            Object[] idSettings = idSettingsVariabiliClass.IdSettingsVariabili(baseCarico, conn);

            //prendo i camion disponibili per la giornata
            DisponibilitaMezziClass disponibilitaMezziClass = new DisponibilitaMezziClass();
            // [IdM,scomparti,captontemp,targatemp1,temp2]
            Object[] disponibilitaMezzi = disponibilitaMezziClass.DisponibilitaMezzi(attivo, data, baseCarico, conn);
            double[][] scomparti = (double[][])disponibilitaMezzi[1];
            double[] capmaxtemp = new double[scomparti.Length];
            for (int i = 0; i < scomparti.Length; i++)
            {
                for(int j = 0; j < scomparti[i].Length; j++)
                {
                    capmaxtemp[i] += scomparti[i][j];
                }
                capmaxtemp[i] /= 1000;
            }

            //ottengo il numero dei mezzi
            int nmezzi = capmaxtemp.Length;

            int[] targatemp = new int[nmezzi];
            for (int i = 0; i < nmezzi; i++)
                targatemp[i] = i;

            //ritorna la capacità massima di tutto il mio parter
            double maxcap = capmaxtemp.Max();

            //calcolo della moda di captontemp (elemento con maggior frequenza)
            double[] captontemp= (double[])disponibilitaMezzi[2];
            double pesoDivisioneOrdini = captontemp.GroupBy(item => item).OrderByDescending(g => g.Count()).Select(g => g.Key).First();  // TODO verificare che la moda ritorni il valore più piccolo in caso di frequenze tra due elementi uguali

            for (int i = 0; i < captontemp.Length; i++)
                captontemp[i] /= 10;


            // prendo gli ordini per la giornata
            // [MioOrdine,pv,ordiniO,ordiniDO,ordiniBDO,ordiniB95O,ordiniBSO,ordiniAlpino,ordiniBluAlpino,ordinipiumeno]
            PrendereOrdiniClass prendereOrdiniClass = new PrendereOrdiniClass();
            Object[] ordiniArray = prendereOrdiniClass.PrendereOrdini(baseCarico, data, pesoDivisioneOrdini, conn);
            var n_ordiniO = ((double[])ordiniArray[2]).Length;

            // quantita massima per ogni PV, per il rate di scarico (successivo)
            // numero di vettori che compongono la matrice: 6
            double[][] prodottiArray = new double[n_ordiniO][];
            for(int i = 0; i < 6; i++)
            {
                // inserisce ogni vettore da ordiniDO a ordiniBluAlpino nella matrice
                // il +3 all'indice è dovuto al fatto che il vettore ordini contiene anche altri vettori che però non servono alla costruzione della matrice
                Array.Copy((double[])ordiniArray[3+i], 0, prodottiArray, i+1, ((double[])ordiniArray[3+i]).Length);
            }

            double[] prodottomax = new double[n_ordiniO];
            for(int i = 0; i < n_ordiniO; i++)
            {
                prodottomax[i] = prodottiArray[i].Max();
            }

            // prendo la matrice delle preferenze
            PrendiPreferenzeClass prendiPreferenzeClass = new PrendiPreferenzeClass();
            var preferenze = prendiPreferenzeClass.PrendiPreferenze(baseCarico, conn);

            //Ordinamento mezzi
            // ordino in modo crescente e decrescente i miei mezzi per tonellate e capacità in KL
            //[capton, capmax, targa]
            OrdinamentoMezziClass ordinamentoMezziClass = new OrdinamentoMezziClass();
            Object[] ordinamentoMezzi = ordinamentoMezziClass.OrdinamentoMezzi(captontemp, capmaxtemp, targatemp);

            // Caricamento matrici delle distanze
            // [od_dep_pv,od_pv_dep,od_pv_pv,od_pv_pv_completa,preferenza_pv_pv]
            PrendiDistanzeClass prendiDistanzeClass = new PrendiDistanzeClass();
            Object[] distanze = prendiDistanzeClass.PrendiDistanze(baseCarico, data, n_ordiniO, (String[])ordiniArray[1], preferenze, conn);
            double[] peso = new double[n_ordiniO];
            var ordinati = (double[])ordiniArray[2];
            var targaOriginale = (String)ordinamentoMezzi[2];

            CreateRunnerClass createRunnerClass = new CreateRunnerClass();
            var IdRunner = createRunnerClass.CreateRunner("sacile", baseCarico, data, conn);

            // -----------------Chiusura connessione close(conn)------------------------

            for(int t = 0; t < ((int[])idSettings[0]).Length; t++)
            {
                var n_OrdiniOriginali = n_ordiniO;
                var OrdiniOriginali = (double[])ordiniArray[2];
                var OrdiniOriginaliD = (double[])ordiniArray[3];
                var OrdiniOriginaliBD = (double[])ordiniArray[4];
                var OrdiniOriginaliB95 = (double[])ordiniArray[5];
                var OrdiniOriginaliBS = (double[])ordiniArray[6];
                var OrdiniOriginaliAlpino = (double[])ordiniArray[7];
                var OrdiniOriginaliBluAlpino = (double[])ordiniArray[8];
                var MioOrdineOriginale = (double[])ordiniArray[0];

                // TOGLIAMO MILLE A TUTTI I PRODOTTI CHA HANNO UNA QUANTITA SOPRA LA SOGLIA MENOMILLE
                // [ordini, ordiniD, ordiniBD, ordiniB95, ordiniBS, ordiniAlpino, ordiniBluAlpino, peso]
                OrdiniMenoMilleClass ordiniMenoMilleClass = new OrdiniMenoMilleClass();
                double[] MENOMILLE = (double[])idSettings[1];
                Object[] ordiniMenoMille = ordiniMenoMilleClass.OrdiniMenoMille(n_OrdiniOriginali, MENOMILLE[t], OrdiniOriginali, OrdiniOriginaliD, OrdiniOriginaliBD, OrdiniOriginaliB95, OrdiniOriginaliBS, OrdiniOriginaliAlpino, OrdiniOriginaliBluAlpino, (double)settings[2], (double)settings[5], (double)settings[1], (double)settings[3]);

                //ASSEGNO PRIORITA'
                // sommo l'andata e il ritorno di un pv dalla base, e prendo quello che ha valore maggiore
                // [p, Valore, od_dep_media]
                AssegnazionePrioritaClass assegnazionePrioritaClass = new AssegnazionePrioritaClass();
                Object[] priorita = assegnazionePrioritaClass.AssegnazionePriorita((double[])distanze[0], (double[])distanze[1], (double[])distanze[2], n_OrdiniOriginali, peso, maxcap, (double[])ordiniMenoMille[0], (int)settings[17], (double)settings[15], (double)settings[16], (double[])distanze[4], (int)settings[14]);

                //ORDINAMENTO DEI PV
                // ordinamento dal più distante
                // ordino in maniera decrescente per il valore della prima colonna
                // [MioOrdine_ord, ordini_ord, pv_ord, valore_ord, od_pv_pv_ord, od_dep_pv_ord, od_pv_dep_ord, ordinipeso_ord, ordini_piumeno_ord, max_product_ord, ordiniD_ord, ordiniBD_ord, ordiniB95_ord, ordiniBS_ord, ordiniAlpino_ord, ordiniBluAlpino_ord, ordinati_ord]
                OrdinamentoPVClass ordinamentoPVClass = new OrdinamentoPVClass();
                Object[] ordinamentoPV = ordinamentoPVClass.OrdinamentoPV((double)priorita[2], (String[])ordiniArray[1], (double[])ordiniMenoMille[0], (double[][])priorita[1], (double[])distanze[2], n_OrdiniOriginali, (double[][])distanze[0], (double[][])distanze[1], (double[])ordiniArray[9], prodottomax, peso, (double[])ordiniArray[3], (double[])ordiniArray[4], (double[])ordiniArray[5], (double[])ordiniArray[6], (double[])ordiniArray[7], (double[])ordiniArray[8], (double[])ordiniArray[0], ordinati);

                // ALGORITMO
                conn = "";
                // [MioOrdineViaggio, targheViaggi, IdM, targa, n_viaggio, scartato, sequenza, giacenza_stored, giacenzapeso, giacenzapeso_stored, viaggio_temp, lun, da_servire, tempo_temp, tempo, viaggio, ordiniD_ord, ordiniBD_ord, ordiniB95_ord, ordiniBS_ord, ordiniAlpino_ord, ordiniBluAlpino_ord]
                CalcoloViaggiClass calcoloViaggiClass = new CalcoloViaggiClass();
                Object[] calcoloViaggi = calcoloViaggiClass.CalcoloViaggi((int[])disponibilitaMezzi[0], targaOriginale, (double[])distanze[3], n_OrdiniOriginali, (double[])ordinamentoPV[2], baseCarico, (double[])ordinamentoPV[7], (double[])settings[7], (double[])settings[8], (double[])settings[9], (double[])ordinamentoPV[9], (double[])ordinamentoPV[5], (double[])ordinamentoPV[6], (double[])ordinamentoPV[4], (double[])settings[10], (double[])settings[11], (double[])ordinamentoPV[3], (double[])ordinamentoPV[1], (double[])ordinamentoPV[10], (double[])ordinamentoPV[12], (double[])ordinamentoPV[11], (double[])ordinamentoPV[13], (double[])ordinamentoPV[14], (double[])ordinamentoPV[15], (double[])ordinamentoPV[8], (double[])ordinamentoPV[0], (double[])settings[0], (double[])settings[13], (double[])ordinamentoMezzi[1], (double[])ordinamentoMezzi[0], (double[])settings[2], (double[])settings[3], (double[])settings[5], (double[])settings[1], (double[])settings[12], MENOMILLE[t], conn);
                //close(conn);

                // Inserisci nel db
                // TargheTempo = TEMPO_MAX * ones(length(IdM), 1);
                conn = "";
                // [IdRunner] = CreateRunner('sacile', baseCarico, data, conn);
                CreateVersionClass createVersionClass = new CreateVersionClass();
                int IdVersione = createVersionClass.CreateVersion(baseCarico, "sacile", data, ((int[])idSettings[0])[t], IdRunner, conn);
                // close(conn)

                int ii = 1;
                int n_viaggio = (int)calcoloViaggi[4];
                while (ii <= n_viaggio)
                {
                    conn = "";
                    double[] capton = (double[])ordinamentoMezzi[0];
                    int[] targheViaggi = (int[])calcoloViaggi[1];

                    double[] tempo = (double[])calcoloViaggi[14];
                    double[] lun = (double[])calcoloViaggi[11];
                    int[] IdM = (int[])calcoloViaggi[2];
                    if (ii <= capton.Length && targheViaggi[ii] != -1)
                    {
                        CreateViaggioClass createViaggioClass = new CreateViaggioClass();
                        int IdViaggio = createViaggioClass.CreateViaggio(IdVersione, data, lun[ii], tempo[ii], IdM[targheViaggi[ii]], conn);
                    }
                    else
                    {
                        CreateViaggioNoMezzoClass createViaggioNoMezzoClass = new CreateViaggioNoMezzoClass();
                        int IdViaggio = createViaggioNoMezzoClass.CreateViaggioNoMezzo(IdVersione, data, lun[ii], tempo[ii], -1, conn);
                    }
                    //close(conn);

                    double[][] viaggio = (double[][])calcoloViaggi[15];
                    for(int j = 0; j < viaggio[ii].Length; j++)
                    {
                        if(viaggio[ii][j] != 0)
                        {
                            // TODO Creare funzione che cerca in due matrici se alla posizione (x,y) di entrambe si trovano due valori specifici.
                            // la ricerca avviene riga per riga e l'array di ritorno conterrà la posizione (x,y)
                        }
                    }
                }

            }
        }
    }
}