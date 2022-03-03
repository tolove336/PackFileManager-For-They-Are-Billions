using PackFileManager.FileManager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PackFileManager.PFMCore
{
    public class HookHelper
    {

        public static bool InstallState = false;

        public static List<string> GetFilePassword(string SourcePath)
        {
            List<string> GamePasswords = new List<string>();

            if (InstallState)
            {
                if (File.Exists(SourcePath + "Password.config"))
                {
                    GamePasswords = DataHelper.ReadFile(SourcePath + "Password.config", Encoding.UTF8);
                }
            }

            return GamePasswords;
        }

        public static void KillGame(string CheckName = "TheyAreBillions.exe")
        {
            foreach (var GetProcess in Process.GetProcessesByName(CheckName.Split('.')[0]))
            {
                GetProcess.Kill();
                GetProcess.Dispose();
            }

        }

        public static void InstallHook(string GamePath)
        {
            KillGame();

            Thread.Sleep(1000);

            if (!InstallState)
            {
                InstallState = true;
            }

            try 
            { 

            if (File.Exists(GamePath + "Ionic.Zip.dll"))
            {
                File.Copy(GamePath + "Ionic.Zip.dll", DeFine.HookPath + @"Ionic.Zip.def", true);
                File.Delete(GamePath + "Ionic.Zip.dll");
            }

            File.Copy(DeFine.HookPath + "Ionic.Zip.dll", GamePath + "Ionic.Zip.dll", true);


            if (File.Exists(GamePath + "Password.config"))
            {
                File.Delete(GamePath + "Password.config");
            }

            }
            catch { }
        }

        public static void RunGame(string GamePath,string CheckName = "TheyAreBillions.exe")
        {
            Process NewProcess = new Process();
            NewProcess.StartInfo.FileName = GamePath + CheckName;
            NewProcess.StartInfo.Arguments = "";
            NewProcess.StartInfo.WorkingDirectory = GamePath;
            NewProcess.StartInfo.UseShellExecute = false;
            //NewProcess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
            NewProcess.Start();

            while (Process.GetProcessesByName(CheckName.Split('.')[0]).Length == 0)
            {
                Thread.Sleep(10);
            }
        }
    }
}
