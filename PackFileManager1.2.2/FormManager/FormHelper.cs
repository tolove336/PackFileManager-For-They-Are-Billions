using DXVision;
using PackFileManager.FileManager;
using PackFileManager.PFMCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using PackFileManager.ConvertManager;
using System.Windows.Documents;

namespace PackFileManager.FormManager
{
    public class FormHelper
    {
        public static MainWindow WorkingForm = null;
        public static void Initialization(MainWindow WorkWin)
        {
            if (WorkingForm == null)
            {
                WorkingForm = WorkWin;
                SQLiteHelper.SetConnectionString("system.db", string.Empty);
                DeFine.CurrentSetting.GetSetting();
                DeFine.CurrentPassword = new DeFine.PasswordArray(DeFine.CurrentSetting.KeySave);
            }
        }
        public static List<string> DeCacheFiles = new List<string>();
        public static void ProcessFile()
        {
            if (PFMHelper.TableItems.Count == 0)
            {
                Thread Current = new Thread(() =>
                {
                    string GetTargetDataPath = string.Empty;
                    CheckGamePath(ref GetTargetDataPath);
                
                    DeFine.GamePath = GetTargetDataPath.Substring(0, GetTargetDataPath.LastIndexOf(@"\") + @"\".Length);

                    DeFine.CurrentSetting.GamePath = DeFine.GamePath;

                    DeFine.CurrentSetting.SetSetting();

                    if (File.Exists(GetTargetDataPath))
                    {
                        var DeList = new FileHelper().ZipExtract(ref GetTargetDataPath);

                        if (DeList.Count > 0)
                        {
                            DeCacheFiles.Clear();
                            DeCacheFiles.AddRange(DeList);

                            PFMHelper.TableItems.Clear();

                            foreach (var GetFileItem in DeList)
                            {
                                if (File.Exists(DeFine.CachePath + GetFileItem))
                                {
                                    //ShowGameDeData
                                    PFMHelper.TableItems.Add(new TableItem(GetFileItem.Split('.')[0], DXTableManager.FromDatFile(DeFine.CachePath + GetFileItem)));
                                }
                            }
                        }

                        DeFine.CurrentSetting.KeySave = DeFine.CurrentPassword.GetJson();
                        DeFine.CurrentSetting.SetSetting();
                    }
                });
                Current.SetApartmentState(ApartmentState.STA);
                Current.Start();
            }
            else
            {
                ShowDate();
            }
        }

        private static string SetSearchText = "";
        public static void ShowDate(bool ShowContent = false)
        {
            WorkingForm.FileNameSelection.Children.Clear();
            foreach (var Get in PFMHelper.TableItems)
            {
                System.Windows.Controls.Label NewLab = new System.Windows.Controls.Label();
                NewLab.Content = Get.TableName;
                NewLab.Background = new SolidColorBrush(Color.FromRgb(15, 121, 255));
                NewLab.MouseDown += TagClick;
                NewLab.Margin = new System.Windows.Thickness(0, 0, 5, 0);
                NewLab.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                NewLab.Width = 100;
                NewLab.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
                NewLab.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
                WorkingForm.FileNameSelection.Children.Add(NewLab);
            }

            if (ShowContent)
            {
                SetSearchText = WorkingForm.CurrentTag.Text;
                TableTagClick(LastClickLabByTableTag, null);
            }
        }

        public static System.Windows.Controls.Label LastClickLabByTableTag = null;

        public static DXTable SelectTable(string ParentName,string TableName)
        {
            foreach (var Get in PFMHelper.TableItems)
            {
                if (Get.TableName.Equals(ParentName))
                {
                    foreach (var GetTableItem in Get.Content.Tables)
                    {
                        if (GetTableItem.Key.Equals(TableName))
                        {
                            return GetTableItem.Value;
                        }
                    }
                }
            }
            return new DXTable();
        }

        public static DXTable SelectTable(List<TableItem> TableItems, string ParentName, string TableName)
        {
            foreach (var Get in TableItems)
            {
                if (Get.TableName.Equals(ParentName))
                {
                    foreach (var GetTableItem in Get.Content.Tables)
                    {
                        if (GetTableItem.Key.Equals(TableName))
                        {
                            return GetTableItem.Value;
                        }
                    }
                }
            }
            return new DXTable();
        }


        public static DXTable SelectTable(string TableName)
        {
            SetSearchText = WorkingForm.CurrentTag.Text;

            foreach (var Get in PFMHelper.TableItems)
            {
                if (Get.TableName.Equals(CurrentSelectTable))
                {
                    foreach (var GetTableItem in Get.Content.Tables)
                    {
                        if (GetTableItem.Key.Equals(TableName))
                        {
                            return GetTableItem.Value;
                        }
                    }
                }
            }
            return new DXTable();
        }

        public static RamSearch RamSearchs = new RamSearch();
        public static void TableTagClick(object sender, MouseButtonEventArgs e)
        {
            WorkingForm.Dispatcher.Invoke(new Action(()=> {
               WorkingForm.CurrentTagNames.Items.Clear();
            }));
            if (sender is System.Windows.Controls.Label)
            {
                System.Windows.Controls.Label LockerLab = (System.Windows.Controls.Label)sender;
                string GetText = ConvertHelper.ObjToStr(LockerLab.Content);
                string CurrentTagName = SetSearchText;

                if (DeFine.CurrentModEdit == null == false)
                {
                    DeFine.CurrentTable = CurrentSelectTable + "." + GetText;
                }
                else
                {
                    DeFine.CurrentTable = CurrentSelectTable + "." + GetText;
                }

                var GetTable = SelectTable(GetText);

                WorkingForm.CurrentCode.Document.Blocks.Clear();

                if (GetTable.Rows.Count > 0)
                {
                    if (LastClickLabByTableTag == null == false)
                    {
                        LastClickLabByTableTag.Opacity = 1;
                    }

                    LockerLab.Opacity = 0.7;

                    List<string> ColNames = new List<string>();
                    string RichCol = "";

                    foreach (var GetColName in GetTable.Cols)
                    {
                        ColNames.Add(GetColName.Key);
                        RichCol += GetColName.Key + ",";
                    }

                    if (RichCol.EndsWith(","))
                    {
                        RichCol = RichCol.Substring(0, RichCol.LastIndexOf(","));
                    }

                    Paragraph SNParagraph = new Paragraph(new Run("FullColumn->"));
                    SNParagraph.FontSize = 13;
                    WorkingForm.CurrentCode.Document.Blocks.Add(SNParagraph);

                    SNParagraph.Foreground = new SolidColorBrush(Color.FromRgb(215, 132, 2));
                    SNParagraph = new Paragraph(new Run(RichCol));
                    SNParagraph.FontSize = 11;

                    SNParagraph.Foreground = new SolidColorBrush(Color.FromRgb(215, 132, 2));
                    WorkingForm.CurrentCode.Document.Blocks.Add(SNParagraph);

                    int RowsCount = 0;

                    foreach (var GetRow in GetTable.Rows)
                    {

                        string ColItems = "";
                        int Count = 0;

                        List<string> ColKeys = new List<string>();
                       
                        foreach (var GetCol in GetTable.Rows[GetRow.Key])
                        {
                            string GetColName = ColNames[Count];
                            string GetColValue = GetCol;

                            if (GetColValue == null == false)
                            {
                                ColItems += "   " + GetColName + "=" + GetColValue + "," + "  //" + GetColName  + "\r\n";
                                ColKeys.Add(GetColName);
                                RamSearchs.Put(GetText, GetColName);
                            }

                            Count++;
                        }

                        if (ColItems.EndsWith(",\r\n"))
                        {
                            ColItems = ColItems.Substring(0, ColItems.LastIndexOf(",\r\n"));
                        }

                        if (CurrentTagName == "" || CurrentTagName.ToLower() == GetRow.Key.ToLower())
                        {
                            List<Run> Runs = new List<Run>();

                            Run NewBlock = new Run("   ");
                            NewBlock.FontSize = 16;
                            Runs.Add(NewBlock);

                            NewBlock = new Run("   " + RowsCount.ToString() + "   ");
                            NewBlock.FontSize = 16;
                            NewBlock.Background = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                            NewBlock.Foreground = new SolidColorBrush(Color.FromRgb(215, 132, 2));
                            Runs.Add(NewBlock);

                            NewBlock = new Run(" " + string.Format("Hide:{0},Show:{1}", (Count - ColKeys.Count), ColKeys.Count) + " ");
                            NewBlock.FontSize = 15;
                            NewBlock.Background = new SolidColorBrush(Color.FromRgb(215, 132, 2));
                            NewBlock.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                            Runs.Add(NewBlock);

                            NewBlock = new Run("   " + GetRow.Key + "   ");
                            NewBlock.FontSize = 18;
                            NewBlock.Foreground = new SolidColorBrush(Color.FromRgb(15, 121, 255));
                            Runs.Add(NewBlock);

                            if (CurrentSelectTable.StartsWith("ZXRules"))
                            {
                                NewBlock = new Run(" //" + LanguageHelper.Translate(GetRow.Key, LanguageHelper.LanguageType.en, LanguageHelper.LanguageType.zh));
                                NewBlock.FontSize = 15;
                                NewBlock.Foreground = new SolidColorBrush(Color.FromRgb(77, 138, 49));
                                Runs.Add(NewBlock);
                            }

                            NewBlock = new Run("\r\n" + "   " + "{\r\n");
                            NewBlock.FontSize = 18;
                            NewBlock.Foreground = new SolidColorBrush(Color.FromRgb(15, 121, 255));
                            Runs.Add(NewBlock);

                            int KeyOffset = 0;
                            foreach (var GetLine in ColItems.Split(new char[2] { '\r', '\n' }))
                            {
                                if (GetLine.Trim().Length > 0)
                                {
                                    string GetLeftText = "";
                                    string GetRightText = "";

                                    if (GetLine.Contains("//"))
                                    {
                                        GetLeftText = "   " + GetLine.Substring(0, GetLine.LastIndexOf("//"));
                                        GetRightText = GetLine.Substring(GetLine.LastIndexOf("//") + "//".Length) + "\r\n";
                                    }
                                    else
                                    {
                                        GetLeftText = GetLine + "\r\n";
                                    }

                                    if (KeyOffset == ColKeys.Count-1)
                                    {
                                        if (GetLeftText.Contains(","))
                                        {
                                            GetLeftText = GetLeftText.Substring(0, GetLeftText.LastIndexOf(","));
                                        }
                                    }

                                    NewBlock = new Run(GetLeftText);
                                    NewBlock.FontSize = 17;
                                    NewBlock.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                                    Runs.Add(NewBlock);

                                    if (GetRightText.Trim().Length > 0)
                                    {
                                        NewBlock = new Run(" //" + RamSearchs.Translate(GetText, GetRightText.Replace("\r\n","").Trim())+ "\r\n");
                                        NewBlock.FontSize = 15;
                                        NewBlock.Foreground = new SolidColorBrush(Color.FromRgb(77, 138, 49));
                                        Runs.Add(NewBlock);
                                    }

                                    KeyOffset++;
                                }
                            }

                            NewBlock = new Run("\r\n" + "   " + "}\r\n");
                            NewBlock.FontSize = 18;
                            NewBlock.Foreground = new SolidColorBrush(Color.FromRgb(15, 121, 255));
                            Runs.Add(NewBlock);

                            Paragraph NParagraph = new Paragraph();
                            NParagraph.Inlines.AddRange(Runs);

                            WorkingForm.CurrentCode.Document.Blocks.Add(NParagraph);

                            WorkingForm.Dispatcher.Invoke(new Action(() => {
                                WorkingForm.CurrentTagNames.Items.Add(GetRow.Key);
                            }));

                            RowsCount++;
                        }
                      
                    }






                    LastClickLabByTableTag = LockerLab;
                }
            }
        }

        public static System.Windows.Controls.Label LastClickLabByTag = null;

        public static string CurrentSelectTable = "";
        public static void TagClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is System.Windows.Controls.Label)
            {
                System.Windows.Controls.Label LockerLab = (System.Windows.Controls.Label)sender;
                string GetText = ConvertHelper.ObjToStr(LockerLab.Content);

                foreach (var Get in PFMHelper.TableItems)
                {
                    if (Get.TableName.Equals(GetText))
                    {
                        if (LastClickLabByTag == null == false)
                        {
                            LastClickLabByTag.Opacity = 1;
                        }
                      
                        LockerLab.Opacity = 0.7;

                        CurrentSelectTable = GetText;

                        WorkingForm.TablesSelection.Children.Clear();

                        foreach (var GetTableItem in Get.Content.Tables)
                        {
                            System.Windows.Controls.Label NewLab = new System.Windows.Controls.Label();
                            NewLab.Content = GetTableItem.Key;
                            NewLab.Background = new SolidColorBrush(Color.FromRgb(215,132,2));
                            NewLab.MouseDown += TableTagClick;
                            NewLab.Margin = new System.Windows.Thickness(0, 0, 5, 0);
                            NewLab.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                            NewLab.Padding= new System.Windows.Thickness(10, 0, 10, 0);
                            NewLab.FontSize = 10;
                            NewLab.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
                            NewLab.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
                            WorkingForm.TablesSelection.Children.Add(NewLab);
                        }

                        LastClickLabByTag = LockerLab;
                    }
                }
            }
        }

