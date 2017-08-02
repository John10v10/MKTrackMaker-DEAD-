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
        /// new OpenTK.Graphics.GraphicsMode(32, 24, 0, 8)
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.viewPortPanel1 = new MarioKartTrackMaker.ViewerResources.ViewPortPanel(new OpenTK.Graphics.GraphicsMode(32, 24, 0, 8));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openATrackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openATrackToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.saveATrackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recentTracksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importObjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeTrackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeTrackToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listView1 = new System.Windows.Forms.ListView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.listView2 = new System.Windows.Forms.ListView();
            this.imageList2 = new System.Windows.Forms.ImageList(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // viewPortPanel1
            // 
            this.viewPortPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.viewPortPanel1.BackColor = System.Drawing.Color.Black;
            this.viewPortPanel1.Location = new System.Drawing.Point(460, 214);
            this.viewPortPanel1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.viewPortPanel1.Name = "viewPortPanel1";
            this.viewPortPanel1.Size = new System.Drawing.Size(890, 512);
            this.viewPortPanel1.TabIndex = 0;
            this.viewPortPanel1.VSync = true;
            this.viewPortPanel1.Load += new System.EventHandler(this.viewPortPanel1_Load);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(28, 28);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1365, 38);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openATrackToolStripMenuItem,
            this.openATrackToolStripMenuItem1,
            this.saveATrackToolStripMenuItem,
            this.recentTracksToolStripMenuItem,
            this.importObjectToolStripMenuItem,
            this.closeTrackToolStripMenuItem,
            this.closeTrackToolStripMenuItem1});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(56, 34);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openATrackToolStripMenuItem
            // 
            this.openATrackToolStripMenuItem.Name = "openATrackToolStripMenuItem";
            this.openATrackToolStripMenuItem.Size = new System.Drawing.Size(256, 34);
            this.openATrackToolStripMenuItem.Text = "New Track";
            // 
            // openATrackToolStripMenuItem1
            // 
            this.openATrackToolStripMenuItem1.Name = "openATrackToolStripMenuItem1";
            this.openATrackToolStripMenuItem1.Size = new System.Drawing.Size(256, 34);
            this.openATrackToolStripMenuItem1.Text = "Open Track";
            this.openATrackToolStripMenuItem1.Click += new System.EventHandler(this.openATrackToolStripMenuItem1_Click);
            // 
            // saveATrackToolStripMenuItem
            // 
            this.saveATrackToolStripMenuItem.Name = "saveATrackToolStripMenuItem";
            this.saveATrackToolStripMenuItem.Size = new System.Drawing.Size(256, 34);
            this.saveATrackToolStripMenuItem.Text = "Save Track";
            // 
            // recentTracksToolStripMenuItem
            // 
            this.recentTracksToolStripMenuItem.Name = "recentTracksToolStripMenuItem";
            this.recentTracksToolStripMenuItem.Size = new System.Drawing.Size(256, 34);
            this.recentTracksToolStripMenuItem.Text = "Recent Tracks";
            // 
            // importObjectToolStripMenuItem
            // 
            this.importObjectToolStripMenuItem.Name = "importObjectToolStripMenuItem";
            this.importObjectToolStripMenuItem.Size = new System.Drawing.Size(256, 34);
            this.importObjectToolStripMenuItem.Text = "Import Object";
            // 
            // closeTrackToolStripMenuItem
            // 
            this.closeTrackToolStripMenuItem.Name = "closeTrackToolStripMenuItem";
            this.closeTrackToolStripMenuItem.Size = new System.Drawing.Size(256, 34);
            this.closeTrackToolStripMenuItem.Text = "Export this Track";
            this.closeTrackToolStripMenuItem.Click += new System.EventHandler(this.closeTrackToolStripMenuItem_Click);
            // 
            // closeTrackToolStripMenuItem1
            // 
            this.closeTrackToolStripMenuItem1.Name = "closeTrackToolStripMenuItem1";
            this.closeTrackToolStripMenuItem1.Size = new System.Drawing.Size(256, 34);
            this.closeTrackToolStripMenuItem1.Text = "Close Track";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(68, 34);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(162, 34);
            this.aboutToolStripMenuItem.Text = "About";
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listView1.GridLines = true;
            this.listView1.LargeImageList = this.imageList1;
            this.listView1.Location = new System.Drawing.Point(22, 214);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(409, 515);
            this.listView1.TabIndex = 2;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth16Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(64, 64);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 186);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 25);
            this.label1.TabIndex = 3;
            this.label1.Text = "Pick a Set:";
            // 
            // listView2
            // 
            this.listView2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView2.LargeImageList = this.imageList2;
            this.listView2.Location = new System.Drawing.Point(22, 71);
            this.listView2.MultiSelect = false;
            this.listView2.Name = "listView2";
            this.listView2.Size = new System.Drawing.Size(655, 112);
            this.listView2.TabIndex = 4;
            this.listView2.UseCompatibleStateImageBehavior = false;
            this.listView2.SelectedIndexChanged += new System.EventHandler(this.listView2_SelectedIndexChanged);
            // 
            // imageList2
            // 
            this.imageList2.ColorDepth = System.Windows.Forms.ColorDepth.Depth16Bit;
            this.imageList2.ImageSize = new System.Drawing.Size(32, 32);
            this.imageList2.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 25);
            this.label2.TabIndex = 3;
            this.label2.Text = "Pick a Set:";
            // 
            // checkBox1
            // 
            this.checkBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(690, 164);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(128, 29);
            this.checkBox1.TabIndex = 5;
            this.checkBox1.Text = "Wireframe";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1365, 741);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.listView2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.viewPortPanel1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Mario Kart Track Maker";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        
        private ViewPortPanel viewPortPanel1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openATrackToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openATrackToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem saveATrackToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem recentTracksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeTrackToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importObjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeTrackToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView listView2;
        private System.Windows.Forms.ImageList imageList2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}

