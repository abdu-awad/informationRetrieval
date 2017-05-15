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
            //IRDBE.Database.Connection.Open();
        }


        string target = System.IO.Directory.GetCurrentDirectory() + "\\Docs";
        IRDBEntities IRDBE = new IRDBEntities();        
        Dictionary<string, int> words = new Dictionary<string, int>();
        

        private void addFiles_button_Click(object sender, EventArgs e)
        {
            
            if (addFiles_dialog.ShowDialog()==DialogResult.OK)
            {
                if(!System.IO.Directory.Exists(target))
                {
                    System.IO.Directory.CreateDirectory(target);
                    MessageBox.Show("target directory created");
                }
                foreach(string s in addFiles_dialog.FileNames)
                {
                    string fileName = System.IO.Path.GetFileName(s);
                    var result = (from q in IRDBE.Docs where q.name == fileName select q).FirstOrDefault<Doc>();
                    
                    if (result == null)
                    {
                        System.IO.File.Copy(s, target + "\\" + System.IO.Path.GetFileName(s), true);
                        IRDBE.Docs.Add(new Doc() { name = System.IO.Path.GetFileName(s) });
                        IRDBE.SaveChanges();
                    }
                    else
                        MessageBox.Show("document already exists");
                   
                }
                MessageBox.Show("Done Adding!");
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string querey = textBox1.Text;
            
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                
                MessageBox.Show("your search is: " + textBox1.Text);
            }
        }
        
        private void Search_Click(object sender, EventArgs e)
        {
            MessageBox.Show("your search is: " + textBox1.Text);
            processQuery(textBox1.Text);
        }

        private void index_button_Click(object sender, EventArgs e)
        {
            createIndex();
        }

        private void delete_files_button_Click(object sender, EventArgs e)
        {
            var DocList = IRDBE.Docs.ToList<Doc>();
            int j = 0;
            while (j<DocList.Count)
            {
                System.IO.File.Delete(target+"\\"+DocList.ElementAt<Doc>(j).name);
                IRDBE.Docs.Remove(DocList.ElementAt<Doc>(j));
                j++;
                IRDBE.SaveChanges();
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
                
                foreach (string file in System.IO.Directory.GetFiles(target))
                {
                    string fileName = System.IO.Path.GetFileName(file);
                    words.Clear();
                    var document = (from q in IRDBE.Docs where q.name == fileName select q).FirstOrDefault();
                    System.IO.StreamReader sr = new System.IO.StreamReader(file);
                    LinkedList<string> beforeStem = new LinkedList<string>(Stopwords.RemoveStopwords(sr.ReadToEnd()));
                    Stemming2 s = new Stemming2();
                    LinkedList<string> w = s.porter2(beforeStem);
                    while (w.First != null)
                    {
                        try
                        {
                            words.Add(w.First.Value, 1);

                        }
                        catch (Exception)
                        {
                            int max = 1;
                            words.TryGetValue(w.First.Value, out max);
                            words.Remove(w.First.Value);
                            words.Add(w.First.Value, max + 1);
                        }
                        w.RemoveFirst();
                    }
                    addTerms(words,Convert.ToInt32(document.Id));
                }                
            }

            MessageBox.Show("ok");
        }


        void addTerms(Dictionary<string,int> words,int id)
        {
            foreach(KeyValuePair<string,int> kvp in words)
            {
                var term = (from q in IRDBE.Terms where q.term1 ==kvp.Key select q).FirstOrDefault();
                
                if (term != null)
                {
                    term.DocNum++;
                    term.totFreq += kvp.Value;
                }
                else
                {
                    IRDBE.Terms.Add(new Term() { term1 = kvp.Key, DocNum = 1, totFreq = kvp.Value });
                    IRDBE.SaveChanges();
                    var termid = (from q in IRDBE.Terms where q.term1 == kvp.Key select q).First<Term>();
                    IRDBE.Dics.Add(new Dic() { termID = termid.Id, docID = id, freq = kvp.Value, weight = 0});
                }
                IRDBE.SaveChanges();
            }
        }

        void processQuery(string q)
        {
            LinkedList<string> qlist = new LinkedList<string>();
            qlist.AddFirst(q);
            Stemming2 s = new Stemming2();
            LinkedList<string> stemmedQ= s.porter2(qlist);
            foreach(string word in stemmedQ)
            {

            }
        }
    }
}
