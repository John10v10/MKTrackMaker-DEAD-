//ObjParser written by Stefangordon, createthis, josh-perry, and alex-shmyga.
//Modified by John10v10 to use in this program.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjParser.Types
{
    interface IType
    {
        void LoadFromStringArray(string[] data);
    }
}
