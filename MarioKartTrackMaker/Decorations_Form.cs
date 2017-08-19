using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MarioKartTrackMaker
{
    public partial class Decorations_Form : Form
    {
        public bool place_mode { get { return radioButton1.Checked && !radioButton2.Checked; } }
        public Decorations_Form()
        {
            InitializeComponent();
        }

        private void Decorations_Form_Load(object sender, EventArgs e)
        {
            radioButton1.Checked = true;
            LoadCategories(listView2);
        }

        private void LoadCategories(ListView sender)
        {
            int imageIndex = 0;
            sender.Items.Clear();
            sender.LargeImageList.Images.Clear();
            try
            {
                foreach (string dir in Directory.GetDirectories("Decorations"))
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
            catch (Exception e)
            {
                Console.WriteLine(e);
                sender.Items.Clear();
                sender.LargeImageList.Images.Clear();
            }
        }
        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            numericUpDown2.Maximum = numericUpDown3.Value;
        }

        private void label3_Click(object sender, EventArgs e)
        {
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView2.SelectedItems.Count > 0)
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
    }
}
