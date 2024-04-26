namespace ChessDiploma.Windows
{
    partial class CreateAccount
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
            this.MainLB = new System.Windows.Forms.Label();
            this.EmailBox = new System.Windows.Forms.TextBox();
            this.LoginBox = new System.Windows.Forms.TextBox();
            this.PasswordBox = new System.Windows.Forms.TextBox();
            this.EmailLB = new System.Windows.Forms.Label();
            this.LoignLB = new System.Windows.Forms.Label();
            this.PasswordLB = new System.Windows.Forms.Label();
            this.CreateBut = new System.Windows.Forms.Button();
            this.BackBut = new System.Windows.Forms.Button();
            this.DateBirth = new System.Windows.Forms.DateTimePicker();
            this.SuspendLayout();
            // 
            // MainLB
            // 
            this.MainLB.AutoSize = true;
            this.MainLB.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.MainLB.Location = new System.Drawing.Point(53, 29);
            this.MainLB.Name = "MainLB";
            this.MainLB.Size = new System.Drawing.Size(214, 31);
            this.MainLB.TabIndex = 0;
            this.MainLB.Text = "User registration";
            // 
            // EmailBox
            // 
            this.EmailBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.EmailBox.Location = new System.Drawing.Point(59, 119);
            this.EmailBox.Name = "EmailBox";
            this.EmailBox.Size = new System.Drawing.Size(215, 31);
            this.EmailBox.TabIndex = 1;
            // 
            // LoginBox
            // 
            this.LoginBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.LoginBox.Location = new System.Drawing.Point(59, 180);
            this.LoginBox.Name = "LoginBox";
            this.LoginBox.Size = new System.Drawing.Size(215, 31);
            this.LoginBox.TabIndex = 2;
            // 
            // PasswordBox
            // 
            this.PasswordBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PasswordBox.Location = new System.Drawing.Point(59, 247);
            this.PasswordBox.Name = "PasswordBox";
            this.PasswordBox.Size = new System.Drawing.Size(215, 31);
            this.PasswordBox.TabIndex = 3;
            // 
            // EmailLB
            // 
            this.EmailLB.AutoSize = true;
            this.EmailLB.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.EmailLB.Location = new System.Drawing.Point(55, 92);
            this.EmailLB.Name = "EmailLB";
            this.EmailLB.Size = new System.Drawing.Size(57, 24);
            this.EmailLB.TabIndex = 4;
            this.EmailLB.Text = "Email";
            // 
            // LoignLB
            // 
            this.LoignLB.AutoSize = true;
            this.LoignLB.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.LoignLB.Location = new System.Drawing.Point(55, 153);
            this.LoignLB.Name = "LoignLB";
            this.LoignLB.Size = new System.Drawing.Size(57, 24);
            this.LoignLB.TabIndex = 5;
            this.LoignLB.Text = "Login";
            // 
            // PasswordLB
            // 
            this.PasswordLB.AutoSize = true;
            this.PasswordLB.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PasswordLB.Location = new System.Drawing.Point(55, 220);
            this.PasswordLB.Name = "PasswordLB";
            this.PasswordLB.Size = new System.Drawing.Size(92, 24);
            this.PasswordLB.TabIndex = 6;
            this.PasswordLB.Text = "Password";
            // 
            // CreateBut
            // 
            this.CreateBut.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.CreateBut.Location = new System.Drawing.Point(59, 335);
            this.CreateBut.Name = "CreateBut";
            this.CreateBut.Size = new System.Drawing.Size(88, 42);
            this.CreateBut.TabIndex = 7;
            this.CreateBut.Text = "Create ";
            this.CreateBut.UseVisualStyleBackColor = true;
            this.CreateBut.Click += new System.EventHandler(this.CreateBut_Click);
            // 
            // BackBut
            // 
            this.BackBut.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.BackBut.Location = new System.Drawing.Point(186, 335);
            this.BackBut.Name = "BackBut";
            this.BackBut.Size = new System.Drawing.Size(88, 42);
            this.BackBut.TabIndex = 8;
            this.BackBut.Text = "Back";
            this.BackBut.UseVisualStyleBackColor = true;
            this.BackBut.Click += new System.EventHandler(this.BackBut_Click);
            // 
            // DateBirth
            // 
            this.DateBirth.CalendarFont = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.DateBirth.Location = new System.Drawing.Point(59, 300);
            this.DateBirth.Name = "DateBirth";
            this.DateBirth.Size = new System.Drawing.Size(193, 20);
            this.DateBirth.TabIndex = 9;
            // 
            // CreateAccount
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 411);
            this.Controls.Add(this.DateBirth);
            this.Controls.Add(this.BackBut);
            this.Controls.Add(this.CreateBut);
            this.Controls.Add(this.PasswordLB);
            this.Controls.Add(this.LoignLB);
            this.Controls.Add(this.EmailLB);
            this.Controls.Add(this.PasswordBox);
            this.Controls.Add(this.LoginBox);
            this.Controls.Add(this.EmailBox);
            this.Controls.Add(this.MainLB);
            this.Name = "CreateAccount";
            this.Text = "CreateAccount";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label MainLB;
        private System.Windows.Forms.TextBox EmailBox;
        private System.Windows.Forms.TextBox LoginBox;
        private System.Windows.Forms.TextBox PasswordBox;
        private System.Windows.Forms.Label EmailLB;
        private System.Windows.Forms.Label LoignLB;
        private System.Windows.Forms.Label PasswordLB;
        private System.Windows.Forms.Button CreateBut;
        private System.Windows.Forms.Button BackBut;
        private System.Windows.Forms.DateTimePicker DateBirth;
    }
}