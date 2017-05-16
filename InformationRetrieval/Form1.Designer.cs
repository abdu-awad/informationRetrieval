namespace InformationRetrieval
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.addFiles_button = new System.Windows.Forms.Button();
            this.addFiles_dialog = new System.Windows.Forms.OpenFileDialog();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.index_button = new System.Windows.Forms.Button();
            this.Search = new System.Windows.Forms.Button();
            this.delete_files_button = new System.Windows.Forms.Button();
            this.Clear_index_button = new System.Windows.Forms.Button();
            this.resultBox = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // addFiles_button
            // 
            this.addFiles_button.Location = new System.Drawing.Point(96, 135);
            this.addFiles_button.Name = "addFiles_button";
            this.addFiles_button.Size = new System.Drawing.Size(139, 52);
            this.addFiles_button.TabIndex = 5;
            this.addFiles_button.Text = "Add New Files";
            this.addFiles_button.UseVisualStyleBackColor = true;
            this.addFiles_button.Click += new System.EventHandler(this.addFiles_button_Click);
            // 
            // addFiles_dialog
            // 
            this.addFiles_dialog.FileName = "addFiles_dialog";
            // 
            // textBox1
            // 
            this.textBox1.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.textBox1.Location = new System.Drawing.Point(271, 12);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(344, 20);
            this.textBox1.TabIndex = 1;
            this.textBox1.Text = "enter your search here";
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            this.textBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
            // 
            // index_button
            // 
            this.index_button.Location = new System.Drawing.Point(96, 194);
            this.index_button.Name = "index_button";
            this.index_button.Size = new System.Drawing.Size(139, 52);
            this.index_button.TabIndex = 6;
            this.index_button.Text = "Index";
            this.index_button.UseVisualStyleBackColor = true;
            this.index_button.Click += new System.EventHandler(this.index_button_Click);
            // 
            // Search
            // 
            this.Search.Location = new System.Drawing.Point(370, 51);
            this.Search.Name = "Search";
            this.Search.Size = new System.Drawing.Size(154, 30);
            this.Search.TabIndex = 2;
            this.Search.Text = "Search";
            this.Search.UseVisualStyleBackColor = true;
            this.Search.Click += new System.EventHandler(this.Search_Click);
            // 
            // delete_files_button
            // 
            this.delete_files_button.Location = new System.Drawing.Point(96, 79);
            this.delete_files_button.Name = "delete_files_button";
            this.delete_files_button.Size = new System.Drawing.Size(138, 48);
            this.delete_files_button.TabIndex = 4;
            this.delete_files_button.Text = "Delete All Files";
            this.delete_files_button.UseVisualStyleBackColor = true;
            this.delete_files_button.Click += new System.EventHandler(this.delete_files_button_Click);
            // 
            // Clear_index_button
            // 
            this.Clear_index_button.Location = new System.Drawing.Point(96, 253);
            this.Clear_index_button.Name = "Clear_index_button";
            this.Clear_index_button.Size = new System.Drawing.Size(139, 46);
            this.Clear_index_button.TabIndex = 7;
            this.Clear_index_button.Text = "Clear Index";
            this.Clear_index_button.UseVisualStyleBackColor = true;
            // 
            // resultBox
            // 
            this.resultBox.BackColor = System.Drawing.Color.White;
            this.resultBox.Location = new System.Drawing.Point(254, 98);
            this.resultBox.Name = "resultBox";
            this.resultBox.ReadOnly = true;
            this.resultBox.Size = new System.Drawing.Size(386, 254);
            this.resultBox.TabIndex = 8;
            this.resultBox.Text = "";
            this.resultBox.TextChanged += new System.EventHandler(this.resultBox_TextChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(662, 373);
            this.Controls.Add(this.resultBox);
            this.Controls.Add(this.Clear_index_button);
            this.Controls.Add(this.delete_files_button);
            this.Controls.Add(this.Search);
            this.Controls.Add(this.index_button);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.addFiles_button);
            this.Name = "Form1";
            this.Text = "Search";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button addFiles_button;
        private System.Windows.Forms.OpenFileDialog addFiles_dialog;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button index_button;
        private System.Windows.Forms.Button Search;
        private System.Windows.Forms.Button delete_files_button;
        private System.Windows.Forms.Button Clear_index_button;
        private System.Windows.Forms.RichTextBox resultBox;
    }
}

