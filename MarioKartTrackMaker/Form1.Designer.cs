using MarioKartTrackMaker.ViewerResources;
namespace MarioKartTrackMaker
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
            this.viewPortPanel1 = new ViewPortPanel(new OpenTK.Graphics.GraphicsMode(32, 32, 0, 4));
            this.SuspendLayout();
            // 
            // viewPortPanel1
            // 
            this.viewPortPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.viewPortPanel1.BackColor = System.Drawing.Color.Black;
            this.viewPortPanel1.Location = new System.Drawing.Point(12, 102);
            this.viewPortPanel1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.viewPortPanel1.Name = "viewPortPanel1";
            this.viewPortPanel1.Size = new System.Drawing.Size(1165, 624);
            this.viewPortPanel1.TabIndex = 0;
            this.viewPortPanel1.VSync = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1189, 741);
            this.Controls.Add(this.viewPortPanel1);
            this.Name = "Form1";
            this.Text = "Mario Kart Track Maker";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion
        
        private ViewPortPanel viewPortPanel1;
    }
}

