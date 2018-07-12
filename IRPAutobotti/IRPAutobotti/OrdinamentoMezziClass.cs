using System;
using System.Linq;

namespace IRPAutobotti
{

    public struct OrdinamentoMezziStruct
    {
        public double[] capton;
        public double[] capmax;
        public double[] targa;
    }
    class OrdinamentoMezziClass
    {
        public OrdinamentoMezziStruct omStruct;

        public OrdinamentoMezziClass()
        {
            omStruct = new OrdinamentoMezziStruct();
        }

        public OrdinamentoMezziStruct OrdinamentoMezzi(double[] captontemp, double[] capmaxtemp, double[] targatemp)
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
            double[] targatempDecr = targatemp;
            Array.Reverse(captontempDecr);
            Array.Reverse(capmaxtempDecr);
            Array.Reverse(targatempDecr);

            double[] capton = new double[captontempDecr.Length + captontemp.Length + captontempDecr.Length];
            double[] capmax = new double[capmaxtempDecr.Length + capmaxtemp.Length + capmaxtempDecr.Length];
            double[] targa = new double[targatempDecr.Length + targatemp.Length + targatempDecr.Length];


            // riempo le matrici capton, capmax e targa con i rispettivi vettori crescenti e decrescenti
            captontempDecr.CopyTo(capton, 0);
            captontemp.CopyTo(capton, captontempDecr.Length);
            captontempDecr.CopyTo(capton, captontempDecr.Length + captontemp.Length);

            capmaxtempDecr.CopyTo(capmax, 0);
            capmaxtemp.CopyTo(capmax, capmaxtempDecr.Length);
            capmaxtempDecr.CopyTo(capmax, capmaxtempDecr.Length + capmaxtemp.Length);

            targatempDecr.CopyTo(targa, 0);
            targatemp.CopyTo(targa, targatempDecr.Length);
            targatempDecr.CopyTo(targa, targatempDecr.Length + targatemp.Length);

            omStruct.capton = capton;
            omStruct.capmax = capmax;
            omStruct.targa = targa;
            return omStruct;
        }
    }
}
