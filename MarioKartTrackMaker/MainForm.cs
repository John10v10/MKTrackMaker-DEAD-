using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using MarioKartTrackMaker.ViewerResources;
using OpenTK;

namespace MarioKartTrackMaker
{
    /// <summary>
    /// The program's tools defined by integers.
    /// </summary>
    public enum Tools : int
    {
        /// <summary>
        /// The selection tool.
        /// </summary>
        Select = 0,
        /// <summary>
        /// The move tool.
        /// </summary>
        Move = 1,
        /// <summary>
        /// The rotation tool.
        /// </summary>
        Rotate = 2,
        /// <summary>
        /// The scale tool.
        /// </summary>
        Scale = 3,
        /// <summary>
        /// The tool that attaches the selected part to another.
        /// </summary>
        Snap = 4,
        /// <summary>
        /// This tool allows you to decorate whatever is selected from the list on to any part.
        /// </summary>
        Decorate = 5
    }
    /// <summary>
    /// The main form of the program.
    /// </summary>
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Loads the form.
        /// </summary>
        private void OnLoad(object sender, EventArgs e)
        {
            LoadSets(SetList);
            UpdateToolStats();
            UpdateObjectStats();
            ObjectList.Height = (int)((Height - ObjectList.Top - 48.0) / 2.0 - 8.0);
            AttachmentList.Top = (int)(ObjectList.Top + ObjectList.Height + 8.0);
            AttachmentList.Height = (int)((Height - AttachmentList.Top - 48.0));
            gameComboBox.SelectedIndex = 1;
        }
        /// <summary>
        /// This static field tells the program what tool is currently active. The selection tool is default.
        /// </summary>
        public static Tools current_tool = Tools.Select;
        /// <summary>
        /// This static field tells the program whether erase mode is on or off. Right now it is only used to tell the tool drawer whether to draw its decoration tool pink or normal.
        /// </summary>
        public static float decorate_erase_mode = float.NaN;
        /// <summary>
        /// This is the bridge to access stuff in the decorations window from stuff in here. 
        /// </summary>
        public Decorations_Form DF = new Decorations_Form();
        /// <summary>
        /// Loads/refreshes all the objects in the scene.
        /// </summary>
        public void DisplayObjectList()
        {
            Object3D activeObj = Object3D.Active_Object;
            ObjectList.Items.Clear();
            foreach (Object3D obj in Object3D.database)
            {
                ObjectList.Items.Add(obj);
            }
            if(activeObj != null)
            {
                ObjectList.SelectedItem = activeObj;
                Object3D.Active_Object = activeObj;
            }
            
        }
        /// <summary>
        /// Loads all sets from the "Parts_n_Models" folder.
        /// </summary>
        /// <param name="sender">The target ListView control</param>
        private void LoadSets(ListView sender)
        {
            int imageIndex = 0;
            sender.Items.Clear();
            sender.LargeImageList.Images.Clear();
            try
            {
                CheckDirectory("Parts_n_Models");
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
        /// When the selected index of the set list changes, it will automatically load the track parts of the selected set.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSelectedSetChanged(object sender, EventArgs e)
        {
            if(SetList.SelectedItems.Count > 0)
            {
                LoadParts(PartList, (string)SetList.SelectedItems[0].Tag);
            }
            else
            {
                PartList.Items.Clear();
                PartList.LargeImageList.Images.Clear();
            }
        }

        /// <summary>
        /// Loads the parts of the selected set.
        /// </summary>
        /// <param name="listView">Target ListView control</param>
        /// <param name="dir">Directory to load the sets.</param>
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

        /// <summary>
        /// Sets the viewport to either render normal or in a wireframe view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWireframeCheckBoxChanged(object sender, EventArgs e)
        {
            ViewPort.wireframemode = ((CheckBox)sender).Checked;
        }

        /// <summary>
        /// Sets the viewport to either render normal, only collisions, or both.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCollisionCheckBoxChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).CheckState == CheckState.Unchecked)
                ViewPort.collisionviewmode = 1;
            else if (((CheckBox)sender).CheckState == CheckState.Checked)
                ViewPort.collisionviewmode = 2;
            else if (((CheckBox)sender).CheckState == CheckState.Indeterminate)
                ViewPort.collisionviewmode = 3;
        }
        
        
        /// <summary>
        /// Sets the viewport's far clipping.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClipFarValueChanged(object sender, EventArgs e)
        {
            ClipNearNumeric.Maximum = ClipFarNumeric.Value;
            ViewPort.cam.clip_far = (float)ClipFarNumeric.Value;
            ViewPort.Invalidate();
        }

        /// <summary>
        /// Sets the viewport's near clipping.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClipNearValueChanged(object sender, EventArgs e)
        {
            ViewPort.cam.clip_near = (float)ClipNearNumeric.Value;
            ViewPort.Invalidate();
        }
        /// <summary>
        /// If you want the active object to set to what you click on in the object list, set this value to true.
        /// </summary>
        public bool DoStuffWhenSelectedObjectIndexChanged = true;
        /// <summary>
        /// Loads the data of the selected object of the object list to all of the object controls, and if DoStuffWhenSelectedObjectIndexChanged is true,
        /// the active object will set to the selected object of the object list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnObjectListIndexChanged(object sender, EventArgs e)
        {
            if (DoStuffWhenSelectedObjectIndexChanged)
            {
                if (((ListBox)sender).SelectedItems.Count > 0)
                {
                    Object3D.Active_Object = ((Object3D)((ListBox)sender).SelectedItems[0]);
                }
                else Object3D.Active_Object = null;
            }
            ViewPort.Invalidate();
            UpdateObjectStats();
        }
        /// <summary>
        /// Refreshes the active object in the object list.
        /// </summary>
        public void UpdateActiveObject()
        {
            ObjectList.SelectedItems.Clear();
            if (Object3D.Active_Object != null)
                ObjectList.SelectedItems.Add(Object3D.Active_Object);
            if (Object3D.Active_Object != null)
                ObjectList.SelectedItems.Add(Object3D.Active_Object);
        }
        /// <summary>
        /// Loads the data of the active object to all of the object controls.
        /// </summary>
        public void UpdateObjectStats()
        {
            AttachmentList.Items.Clear();
            ColorButton.Enabled = posXnm.Enabled = posYnm.Enabled = posZnm.Enabled = rotXnm.Enabled = rotYnm.Enabled = rotZnm.Enabled = sclXnm.Enabled = sclYnm.Enabled = sclZnm.Enabled = false;
            if (Object3D.Active_Object != null)
            {
                posXnm.Value = (decimal)Object3D.Active_Object.position.X;
                posYnm.Value = (decimal)Object3D.Active_Object.position.Y;
                posZnm.Value = (decimal)Object3D.Active_Object.position.Z;
                //We need to convert the active object's rotation into 3 euler angles.
                Quaternion q = Object3D.Active_Object.rotation;
                rotXnm.Value = (decimal)(Math.Round(Math.Atan2(Math.Max(-1, Math.Min(1, (2 * q.X * q.W - 2 * q.Y * q.Z))), Math.Max(-1, Math.Min(1, (1 - 2 * q.X * q.X - 2 * q.Z * q.Z)))) * 180 / Math.PI, 3));
                rotYnm.Value = (decimal)(Math.Round(Math.Atan2(Math.Max(-1, Math.Min(1, (2 * q.Y * q.W - 2 * q.X * q.Z))), Math.Max(-1, Math.Min(1, (1 - 2 * q.Y * q.Y - 2 * q.Z * q.Z)))) * 180 / Math.PI, 3));
                rotZnm.Value = (decimal)(Math.Round(Math.Asin(Math.Max(-1, Math.Min(1, (2 * q.X * q.Y + 2 * q.Z * q.W)))) * 180 / Math.PI, 3));
                sclXnm.Value = (decimal)Object3D.Active_Object.scale.X;
                sclYnm.Value = (decimal)Object3D.Active_Object.scale.Y;
                sclZnm.Value = (decimal)Object3D.Active_Object.scale.Z;
                ColorButton.BackColor = Color.FromArgb((int)(Object3D.Active_Object.Color.X * 255), (int)(Object3D.Active_Object.Color.Y * 255), (int)(Object3D.Active_Object.Color.Z * 255));
                ColorButton.ForeColor = Color.FromArgb(255-(int)(Object3D.Active_Object.Color.X * 255), 255 - (int)(Object3D.Active_Object.Color.Y * 255), 255 - (int)(Object3D.Active_Object.Color.Z * 255));
                posXnm.Enabled = posYnm.Enabled = posZnm.Enabled = rotXnm.Enabled = rotYnm.Enabled = rotZnm.Enabled = sclXnm.Enabled = sclYnm.Enabled = sclZnm.Enabled = true;
                ColorButton.Enabled = Object3D.Active_Object.model.useColor;
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
                ColorButton.BackColor = Color.White;
                ColorButton.ForeColor = Color.Black;
            }
            AttachmentList.Items.Clear();
            AttachmentList.SelectedItems.Clear();
            if (Object3D.Active_Object != null)
            {
                foreach (Attachment atch in Object3D.Active_Object.model.attachments)
                {
                    foreach (Object3D.attachmentInfo atif in Object3D.Active_Object.atch_info)
                        if (atif.thisAtch == atch)
                            goto no;
                    AttachmentList.Items.Add(atch);
                    if (atch == Object3D.Active_Object.Active_Attachment)
                    {
                        AttachmentList.SelectedItems.Add(atch);
                    }
                    no:;
                }
                if(AttachmentList.SelectedItems.Count == 0 && AttachmentList.Items.Count > 0)
                {
                    AttachmentList.SelectedItems.Add(AttachmentList.Items[0]);
                }
            }
        }

        /// <summary>
        /// When the object list is double clicked, it will navigate the viewport to the selected object.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnObjectListDoubleClick(object sender, EventArgs e)
        {
            if (ObjectList.SelectedItems.Count > 0)
            {
                ViewPort.GoToObject((Object3D)ObjectList.SelectedItems[0], false);
            }
        }
        
        /// <summary>
        /// Refreshes the tool buttons to display which one is selected.
        /// </summary>
        public void UpdateToolStats()
        {
            Color sc = Color.LightSkyBlue;
            SelectToolButton.BackColor = (current_tool == Tools.Select) ? sc : Color.White;
            MoveToolButton.BackColor = (current_tool == Tools.Move) ? sc : Color.White;
            RotationToolButton.BackColor = (current_tool == Tools.Rotate) ? sc : Color.White;
            ScaleToolButton.BackColor = (current_tool == Tools.Scale) ? sc : Color.White;
            SnapToolButton.BackColor = (current_tool == Tools.Snap) ? sc : Color.White;
            DecorationToolButton.BackColor = (current_tool == Tools.Decorate) ? sc : Color.White;
        }

        /// <summary>
        /// Sets the active tool to the select tool.
        /// </summary>
        private void OnSelectToolClick(object sender, EventArgs e)
        {
            if (DF.Visible)
            {
                DF.Hide();
            }
            current_tool = Tools.Select;
            UpdateToolStats();
            ViewPort.Invalidate();
        }

        /// <summary>
        /// Sets the active tool to the move tool.
        /// </summary>
        private void OnMoveToolClick(object sender, EventArgs e)
        {

            if (DF.Visible)
            {
                DF.Hide();
            }
            current_tool = Tools.Move;
            UpdateToolStats();
            ViewPort.Invalidate();
        }

        /// <summary>
        /// Sets the active tool to the rotate tool.
        /// </summary>
        private void OnRotateToolClick(object sender, EventArgs e)
        {
            if (DF.Visible)
            {
                DF.Hide();
            }
            current_tool = Tools.Rotate;
            UpdateToolStats();
            ViewPort.Invalidate();
        }

        /// <summary>
        /// Sets the active tool to the scale tool.
        /// </summary>
        private void OnScaleToolClick(object sender, EventArgs e)
        {

            if (DF.Visible)
            {
                DF.Hide();
            }
            current_tool = Tools.Scale;
            UpdateToolStats();
            ViewPort.Invalidate();
        }

        /// <summary>
        /// Sets the active tool to the attach tool.
        /// </summary>
        private void OnAttachToolClick(object sender, EventArgs e)
        {
            if (DF.Visible)
            {
                DF.Hide();
            }
            current_tool = Tools.Snap;
            UpdateToolStats();
            ViewPort.Invalidate();
        }

        /// <summary>
        /// Sets the active tool to the decoration tool.
        /// </summary>
        private void OnDecorationToolClick(object sender, EventArgs e)
        {
            if (!DF.Visible) {
                DF.Show();
                Activate();
            }
            current_tool = Tools.Decorate;
            UpdateToolStats();
            ViewPort.Invalidate();
        }

        /// <summary>
        /// This tells the program to load a part from the selection of the part list.
        /// </summary>
        private void OnPartsListDoubleClick(object sender, MouseEventArgs e)
        {
            if (PartList.SelectedItems.Count > 0)
            {
                ViewPort.InsertObject((string)PartList.SelectedItems[0].Tag, (AttachmentList.SelectedItems.Count > 0)?((Attachment)AttachmentList.SelectedItems[0]):null);
                ViewPort.Refresh();
                DisplayObjectList();
            }
        }

        /// <summary>
        /// Sets the X position of the active object.
        /// </summary>
        private void posXnm_ValueChanged(object sender, EventArgs e)
        {
            if (((NumericUpDown)sender).Enabled)
            {
                Vector3 pos = Object3D.Active_Object.position;
                pos.X = (float)((NumericUpDown)sender).Value;
                Object3D.Active_Object.position = pos;
                Object3D.Active_Object.FixAttachments();
                ViewPort.Invalidate();
                DisplayObjectList();
            }
        }

        /// <summary>
        /// Sets the Y position of the active object.
        /// </summary>
        private void posYnm_ValueChanged(object sender, EventArgs e)
        {

            if (((NumericUpDown)sender).Enabled)
            {
                Vector3 pos = Object3D.Active_Object.position;
                pos.Y = (float)((NumericUpDown)sender).Value;
                Object3D.Active_Object.position = pos;
                Object3D.Active_Object.FixAttachments();
                ViewPort.Invalidate();
                DisplayObjectList();
            }
        }

        /// <summary>
        /// Sets the Z position of the active object.
        /// </summary>
        private void posZnm_ValueChanged(object sender, EventArgs e)
        {
            if (((NumericUpDown)sender).Enabled)
            {
                Vector3 pos = Object3D.Active_Object.position;
                pos.Z = (float)((NumericUpDown)sender).Value;
                Object3D.Active_Object.position = pos;
                Object3D.Active_Object.FixAttachments();
                ViewPort.Invalidate();
                DisplayObjectList();
            }
        }

        /// <summary>
        /// Sets the X rotation of the active object.
        /// </summary>
        private void rotXnm_ValueChanged(object sender, EventArgs e)
        {

            if (((NumericUpDown)sender).Enabled)
            {
                Object3D.Active_Object.rotation = Quaternion.FromEulerAngles(new Vector3((float)((double)(rotZnm.Value) / 180 * Math.PI), (float)((double)(rotYnm.Value) / 180 * Math.PI), (float)((double)(rotXnm.Value) / 180 * Math.PI)));

                Object3D.Active_Object.FixAttachments();
                ViewPort.Invalidate();
                DisplayObjectList();
            }
        }

        /// <summary>
        /// Sets the Y rotation of the active object.
        /// </summary>
        private void rotYnm_ValueChanged(object sender, EventArgs e)
        {

            if (((NumericUpDown)sender).Enabled)
            {
                Object3D.Active_Object.rotation = Quaternion.FromEulerAngles(new Vector3((float)((double)(rotZnm.Value) / 180 * Math.PI), (float)((double)(rotYnm.Value) / 180 * Math.PI), (float)((double)(rotXnm.Value) / 180 * Math.PI)));

                Object3D.Active_Object.FixAttachments();
                ViewPort.Invalidate();
                DisplayObjectList();
            }
        }

        /// <summary>
        /// Sets the Z rotation of the active object.
        /// </summary>
        private void rotZnm_ValueChanged(object sender, EventArgs e)
        {
            if (((NumericUpDown)sender).Enabled)
            {
                Object3D.Active_Object.rotation = Quaternion.FromEulerAngles(new Vector3((float)((double)(rotZnm.Value) / 180 * Math.PI), (float)((double)(rotYnm.Value) / 180 * Math.PI), (float)((double)(rotXnm.Value) / 180 * Math.PI)));

                Object3D.Active_Object.FixAttachments();
                ViewPort.Invalidate();
                DisplayObjectList();
            }
        }

        /// <summary>
        /// Sets the X scale of the active object.
        /// </summary>
        private void sclXnm_ValueChanged(object sender, EventArgs e)
        {

            if (((NumericUpDown)sender).Enabled)
            {
                Vector3 scale = Object3D.Active_Object.scale;
                scale.X = (float)((NumericUpDown)sender).Value;
                Object3D.Active_Object.scale = scale;
                Object3D.Active_Object.FixAttachments();
                ViewPort.Invalidate();
                DisplayObjectList();
            }
        }

        /// <summary>
        /// Sets the Y scale of the active object.
        /// </summary>
        private void sclYnm_ValueChanged(object sender, EventArgs e)
        {

            if (((NumericUpDown)sender).Enabled)
            {
                Vector3 scale = Object3D.Active_Object.scale;
                scale.Y = (float)((NumericUpDown)sender).Value;
                Object3D.Active_Object.scale = scale;
                Object3D.Active_Object.FixAttachments();
                ViewPort.Invalidate();
                DisplayObjectList();
            }
        }

        /// <summary>
        /// Sets the Z scale of the active object.
        /// </summary>
        private void sclZnm_ValueChanged(object sender, EventArgs e)
        {
            if (((NumericUpDown)sender).Enabled)
            {
                Vector3 scale = Object3D.Active_Object.scale;
                scale.Z = (float)((NumericUpDown)sender).Value;
                Object3D.Active_Object.scale = scale;
                Object3D.Active_Object.FixAttachments();
                ViewPort.Invalidate();
                DisplayObjectList();
            }
        }

        /// <summary>
        /// Globally fixes the camera tilt.
        /// </summary>
        private void OnOrbitViewResetButtonClick(object sender, EventArgs e)
        {
            ViewPort.cam.UpDirection = Vector3.UnitZ;
            ViewPort.Invalidate();
        }

        /// <summary>
        /// Fixes the camera tilt to the selected object.
        /// </summary>
        private void OnAlignOrbitButtonClick(object sender, EventArgs e)
        {
            ViewPort.cam.UpDirection = Vector3.UnitZ*Matrix3.CreateFromQuaternion(Object3D.Active_Object.rotation);
            ViewPort.Invalidate();
        }

        /// <summary>
        /// Fizes the controls to the new size of the form.
        /// </summary>
        private void OnResize(object sender, EventArgs e)
        {
            ObjectList.Height = (int)((Height - ObjectList.Top - 48.0) / 2.0 - 8.0);
            AttachmentList.Top = (int)(ObjectList.Top + ObjectList.Height + 8.0);
            AttachmentList.Height = (int)((Height - AttachmentList.Top - 48.0));
        }

        /// <summary>
        /// Selects the attachment from the active object.
        /// </summary>
        private void OnSelectedAttachmentChanged(object sender, EventArgs e)
        {
            if(AttachmentList.SelectedItems.Count > 0 && Object3D.Active_Object != null)
            {
                Object3D.Active_Object.Active_Attachment = (Attachment)AttachmentList.SelectedItems[0];
                ViewPort.Invalidate();
            }
        }

        /// <summary>
        /// Do stuff when a key is pressed. This is to select a tool by simply pressing 1, 2, 3, 4, 5, or 6.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!(ActiveControl is NumericUpDown)) {
                if (e.KeyCode == Keys.D1)
                {

                    if (DF.Visible)
                    {
                        DF.Hide();
                    }
                    current_tool = Tools.Select;
                    UpdateToolStats();
                    ViewPort.Invalidate();
                }
                else if (e.KeyCode == Keys.D2)
                {

                    if (DF.Visible)
                    {
                        DF.Hide();
                    }
                    current_tool = Tools.Move;
                    UpdateToolStats();
                    ViewPort.Invalidate();
                }
                else if (e.KeyCode == Keys.D3)
                {

                    if (DF.Visible)
                    {
                        DF.Hide();
                    }
                    current_tool = Tools.Rotate;
                    UpdateToolStats();
                    ViewPort.Invalidate();
                }
                else if (e.KeyCode == Keys.D4)
                {
                    if (DF.Visible)
                    {
                        DF.Hide();
                    }

                    current_tool = Tools.Scale;
                    UpdateToolStats();
                    ViewPort.Invalidate();
                }
                else if (e.KeyCode == Keys.D5)
                {
                    if (DF.Visible)
                    {
                        DF.Hide();
                    }

                    current_tool = Tools.Snap;
                    UpdateToolStats();
                    ViewPort.Invalidate();
                }
                else if (e.KeyCode == Keys.D6)
                {

                    if (!DF.Visible)
                    {
                        DF.Show();
                        this.Activate();
                    }
                    current_tool = Tools.Decorate;
                    UpdateToolStats();
                    ViewPort.Invalidate();
                }
                else if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
                {
                    if(Object3D.Active_Object != null) Object3D.Active_Object.Dispose();
                    ViewPort.Invalidate();
                    DisplayObjectList();
                    UpdateActiveObject();
                    UpdateToolStats();
                    UpdateObjectStats();
                }
            }
        }

        /// <summary>
        /// Activates the color dialogue, when the user selects the color, the active object will display in that color.
        /// </summary>
        private void OnColorButtonClick(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
            Object3D.Active_Object.Color = new Vector3(colorDialog1.Color.R / 255F, colorDialog1.Color.G / 255F, colorDialog1.Color.B / 255F);
            ColorButton.BackColor = colorDialog1.Color;
            ColorButton.ForeColor = Color.FromArgb(255 - colorDialog1.Color.R, 255 - colorDialog1.Color.G, 255 - colorDialog1.Color.B);
            ViewPort.Invalidate();
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }
}
