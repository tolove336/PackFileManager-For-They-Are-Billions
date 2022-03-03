using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackFileManager.PFMCore
{
    public class SteamHelper
    {
        public static string SearchSteamFile()
        {
            string filert = "";
            System.IO.DriveInfo[] disk = System.IO.DriveInfo.GetDrives();
            foreach (System.IO.DriveInfo di in disk)
            {
                string panfu = di.Name;
                if (File.Exists(di.Name + @"SteamLibrary\steamapps\common\They Are Billions\ZXRules.dat"))
                {
                    filert = di.Name + @"SteamLibrary\steamapps\common\They Are Billions";

                }
            }
            return filert;
        }
    }
}
