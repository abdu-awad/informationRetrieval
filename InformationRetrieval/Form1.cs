﻿using System;
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

        }


        string target = System.IO.Directory.GetCurrentDirectory() + "\\Docs";
        IRDBEntities IRDBE = new IRDBEntities();
        Dictionary<string, int> words = new Dictionary<string, int>();


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
            if (e.KeyCode == Keys.Enter)
            {
                Search_Click(sender, e);
                //MessageBox.Show("your search is: " + textBox1.Text);
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
            IRDBE.Dics.RemoveRange(IRDBE.Dics);
            IRDBE.Terms.RemoveRange(IRDBE.Terms);
            IRDBE.Docs.RemoveRange(IRDBE.Docs);
            IRDBE.SaveChanges();
            foreach(string file in System.IO.Directory.GetFiles(target))
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
                    addTerms(words, Convert.ToInt32(document.Id));
                }
            }

            cosineSim();

            MessageBox.Show("ok");
        }


        void addTerms(Dictionary<string, int> words, int id)
        {
            foreach (KeyValuePair<string, int> kvp in words)
            {
                var term = (from q in IRDBE.Terms where q.term1 == kvp.Key select q).FirstOrDefault();

                if (term != null)
                {
                    term.DocNum++;
                    term.totFreq += kvp.Value;
                    IRDBE.Dics.Add(new Dic() { termID = term.Id, docID = id, freq = kvp.Value, weight = 0 });
                }
                else
                {
                    IRDBE.Terms.Add(new Term() { term1 = kvp.Key, DocNum = 1, totFreq = kvp.Value });
                    IRDBE.SaveChanges();
                    var termid = (from q in IRDBE.Terms where q.term1 == kvp.Key select q).First<Term>();
                    IRDBE.Dics.Add(new Dic() { termID = termid.Id, docID = id, freq = kvp.Value, weight = 0 });
                }
                IRDBE.SaveChanges();
            }
        }

        void cosineSim()
        {
            
           
            Dictionary<int, Dictionary<int, int>> docList = new Dictionary<int, Dictionary<int, int>>();

            var dl = (from d in IRDBE.Docs select d.Id).ToList<int>();
            foreach(int doc_id in dl)
            {
                Dictionary<int, int> docVector = new Dictionary<int, int>();
                var dicList = (from di in IRDBE.Dics where di.docID == doc_id select di).ToList<Dic>();
                foreach(Dic dicItem in dicList)
                {
                    docVector.Add(Convert.ToInt32(dicItem.termID), Convert.ToInt32(dicItem.freq));
                }
                docList.Add(doc_id, docVector);
                
            }                        
        }

        void queryCosine()
        {

        }

        void processQuery(string q)
        {
            LinkedList<string> qlist = new LinkedList<string>(Stopwords.RemoveStopwords(q));
            Stemming2 s = new Stemming2();
            LinkedList<string> stemmedQ = s.porter2(qlist);
            List<Doc> docList = new List<Doc>();
            var termList = (from t in IRDBE.Terms select t.term1).ToList<string>();
            foreach (string word in stemmedQ)
            {
                if (termList.Contains(word))
                {
                    var tID = (from t in IRDBE.Terms where t.term1 == word select t).FirstOrDefault<Term>();
                    docList.AddRange((from di in IRDBE.Dics where di.termID == tID.Id select di.Doc).ToList<Doc>());
                }
            }
            resultBox.Text = "Your search result is :" + System.Environment.NewLine+ System.Environment.NewLine;

            foreach (var d in docList)
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
            resultBox.Text += System.Environment.NewLine + "search result is Done.";        }

        void clicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("notepad.txt", target + "\\" + e.Link);
        }
        private void resultBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
