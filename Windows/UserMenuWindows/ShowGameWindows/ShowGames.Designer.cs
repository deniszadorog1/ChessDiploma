namespace ChessDiploma.Windows.UserMenuWindows.ShowGameWindows
{
    partial class ShowGames
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
            this.GamesPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.WatchBut = new System.Windows.Forms.Button();
            this.BackBut = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.ChosenGameLB = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // GamesPanel
            // 
            this.GamesPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.GamesPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.GamesPanel.Location = new System.Drawing.Point(0, 0);
            this.GamesPanel.Name = "GamesPanel";
            this.GamesPanel.Size = new System.Drawing.Size(237, 449);
            this.GamesPanel.TabIndex = 0;
            this.GamesPanel.WrapContents = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(252, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(209, 25);
            this.label1.TabIndex = 1;
            this.label1.Text = "Search by user login";
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBox1.Location = new System.Drawing.Point(257, 48);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(204, 29);
            this.textBox1.TabIndex = 2;
            // 
            // WatchBut
            // 
            this.WatchBut.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.WatchBut.Location = new System.Drawing.Point(257, 320);
            this.WatchBut.Name = "WatchBut";
            this.WatchBut.Size = new System.Drawing.Size(204, 56);
            this.WatchBut.TabIndex = 3;
            this.WatchBut.Text = "WatchButton";
            this.WatchBut.UseVisualStyleBackColor = true;
            this.WatchBut.Click += new System.EventHandler(this.WatchBut_Click);
            // 
            // BackBut
            // 
            this.BackBut.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.BackBut.Location = new System.Drawing.Point(257, 382);
            this.BackBut.Name = "BackBut";
            this.BackBut.Size = new System.Drawing.Size(204, 56);
            this.BackBut.TabIndex = 4;
            this.BackBut.Text = "Back";
            this.BackBut.UseVisualStyleBackColor = true;
            this.BackBut.Click += new System.EventHandler(this.BackBut_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(252, 166);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(155, 25);
            this.label2.TabIndex = 5;
            this.label2.Text = "Chosen Game:";
            // 
            // ChosenGameLB
            // 
            this.ChosenGameLB.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ChosenGameLB.Location = new System.Drawing.Point(252, 200);
            this.ChosenGameLB.Name = "ChosenGameLB";
            this.ChosenGameLB.Size = new System.Drawing.Size(209, 83);
            this.ChosenGameLB.TabIndex = 6;
            this.ChosenGameLB.Text = "There will be chosen game";
            // 
            // ShowGames
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(492, 450);
            this.Controls.Add(this.ChosenGameLB);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.BackBut);
            this.Controls.Add(this.WatchBut);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.GamesPanel);
            this.Name = "ShowGames";
            this.Text = "ShowGames";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel GamesPanel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button WatchBut;
        private System.Windows.Forms.Button BackBut;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label ChosenGameLB;
    }
}