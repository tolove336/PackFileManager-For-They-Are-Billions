using DXVision;
using Ionic.Zip;
using PackFileManager.ConvertManager;
using PackFileManager.FormManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackFileManager.PFMCore
{
    public class CodeCreatHelper
    {
        public static TheyAreBillionsEntity GetTheyAreBillionsEntity(string Code)
        {
            TheyAreBillionsEntity NTheyAreBillionsEntity = new TheyAreBillionsEntity();

            string CurrentFunction = "";

            string TableName = "";
            string BodyCode = "";
            string ChangeMode = "";

            bool NewSign = false;
            foreach (var GetTLine in Code.Split(new string[] { "\r\n" }, StringSplitOptions.None))
            {
                var GetLine = GetTLine;

                if (GetTLine.Contains("//"))
                {
                    GetLine =" " + GetTLine.Substring(0, GetTLine.IndexOf("//"));
                }

                GetLine = new System.Text.RegularExpressions.Regex("[\\s]+").Replace(GetLine, " ");

                if (GetLine.Contains("<") && GetLine.Contains(">"))
                {
                    ChangeMode = GetLine.Split('<')[0];

                    if (ChangeMode.StartsWith(" "))
                    {
                        ChangeMode = ChangeMode.Substring(ChangeMode.IndexOf(" ") + " ".Length);
                    }
                    else
                    {
                        ChangeMode.Trim();
                    }


                    CurrentFunction = ChangeMode.Substring(ChangeMode.IndexOf(" "));
                    ChangeMode = ChangeMode.Substring(0, ChangeMode.IndexOf(" "));
                    ChangeMode = ChangeMode.Trim().Replace(" ", "");


                    TableName = ConvertHelper.StringDivision(GetLine, "<", ">");
                    NewSign = true;
                }

                if (NewSign)
                {
                    BodyCode += GetLine + "\r\n";
                }

                if (GetLine.Contains("}"))
                {
                    NTheyAreBillionsEntity.Entitys.Add(new TheyAreBillionsItem(ChangeMode, TableName, CurrentFunction, BodyCode));
                    BodyCode = string.Empty;
                    NewSign = false;
                }
            }

            return NTheyAreBillionsEntity;
        }

        public static int ChangeGameDate(TheyAreBillionsEntity Dates)
        {
            if (Dates.Entitys.Count == 0) return -1;
            int Sucess = 0;
            List<TableItem> TableItems = new List<TableItem>();

            foreach (var GetFileItem in FormHelper.DeCacheFiles)
            {
                if (File.Exists(DeFine.CachePath + GetFileItem))
                {
                    //ShowGameDeData
                    TableItems.Add(new TableItem(GetFileItem.Split('.')[0], DXTableManager.FromDatFile(DeFine.CachePath + GetFileItem)));
                }
            }

            foreach (var GetDate in Dates.Entitys)
            {
                var CurrentTab = FormHelper.SelectTable(TableItems, GetDate.TableName[0], GetDate.TableName[1]);

                foreach (var GetRow in CurrentTab.Rows)
                {
                    foreach (var GetUnitName in GetDate.UnitName)
                    {
                        if (GetUnitName.ChangeMode == "Modify")
                        {
                            if (GetRow.Key.Equals(GetUnitName.Name))
                            {
                                foreach (var GetCol in CurrentTab.Cols)
                                {
                                    foreach (var OneParam in GetDate.KeyItems)
                                    {
                                            if (GetCol.Key.Equals(OneParam.Name))
                                            {
                                                Sucess++;
                                                CurrentTab.Rows[GetRow.Key][GetCol.Value] = OneParam.Value;
                                            }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (Directory.Exists(DeFine.GamePath))
            {
                foreach (var GetDat in TableItems)
                {
                    foreach (var GetFile in Directory.GetFiles(DeFine.CreatPath))
                    {
                        if (GetFile.EndsWith(".dat"))
                        {
                            File.Delete(GetFile);
                        }
                        //if (GetFile.EndsWith(".newdat"))
                        //{
                        //    File.Delete(GetFile);
                        //}
                    }

                    DXTableManager.ToDatFile(GetDat.Content, DeFine.CreatPath + GetDat.TableName + ".dat");

                    List<string> list = new List<string>();

                    using (ZipFile zipFile = ZipFile.Read(DeFine.CreatPath + GetDat.TableName + ".dat"))
                    {
                        foreach (ZipEntry zipEntry in zipFile)
                        {
                            string fileName = zipEntry.FileName;
                            zipEntry.Extract(DeFine.CreatPath + GetDat.TableName + ".newdat", ExtractExistingFileAction.OverwriteSilently);
                            bool flag = !zipEntry.IsDirectory;
                            if (flag)
                            {
                                list.Add(DeFine.CreatPath + GetDat.TableName + @".newdat\" + fileName);
                            }
                        }
                    }

                    if (list.Count > 0)
                    {
                        using (ZipFile zipFile2 = new ZipFile())
                        {
                            string GetPassword = DeFine.CurrentPassword.GetPassword(GetDat.TableName + ".dat");

                            if (GetPassword.Trim().Length > 0)
                            {
                                zipFile2.Password = GetPassword;

                                for (int i = 0; i < list.Count; i++)
                                {
                                    zipFile2.AddFile(list[i], "");
                                }

                                if (File.Exists(DeFine.GamePath + GetDat.TableName + ".dat"))
                                {
                                    if (!File.Exists(DeFine.GamePath + GetDat.TableName + ".backup"))
                                    {
                                        File.Copy(DeFine.GamePath + GetDat.TableName + ".dat", DeFine.GamePath + GetDat.TableName + ".backup");
                                    }

                                    File.Delete(DeFine.GamePath + GetDat.TableName + ".dat");
                                    zipFile2.Save(DeFine.GamePath + GetDat.TableName + ".dat");
                                }

                            }

                        }

                    }
                }

            }


            return Sucess;
        }

    }


    public class KeyItem
    {
        public string ChangeMode = "";
        public string Name = "";
        public string Value = "";

        public KeyItem(string ChangeMode, string Name, string Value,bool ValueCheck=false)
        {
            this.Name = Name.Replace(" ", "").Trim();
            this.Value = Value.Replace(" ", "").Trim();

            if (ValueCheck)
            {
                if (this.Name.StartsWith("this."))
                {
                    this.Name = this.Name.Substring(this.Name.IndexOf("this.") + "this.".Length);
                }

                if (this.Value.EndsWith(";"))
                {
                    this.Value = this.Value.Substring(0, this.Value.Length - 1);
                }
                this.Value = this.Value.Replace("\"", "");
            }

            this.ChangeMode = ChangeMode;
        }

    }

    public class TheyAreBillionsItem
    {
        public List<string> TableName = new List<string>();

        public List<KeyItem> UnitName = new List<KeyItem>();

        public List<KeyItem> KeyItems = new List<KeyItem>();

        public TheyAreBillionsItem(string ChangeMode, string TableName, string UnitName, string KeyItems)
        {
            this.TableName.Clear();
            this.UnitName.Clear();
            this.KeyItems.Clear();

            if (TableName.Contains("."))
            {
                foreach (var GetTable in TableName.Split('.'))
                {
                    this.TableName.Add(GetTable.Trim().Replace("\r\n", "").Replace(" ", ""));
                }
            }
            else
            {
                this.TableName.Add(TableName.Trim().Replace("\r\n", "").Replace(" ", ""));
            }

            if (UnitName.Contains(","))
            {
                foreach (var GetUnit in UnitName.Split(','))
                {
                    if (GetUnit.Trim().Replace("\r\n", "").Replace(" ", "").Length > 0)
                    {
                        this.UnitName.Add(new KeyItem(ChangeMode, GetUnit.Trim().Replace("\r\n", "").Replace(" ", ""), ""));
                    }
                }
            }
            else
            {
                this.UnitName.Add(new KeyItem(ChangeMode, UnitName.Trim().Replace("\r\n", "").Replace(" ", ""), ""));
            }

            foreach (var GetLine in KeyItems.Split(new string[] { "\r\n" }, StringSplitOptions.None))
            {
                //ProcessCode
                string ProcessLine = GetLine;

                if (ProcessLine.Contains("//"))
                {
                    ProcessLine = ProcessLine.Substring(0, ProcessLine.LastIndexOf("//"));
                }
                if (ProcessLine.Contains("="))
                {
                    List<KeyItem> Select = this.KeyItems.Where(n => n.Name == ProcessLine.Split('=')[0]).ToList();

                    if (Select.Count == 0)
                    {
                        this.KeyItems.Add(new KeyItem(string.Empty, ProcessLine.Split('=')[0], ProcessLine.Split('=')[1].Replace(",", ""),true));
                    }
                    else
                    {

                    }
                }
            }
        }
    }

    public class TheyAreBillionsEntity
    {
        public List<TheyAreBillionsItem> Entitys = new List<TheyAreBillionsItem>();
    }
}
