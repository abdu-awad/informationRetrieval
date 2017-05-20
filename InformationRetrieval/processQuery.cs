using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformationRetrieval
{
    static class processQuery
    {

        public static double vectorlength(Dictionary<int, int> vector)
        {
            double length = 0.0;
            foreach (KeyValuePair<int,int> elem in vector)
            {
                length += Math.Pow(elem.Value, 2);
            }

            return Math.Sqrt(length);
        }

        public static double dotproduct(Dictionary<int, int> queryVec, Dictionary<int, int> docVec)
        {
            double product = 0.0;
            foreach (KeyValuePair<int,int> elem in queryVec)
            {
                if(docVec.ContainsKey(elem.Key))
                {
                    int val;
                    docVec.TryGetValue(elem.Key,out val);
                    product += elem.Value * val;
                }
            }
            return product;
        }


        public static double cosinetheta(Dictionary<int,int> queryVec,Dictionary<int,int> docVec)
        {
            double lengthV1 = vectorlength(queryVec);
            double lengthV2 = vectorlength(docVec);

            double dotprod = dotproduct(queryVec,docVec);

            return dotprod / (lengthV1 * lengthV2);

        }


        
        public static List<KeyValuePair<int, double>> cosinetheta_forAllDocs(Dictionary<int, int> queryVec, Dictionary<int, Dictionary<int, int>> docVectors)
        {
            
            Dictionary<int, double> Cosine_DocID = new Dictionary<int, double>();
            Double Cosine;
            foreach(KeyValuePair<int,Dictionary<int,int>> docvec in docVectors)
            {
                Cosine = cosinetheta(queryVec, docvec.Value);
                Cosine_DocID.Add(docvec.Key, Cosine);

            }
            var result = Cosine_DocID.ToList();
            result.Sort((x, y) => x.Value.CompareTo(y.Value));            
            return result;
        }
    }


}


