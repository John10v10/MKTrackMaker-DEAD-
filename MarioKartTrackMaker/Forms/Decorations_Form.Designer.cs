namespace MarioKartTrackMaker
{
    partial class Decorations_Form
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
            this.components = new System.ComponentModel.Container();
            this.PlaceRadioButton = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.PaintRadioButton = new System.Windows.Forms.RadioButton();
            this.FlowNumeric = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.SizeJitterMinNumeric = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.SizeJitterMaxNumeric = new System.Windows.Forms.NumericUpDown();
            this.TurnJitterNumeric = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.JitterNumeric = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.CategoriesList = new System.Windows.Forms.ListView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.DecorationsList = new System.Windows.Forms.ListView();
            this.imageList2 = new System.Windows.Forms.ImageList(this.components);
            this.EraseRadioButton = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.FlowNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SizeJitterMinNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SizeJitterMaxNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TurnJitterNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.JitterNumeric)).BeginInit();
            this.SuspendLayout();
            // 
            // PlaceRadioButton
            // 
            this.PlaceRadioButton.AutoSize = true;
            this.PlaceRadioButton.Checked = true;
            this.PlaceRadioButton.Location = new System.Drawing.Point(10, 34);
            this.PlaceRadioButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.PlaceRadioButton.Name = "PlaceRadioButton";
            this.PlaceRadioButton.Size = new System.Drawing.Size(73, 24);
            this.PlaceRadioButton.TabIndex = 0;
            this.PlaceRadioButton.TabStop = true;
            this.PlaceRadioButton.Text = "Place";
            this.PlaceRadioButton.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 11);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Mode:";
            // 
            // PaintRadioButton
            // 
            this.PaintRadioButton.AutoSize = true;
            this.PaintRadioButton.Location = new System.Drawing.Point(87, 34);
            this.PaintRadioButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.PaintRadioButton.Name = "PaintRadioButton";
            this.PaintRadioButton.Size = new System.Drawing.Size(70, 24);
            this.PaintRadioButton.TabIndex = 0;
            this.PaintRadioButton.Text = "Paint";
            this.PaintRadioButton.UseVisualStyleBackColor = true;
            // 
            // FlowNumeric
            // 
            this.FlowNumeric.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.FlowNumeric.Location = new System.Drawing.Point(293, 35);
            this.FlowNumeric.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.FlowNumeric.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.FlowNumeric.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.FlowNumeric.Name = "FlowNumeric";
            this.FlowNumeric.Size = new System.Drawing.Size(92, 26);
            this.FlowNumeric.TabIndex = 2;
            this.FlowNumeric.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.FlowNumeric.ValueChanged += new System.EventHandler(this.FlowNumeric_ValueChanged);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(240, 37);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "Flow:";
            // 
            // SizeJitterMinNumeric
            // 
            this.SizeJitterMinNumeric.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SizeJitterMinNumeric.DecimalPlaces = 3;
            this.SizeJitterMinNumeric.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.SizeJitterMinNumeric.Location = new System.Drawing.Point(208, 131);
            this.SizeJitterMinNumeric.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.SizeJitterMinNumeric.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.SizeJitterMinNumeric.Name = "SizeJitterMinNumeric";
            this.SizeJitterMinNumeric.Size = new System.Drawing.Size(80, 26);
            this.SizeJitterMinNumeric.TabIndex = 3;
            this.SizeJitterMinNumeric.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 131);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(135, 20);
            this.label3.TabIndex = 4;
            this.label3.Text = "Size Jitter Range:";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(289, 132);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(14, 20);
            this.label4.TabIndex = 4;
            this.label4.Text = "-";
            // 
            // SizeJitterMaxNumeric
            // 
            this.SizeJitterMaxNumeric.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SizeJitterMaxNumeric.DecimalPlaces = 3;
            this.SizeJitterMaxNumeric.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.SizeJitterMaxNumeric.Location = new System.Drawing.Point(305, 131);
            this.SizeJitterMaxNumeric.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.SizeJitterMaxNumeric.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.SizeJitterMaxNumeric.Name = "SizeJitterMaxNumeric";
            this.SizeJitterMaxNumeric.Size = new System.Drawing.Size(80, 26);
            this.SizeJitterMaxNumeric.TabIndex = 3;
            this.SizeJitterMaxNumeric.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.SizeJitterMaxNumeric.ValueChanged += new System.EventHandler(this.OnSizeJitterMaxNumericChanged);
            // 
            // TurnJitterNumeric
            // 
            this.TurnJitterNumeric.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.TurnJitterNumeric.DecimalPlaces = 3;
            this.TurnJitterNumeric.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.TurnJitterNumeric.Location = new System.Drawing.Point(208, 102);
            this.TurnJitterNumeric.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.TurnJitterNumeric.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.TurnJitterNumeric.Name = "TurnJitterNumeric";
            this.TurnJitterNumeric.Size = new System.Drawing.Size(177, 26);
            this.TurnJitterNumeric.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 102);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(136, 20);
            this.label5.TabIndex = 4;
            this.label5.Text = "Turn Jitter Range:";
            // 
            // JitterNumeric
            // 
            this.JitterNumeric.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.JitterNumeric.DecimalPlaces = 3;
            this.JitterNumeric.Location = new System.Drawing.Point(208, 72);
            this.JitterNumeric.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.JitterNumeric.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.JitterNumeric.Name = "JitterNumeric";
            this.JitterNumeric.Size = new System.Drawing.Size(177, 26);
            this.JitterNumeric.TabIndex = 3;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(14, 72);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(100, 20);
            this.label6.TabIndex = 4;
            this.label6.Text = "Jitter Range:";
            // 
            // CategoriesList
            // 
            this.CategoriesList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CategoriesList.LargeImageList = this.imageList1;
            this.CategoriesList.Location = new System.Drawing.Point(18, 182);
            this.CategoriesList.MultiSelect = false;
            this.CategoriesList.Name = "CategoriesList";
            this.CategoriesList.Size = new System.Drawing.Size(368, 95);
            this.CategoriesList.TabIndex = 6;
            this.CategoriesList.UseCompatibleStateImageBehavior = false;
            this.CategoriesList.SelectedIndexChanged += new System.EventHandler(this.OnCategoriesListSelectedIndexChanged);
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth16Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(32, 32);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(14, 158);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(123, 20);
            this.label7.TabIndex = 5;
            this.label7.Text = "Pick a Category:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(14, 279);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(137, 20);
            this.label8.TabIndex = 8;
            this.label8.Text = "Pick a Decoration:";
            // 
            // DecorationsList
            // 
            this.DecorationsList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DecorationsList.GridLines = true;
            this.DecorationsList.LargeImageList = this.imageList2;
            this.DecorationsList.Location = new System.Drawing.Point(18, 304);
            this.DecorationsList.MultiSelect = false;
            this.DecorationsList.Name = "DecorationsList";
            this.DecorationsList.Size = new System.Drawing.Size(368, 562);
            this.DecorationsList.TabIndex = 7;
            this.DecorationsList.UseCompatibleStateImageBehavior = false;
            // 
            // imageList2
            // 
            this.imageList2.ColorDepth = System.Windows.Forms.ColorDepth.Depth16Bit;
            this.imageList2.ImageSize = new System.Drawing.Size(64, 64);
            this.imageList2.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // EraseRadioButton
            // 
            this.EraseRadioButton.AutoSize = true;
            this.EraseRadioButton.Location = new System.Drawing.Point(161, 34);
            this.EraseRadioButton.Margin = new System.Windows.Forms.Padding(2);
            this.EraseRadioButton.Name = "EraseRadioButton";
            this.EraseRadioButton.Size = new System.Drawing.Size(76, 24);
            this.EraseRadioButton.TabIndex = 0;
            this.EraseRadioButton.Text = "Erase";
            this.EraseRadioButton.UseVisualStyleBackColor = true;
            this.EraseRadioButton.CheckedChanged += new System.EventHandler(this.EraseRadioButton_CheckedChanged);
            // 
            // Decorations_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(402, 884);
            this.ControlBox = false;
            this.Controls.Add(this.label8);
            this.Controls.Add(this.DecorationsList);
            this.Controls.Add(this.CategoriesList);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.JitterNumeric);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.TurnJitterNumeric);
            this.Controls.Add(this.SizeJitterMaxNumeric);
            this.Controls.Add(this.SizeJitterMinNumeric);
            this.Controls.Add(this.FlowNumeric);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.EraseRadioButton);
            this.Controls.Add(this.PaintRadioButton);
            this.Controls.Add(this.PlaceRadioButton);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MaximumSize = new System.Drawing.Size(424, 1200);
            this.MinimumSize = new System.Drawing.Size(424, 543);
            this.Name = "Decorations_Form";
            this.Text = "Decorations";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.OnLoad);
            ((System.ComponentModel.ISupportInitialize)(this.FlowNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SizeJitterMinNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SizeJitterMaxNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TurnJitterNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.JitterNumeric)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton PlaceRadioButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton PaintRadioButton;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.NumericUpDown FlowNumeric;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.NumericUpDown SizeJitterMinNumeric;
        public System.Windows.Forms.NumericUpDown SizeJitterMaxNumeric;
        public System.Windows.Forms.NumericUpDown TurnJitterNumeric;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.NumericUpDown JitterNumeric;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ListView CategoriesList;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ImageList imageList2;
        public System.Windows.Forms.ListView DecorationsList;
        private System.Windows.Forms.RadioButton EraseRadioButton;
    }
}