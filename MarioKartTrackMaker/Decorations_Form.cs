using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MarioKartTrackMaker
{
    /// <summary>
    /// The form that controls the settings for the decoration tool.
    /// </summary>
    public partial class Decorations_Form : Form
    {
        /// <summary>
        /// This value defines whether the decoration tool should place, or paint. Returns true if place.
        /// </summary>
        public bool place_mode { get { return PlaceRadioButton.Checked && !PaintRadioButton.Checked; } }
        public Decorations_Form()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Do this stuff after the form is initiated.
        /// </summary>
        private void OnLoad(object sender, EventArgs e)
        {
            PlaceRadioButton.Checked = true;
            LoadCategories(CategoriesList);
        }

        /// <summary>
        /// Loads the categories in the Decorations folder.
        /// </summary>
        /// <param name="sender">Target ListView.</param>
        private void LoadCategories(ListView sender)
        {
            int imageIndex = 0;
            sender.Items.Clear();
            sender.LargeImageList.Images.Clear();
            try
            {
                CheckDirectory("Decorations");
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
        /// <summary>
        /// Checks to see if the directory exists. If it doesn't it will pop up an error message.
        /// </summary>
        /// <param name="v">The directory to check</param>
        private void CheckDirectory(string v)
        {
            if (!Directory.Exists(v))
            {
                MessageBox.Show(string.Format("{0} directory is not found. Please add {0} in the directory of this program.", v), "Missing Directory:");
            }
        }
        /// <summary>
        /// Sets the maximum value of the minimum numeric control with the maximum numeric value.
        /// </summary>
        private void OnSizeJitterMaxNumericChanged(object sender, EventArgs e)
        {
            SizeJitterMinNumeric.Maximum = SizeJitterMaxNumeric.Value;
        }
        
        /// <summary>
        /// Tells this form to load all the decorations of the selected decoration category.
        /// </summary>
        private void OnCategoriesListSelectedIndexChanged(object sender, EventArgs e)
        {
            if (CategoriesList.SelectedItems.Count > 0)
            {
                LoadParts(DecorationsList, (string)CategoriesList.SelectedItems[0].Tag);
            }
            else
            {
                DecorationsList.Items.Clear();
                DecorationsList.LargeImageList.Images.Clear();
            }
        }
        /// <summary>
        /// Loads the decorations of the selected decoration category.
        /// </summary>
        /// <param name="listView">The target ListView.</param>
        /// <param name="dir">The directory to find all the decoration pieces.</param>
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
