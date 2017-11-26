namespace MarioKartTrackMaker
{
    partial class Terrain_Config
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
            this.AddTerrainButton = new System.Windows.Forms.Button();
            this.TerrainObjectList = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // AddTerrainButton
            // 
            this.AddTerrainButton.Location = new System.Drawing.Point(227, 12);
            this.AddTerrainButton.Name = "AddTerrainButton";
            this.AddTerrainButton.Size = new System.Drawing.Size(213, 56);
            this.AddTerrainButton.TabIndex = 0;
            this.AddTerrainButton.Text = "Add Terrain Map";
            this.AddTerrainButton.UseVisualStyleBackColor = true;
            this.AddTerrainButton.Click += new System.EventHandler(this.AddTerrainButton_Click);
            // 
            // TerrainObjectList
            // 
            this.TerrainObjectList.FormattingEnabled = true;
            this.TerrainObjectList.ItemHeight = 20;
            this.TerrainObjectList.Location = new System.Drawing.Point(227, 75);
            this.TerrainObjectList.Name = "TerrainObjectList";
            this.TerrainObjectList.Size = new System.Drawing.Size(213, 304);
            this.TerrainObjectList.TabIndex = 1;
            // 
            // Terrain_Config
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(452, 626);
            this.Controls.Add(this.TerrainObjectList);
            this.Controls.Add(this.AddTerrainButton);
            this.Name = "Terrain_Config";
            this.Text = "Terrain";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button AddTerrainButton;
        private System.Windows.Forms.ListBox TerrainObjectList;
    }
}