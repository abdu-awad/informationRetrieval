using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformationRetrieval
{
    static class TF_IDF
    {

        public static Dictionary<string, double> addTF(Dictionary<int,string> docName,Dictionary<int,string> termName , Dictionary<int,Dictionary<int,int>> docList , Dictionary<int,Dictionary<int,int>> termList)
        {
            Dictionary<string, double> weight = new Dictionary<string, double>();
            foreach(var t in termList)
            {
                foreach(var d in t.Value)
                {
                    weight.Add(t.Key + "-" + d.Key, 0);
                    double TF = (double)d.Value / docList[d.Key].Count;
                    double IDF = Math.Log((double)docName.Count / t.Value.Count);
                    weight[t.Key + "-" + d.Key] = TF * IDF;
                }
            }
            return weight;               
        }
    }
}
