using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MarioKartTrackMaker.ViewerResources;
namespace MarioKartTrackMaker
{
    public partial class Terrain_Config : Form
    {
        private MainForm mf;
        public Terrain_Config(MainForm mf)
        {
            this.mf = mf;
            InitializeComponent();
        }
        private void AddTerrainButton_Click(object sender, EventArgs e)
        {
            TerrainMap tm = new TerrainMap();
            tm.constructMesh();
            Object3D obj = new Object3D(tm);
            Object3D.database.Add(obj);
            Object3D.Active_Object = obj;
            mf.ViewPort.GoToObject(obj, true);
            mf.ViewPort.Invalidate();
            mf.UpdateObjectStats();
            mf.DisplayObjectList();
            mf.UpdateActiveObject();
        }
    }
}
