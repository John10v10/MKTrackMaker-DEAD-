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

namespace MarioKartTrackMaker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadSets(listView2);
        }

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

        private void LoadSets(ListView sender)
        {
            int imageIndex = 0;
            sender.Items.Clear();
            sender.LargeImageList.Images.Clear();
            try
            {
                foreach (string dir in Directory.GetDirectories("Parts_n_Models"))
                {
                    sender.LargeImageList.Images.Add(imageIndex.ToString(), Image.FromFile(dir + @"\Icon.png"));
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
                    if (Path.GetExtension(fdir).ToUpper() == ".OBJ")
                    {
                        ListViewItem item = listView.Items.Add(Path.GetFileNameWithoutExtension(fdir).Replace('_', ' '), imageIndex.ToString());
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
