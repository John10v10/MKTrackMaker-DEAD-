using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using MarioKartTrackMaker.ViewerResources;
using OpenTK;

namespace MarioKartTrackMaker
{
    public enum Tools : int
    {
        Select = 0,
        Move = 1,
        Rotate = 2,
        Scale = 3,
        Snap = 4,
        Decorate = 5
    }
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadSets(listView2);
            UpdateToolStats();
            UpdateObjectStats();
        }
        public static Tools current_tool = Tools.Select;
        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void openATrackToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void closeTrackToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
        void DisplayObjectList()
        {
            listBox1.Items.Clear();
            foreach (Object3D obj in Object3D.database)
                listBox1.Items.Add(obj);
        }
        private void LoadSets(ListView sender)
        {
            int imageIndex = 0;
            sender.Items.Clear();
            sender.LargeImageList.Images.Clear();
            try
            {
                foreach (string dir in Directory.GetDirectories("Parts_n_Models"))
                {
					int plat = (int)Environment.OSVersion.Platform;
					if ((plat == 4) || (plat == 128))
					{
						sender.LargeImageList.Images.Add(imageIndex.ToString(), Image.FromFile(dir + @"/Icon.png"));
					}
					else
					{
						sender.LargeImageList.Images.Add(imageIndex.ToString(), Image.FromFile(dir + @"\Icon.png"));
					}

                    ListViewItem item = sender.Items.Add(Path.GetFileName(dir).Replace('_', ' '), imageIndex.ToString());
                    item.Tag = dir;
                    imageIndex++;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                sender.Items.Clear();
                sender.LargeImageList.Images.Clear();
            }
        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(listView2.SelectedItems.Count > 0)
            {
                LoadParts(listView1, (string)listView2.SelectedItems[0].Tag);
            }
            else
            {
                listView1.Items.Clear();
                listView1.LargeImageList.Images.Clear();
            }
        }

        private void LoadParts(ListView listView, string dir)
        {
            int imageIndex = 0;
            listView.Items.Clear();
            listView.LargeImageList.Images.Clear();
            try
            {
                foreach (string fdir in Directory.GetFiles(dir))
                {
                    if (Path.GetExtension(fdir).ToUpper() == ".OBJ" && !Path.GetFileNameWithoutExtension(fdir).ToUpper().EndsWith("_KCL"))
                    {
                        ListViewItem item = listView.Items.Add(Path.GetFileNameWithoutExtension(fdir).Replace('_', ' '), imageIndex.ToString());
                        item.Tag = fdir;
                        listView.LargeImageList.Images.Add(imageIndex.ToString(), Image.FromFile(Path.ChangeExtension(fdir, ".png")));
                        imageIndex++;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                listView.Items.Clear();
                listView.LargeImageList.Images.Clear();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            viewPortPanel1.wireframemode = ((CheckBox)sender).Checked;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).CheckState == CheckState.Unchecked)
                viewPortPanel1.collisionviewmode = 1;
            else if (((CheckBox)sender).CheckState == CheckState.Checked)
                viewPortPanel1.collisionviewmode = 2;
            else if (((CheckBox)sender).CheckState == CheckState.Indeterminate)
                viewPortPanel1.collisionviewmode = 3;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            viewPortPanel1.Invoke((EventHandler)delegate {viewPortPanel1.Invalidate();});
        }

        private void viewPortPanel1_Load(object sender, EventArgs e)
        {

        }

        private void viewPortPanel1_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void viewPortPanel1_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            numericUpDown1.Maximum = numericUpDown2.Value;
            viewPortPanel1.cam.clip_far = (float)numericUpDown2.Value;
            viewPortPanel1.Invalidate();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            viewPortPanel1.cam.clip_near = (float)numericUpDown1.Value;
            viewPortPanel1.Invalidate();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(((ListBox)sender).SelectedItems.Count > 0)
            {
                Object3D.Active_Object = ((Object3D)((ListBox)sender).SelectedItems[0]);
            }
            else Object3D.Active_Object = null;
            viewPortPanel1.Invalidate();
            UpdateObjectStats();
        }

        private void UpdateObjectStats()
        {
            listBox2.Items.Clear();
            posXnm.Enabled = posYnm.Enabled = posZnm.Enabled = rotXnm.Enabled = rotYnm.Enabled = rotZnm.Enabled = sclXnm.Enabled = sclYnm.Enabled = sclZnm.Enabled = false;
            if (Object3D.Active_Object != null)
            {
                posXnm.Value = (decimal)Object3D.Active_Object.position.X;
                posYnm.Value = (decimal)Object3D.Active_Object.position.Y;
                posZnm.Value = (decimal)Object3D.Active_Object.position.Z;
                rotXnm.Value = (decimal)Object3D.Active_Object.rotation.X;
                rotYnm.Value = (decimal)Object3D.Active_Object.rotation.Y;
                rotZnm.Value = (decimal)Object3D.Active_Object.rotation.Z;
                sclXnm.Value = (decimal)Object3D.Active_Object.scale.X;
                sclYnm.Value = (decimal)Object3D.Active_Object.scale.Y;
                sclZnm.Value = (decimal)Object3D.Active_Object.scale.Z;
                posXnm.Enabled = posYnm.Enabled = posZnm.Enabled = rotXnm.Enabled = rotYnm.Enabled = rotZnm.Enabled = sclXnm.Enabled = sclYnm.Enabled = sclZnm.Enabled = true;
                foreach (Attachment atch in Object3D.Active_Object.model.attachments)
                {
                    foreach (Object3D.attachmentInfo atif in Object3D.Active_Object.atch_info)
                        if (atif.thisAtch == atch)
                            goto no;
                    listBox2.Items.Add(atch);
                    no:;
                }
            }
            else
            {
                posXnm.Value = 0;
                posYnm.Value = 0;
                posZnm.Value = 0;
                rotXnm.Value = 0;
                rotYnm.Value = 0;
                rotZnm.Value = 0;
                sclXnm.Value = 1;
                sclYnm.Value = 1;
                sclYnm.Value = 1;
            }
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count > 0)
            {
                viewPortPanel1.GoToObject((Object3D)listBox1.SelectedItems[0]);
            }
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        void UpdateToolStats()
        {
            Color sc = Color.LightSkyBlue;
            pictureBox1.BackColor = (current_tool == Tools.Select) ? sc : Color.White;
            pictureBox2.BackColor = (current_tool == Tools.Move) ? sc : Color.White;
            pictureBox3.BackColor = (current_tool == Tools.Rotate) ? sc : Color.White;
            pictureBox4.BackColor = (current_tool == Tools.Scale) ? sc : Color.White;
            pictureBox5.BackColor = (current_tool == Tools.Snap) ? sc : Color.White;
            pictureBox6.BackColor = (current_tool == Tools.Decorate) ? sc : Color.White;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            current_tool = Tools.Select;
            UpdateToolStats();
            viewPortPanel1.Invalidate();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

            current_tool = Tools.Move;
            UpdateToolStats();
            viewPortPanel1.Invalidate();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            current_tool = Tools.Rotate;
            UpdateToolStats();
            viewPortPanel1.Invalidate();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

            current_tool = Tools.Scale;
            UpdateToolStats();
            viewPortPanel1.Invalidate();
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {

            current_tool = Tools.Snap;
            UpdateToolStats();
            viewPortPanel1.Invalidate();
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {

            current_tool = Tools.Decorate;
            UpdateToolStats();
            viewPortPanel1.Invalidate();
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                viewPortPanel1.InsertObjects((string)listView1.SelectedItems[0].Tag);
                viewPortPanel1.Refresh();
                DisplayObjectList();
            }
        }

        private void posXnm_ValueChanged(object sender, EventArgs e)
        {
            if (((NumericUpDown)sender).Enabled)
            {
                Object3D.Active_Object.position.X = (float)((NumericUpDown)sender).Value;
                viewPortPanel1.Invalidate();
            }
        }

        private void posYnm_ValueChanged(object sender, EventArgs e)
        {

            if (((NumericUpDown)sender).Enabled)
            {
                Object3D.Active_Object.position.Y = (float)((NumericUpDown)sender).Value;
                viewPortPanel1.Invalidate();
            }
        }

        private void posZnm_ValueChanged(object sender, EventArgs e)
        {
            if (((NumericUpDown)sender).Enabled)
            {
                Object3D.Active_Object.position.Z = (float)((NumericUpDown)sender).Value;
                viewPortPanel1.Invalidate();
            }
        }

        private void rotXnm_ValueChanged(object sender, EventArgs e)
        {

            if (((NumericUpDown)sender).Enabled)
            {
                Object3D.Active_Object.rotation.Z = (float)((double)((NumericUpDown)sender).Value/180*Math.PI);
                viewPortPanel1.Invalidate();
            }
        }

        private void rotYnm_ValueChanged(object sender, EventArgs e)
        {

            if (((NumericUpDown)sender).Enabled)
            {
                Object3D.Active_Object.rotation.Y = (float)((double)((NumericUpDown)sender).Value / 180 * Math.PI);
                viewPortPanel1.Invalidate();
            }
        }

        private void rotZnm_ValueChanged(object sender, EventArgs e)
        {
            if (((NumericUpDown)sender).Enabled)
            {
                Object3D.Active_Object.rotation.X = (float)((double)((NumericUpDown)sender).Value / 180 * Math.PI);
                viewPortPanel1.Invalidate();
            }
        }

        private void sclXnm_ValueChanged(object sender, EventArgs e)
        {

            if (((NumericUpDown)sender).Enabled)
            {
                Object3D.Active_Object.scale.X = (float)((NumericUpDown)sender).Value;
                viewPortPanel1.Invalidate();
            }
        }

        private void sclYnm_ValueChanged(object sender, EventArgs e)
        {

            if (((NumericUpDown)sender).Enabled)
            {
                Object3D.Active_Object.scale.Y = (float)((NumericUpDown)sender).Value;
            viewPortPanel1.Invalidate();
        }
        }

        private void sclZnm_ValueChanged(object sender, EventArgs e)
        {
            if (((NumericUpDown)sender).Enabled)
            {
                Object3D.Active_Object.scale.Z = (float)((NumericUpDown)sender).Value;
                viewPortPanel1.Invalidate();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            viewPortPanel1.cam.UpDirection = Vector3.UnitZ;
            viewPortPanel1.Invalidate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            viewPortPanel1.cam.UpDirection = Vector3.UnitZ*Matrix3.CreateFromQuaternion(Quaternion.FromEulerAngles(Object3D.Active_Object.rotation));
            viewPortPanel1.Invalidate();
        }
    }
}