        public static void CheckGamePath(ref string SelectPath)
        {
            if (Directory.Exists(DeFine.CurrentSetting.GamePath))
            {
                if (File.Exists(DeFine.CurrentSetting.GamePath + "ZXRules.dat"))
                {
                    SelectPath = DeFine.CurrentSetting.GamePath + "ZXRules.dat";
                    return;
                }
            }
            OpenFileDialog FileDialog = new OpenFileDialog
            {
                Title = "Please Select Game Encrypt Data!",
                Filter = "Data files(*.dat)|*.dat"
            };

            FileDialog.InitialDirectory = SteamHelper.SearchSteamFile();

            if (FileDialog.ShowDialog() == DialogResult.OK)
            {
                string GetPath = FileDialog.FileName;
                GetPath = GetPath.Substring(0, GetPath.LastIndexOf(@"\") + @"\".Length) + "ZXRules.dat";
                SelectPath = GetPath;
            }
        }
        public static void ShowGameDataReader(string SourcePath)
        {
            bool SelectSucess = false;

            Thread NewCreatForm = new Thread(

                         new ThreadStart(
                         () =>
                         {
                             HookWin NHookWin = new HookWin();

                             NHookWin.Closed += (Sender, E) =>
                             NHookWin.Dispatcher.BeginInvokeShutdown(DispatcherPriority.Background);
                             NHookWin.SetSourcePath(SourcePath);

                             Dispatcher.Run();
                             SelectSucess = true;
                         }));

            NewCreatForm.ApartmentState = ApartmentState.STA;
            NewCreatForm.IsBackground = true;
            NewCreatForm.Start();

            while (!SelectSucess)
            {
                Thread.Sleep(10);
            }
        }
    }
}
