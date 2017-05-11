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
            cmd.Connection = conn;
        }
        string target = System.IO.Directory.GetCurrentDirectory() + "\\Docs";
        Database4Entities dbe = new Database4Entities();
        SqlConnection conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\abdua\Source\Repos\informationRetrieval\InformationRetrieval\bin\Debug\Database4.mdf;Integrated Security=True");
        SqlCommand cmd = new SqlCommand();
        SqlDataReader dr;
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
                    bool exist = true;
                    System.IO.File.Copy(s, target+"\\"+System.IO.Path.GetFileName(s), true);
                    conn.Open();
                    cmd.CommandText = "select id from Docs where link = '" + target + "\\" + System.IO.Path.GetFileName(s) + "'";
                    try
                    {
                        cmd.ExecuteScalar().ToString();
                    }
                    catch (System.NullReferenceException)
                    {
                        cmd.CommandText = "insert into Docs (link) values ('" + target + "\\" + System.IO.Path.GetFileName(s) + "')";
                        cmd.ExecuteNonQuery();
                        exist = false;
                    }
                    if (exist)
                    {
                        MessageBox.Show("document already exists");
                    }
                    conn.Close();
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
        }

        private void index_button_Click(object sender, EventArgs e)
        {
            createIndex();
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
                conn.Open();
                foreach (string file in System.IO.Directory.GetFiles(target))
                {

                    words.Clear();
                    cmd.CommandText = "select id from Docs where link='" + file + "'";
                    int id = Convert.ToInt32(cmd.ExecuteScalar().ToString());
                    cmd.Dispose();
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

                    addTerms(words, id);
                }
                conn.Close();
            }
                
            MessageBox.Show("ok");
        }

        private void delete_files_button_Click(object sender, EventArgs e)
        {
            foreach(string file in System.IO.Directory.GetFiles(target))
            {
                System.IO.File.Delete(file);                
                conn.Open();
                cmd.CommandText = "delete from Docs where link='" + file + "'";
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                conn.Close();                
            }
            MessageBox.Show("Done Deleting");
        }

        void addTerms(Dictionary<string,int> words,int id)
        {
            foreach(KeyValuePair<string,int> kvp in words)
            {
                cmd.CommandText = "select id from Terms where term = '" + kvp.Key + "'";
                if (cmd.ExecuteNonQuery() > 0)
                {
                    string termID = cmd.ExecuteScalar().ToString();
                    cmd.Dispose();
                    cmd.CommandText = "select numberOfDocuments from Terms where id = "+termID;
                    int docNum = Convert.ToInt32(cmd.ExecuteScalar())+1;
                    cmd.Dispose();
                    cmd.CommandText = "select totalFreq from Terms where id = " + termID;
                    int freq = Convert.ToInt32(cmd.ExecuteScalar())+kvp.Value;
                    cmd.Dispose();
                    cmd.CommandText = "update Terms set numberOfDocuments= " + docNum + " , totalFreq = " + freq + " where id = " + id;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    cmd.CommandText = "insert into index (term_id,doc_id,doc_freq) values ("+termID+","+id+","+kvp.Value+")";
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
                else
                {
                    cmd.CommandText = "insert into Terms (term,numberOfDocuments,totalFreq) values ('" + kvp.Key + "'," + 1 + "," + kvp.Value + ")";
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    cmd.CommandText = "select id from Terms where term = '" + kvp.Key + "'";
                    string termID = cmd.ExecuteScalar().ToString();
                    cmd.Dispose();
                    cmd.CommandText = "insert into [Index] (term_ID,Doc_ID,Doc_Freq,weight) values (" + termID + "," + id + "," + kvp.Value + ","+0+")";
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
            }
        }
    }
}
