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

namespace InformationRetrieval
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

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
                //using (IRDBEntities IRDBE = new IRDBEntities())
                //{
                //    foreach (var doc in docName)
                //    {
                //        IRDBE.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Docs] ON");
                //        IRDBE.Docs.Add(new Doc() { name = doc.Value });
                //    }
                    
                //    IRDBE.SaveChanges();
                //    IRDBE.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Docs] OFF");
                //    IRDBE.Dispose();
                //}
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
            addToDB();           
            TF_IDF.addTF();

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
                        IRDBE.Dics.Add(new Dic() { Id = count, termID = t.Key, docID = elem.Key, freq = elem.Value });
                        count++;
                        if (count % 1000 == 0)
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
            Stemming2 s = new Stemming2();
            LinkedList<string> stemmedQ = s.porter2(qlist);
            List<Doc> documentList = new List<Doc>();                               //contains docs IDs that match the query
            var termList = (from t in IRDBE.Terms select t.term1).ToList<string>();
            foreach (string word in stemmedQ)
            {
                if (termList.Contains(word))
                {
                    var tID = (from t in IRDBE.Terms where t.term1 == word select t).FirstOrDefault<Term>();
                    documentList.AddRange((from di in IRDBE.Dics where di.termID == tID.Id select di.Doc).ToList<Doc>());
                }
            }
            resultBox.Text = "Your search result is :" + System.Environment.NewLine+ System.Environment.NewLine;

            foreach (var d in documentList)
            {
                var clickable = new LinkLabel();
                clickable.Text = d.name;
                clickable.ActiveLinkColor = System.Drawing.Color.Chartreuse;
                clickable.AutoSize = true;
                clickable.Enabled = true;
                clickable.LinkArea= new LinkArea(0, d.name.Length);
                //clickable.LinkArea.Length = d.name.Length;
                clickable.LinkClicked += clicked;
                resultBox.Text += clickable.Text + System.Environment.NewLine;
            }
            Dictionary<int, int> queryVec = new Dictionary<int, int>();
            foreach(string word in stemmedQ)
            {
                if (termList.Contains(word))
                {
                    var termid = (from t in IRDBE.Terms where t.term1 == word select t).First();
                    if (queryVec.ContainsKey(termid.Id))
                    {
                        int i;
                        queryVec.TryGetValue(termid.Id, out i);
                        queryVec.Remove(termid.Id);
                        queryVec.Add(termid.Id, i + 1);
                    }
                    else
                        queryVec.Add(termid.Id, 1);
                }                
            }
            resultBox.Text += System.Environment.NewLine + "search result is Done."+ System.Environment.NewLine ;
            List<KeyValuePair<int, double>> result = new List<KeyValuePair<int, double>>();
            result = processQuery.cosinetheta_forAllDocs(queryVec, docList);
            int j = 0;
            while ((result.Any()) && (j < 10))
            {
                resultBox.Text += result.Last().Key + "   " + result.Last().Value + System.Environment.NewLine;
                result.Remove(result.Last());
                j++;
            }
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
    }
}
