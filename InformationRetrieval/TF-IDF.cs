using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformationRetrieval
{
    static class TF_IDF
    {
        static IRDBEntities IRDBE = new IRDBEntities();
        public static double calculateTF(int termFreq, int docID)
        {   
            var termsInDoc = (from d in IRDBE.Dics where d.docID == docID select d).ToList<Dic>();
            int allTermsInDoc = termsInDoc.Count;

           double res = (double)allTermsInDoc / (double)termFreq;
            return res;
            
        }
        public static double calculateIDF(int termID, int docID)
        {
            var DocsInTerm = (from d in IRDBE.Dics where (d.termID == termID) select d).ToList<Dic>();
            int DocNum = DocsInTerm.Count;
            var totalDoc = (from d in IRDBE.Docs select d).ToList<Doc>();
            int total = totalDoc.Count;
            return Math.Log(total / DocNum);

        }
        public static double calculatTF_IDF(int termFreq, int termID, int docID)
        {
            double TF = calculateTF(termFreq, docID);
            double IDF = calculateIDF(termID, docID);
            return  TF*IDF ;
        }

        public static void addTF()
        {
            var tlist = (from te in IRDBE.Terms select te).ToList<Term>();
            var dlist = (from de in IRDBE.Docs select de).ToList<Doc>();
            foreach(var t in tlist)
            {
                foreach(var d in dlist)
                {
                    try
                    {
                        var termInDoc = (from di in IRDBE.Dics where (di.docID == d.Id) && (di.termID == t.Id) select di).First<Dic>();
                        double termWeight = calculatTF_IDF(Convert.ToInt16(termInDoc.freq), t.Id, d.Id);
                        termInDoc.weight = termWeight;
                        IRDBE.SaveChanges();
                    }
                    catch (Exception)
                    {
                        

                    }                    
                }
            }
        }
    }
}
