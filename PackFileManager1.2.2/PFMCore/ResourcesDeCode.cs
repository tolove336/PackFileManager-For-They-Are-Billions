using PackFileManager.FileManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackFileManager.PFMCore
{
    public class ResourcesDeCode
    {
        public static bool LockerOneChange = false;
        public static void TryConvertResources(string MainPath)
        {
            if (!LockerOneChange)
            {
                LockerOneChange = true;
            }
            else
            {
                return;
            }

            if (!Directory.Exists(DeFine.GameObjPath + @"Sprites\"))
            {
                Directory.CreateDirectory(DeFine.GameObjPath + @"Sprites\");
            }

            string TargetSpritesPath = DeFine.GameObjPath + @"Sprites\";

            string LockerSpritesPath = MainPath + @"\" + @"ZXGame_Data\Sprites\";

            if (Directory.Exists(LockerSpritesPath))
            {
                var FileArray = DataHelper.GetAllFile(LockerSpritesPath);

                for (int i=0;i<FileArray.Count;i++)
                {
                    var Get = FileArray[i];

                    if (!Get.FilePath.Contains("副本"))
                    {
                        if (Get.Filetype.ToLower() == ".dat")
                        {
                            string NewFileName = Get.FileName.Substring(0, Get.FileName.LastIndexOf(Get.Filetype)) + ".png";

                            if (!File.Exists(TargetSpritesPath + NewFileName))
                            {
                                File.Copy(Get.FilePath,TargetSpritesPath + NewFileName);
                            }
                        }
                        else
                        if (Get.Filetype.ToLower() == ".dxatlas")
                        {
                            string NewFileName = Get.FileName.Substring(0, Get.FileName.LastIndexOf(Get.Filetype));

                            string OneError = "";

                            if (!File.Exists(TargetSpritesPath + NewFileName))
                            {
                                new FileHelper().ZipDePack(Get.FilePath, TargetSpritesPath + NewFileName, ref OneError);
                            }
                        }
                        else
                        if (Get.Filetype.ToLower() == ".trans")
                        {
                            string NewFileName = Get.FileName.Substring(0, Get.FileName.LastIndexOf(Get.Filetype));

                            string OneError = "";

                            if (!File.Exists(TargetSpritesPath + NewFileName))
                            {
                                new FileHelper().ZipDePack(Get.FilePath, TargetSpritesPath + NewFileName, ref OneError);
                            }
                        }
                    }

                }
            }



        }

    }
}
