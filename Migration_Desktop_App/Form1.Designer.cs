namespace Migration_Desktop_App
{
    partial class Index
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Index));
            this.Result = new System.Windows.Forms.TextBox();
            this.Issues2021 = new System.Windows.Forms.Button();
            this.title1 = new System.Windows.Forms.Label();
            this.title2 = new System.Windows.Forms.Label();
            this.counter = new System.Windows.Forms.Label();
            this.Iterations = new System.Windows.Forms.Button();
            this.Issues2020 = new System.Windows.Forms.Button();
            this.Issues2022 = new System.Windows.Forms.Button();
            this.Last10Issues = new System.Windows.Forms.Button();
            this.lastIssue = new System.Windows.Forms.Button();
            this.Issues2019 = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Result
            // 
            this.Result.BackColor = System.Drawing.Color.White;
            this.Result.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Result.Font = new System.Drawing.Font("Nirmala UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Result.ForeColor = System.Drawing.Color.Black;
            this.Result.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.Result.Location = new System.Drawing.Point(206, 86);
            this.Result.Multiline = true;
            this.Result.Name = "Result";
            this.Result.ReadOnly = true;
            this.Result.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.Result.Size = new System.Drawing.Size(659, 313);
            this.Result.TabIndex = 2;
            this.Result.TextChanged += new System.EventHandler(this.Result_TextChanged);
            // 
            // Issues2021
            // 
            this.Issues2021.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(74)))), ((int)(((byte)(84)))));
            this.Issues2021.FlatAppearance.BorderSize = 0;
            this.Issues2021.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Issues2021.Font = new System.Drawing.Font("Nirmala UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Issues2021.ForeColor = System.Drawing.Color.White;
            this.Issues2021.Location = new System.Drawing.Point(3, 174);
            this.Issues2021.Name = "Issues2021";
            this.Issues2021.Size = new System.Drawing.Size(196, 54);
            this.Issues2021.TabIndex = 1;
            this.Issues2021.Text = "Issues2021";
            this.Issues2021.UseVisualStyleBackColor = false;
            this.Issues2021.Click += new System.EventHandler(this.issuesFrom2021_Click);
            // 
            // title1
            // 
            this.title1.AutoSize = true;
            this.title1.BackColor = System.Drawing.Color.Transparent;
            this.title1.Font = new System.Drawing.Font("Nirmala UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.title1.ForeColor = System.Drawing.Color.White;
            this.title1.Location = new System.Drawing.Point(272, 9);
            this.title1.Name = "title1";
            this.title1.Size = new System.Drawing.Size(137, 30);
            this.title1.TabIndex = 6;
            this.title1.Text = "Jira to Azure";
            // 
            // title2
            // 
            this.title2.AutoSize = true;
            this.title2.BackColor = System.Drawing.Color.Transparent;
            this.title2.Font = new System.Drawing.Font("Nirmala UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.title2.ForeColor = System.Drawing.Color.White;
            this.title2.Location = new System.Drawing.Point(287, 39);
            this.title2.Name = "title2";
            this.title2.Size = new System.Drawing.Size(100, 30);
            this.title2.TabIndex = 6;
            this.title2.Text = "Migrator";
            // 
            // counter
            // 
            this.counter.BackColor = System.Drawing.Color.White;
            this.counter.Font = new System.Drawing.Font("Nirmala UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.counter.ForeColor = System.Drawing.Color.Black;
            this.counter.Location = new System.Drawing.Point(740, 3);
            this.counter.Name = "counter";
            this.counter.Size = new System.Drawing.Size(115, 80);
            this.counter.TabIndex = 6;
            this.counter.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Iterations
            // 
            this.Iterations.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(74)))), ((int)(((byte)(84)))));
            this.Iterations.FlatAppearance.BorderSize = 0;
            this.Iterations.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Iterations.Font = new System.Drawing.Font("Nirmala UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Iterations.ForeColor = System.Drawing.Color.White;
            this.Iterations.Location = new System.Drawing.Point(3, 348);
            this.Iterations.Name = "Iterations";
            this.Iterations.Size = new System.Drawing.Size(196, 51);
            this.Iterations.TabIndex = 1;
            this.Iterations.Text = "Migrate Iterations";
            this.Iterations.UseVisualStyleBackColor = false;
            this.Iterations.Click += new System.EventHandler(this.Iterations_Click);
            // 
            // Issues2020
            // 
            this.Issues2020.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(74)))), ((int)(((byte)(84)))));
            this.Issues2020.FlatAppearance.BorderSize = 0;
            this.Issues2020.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Issues2020.Font = new System.Drawing.Font("Nirmala UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Issues2020.ForeColor = System.Drawing.Color.White;
            this.Issues2020.Location = new System.Drawing.Point(3, 234);
            this.Issues2020.Name = "Issues2020";
            this.Issues2020.Size = new System.Drawing.Size(196, 51);
            this.Issues2020.TabIndex = 1;
            this.Issues2020.Text = "Issues 2020";
            this.Issues2020.UseVisualStyleBackColor = false;
            this.Issues2020.Click += new System.EventHandler(this.issuesFrom2020_Click);
            // 
            // Issues2022
            // 
            this.Issues2022.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(74)))), ((int)(((byte)(84)))));
            this.Issues2022.FlatAppearance.BorderSize = 0;
            this.Issues2022.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Issues2022.Font = new System.Drawing.Font("Nirmala UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Issues2022.ForeColor = System.Drawing.Color.White;
            this.Issues2022.Location = new System.Drawing.Point(3, 117);
            this.Issues2022.Name = "Issues2022";
            this.Issues2022.Size = new System.Drawing.Size(196, 51);
            this.Issues2022.TabIndex = 1;
            this.Issues2022.Text = "Issues 2022";
            this.Issues2022.UseVisualStyleBackColor = false;
            this.Issues2022.Click += new System.EventHandler(this.issuesFrom2022_Click);
            // 
            // Last10Issues
            // 
            this.Last10Issues.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(74)))), ((int)(((byte)(84)))));
            this.Last10Issues.FlatAppearance.BorderSize = 0;
            this.Last10Issues.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Last10Issues.Font = new System.Drawing.Font("Nirmala UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Last10Issues.ForeColor = System.Drawing.Color.White;
            this.Last10Issues.Location = new System.Drawing.Point(3, 60);
            this.Last10Issues.Name = "Last10Issues";
            this.Last10Issues.Size = new System.Drawing.Size(196, 51);
            this.Last10Issues.TabIndex = 1;
            this.Last10Issues.Text = "Last 10 Issues";
            this.Last10Issues.UseVisualStyleBackColor = false;
            this.Last10Issues.Click += new System.EventHandler(this.last10Issues_Click);
            // 
            // lastIssue
            // 
            this.lastIssue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(74)))), ((int)(((byte)(84)))));
            this.lastIssue.FlatAppearance.BorderSize = 0;
            this.lastIssue.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lastIssue.Font = new System.Drawing.Font("Nirmala UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lastIssue.ForeColor = System.Drawing.Color.White;
            this.lastIssue.Location = new System.Drawing.Point(3, 3);
            this.lastIssue.Name = "lastIssue";
            this.lastIssue.Size = new System.Drawing.Size(196, 51);
            this.lastIssue.TabIndex = 1;
            this.lastIssue.Text = "Last Issue";
            this.lastIssue.UseVisualStyleBackColor = false;
            this.lastIssue.Click += new System.EventHandler(this.lastIssue_Click);
            // 
            // Issues2019
            // 
            this.Issues2019.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(74)))), ((int)(((byte)(84)))));
            this.Issues2019.FlatAppearance.BorderSize = 0;
            this.Issues2019.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Issues2019.Font = new System.Drawing.Font("Nirmala UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Issues2019.ForeColor = System.Drawing.Color.White;
            this.Issues2019.Location = new System.Drawing.Point(3, 291);
            this.Issues2019.Name = "Issues2019";
            this.Issues2019.Size = new System.Drawing.Size(196, 51);
            this.Issues2019.TabIndex = 1;
            this.Issues2019.Text = "Issues 2019";
            this.Issues2019.UseVisualStyleBackColor = false;
            this.Issues2019.Click += new System.EventHandler(this.issuesFrom2019_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.lastIssue);
            this.flowLayoutPanel1.Controls.Add(this.Last10Issues);
            this.flowLayoutPanel1.Controls.Add(this.Issues2022);
            this.flowLayoutPanel1.Controls.Add(this.Issues2021);
            this.flowLayoutPanel1.Controls.Add(this.Issues2020);
            this.flowLayoutPanel1.Controls.Add(this.Issues2019);
            this.flowLayoutPanel1.Controls.Add(this.Iterations);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(1, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(199, 399);
            this.flowLayoutPanel1.TabIndex = 3;
            // 
            // Index
            // 
            this.AccessibleName = "";
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(86)))), ((int)(((byte)(185)))), ((int)(((byte)(163)))));
            this.ClientSize = new System.Drawing.Size(867, 450);
            this.Controls.Add(this.counter);
            this.Controls.Add(this.title2);
            this.Controls.Add(this.title1);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.Result);
            this.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.Name = "Index";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Jira to Azure Migrator";
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private TextBox Result;
        private Button Issues2021;
        private Label title1;
        private Label title2;
        private Label counter;
        private Button Iterations;
        private Button Issues2020;
        private Button Issues2022;
        private Button Last10Issues;
        private Button lastIssue;
        private Button Issues2019;
        private FlowLayoutPanel flowLayoutPanel1;
        private Button project_confirmation;
    }
}