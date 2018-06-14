using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRPAutobotti
{

    public struct OrdinamentoMezziStruct
    {
        public double[,] capton;
        public double[,] capmax;
        public int[,] targa;
    }
    class OrdinamentoMezziClass
    {
        public OrdinamentoMezziStruct omStruct;
        public OrdinamentoMezziClass()
        {
            omStruct = new OrdinamentoMezziStruct();
        }

        public OrdinamentoMezziStruct OrdinamentoMezzi(double[] captontemp, double[] capmaxtemp, int[] targatemp)
        {
            int[] indexes = Enumerable.Range(0, captontemp.Length).ToArray();//creo un array di indici ordinati
            Array.Sort(captontemp, indexes);//ordino captontemp e allo stesso tempo ordino l'array di indici allo stesso modo
            // ordino anche gli altri due array secondo gli indici contenuti in indexes

            // l'ordine risultate sarà quello crescente
            capmaxtemp = indexes.Select(index => capmaxtemp[index]).ToArray();
            targatemp = indexes.Select(index => targatemp[index]).ToArray();

            //ottengo l'ordine decrescente invertendo gli array
            double[] captontempDecr = captontemp;
            double[] capmaxtempDecr = capmaxtemp;
            int[] targatempDecr = targatemp;
            Array.Reverse(captontempDecr);
            Array.Reverse(capmaxtempDecr);
            Array.Reverse(targatempDecr);

            double[,] capton = new double[captontemp.Length,3];
            double[,] capmax = new double[captontemp.Length, 3];
            int[,] targa = new int[captontemp.Length, 3];


            // riempo le matrici capton, capmax e targa con i rispettivi vettori crescenti e decrescenti
            for (int i = 0; i < captontemp.Length; i++)
            {
                capton[i, 0] = captontempDecr[i];
                capton[i, 1] = captontemp[i];
                capton[i, 2] = captontempDecr[i];

                capmax[i, 0] = capmaxtempDecr[i];
                capmax[i, 1] = capmaxtemp[i];
                capmax[i, 2] = capmaxtempDecr[i];

                targa[i, 0] = targatempDecr[i];
                targa[i, 1] = targatemp[i];
                targa[i, 2] = targatempDecr[i];
            }

            omStruct.capton = capton;
            omStruct.capmax = capmax;
            omStruct.targa = targa;
            return omStruct;
        }
    }
}
