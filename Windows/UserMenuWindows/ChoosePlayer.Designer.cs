namespace ChessDiploma.Windows.UserMenuWindows
{
    partial class ChoosePlayer
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
            this.PlayersList = new System.Windows.Forms.FlowLayoutPanel();
            this.Searcher = new System.Windows.Forms.TextBox();
            this.ShowParamsBut = new System.Windows.Forms.Button();
            this.BackBut = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.ChosenPlayerLB = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // PlayersList
            // 
            this.PlayersList.Location = new System.Drawing.Point(12, 12);
            this.PlayersList.Name = "PlayersList";
            this.PlayersList.Size = new System.Drawing.Size(182, 416);
            this.PlayersList.TabIndex = 0;
            // 
            // Searcher
            // 
            this.Searcher.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Searcher.Location = new System.Drawing.Point(210, 39);
            this.Searcher.Name = "Searcher";
            this.Searcher.Size = new System.Drawing.Size(141, 31);
            this.Searcher.TabIndex = 1;
            this.Searcher.TextChanged += new System.EventHandler(this.Searcher_TextChanged);
            // 
            // ShowParamsBut
            // 
            this.ShowParamsBut.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ShowParamsBut.Location = new System.Drawing.Point(200, 342);
            this.ShowParamsBut.Name = "ShowParamsBut";
            this.ShowParamsBut.Size = new System.Drawing.Size(154, 40);
            this.ShowParamsBut.TabIndex = 2;
            this.ShowParamsBut.Text = "Show Parms";
            this.ShowParamsBut.UseVisualStyleBackColor = true;
            this.ShowParamsBut.Click += new System.EventHandler(this.ShowParamsBut_Click);
            // 
            // BackBut
            // 
            this.BackBut.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.BackBut.Location = new System.Drawing.Point(200, 388);
            this.BackBut.Name = "BackBut";
            this.BackBut.Size = new System.Drawing.Size(154, 40);
            this.BackBut.TabIndex = 3;
            this.BackBut.Text = "Back";
            this.BackBut.UseVisualStyleBackColor = true;
            this.BackBut.Click += new System.EventHandler(this.BackBut_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(196, 159);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(148, 24);
            this.label1.TabIndex = 4;
            this.label1.Text = "Chosen player - ";
            // 
            // ChosenPlayerLB
            // 
            this.ChosenPlayerLB.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ChosenPlayerLB.Location = new System.Drawing.Point(196, 183);
            this.ChosenPlayerLB.Name = "ChosenPlayerLB";
            this.ChosenPlayerLB.Size = new System.Drawing.Size(158, 79);
            this.ChosenPlayerLB.TabIndex = 5;
            this.ChosenPlayerLB.Text = "There will be chosen player login ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(206, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 24);
            this.label2.TabIndex = 6;
            this.label2.Text = "Search";
            // 
            // ChoosePlayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(363, 450);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ChosenPlayerLB);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BackBut);
            this.Controls.Add(this.ShowParamsBut);
            this.Controls.Add(this.Searcher);
            this.Controls.Add(this.PlayersList);
            this.Name = "ChoosePlayer";
            this.Text = "ChoosePlayer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel PlayersList;
        private System.Windows.Forms.TextBox Searcher;
        private System.Windows.Forms.Button ShowParamsBut;
        private System.Windows.Forms.Button BackBut;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label ChosenPlayerLB;
        private System.Windows.Forms.Label label2;
    }
}