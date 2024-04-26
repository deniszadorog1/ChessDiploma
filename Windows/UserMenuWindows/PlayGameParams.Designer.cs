namespace ChessDiploma.Windows.UserMenuWindows
{
    partial class PlayGameParams
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
            this.BackBut = new System.Windows.Forms.Button();
            this.PlayerBut = new System.Windows.Forms.Button();
            this.UserRadio = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.BotRadio = new System.Windows.Forms.RadioButton();
            this.EnemyPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.EnemyLoginLB = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.WhiteRadio = new System.Windows.Forms.RadioButton();
            this.BlackRadio = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // BackBut
            // 
            this.BackBut.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.BackBut.Location = new System.Drawing.Point(242, 382);
            this.BackBut.Name = "BackBut";
            this.BackBut.Size = new System.Drawing.Size(150, 50);
            this.BackBut.TabIndex = 10;
            this.BackBut.Text = "Back";
            this.BackBut.UseVisualStyleBackColor = true;
            this.BackBut.Click += new System.EventHandler(this.BackBut_Click);
            // 
            // PlayerBut
            // 
            this.PlayerBut.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PlayerBut.Location = new System.Drawing.Point(242, 326);
            this.PlayerBut.Name = "PlayerBut";
            this.PlayerBut.Size = new System.Drawing.Size(150, 50);
            this.PlayerBut.TabIndex = 9;
            this.PlayerBut.Text = "Start Game";
            this.PlayerBut.UseVisualStyleBackColor = true;
            this.PlayerBut.Click += new System.EventHandler(this.PlayerBut_Click);
            // 
            // UserRadio
            // 
            this.UserRadio.AutoSize = true;
            this.UserRadio.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.UserRadio.Location = new System.Drawing.Point(241, 86);
            this.UserRadio.Name = "UserRadio";
            this.UserRadio.Size = new System.Drawing.Size(67, 28);
            this.UserRadio.TabIndex = 8;
            this.UserRadio.TabStop = true;
            this.UserRadio.Text = "User";
            this.UserRadio.UseVisualStyleBackColor = true;
            this.UserRadio.CheckedChanged += new System.EventHandler(this.RadioChecked_RadioChecked);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(237, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(151, 37);
            this.label1.TabIndex = 7;
            this.label1.Text = "Play against";
            // 
            // BotRadio
            // 
            this.BotRadio.AutoSize = true;
            this.BotRadio.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.BotRadio.Location = new System.Drawing.Point(242, 52);
            this.BotRadio.Name = "BotRadio";
            this.BotRadio.Size = new System.Drawing.Size(55, 28);
            this.BotRadio.TabIndex = 5;
            this.BotRadio.TabStop = true;
            this.BotRadio.Text = "Bot";
            this.BotRadio.UseVisualStyleBackColor = true;
            this.BotRadio.CheckedChanged += new System.EventHandler(this.RadioChecked_RadioChecked);
            // 
            // EnemyPanel
            // 
            this.EnemyPanel.AutoScroll = true;
            this.EnemyPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.EnemyPanel.Location = new System.Drawing.Point(12, 12);
            this.EnemyPanel.Name = "EnemyPanel";
            this.EnemyPanel.Size = new System.Drawing.Size(219, 420);
            this.EnemyPanel.TabIndex = 11;
            this.EnemyPanel.WrapContents = false;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(238, 117);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(151, 27);
            this.label2.TabIndex = 12;
            this.label2.Text = "Enemy Login: ";
            // 
            // EnemyLoginLB
            // 
            this.EnemyLoginLB.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.EnemyLoginLB.Location = new System.Drawing.Point(237, 144);
            this.EnemyLoginLB.Name = "EnemyLoginLB";
            this.EnemyLoginLB.Size = new System.Drawing.Size(151, 59);
            this.EnemyLoginLB.TabIndex = 13;
            this.EnemyLoginLB.Text = "There will be enemy login";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.BlackRadio);
            this.groupBox1.Controls.Add(this.WhiteRadio);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBox1.Location = new System.Drawing.Point(242, 206);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(147, 114);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Player color ";
            // 
            // WhiteRadio
            // 
            this.WhiteRadio.AutoSize = true;
            this.WhiteRadio.Location = new System.Drawing.Point(6, 25);
            this.WhiteRadio.Name = "WhiteRadio";
            this.WhiteRadio.Size = new System.Drawing.Size(68, 24);
            this.WhiteRadio.TabIndex = 0;
            this.WhiteRadio.TabStop = true;
            this.WhiteRadio.Text = "White";
            this.WhiteRadio.UseVisualStyleBackColor = true;
            // 
            // BlackRadio
            // 
            this.BlackRadio.AutoSize = true;
            this.BlackRadio.Location = new System.Drawing.Point(6, 55);
            this.BlackRadio.Name = "BlackRadio";
            this.BlackRadio.Size = new System.Drawing.Size(66, 24);
            this.BlackRadio.TabIndex = 1;
            this.BlackRadio.TabStop = true;
            this.BlackRadio.Text = "Black";
            this.BlackRadio.UseVisualStyleBackColor = true;
            // 
            // PlayGameParams
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(411, 450);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.EnemyLoginLB);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.EnemyPanel);
            this.Controls.Add(this.BackBut);
            this.Controls.Add(this.PlayerBut);
            this.Controls.Add(this.UserRadio);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BotRadio);
            this.Name = "PlayGameParams";
            this.Text = "PlayGameParams";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BackBut;
        private System.Windows.Forms.Button PlayerBut;
        private System.Windows.Forms.RadioButton UserRadio;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton BotRadio;
        private System.Windows.Forms.FlowLayoutPanel EnemyPanel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label EnemyLoginLB;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton BlackRadio;
        private System.Windows.Forms.RadioButton WhiteRadio;
    }
}