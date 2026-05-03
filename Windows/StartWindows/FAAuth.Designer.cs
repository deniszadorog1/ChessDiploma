namespace ChessDiploma.Windows.StartWindows
{
    partial class FAAuth
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
            this.label1 = new System.Windows.Forms.Label();
            this.CodeBox = new System.Windows.Forms.TextBox();
            this.EnterBut = new System.Windows.Forms.Button();
            this.CloseBut = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(12, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(220, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "Enter Code from SMS";
            // 
            // CodeBox
            // 
            this.CodeBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.CodeBox.Location = new System.Drawing.Point(12, 70);
            this.CodeBox.Name = "CodeBox";
            this.CodeBox.Size = new System.Drawing.Size(220, 31);
            this.CodeBox.TabIndex = 1;
            // 
            // EnterBut
            // 
            this.EnterBut.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.EnterBut.Location = new System.Drawing.Point(17, 119);
            this.EnterBut.Name = "EnterBut";
            this.EnterBut.Size = new System.Drawing.Size(101, 33);
            this.EnterBut.TabIndex = 2;
            this.EnterBut.Text = "Enter";
            this.EnterBut.UseVisualStyleBackColor = true;
            this.EnterBut.Click += new System.EventHandler(this.EnterBut_Click);
            // 
            // CloseBut
            // 
            this.CloseBut.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.CloseBut.Location = new System.Drawing.Point(124, 119);
            this.CloseBut.Name = "CloseBut";
            this.CloseBut.Size = new System.Drawing.Size(108, 33);
            this.CloseBut.TabIndex = 3;
            this.CloseBut.Text = "Close";
            this.CloseBut.UseVisualStyleBackColor = true;
            this.CloseBut.Click += new System.EventHandler(this.CloseBut_Click);
            // 
            // FAAuth
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(263, 176);
            this.Controls.Add(this.CloseBut);
            this.Controls.Add(this.EnterBut);
            this.Controls.Add(this.CodeBox);
            this.Controls.Add(this.label1);
            this.Name = "FAAuth";
            this.Text = "FAAuth";
            this.Load += new System.EventHandler(this.FAAuth_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox CodeBox;
        private System.Windows.Forms.Button EnterBut;
        private System.Windows.Forms.Button CloseBut;
    }
}