using Ionic.Zip;
using PackFileManager.FormManager;
using PackFileManager.PFMCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PackFileManager.FileManager
{
    public class FileHelper
    {
        public static bool DelDir(string path)
        {
            bool result;
            try
            {
                string directoryName = Path.GetDirectoryName(path);
                bool flag = Directory.Exists(directoryName);
                if (flag)
                {
                    Directory.Delete(directoryName, true);
                }
                result = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                result = false;
            }
            return result;
        }

        public static bool NewDir(string path)
        {
            bool result;
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                result = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                result = false;
            }
            return result;
        }
        private List<string> InputPassword(string TargetPath, ref string ErrorMsg)
        {
            List<string> Files = new List<string>();

            try
            {

                using (ZipFile ZipItem = ZipFile.Read(TargetPath))
                {
                    string GetFileName = TargetPath.Substring(TargetPath.LastIndexOf(@"\") + @"\".Length);

                    if (GetFileName.StartsWith("ZX")|| !GetFileName.Contains("SteamConfig.dat"))
                    {
                        ZipItem.Password = DeFine.CurrentPassword.GetPassword(GetFileName);

                        foreach (ZipEntry ZipEntry in ZipItem)
                        {
                            try
                            {
                                string FileName = ZipEntry.FileName;

                                ZipEntry.Extract(DeFine.CachePath, ExtractExistingFileAction.OverwriteSilently);

                                bool OneFlag = !ZipEntry.IsDirectory;
                                if (OneFlag)
                                {
                                    Files.Add(FileName);
                                }

                            }
                            catch (Exception Ex)
                            {
                                ErrorMsg = Ex.Message;
                            }
                        }
                    }


                }

            }
            catch { }
            return Files;
        }


        public void ZipDePack(string SourcePath,string SavePath, ref string ErrorMsg)
        {
            try
            {

                using (ZipFile ZipItem = ZipFile.Read(SourcePath))
                {
                    string GetFileName = SourcePath.Substring(SourcePath.LastIndexOf(@"\") + @"\".Length);

               
                        foreach (ZipEntry ZipEntry in ZipItem)
                        {
                            try
                            {
                                string FileName = ZipEntry.FileName;

                                ZipEntry.Extract(SavePath, ExtractExistingFileAction.OverwriteSilently);

                                bool OneFlag = !ZipEntry.IsDirectory;

                            }
                            catch (Exception Ex)
                            {
                                ErrorMsg = Ex.Message;
                            }
                        }
                }

            }
            catch { }
        }
        public void TrySaveData(ref List<string> IList,string GetFilePath,ref string ErrorMsg)
        {
            foreach (var GetFile in DataHelper.GetAllFile(GetFilePath, new List<string>() { ".dat" }))
            {
                List<string> TempList = InputPassword(GetFile.FilePath, ref ErrorMsg);
                IList.AddRange(TempList);

                if (TempList.Count > 0)
                {
                    using (ZipFile NewZip = new ZipFile())
                    {
                        for (int i = 0; i < TempList.Count; i++)
                        {
                            string SourceName = TempList[i];

                            NewZip.AddFile(DeFine.CachePath + SourceName, "");
                        }

                        NewZip.Save(DeFine.CachePath + GetFile.FileName);
                    }
                }
               
            }
        }
        public List<string> ZipExtract(ref string TargetPath)
        {
            string TempPath = string.Copy(TargetPath);
            string GetFileName = TempPath.Substring(TempPath.LastIndexOf(@"\") + @"\".Length);
            string GetFilePath = TempPath.Substring(0, TempPath.LastIndexOf(@"\") + @"\".Length);

            List<string> IList = new List<string>();

            DelDir(DeFine.CachePath);
            NewDir(DeFine.CachePath);

            bool IsTruePassword = false;

            string ErrorMsg = "";

            TrySaveData(ref IList,GetFilePath,ref ErrorMsg);

            if (IList.Count > 0)
            {
                IsTruePassword = true;
            }
            else
            {
                DeFine.CurrentPassword.CurrentPassword.Clear();

                if (!IsTruePassword)
                {
                    string Content = ErrorMsg.ToLower();
                    bool ExState = Content.IndexOf("password") > -1 && Content.IndexOf("did") > -1 && Content.IndexOf("not") > -1 && Content.IndexOf("match") > -1;

                    if (ExState)
                    {
                        if (!HookHelper.InstallState)
                        {
                            FormHelper.ShowGameDataReader(TempPath);

                            if (DeFine.CurrentPassword.CurrentPassword.Count > 0)
                            {
                                TrySaveData(ref IList, GetFilePath, ref ErrorMsg);
                            }
                            else
                            {
                                TargetPath = string.Empty;
                            }
                        }
                    }
                    else
                    {
                        TargetPath = string.Empty;
                    }
                }
            }

            return IList;
        }



    }
}
