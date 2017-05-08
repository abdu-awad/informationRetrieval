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
        SqlConnection conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\abdua\Documents\Visual Studio 2015\Projects\InformationRetrieval\InformationRetrieval\bin\Debug\Database4.mdf;Integrated Security=True");
        SqlCommand cmd = new SqlCommand();
        SqlDataReader dr;
        
        

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
                foreach(string file in System.IO.Directory.GetFiles(target))
                {
                    conn.Open();
                    cmd.CommandText = "select id from Docs where link='" + file + "'";
                    string id = cmd.ExecuteScalar().ToString();
                    cmd.Dispose();
                    LinkedList<string> words = new LinkedList<string>(Stopwords.RemoveStopwords(file));
                    while (words.First != null)
                    {
                        cmd.CommandText = "insert into Terms (term) value ('" + words.First.ToString() + "')";

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
    }
}
