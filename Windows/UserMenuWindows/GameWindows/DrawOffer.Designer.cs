namespace ChessDiploma.Windows.UserMenuWindows.GameWindows
{
    partial class DrawOffer
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
            this.DrawOfferLB = new System.Windows.Forms.Label();
            this.AcceptBut = new System.Windows.Forms.Button();
            this.DeclineBut = new System.Windows.Forms.Button();
            this.FromLb = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // DrawOfferLB
            // 
            this.DrawOfferLB.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.DrawOfferLB.Location = new System.Drawing.Point(12, 9);
            this.DrawOfferLB.Name = "DrawOfferLB";
            this.DrawOfferLB.Size = new System.Drawing.Size(308, 50);
            this.DrawOfferLB.TabIndex = 0;
            this.DrawOfferLB.Text = "You recived a draw offer!";
            this.DrawOfferLB.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // AcceptBut
            // 
            this.AcceptBut.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.AcceptBut.Location = new System.Drawing.Point(38, 113);
            this.AcceptBut.Name = "AcceptBut";
            this.AcceptBut.Size = new System.Drawing.Size(250, 55);
            this.AcceptBut.TabIndex = 1;
            this.AcceptBut.Text = "Accept";
            this.AcceptBut.UseVisualStyleBackColor = true;
            this.AcceptBut.Click += new System.EventHandler(this.AcceptBut_Click);
            // 
            // DeclineBut
            // 
            this.DeclineBut.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.DeclineBut.Location = new System.Drawing.Point(38, 183);
            this.DeclineBut.Name = "DeclineBut";
            this.DeclineBut.Size = new System.Drawing.Size(250, 55);
            this.DeclineBut.TabIndex = 2;
            this.DeclineBut.Text = "Decline";
            this.DeclineBut.UseVisualStyleBackColor = true;
            this.DeclineBut.Click += new System.EventHandler(this.DeclineBut_Click);
            // 
            // FromLb
            // 
            this.FromLb.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FromLb.Location = new System.Drawing.Point(12, 60);
            this.FromLb.Name = "FromLb";
            this.FromLb.Size = new System.Drawing.Size(308, 50);
            this.FromLb.TabIndex = 3;
            this.FromLb.Text = "From - Player";
            this.FromLb.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TradeOffer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(324, 266);
            this.Controls.Add(this.FromLb);
            this.Controls.Add(this.DeclineBut);
            this.Controls.Add(this.AcceptBut);
            this.Controls.Add(this.DrawOfferLB);
            this.Name = "TradeOffer";
            this.Text = "TradeOffer";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label DrawOfferLB;
        private System.Windows.Forms.Button AcceptBut;
        private System.Windows.Forms.Button DeclineBut;
        private System.Windows.Forms.Label FromLb;
    }
}