namespace ChessDiploma.Windows.UserMenuWindows
{
    partial class UserCorrection
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
            this.DateBirth = new System.Windows.Forms.DateTimePicker();
            this.BackBut = new System.Windows.Forms.Button();
            this.CorrectBut = new System.Windows.Forms.Button();
            this.PasswordLB = new System.Windows.Forms.Label();
            this.LoignLB = new System.Windows.Forms.Label();
            this.EmailLB = new System.Windows.Forms.Label();
            this.PasswordBox = new System.Windows.Forms.TextBox();
            this.LoginBox = new System.Windows.Forms.TextBox();
            this.EmailBox = new System.Windows.Forms.TextBox();
            this.MainLB = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // DateBirth
            // 
            this.DateBirth.CalendarFont = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.DateBirth.Location = new System.Drawing.Point(54, 308);
            this.DateBirth.Name = "DateBirth";
            this.DateBirth.Size = new System.Drawing.Size(215, 20);
            this.DateBirth.TabIndex = 19;
            // 
            // BackBut
            // 
            this.BackBut.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.BackBut.Location = new System.Drawing.Point(181, 343);
            this.BackBut.Name = "BackBut";
            this.BackBut.Size = new System.Drawing.Size(88, 42);
            this.BackBut.TabIndex = 18;
            this.BackBut.Text = "Back";
            this.BackBut.UseVisualStyleBackColor = true;
            this.BackBut.Click += new System.EventHandler(this.BackBut_Click);
            // 
            // CorrectBut
            // 
            this.CorrectBut.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.CorrectBut.Location = new System.Drawing.Point(54, 343);
            this.CorrectBut.Name = "CorrectBut";
            this.CorrectBut.Size = new System.Drawing.Size(88, 42);
            this.CorrectBut.TabIndex = 17;
            this.CorrectBut.Text = "Correct";
            this.CorrectBut.UseVisualStyleBackColor = true;
            this.CorrectBut.Click += new System.EventHandler(this.CorrectBut_Click);
            // 
            // PasswordLB
            // 
            this.PasswordLB.AutoSize = true;
            this.PasswordLB.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PasswordLB.Location = new System.Drawing.Point(50, 228);
            this.PasswordLB.Name = "PasswordLB";
            this.PasswordLB.Size = new System.Drawing.Size(92, 24);
            this.PasswordLB.TabIndex = 16;
            this.PasswordLB.Text = "Password";
            // 
            // LoignLB
            // 
            this.LoignLB.AutoSize = true;
            this.LoignLB.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.LoignLB.Location = new System.Drawing.Point(50, 161);
            this.LoignLB.Name = "LoignLB";
            this.LoignLB.Size = new System.Drawing.Size(57, 24);
            this.LoignLB.TabIndex = 15;
            this.LoignLB.Text = "Login";
            // 
            // EmailLB
            // 
            this.EmailLB.AutoSize = true;
            this.EmailLB.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.EmailLB.Location = new System.Drawing.Point(50, 100);
            this.EmailLB.Name = "EmailLB";
            this.EmailLB.Size = new System.Drawing.Size(57, 24);
            this.EmailLB.TabIndex = 14;
            this.EmailLB.Text = "Email";
            // 
            // PasswordBox
            // 
            this.PasswordBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PasswordBox.Location = new System.Drawing.Point(54, 255);
            this.PasswordBox.Name = "PasswordBox";
            this.PasswordBox.Size = new System.Drawing.Size(215, 31);
            this.PasswordBox.TabIndex = 13;
            // 
            // LoginBox
            // 
            this.LoginBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.LoginBox.Location = new System.Drawing.Point(54, 188);
            this.LoginBox.Name = "LoginBox";
            this.LoginBox.Size = new System.Drawing.Size(215, 31);
            this.LoginBox.TabIndex = 12;
            // 
            // EmailBox
            // 
            this.EmailBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.EmailBox.Location = new System.Drawing.Point(54, 127);
            this.EmailBox.Name = "EmailBox";
            this.EmailBox.Size = new System.Drawing.Size(215, 31);
            this.EmailBox.TabIndex = 11;
            // 
            // MainLB
            // 
            this.MainLB.AutoSize = true;
            this.MainLB.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.MainLB.Location = new System.Drawing.Point(48, 37);
            this.MainLB.Name = "MainLB";
            this.MainLB.Size = new System.Drawing.Size(205, 31);
            this.MainLB.TabIndex = 10;
            this.MainLB.Text = "User Correction";
            // 
            // UserCorrection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(333, 416);
            this.Controls.Add(this.DateBirth);
            this.Controls.Add(this.BackBut);
            this.Controls.Add(this.CorrectBut);
            this.Controls.Add(this.PasswordLB);
            this.Controls.Add(this.LoignLB);
            this.Controls.Add(this.EmailLB);
            this.Controls.Add(this.PasswordBox);
            this.Controls.Add(this.LoginBox);
            this.Controls.Add(this.EmailBox);
            this.Controls.Add(this.MainLB);
            this.Name = "UserCorrection";
            this.Text = "UserCorrection";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker DateBirth;
        private System.Windows.Forms.Button BackBut;
        private System.Windows.Forms.Button CorrectBut;
        private System.Windows.Forms.Label PasswordLB;
        private System.Windows.Forms.Label LoignLB;
        private System.Windows.Forms.Label EmailLB;
        private System.Windows.Forms.TextBox PasswordBox;
        private System.Windows.Forms.TextBox LoginBox;
        private System.Windows.Forms.TextBox EmailBox;
        private System.Windows.Forms.Label MainLB;
    }
}