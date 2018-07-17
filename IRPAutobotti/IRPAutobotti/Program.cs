using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace IRPAutobotti
{
    class Program
    {
        static void Main(string[] args)
        {
            int attivo = 1;
            int baseCarico = 9;
            String data = "2018-01-15";
            String formatOut = "yyyy-mm-dd";

            //Carico i settings dal database
            SqlConnection conn = new SqlConnection();
            //"Server=localhost\\SQLEXPRESS;"
            conn.ConnectionString =
            "Server=localhost\\SQLEXPRESS;" +
            "Database=Matlab;" +
            "Integrated Security=True";

            CaricaSettingsClass caricaSettingsClass = new CaricaSettingsClass();
            // [GIACENZA_MIN,dens_BS,dens_D,dens_B95,dens_GA,dens_BD,CARICA,SCARICA,SCARICALITRO,V_MEDIA,MINxKM,TEMPO_MAX,MAXDROP,KM_MIN,DISTANZA_MAX_PVPV,ELLISSE,beta,esponente]
            CaricaSettingsStruct settings = caricaSettingsClass.CaricaSettings(baseCarico, conn);
            
            IdSettingsVariabiliClass idSettingsVariabiliClass = new IdSettingsVariabiliClass();
            // [IdSettings,MENOMILLE,RIEMPIMENTOMAX]
            IdSettingsVariabiliStruct idSettings = idSettingsVariabiliClass.IdSettingsVariabili(baseCarico, conn);

            //prendo i camion disponibili per la giornata
            DisponibilitaMezziClass disponibilitaMezziClass = new DisponibilitaMezziClass();
            // [IdM,scomparti,captontemp,targatemp1,temp2]
            DisponibilitaMezziStruct disponibilitaMezzi = disponibilitaMezziClass.DisponibilitaMezzi(attivo, data, baseCarico, conn);
            double[,] scomparti = disponibilitaMezzi.scomparti;
            double[] capmaxtemp = new double[scomparti.Length];
            for (int i = 0; i < scomparti.GetLength(0); i++)
            {
                for(int j = 0; j < scomparti.GetLength(1); j++)
                {
                    capmaxtemp[i] += scomparti[i,j];
                }
                capmaxtemp[i] /= 1000;
            }

            //ottengo il numero dei mezzi
            int nmezzi = capmaxtemp.Length;

            double[] targatemp = new double[nmezzi];
            for (int i = 0; i < nmezzi; i++)
                targatemp[i] = i;

            //ritorna la capacità massima di tutto il mio parter
            double maxcap = capmaxtemp.Max();

            //calcolo della moda di captontemp (elemento con maggior frequenza)
            double[] captontemp=disponibilitaMezzi.captontemp;
            double pesoDivisioneOrdini = captontemp.GroupBy(item => item).OrderByDescending(g => g.Count()).Select(g => g.Key).First();  // TODO verificare che la moda ritorni il valore più piccolo in caso di frequenze tra due elementi uguali

            for (int i = 0; i < captontemp.Length; i++)
                captontemp[i] /= 10;


            // prendo gli ordini per la giornata
            // [MioOrdine,pv,ordini,ordiniD,ordiniBD,ordiniB95,ordiniBS,ordiniAlpino,ordiniBluAlpino,ordinipiumeno]
            PrendereOrdiniClass prendereOrdiniClass = new PrendereOrdiniClass();
            PrendereOrdiniStruct ordiniStruct = prendereOrdiniClass.PrendereOrdini(baseCarico, data, pesoDivisioneOrdini, conn);
            var n_ordini = ordiniStruct.ordini.Length;

            // quantita massima per ogni PV, per il rate di scarico (successivo)
            // numero di vettori che compongono la matrice: 6
            // inserisce ogni vettore da ordiniDO a ordiniBluAlpino nella matrice
            List<double[]> prodottiArray = new List<double[]>();
            prodottiArray.Add(ordiniStruct.ordiniD);
            prodottiArray.Add(ordiniStruct.ordiniBD);
            prodottiArray.Add(ordiniStruct.ordiniB95);
            prodottiArray.Add(ordiniStruct.ordiniBS);
            prodottiArray.Add(ordiniStruct.ordiniAlpino);
            prodottiArray.Add(ordiniStruct.ordiniBluAlpino);
            prodottiArray.ToArray();

            double[] prodottomax = new double[n_ordini];
            for(int i = 0; i < n_ordini; i++)
            {
                List<double> productsArray = new List<double>();
                for (int ii = 0; ii < prodottiArray.Count; ii++)
                    productsArray.Add(prodottiArray[ii][i]);
                productsArray.ToArray();
                prodottomax[i] = productsArray.Max();
            }

            // prendo la matrice delle preferenze
            PrendiPreferenzeClass prendiPreferenzeClass = new PrendiPreferenzeClass();
            PrendiPreferenzeStruct preferenze = prendiPreferenzeClass.PrendiPreferenze(baseCarico, conn);

            //Ordinamento mezzi
            // ordino in modo crescente e decrescente i miei mezzi per tonellate e capacità in KL
            //[capton, capmax, targa]
            OrdinamentoMezziClass ordinamentoMezziClass = new OrdinamentoMezziClass();
            OrdinamentoMezziStruct ordinamentoMezzi = ordinamentoMezziClass.OrdinamentoMezzi(captontemp, capmaxtemp, targatemp);

            // Caricamento matrici delle distanze
            // [od_dep_pv,od_pv_dep,od_pv_pv,od_pv_pv_completa,preferenza_pv_pv]
            PrendiDistanzeClass prendiDistanzeClass = new PrendiDistanzeClass();
            PrendiDistanzeStruct distanze = prendiDistanzeClass.PrendiDistanze(baseCarico, data, n_ordini, ordiniStruct.pv, preferenze.preferenze, conn);
            double[] peso = new double[n_ordini];
            var ordinati = ordiniStruct.ordini;
            var targaOriginale = ordinamentoMezzi.targa;

            CreateRunnerClass createRunnerClass = new CreateRunnerClass();
            int IdRunner = createRunnerClass.CreateRunner("sacile", baseCarico, data, conn);

            // -----------------Chiusura connessione close(conn)------------------------

            for(int t = 0; t < idSettings.Id.Length; t++)
            {
                var n_OrdiniOriginali = n_ordini;
                var OrdiniOriginali = ordiniStruct.ordini;
                var OrdiniOriginaliD = ordiniStruct.ordiniD;
                var OrdiniOriginaliBD = ordiniStruct.ordiniBD;
                var OrdiniOriginaliB95 = ordiniStruct.ordiniB95;
                var OrdiniOriginaliBS = ordiniStruct.ordiniBS;
                var OrdiniOriginaliAlpino = ordiniStruct.ordiniAlpino;
                var OrdiniOriginaliBluAlpino = ordiniStruct.ordiniBluAlpino;
                var MioOrdineOriginale = ordiniStruct.MioOrdine;

                // TOGLIAMO MILLE A TUTTI I PRODOTTI CHA HANNO UNA QUANTITA SOPRA LA SOGLIA MENOMILLE
                // [ordini, ordiniD, ordiniBD, ordiniB95, ordiniBS, ordiniAlpino, ordiniBluAlpino, peso]
                OrdiniMenoMilleClass ordiniMenoMilleClass = new OrdiniMenoMilleClass();
                double[] MENOMILLE = idSettings.MENOMILLE;
                OrdiniMenoMilleStruct ordiniMenoMille = ordiniMenoMilleClass.OrdiniMenoMille(n_OrdiniOriginali, MENOMILLE[t], OrdiniOriginali, OrdiniOriginaliD, OrdiniOriginaliBD, OrdiniOriginaliB95, OrdiniOriginaliBS, OrdiniOriginaliAlpino, OrdiniOriginaliBluAlpino, settings.dens_D, 
                    settings.dens_BD, settings.dens_BS, settings.dens_B95);

                //ASSEGNO PRIORITA'
                // sommo l'andata e il ritorno di un pv dalla base, e prendo quello che ha valore maggiore
                // [p, Valore, od_dep_media]
                AssegnazionePrioritaClass assegnazionePrioritaClass = new AssegnazionePrioritaClass();
                AssegnazionePrioritaStruct priorita = assegnazionePrioritaClass.AssegnazionePriorita(distanze.od_dep_pv, distanze.od_pv_dep, distanze.od_pv_pv, n_OrdiniOriginali, peso, maxcap, ordiniMenoMille.ordini, settings.esponente, 
                    settings.ELLISSE, settings.beta, distanze.preferenza_pv_pv, settings.DISTANZA_MAX_PVPV);

                //ORDINAMENTO DEI PV
                // ordinamento dal più distante
                // ordino in maniera decrescente per il valore della prima colonna
                // [MioOrdine_ord, ordini_ord, pv_ord, valore_ord, od_pv_pv_ord, od_dep_pv_ord, od_pv_dep_ord, ordinipeso_ord, ordini_piumeno_ord, max_product_ord, ordiniD_ord, ordiniBD_ord, ordiniB95_ord, ordiniBS_ord, ordiniAlpino_ord, ordiniBluAlpino_ord, ordinati_ord]
                OrdinamentoPVClass ordinamentoPVClass = new OrdinamentoPVClass();
                OrdinamentoPVStruct ordinamentoPV = ordinamentoPVClass.OrdinamentoPV(priorita.od_dep_media, ordiniStruct.pv, ordiniMenoMille.ordini, priorita.Valore, distanze.od_pv_pv, n_OrdiniOriginali, distanze.od_dep_pv, distanze.od_pv_dep, ordiniStruct.ordinipiumeno,
                    prodottomax, peso, ordiniStruct.ordiniD, ordiniStruct.ordiniBD, ordiniStruct.ordiniB95, ordiniStruct.ordiniBS, ordiniStruct.ordiniAlpino, ordiniStruct.ordiniBluAlpino, ordiniStruct.MioOrdine, ordinati);

                // ALGORITMO
                /*conn.ConnectionString =
                    "Server=LAPTOP-DT8KB2TQ;" +
                    "Database=Matlab;" +
                    "Integrated Security=True";*/
                // [MioOrdineViaggio, targheViaggi, IdM, targa, n_viaggio, scartato, sequenza, giacenza_stored, giacenzapeso, giacenzapeso_stored, viaggio_temp, lun, da_servire, tempo_temp, tempo, viaggio, ordiniD_ord, ordiniBD_ord, ordiniB95_ord, ordiniBS_ord, ordiniAlpino_ord, ordiniBluAlpino_ord]
                CalcoloViaggiClass calcoloViaggiClass = new CalcoloViaggiClass();
                CalcoloViaggiStruct calcoloViaggi = calcoloViaggiClass.CalcoloViaggi(disponibilitaMezzi.IdM, targaOriginale, distanze.od_pv_pv_completa, n_OrdiniOriginali, ordinamentoPV.pv_ord, baseCarico, ordinamentoPV.ordinipeso_ord, settings.CARICA, settings.SCARICA, settings.SCARICALITRO, ordinamentoPV.max_product_ord,
                    ordinamentoPV.od_dep_pv_ord, ordinamentoPV.od_pv_dep_ord, ordinamentoPV.od_pv_pv_ord, settings.MINxKM, settings.TEMPO_MAX, ordinamentoPV.valore_ord, ordinamentoPV.ordini_ord, ordinamentoPV.ordiniD_ord, ordinamentoPV.ordiniB95_ord, ordinamentoPV.ordiniBD_ord, ordinamentoPV.ordiniBS_ord, 
                    ordinamentoPV.ordiniAlpino_ord, ordinamentoPV.ordiniBluAlpino_ord, ordinamentoPV.ordini_piumeno_ord, ordinamentoPV.MioOrdine_ord, settings.GIACENZA_MIN, settings.KM_MIN, ordinamentoMezzi.capmax, ordinamentoMezzi.capton, settings.dens_D, settings.dens_B95, settings.dens_BD, settings.dens_BS, 
                    settings.MAXDROP, MENOMILLE[t], conn);
                //close(conn);

                // Inserisci nel db
                // TargheTempo = TEMPO_MAX * ones(length(IdM), 1);
                /*conn.ConnectionString =
                    "Server=LAPTOP-DT8KB2TQ;" +
                    "Database=Matlab;" +
                    "Integrated Security=True";*/
                // [IdRunner] = CreateRunner('sacile', baseCarico, data, conn);
                CreateVersionClass createVersionClass = new CreateVersionClass();
                int IdVersione = createVersionClass.CreateVersion(baseCarico, "sacile", data, idSettings.Id[t], IdRunner, conn);
                // close(conn)

                int ii = 1;
                int n_viaggio = (int)calcoloViaggi.n_viaggio;
                while (ii <= n_viaggio)
                {
                    /*conn.ConnectionString =
                    "Server=LAPTOP-DT8KB2TQ;" +
                    "Database=Matlab;" +
                    "Integrated Security=True";*/
                    double[] capton = ordinamentoMezzi.capton;
                    double[] targheViaggi = calcoloViaggi.TargheViaggi;

                    List<short> tempo = calcoloViaggi.tempo;
                    List<double> lun = calcoloViaggi.lun;
                    int[] IdM = calcoloViaggi.IdM;
                    int IdViaggio;
                    if (ii <= capton.Length && targheViaggi[ii] != -1)
                    {
                        CreateViaggioClass createViaggioClass = new CreateViaggioClass();
                        IdViaggio = createViaggioClass.CreateViaggio(IdVersione, data, lun[ii], tempo[ii], IdM[(int)targheViaggi[ii]], conn);
                    }
                    else
                    {
                        CreateViaggioNoMezzoClass createViaggioNoMezzoClass = new CreateViaggioNoMezzoClass();
                        IdViaggio = createViaggioNoMezzoClass.CreateViaggioNoMezzo(IdVersione, data, lun[ii], tempo[ii], -1, conn);
                    }
                    //close(conn);

                    List<List<double>> viaggio = calcoloViaggi.viaggio;
                    for(int j = 0; j < viaggio[ii].Capacity; j++)
                    {
                        if(viaggio[ii][j] != 0)
                        {
                            List<int> posizione = new List<int>();
                            for(int l=0;l<ordinamentoPV.pv_ord.Length;l++)
                            {
                                if (ordinamentoPV.pv_ord[l] == viaggio[ii][j] && ordinamentoPV.MioOrdine_ord[l] == calcoloViaggi.MioOrdineViaggio[ii][j])
                                    posizione[l] = l;
                                    
                            }
                            string Quantita;
                            for (int k = 0; k < posizione.Capacity; k++)
                            {
                                conn.ConnectionString =
                                "Server=LAPTOP-DT8KB2TQ;" +
                                "Database=Matlab;" +
                                "Integrated Security=True";
                                Quantita = ordinamentoPV.ordiniD_ord[posizione[k]].ToString() + "," +
                                   ordinamentoPV.ordiniB95_ord[posizione[k]].ToString() + "," +
                                   ordinamentoPV.ordiniBD_ord[posizione[k]].ToString() + "," +
                                   ordinamentoPV.ordiniBS_ord[posizione[k]].ToString() + "," +
                                   ordinamentoPV.ordiniAlpino_ord[posizione[k]].ToString() + "," +
                                   ordinamentoPV.ordiniBluAlpino_ord[posizione[k]].ToString();
                                RiempiViaggioClass riempiViaggio = new RiempiViaggioClass();
                                riempiViaggio.RiempiViaggio(IdViaggio, (int)viaggio[ii][j], j, Quantita, conn);
                                conn.Close();
                            }
                        }
                    }
                    ii++;
                }
                conn.Close();
                conn.ConnectionString =
                "Server=LAPTOP-DT8KB2TQ;" +
                "Database=Matlab;" +
                "Integrated Security=True";
                CambiaNotificaClass cambiaNotifica = new CambiaNotificaClass();
                cambiaNotifica.CambiaNotifica(IdRunner, IdVersione, conn);
                conn.Close();
                Console.WriteLine("Fine");
            }
        }
    }
}