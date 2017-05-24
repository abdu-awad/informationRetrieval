using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace InformationRetrieval
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            // intialise process            
            pProcess.StartInfo.FileName = "cmd.exe";
            pProcess.StartInfo.CreateNoWindow = true;
            pProcess.StartInfo.RedirectStandardInput = true;
            pProcess.StartInfo.RedirectStandardOutput = true;
            pProcess.StartInfo.UseShellExecute = false;            
            
            addFiles_dialog.Multiselect = true;
            addFiles_dialog.Filter = "text files (*.txt)|*.txt|word files (*.doc,*.docx)|*.doc,*.docx|All files (*.*)|*.*";
            foreach(var elem in IRDBE.Docs)
            {
                docName.Add(elem.Id, elem.name);
            }
            foreach(var elem in IRDBE.Terms)
            {
                termName.Add(elem.Id, elem.term1);                
            }
            foreach(var doc in docName)
            {
                var tlist = (from t in IRDBE.Dics where t.docID == doc.Key select t).ToList<Dic>();
                Dictionary<int, int> termFreq = new Dictionary<int, int>();
                foreach(var d in tlist)
                {
                    termFreq.Add(Convert.ToInt16(d.termID),Convert.ToInt16(d.freq));
                    weight.Add(d.termID + "-" + doc.Key, (double)d.weight);
                    if (termList.ContainsKey((int)d.termID))
                    {
                        termList[(int)d.termID].Add(doc.Key, (int)d.freq);
                    }
                    else
                    {
                        termList.Add((int)d.termID, new Dictionary<int, int>());
                        termList[(int)d.termID].Add(doc.Key, (int)d.freq);
                    }
                }
                docList.Add(doc.Key, termFreq);
            }
                                   
            IRDBE.Configuration.AutoDetectChangesEnabled = false;
            IRDBE.Configuration.ValidateOnSaveEnabled = false;
            //buildVector();
        }

        
        string target = System.IO.Directory.GetCurrentDirectory() + "\\Docs";
        IRDBEntities IRDBE = new IRDBEntities();
        Dictionary<string, int> words = new Dictionary<string, int>();        
        Dictionary<int, string> termName = new Dictionary<int, string>();
        Dictionary<int, string> docName = new Dictionary<int, string>();
        Dictionary<int, Dictionary<int, int>> docList = new Dictionary<int, Dictionary<int, int>>();
        Dictionary<int, Dictionary<int, int>> termList = new Dictionary<int, Dictionary<int, int>>();
        Dictionary<string, double> weight = new Dictionary<string, double>();
        Process pProcess = new Process();
        List<LinkLabel> allLinks = new List<LinkLabel>();



        private void addFiles_button_Click(object sender, EventArgs e)
        {

            if (addFiles_dialog.ShowDialog() == DialogResult.OK)
            {
                if (!System.IO.Directory.Exists(target))
                {
                    System.IO.Directory.CreateDirectory(target);
                    MessageBox.Show("target directory created");
                }
                foreach (string s in addFiles_dialog.FileNames)
                {
                    if (s.Contains(".ALL"))
                    {
                        allToTxt(s);
                        foreach (string file in System.IO.Directory.GetFiles(target))
                        {
                            string fileName = System.IO.Path.GetFileName(file);
                            if (docName.ContainsValue(fileName))
                            {
                                MessageBox.Show("document already exists");
                            }
                            else
                            {
                                docName.Add(docName.Count + 1, fileName);
                            }
                        }                            
                    }
                    else
                    {
                        string fileName = System.IO.Path.GetFileName(s);
                        if (docName.ContainsValue(fileName))
                        {
                            MessageBox.Show("document already exists");
                        }
                        else
                        {
                            docName.Add(docName.Count + 1, fileName);
                            System.IO.File.Copy(s, target + "\\" + System.IO.Path.GetFileName(s), true);
                        }                        
                    }
                }
               
                MessageBox.Show("Done Adding!");
            }
        }        

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Search_Click(sender, e);                
            }
        }

        private void Search_Click(object sender, EventArgs e)
        {
            processQuery_method(textBox1.Text);
        }

        private void index_button_Click(object sender, EventArgs e)
        {
            createIndex();
        }

        private void delete_files_button_Click(object sender, EventArgs e)
        {
            using (IRDBEntities IRDBE = new IRDBEntities())
            {
                IRDBE.Database.ExecuteSqlCommand("TRUNCATE table Dic");
                IRDBE.Database.ExecuteSqlCommand("Alter table Dic drop constraint FK_doc");
                IRDBE.Database.ExecuteSqlCommand("TRUNCATE table Docs");
                IRDBE.Database.ExecuteSqlCommand("Alter table Dic drop constraint FK_term");
                IRDBE.Database.ExecuteSqlCommand("TRUNCATE table Terms");
                IRDBE.Database.ExecuteSqlCommand("Alter table Dic add constraint FK_term FOREIGN KEY ([termID]) REFERENCES [dbo].[Terms] ([Id])");
                IRDBE.Database.ExecuteSqlCommand("Alter table Dic add constraint FK_doc FOREIGN KEY ([docID]) REFERENCES [dbo].[Docs] ([Id])");
                IRDBE.SaveChanges();
            }            
            termList.Clear();
            termName.Clear();
            docName.Clear();
            docList.Clear();
            foreach (string file in System.IO.Directory.GetFiles(target))
            {
                System.IO.File.Delete(file);
            }
            

            MessageBox.Show("Done Deleting");
        }

        void createIndex()
        {
            if (!System.IO.Directory.Exists(target))
            {
                MessageBox.Show("there is no Docs directory");
            }
            else if (!System.IO.Directory.GetFiles(target).Any())
            {
                MessageBox.Show("there is no files in Docs directory");
            }
            else
            {
                IRDBE.Database.ExecuteSqlCommand("TRUNCATE table Dic");
                IRDBE.SaveChanges();
                foreach (string file in System.IO.Directory.GetFiles(target))
                {
                    string fileName = System.IO.Path.GetFileName(file);
                    words.Clear();                    
                    System.IO.StreamReader sr = new System.IO.StreamReader(file);
                    LinkedList<string> beforeStem = new LinkedList<string>(Stopwords.RemoveStopwords(sr.ReadToEnd()));
                    Stemming2 s = new Stemming2();
                    LinkedList<string> w = s.porter2(beforeStem);
                    while (w.First != null)
                    {

                        if (words.ContainsKey(w.First.Value))                        
                            words[w.First.Value]++;                        
                        else                        
                            words.Add(w.First.Value, 1);
                                                
                        w.RemoveFirst();
                    }                    
                    addTerms(words, docName.FirstOrDefault(x => x.Value == fileName).Key);
                }
            }
            weight = TF_IDF.addTF(docName, termName, docList, termList);
            addToDB();
            MessageBox.Show("ok");
        }


        void addTerms(Dictionary<string, int> words, int id)
        {
            docList.Add(id, new Dictionary<int, int>());
            foreach(KeyValuePair<string,int> kvp in words)
            {
                if (termName.ContainsValue(kvp.Key))
                {
                    int termID = termName.FirstOrDefault(x => x.Value == kvp.Key).Key;
                    termList[termID].Add(id, kvp.Value);
                    docList[id].Add(termID, kvp.Value);
                }
                else
                {
                    termName.Add(termName.Count + 1, kvp.Key.ToString());
                    termList.Add(termName.Count, new Dictionary<int, int>());
                    termList[termName.Count].Add(id, kvp.Value);
                    docList[id].Add(termName.Count, kvp.Value);
                }
            }
        }

        void addToDB()
        {
            using (IRDBE = new IRDBEntities())
            {
                IRDBE.Configuration.AutoDetectChangesEnabled = false;
                IRDBE.Configuration.ValidateOnSaveEnabled = false;
                IRDBE.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Terms] ON");
                IRDBE.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Dic] ON");
                int count = 0;
                foreach (var doc in docName)
                {
                    IRDBE.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Docs] ON");
                    IRDBE.Docs.Add(new Doc() { Id = doc.Key, name = doc.Value });
                    count++;
                }
                IRDBE.SaveChanges();
                foreach (var t in termName)
                {
                    var listOfDocs = new Dictionary<int, int>();
                    termList.TryGetValue(t.Key, out listOfDocs);
                    IRDBE.Terms.Add(new Term() { Id = t.Key, term1 = t.Value, DocNum = listOfDocs.Count, totFreq = listOfDocs.Values.Sum() });
                    foreach (var elem in listOfDocs)
                    {
                        IRDBE.Dics.Add(new Dic() { Id = count, termID = t.Key, docID = elem.Key, freq = elem.Value, weight = weight[t.Key + "-" + elem.Key] });
                        count++;
                        if (count % 500 == 0)
                        {
                            IRDBE.SaveChanges();
                            IRDBE.Dispose();
                            IRDBE = new IRDBEntities();
                            IRDBE.Configuration.AutoDetectChangesEnabled = false;
                            IRDBE.Configuration.ValidateOnSaveEnabled = false;
                            IRDBE.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Terms] ON");
                            IRDBE.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Dic] ON");
                        }
                    }
                }
            }
        }
               
        void processQuery_method(string q)
        {
            LinkedList<string> qlist = new LinkedList<string>(Stopwords.RemoveStopwords(q));
            Dictionary<int, List<int>> voca = new Dictionary<int, List<int>>();
            Stemming2 s = new Stemming2();
            LinkedList<string> stemmedQ = s.porter2(qlist);
            List<int> qeueDocs = new List<int>();
            Dictionary<int, int> queryVec = new Dictionary<int, int>();
            while (allLinks.Any())
            {
                allLinks.First().Dispose();
                allLinks.Remove(allLinks.First());
            }
            resultBox.Clear();
            TFBox.Clear();
            sumBox.Clear();
            foreach (string word in stemmedQ)
            {                
                if (termName.ContainsValue(word))
                {                    
                    int termID = termName.FirstOrDefault(t => t.Value == word).Key;
                    qeueDocs.AddRange(termList[termID].Keys.ToList<int>());
                    if (queryVec.ContainsKey(termID))
                        queryVec[termID]++;
                    else
                        queryVec.Add(termID, 1);
                    List<int> vocID = new List<int>();
                    foreach(var voc in s.porter2(getVocab(word)))
                    {
                        if (termName.ContainsValue(voc))
                            vocID.Add(termName.FirstOrDefault(t => t.Value == voc).Key);
                    }
                    voca.Add(termID, vocID);
                }
            }
           
            List<KeyValuePair<int, double>> result = new List<KeyValuePair<int, double>>();
            result = processQuery.cosinetheta_forAllDocs(queryVec, docList);
            int j = 0;
            List<int> docweight = new List<int>();
            Dictionary<int, double> cos = new Dictionary<int, double>();
            Dictionary<int, double> tfi = new Dictionary<int, double>();

            resultBox.Text += "top 20 with COSINE Value : " + System.Environment.NewLine + System.Environment.NewLine;
            while ((result.Any())&&(j<20))
            {
                if (result.Last().Value == 0)
                    break;
                LinkLabel link = new LinkLabel();
                link.Text = docName[result.Last().Key];
                link.LinkColor = System.Drawing.Color.DarkGray;
                link.LinkClicked += new LinkLabelLinkClickedEventHandler(link_LinkClicked);
                LinkLabel.Link data = new LinkLabel.Link();
                data.LinkData = result.Last().Key;
                link.Links.Add(data);
                link.AutoSize = true;
                link.Location = resultBox.GetPositionFromCharIndex(this.resultBox.TextLength);
                allLinks.Add(link);
                resultBox.Controls.Add(link);
                resultBox.AppendText(link.Text + "   ");
                resultBox.SelectionStart = this.resultBox.TextLength;
                resultBox.Text += "\t ==> \t" + result.Last().Value.ToString("f5") + System.Environment.NewLine;
                docweight.Add(result.Last().Key);
                cos.Add(result.Last().Key, result.Last().Value);
                result.Remove(result.Last());
                j++;
            }
            
            // ============== TF-IDF            
            foreach(var docid in docweight)
            {
                double tf=0;
                foreach(var t in voca)
                {   
                    if (weight.ContainsKey(t.Key + "-" + docid))
                        tf += weight[t.Key + "-" + docid];                    
                    foreach (var v in voca[t.Key])
                        if (weight.ContainsKey(t.Key + "-" + docid))
                            tf += weight[t.Key + "-" + docid]/2;                    
                }
                cos[docid] += tf;
                tfi.Add(docid, tf);               
            }
            var sortedTF = tfi.ToList();
            sortedTF.Sort((x, y) => x.Value.CompareTo(y.Value));
            sortedTF.Reverse();
            TFBox.Text += "Doc name \t TF-IDF Value" + System.Environment.NewLine;
            foreach(var elem in sortedTF)
            {
                TFBox.Text += System.Environment.NewLine + elem.Key + "\t ==> \t" + elem.Value.ToString("f5");
                TFBox.Text += System.Environment.NewLine + "contains : ";
                foreach (var t in voca)
                {                    
                    if (weight.ContainsKey(t.Key + "-" + elem.Key))
                        TFBox.Text += t.Key + "\t" + weight[t.Key + "-" + elem.Key].ToString("f5") + System.Environment.NewLine;                    
                }
            }            
            sumBox.Text += "Doc Name \t ranking(sum) " + System.Environment.NewLine + System.Environment.NewLine;            
            var sum = cos.ToList();
            sum.Sort((x, y) => x.Value.CompareTo(y.Value));
            sum.Reverse();
            foreach(var elem in sum)
            {
                LinkLabel link = new LinkLabel();
                link.Text = elem.Key.ToString();
                link.LinkClicked += new LinkLabelLinkClickedEventHandler(link_LinkClicked);
                LinkLabel.Link data = new LinkLabel.Link();
                data.LinkData = docName[elem.Key];
                link.Links.Add(data);
                link.AutoSize = true;
                link.Location = sumBox.GetPositionFromCharIndex(this.sumBox.TextLength);
                allLinks.Add(link);
                sumBox.Controls.Add(link);
                sumBox.AppendText(link.Text + "   ");
                sumBox.SelectionStart = this.sumBox.TextLength;

                sumBox.Text += "\t ==> \t" + elem.Value.ToString("f5") + System.Environment.NewLine;
            }
            
            
        }

        private void link_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {            
            pProcess.Start();

            pProcess.StandardInput.WriteLine("cd "+target);
            pProcess.StandardInput.Flush();
            string filename = e.Link.LinkData.ToString();            
            pProcess.StandardInput.WriteLine(filename);
            pProcess.StandardInput.Flush();
            pProcess.StandardInput.Close();
            pProcess.WaitForExit();
        }

        public LinkedList<String> get_senses(string newsubstring)
        {
            string[] lines = newsubstring.Replace("\r", "").Split('\n');
            int num_of_senses = Regex.Matches(newsubstring, "Sense").Count;
            string[] linesofSenses = new String[num_of_senses];


            // getting lines of seneses 
            int num_empty_line = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("Sense"))
                {
                    linesofSenses[num_empty_line] = lines[i + 1];
                    num_empty_line++;
                }
            }
            // getting senses
            string[] current_senses;
            LinkedList<String> allSenses = new LinkedList<string>();
            for (int i = 0; i < num_of_senses; i++)
            {
                current_senses = linesofSenses[i].Split(',');
                foreach (string s in current_senses)
                    allSenses.AddFirst(s);
            }
            return allSenses;
        }

        LinkedList<string> getVocab(string word)
        {
            //Create process for cmd 
            Process pProcess = new Process();
            pProcess.StartInfo.FileName = "cmd.exe";
            pProcess.StartInfo.CreateNoWindow = true;
            pProcess.StartInfo.RedirectStandardInput = true;
            pProcess.StartInfo.RedirectStandardOutput = true;
            pProcess.StartInfo.UseShellExecute = false;
            pProcess.Start();

            // run the wordnet and get the vocabularies
            pProcess.StandardInput.WriteLine("cd C:\\Program Files (x86) \\WordNet \\2.1 \\bin");
            pProcess.StandardInput.Flush();
            string wn = "wn ";            
            string synsn = " -synsn";
            pProcess.StandardInput.WriteLine(wn + word + synsn);
            pProcess.StandardInput.Flush();
            pProcess.StandardInput.Close();
            pProcess.WaitForExit();

            // spliting the vocabularies
            string strOutput = pProcess.StandardOutput.ReadToEnd();
            //MessageBox.Show(strOutput);
            LinkedList<String> all_senses = get_senses(strOutput);
            return all_senses;                           
        }

        void clicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("notepad.txt", target + "\\" + e.Link);
        }

        void allToTxt(string file)
        {
            System.IO.StreamReader sr = new System.IO.StreamReader(file);
            string all = sr.ReadToEnd();            
            var txts = all.Split(new char[] { '*' }, StringSplitOptions.RemoveEmptyEntries);
            foreach(string content in txts)
            {
                string cont = content.Replace("TEXT ", " ");
                int i = 1;
                string name = "";
                if (cont[0] != 'S')
                {
                    while (cont[i] != ' ')
                    {
                        name += cont[i];
                        i++;
                    }
                    System.IO.StreamWriter sw = new System.IO.StreamWriter(target + "\\" + name + ".txt");
                    sw.Write(cont);
                    sw.Flush();
                    sw.Dispose();
                }                
            }
        }

        private void Clear_index_button_Click(object sender, EventArgs e)
        {
            IRDBE.Database.ExecuteSqlCommand("TRUNCATE table Dic");
            termList.Clear();
            termName.Clear();
            IRDBE.SaveChanges();
        }

        private void resultBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            textBox1.SelectAll();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }
    }
}
