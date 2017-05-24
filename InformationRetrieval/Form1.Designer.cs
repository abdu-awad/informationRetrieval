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
            this.label1 = new System.Windows.Forms.Label();
            this.TFBox = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.sumBox = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // addFiles_button
            // 
            this.addFiles_button.Location = new System.Drawing.Point(115, 12);
            this.addFiles_button.Name = "addFiles_button";
            this.addFiles_button.Size = new System.Drawing.Size(97, 48);
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
            this.textBox1.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.textBox1.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.textBox1.Location = new System.Drawing.Point(510, 12);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(344, 20);
            this.textBox1.TabIndex = 1;
            this.textBox1.Text = "enter your search here";
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            this.textBox1.Enter += new System.EventHandler(this.textBox1_Enter);
            this.textBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
            // 
            // index_button
            // 
            this.index_button.Location = new System.Drawing.Point(218, 12);
            this.index_button.Name = "index_button";
            this.index_button.Size = new System.Drawing.Size(97, 48);
            this.index_button.TabIndex = 6;
            this.index_button.Text = "Index";
            this.index_button.UseVisualStyleBackColor = true;
            this.index_button.Click += new System.EventHandler(this.index_button_Click);
            // 
            // Search
            // 
            this.Search.Location = new System.Drawing.Point(613, 38);
            this.Search.Name = "Search";
            this.Search.Size = new System.Drawing.Size(154, 30);
            this.Search.TabIndex = 2;
            this.Search.Text = "Search";
            this.Search.UseVisualStyleBackColor = true;
            this.Search.Click += new System.EventHandler(this.Search_Click);
            // 
            // delete_files_button
            // 
            this.delete_files_button.Location = new System.Drawing.Point(12, 12);
            this.delete_files_button.Name = "delete_files_button";
            this.delete_files_button.Size = new System.Drawing.Size(97, 48);
            this.delete_files_button.TabIndex = 4;
            this.delete_files_button.Text = "Delete All Files";
            this.delete_files_button.UseVisualStyleBackColor = true;
            this.delete_files_button.Click += new System.EventHandler(this.delete_files_button_Click);
            // 
            // Clear_index_button
            // 
            this.Clear_index_button.Location = new System.Drawing.Point(321, 12);
            this.Clear_index_button.Name = "Clear_index_button";
            this.Clear_index_button.Size = new System.Drawing.Size(97, 48);
            this.Clear_index_button.TabIndex = 7;
            this.Clear_index_button.Text = "Clear Index";
            this.Clear_index_button.UseVisualStyleBackColor = true;
            this.Clear_index_button.Click += new System.EventHandler(this.Clear_index_button_Click);
            // 
            // resultBox
            // 
            this.resultBox.BackColor = System.Drawing.Color.White;
            this.resultBox.Location = new System.Drawing.Point(12, 96);
            this.resultBox.Name = "resultBox";
            this.resultBox.ReadOnly = true;
            this.resultBox.Size = new System.Drawing.Size(260, 311);
            this.resultBox.TabIndex = 8;
            this.resultBox.Text = "";
            this.resultBox.TextChanged += new System.EventHandler(this.resultBox_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 80);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Cosine Order :";
            // 
            // TFBox
            // 
            this.TFBox.BackColor = System.Drawing.Color.White;
            this.TFBox.Location = new System.Drawing.Point(294, 96);
            this.TFBox.Name = "TFBox";
            this.TFBox.ReadOnly = true;
            this.TFBox.Size = new System.Drawing.Size(284, 311);
            this.TFBox.TabIndex = 8;
            this.TFBox.Text = "";
            this.TFBox.TextChanged += new System.EventHandler(this.resultBox_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(291, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "TF-IDF Order :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(591, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Overal Order :";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // sumBox
            // 
            this.sumBox.BackColor = System.Drawing.Color.White;
            this.sumBox.Location = new System.Drawing.Point(594, 96);
            this.sumBox.Name = "sumBox";
            this.sumBox.ReadOnly = true;
            this.sumBox.Size = new System.Drawing.Size(284, 311);
            this.sumBox.TabIndex = 8;
            this.sumBox.Text = "";
            this.sumBox.TextChanged += new System.EventHandler(this.resultBox_TextChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(920, 419);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.sumBox);
            this.Controls.Add(this.TFBox);
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
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox TFBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RichTextBox sumBox;
    }
}

