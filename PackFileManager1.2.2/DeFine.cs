using DXVision;
using ICSharpCode.AvalonEdit;
using PackFileManager.ConvertManager;
using PackFileManager.FileManager;
using PackFileManager.FormManager;
using PackFileManager.PINManager;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PackFileManager
{
    public class DeFine
    {
        public static string CurrentVersion = "2.1.15A";

        public static SysSetting CurrentSetting = new SysSetting();

        public static string GamePath = "";

        public static string CreatPath = Environment.CurrentDirectory + "\\CreatedFile\\";

        public static string CachePath = Environment.CurrentDirectory + "\\Cache\\";

        public static string HookPath = Environment.CurrentDirectory + "\\HookFile\\";

        public static string GameObjPath = Environment.CurrentDirectory + "\\GameObj\\";

        public static PasswordArray CurrentPassword = new PasswordArray();

        public static ModEdit CurrentModEdit = null;

        public static List<GameStateAction> GameStartActions = new List<GameStateAction>();
        public static List<GameStateAction> GameEndActions = new List<GameStateAction>();

        private static bool LockerGameListenService = false;
        public static void StartGameListenService(bool Check)
        {
            if (Check)
            {
                if (!LockerGameListenService)
                {
                    LockerGameListenService = true;

                    new Thread(() => {

                        bool FindProcess = false;

                        while (LockerGameListenService)
                        {
                            Thread.Sleep(1000);
                           
                            var CurrentProcess = Process.GetProcessesByName("TheyAreBillions");

                            if (CurrentProcess.Length > 0)
                            {
                                if (!FindProcess)
                                {
                                    FindProcess = true;

                                    for (int i = 0; i < GameStartActions.Count; i++)
                                    {
                                        if (GameStartActions[i].MaxCallCount == -1)
                                        {
                                            GameStartActions[i].OneAction.Invoke(CurrentProcess[0].Id);
                                        }
                                        else
                                        if (GameStartActions[i].MaxCallCount > 0)
                                        {
                                            GameStartActions[i].MaxCallCount--;
                                            GameStartActions[i].OneAction.Invoke(CurrentProcess[0].Id);
                                        }
                                    }
                                }

                                FormHelper.WorkingForm.Dispatcher.Invoke(new Action(()=> {
                                    FormHelper.WorkingForm.OneAction.Fill = new SolidColorBrush(Color.FromRgb(8, 128, 0));
                                    FormHelper.WorkingForm.OneActionMsg.Content = "Game Is Run! LockerID:"+ CurrentProcess[0].Id;
                                }));
                            }
                            else
                            {
                                if (FindProcess)
                                {
                                    FindProcess = false;

                                    for (int i = 0; i < GameEndActions.Count; i++)
                                    {
                                        if (GameEndActions[i].MaxCallCount == -1)
                                        {
                                            GameEndActions[i].OneAction.Invoke(-1);
                                        }
                                        else
                                        if (GameEndActions[i].MaxCallCount > 0)
                                        {
                                            GameEndActions[i].MaxCallCount--;
                                            GameEndActions[i].OneAction.Invoke(-1);
                                        }
                                    }

                                    FormHelper.WorkingForm.Dispatcher.Invoke(new Action(() =>
                                    {
                                        FormHelper.WorkingForm.OneAction.Fill = new SolidColorBrush(Color.FromRgb(159, 3, 3));
                                        FormHelper.WorkingForm.OneActionMsg.Content = "Game Not Run!";
                                    }));
                                }
                            }
                            
                        }

                    }).Start();

                }
            }
            else
            {
                LockerGameListenService = false;
            }
        }

        public static bool LockerCheckProcess = false;

        public static TextEditor ActiveIDE = null;

        public static DXTable LastTable;

        public static string CurrentTable = "";

        public static string ModEditTittle = "NewMod";
        public static void StartCheckProcessService(bool Check)
        {
            int CurrentID = Process.GetCurrentProcess().Id;
            if (Check)
            {
                if (!LockerCheckProcess)
                {
                    LockerCheckProcess = true;

                    new Thread(() =>
                    {

                        Thread.Sleep(3000);

                        while (true)
                        {
                            Thread.Sleep(1000);

                            foreach (var Get in Process.GetProcesses())
                            {
                                if (Get.ProcessName.Equals("PackFileManager"))
                                {
                                    if (Get.Id == CurrentID == false)
                                    {
                                        Thread.Sleep(300);
                                        Get.Kill();
                                        NotifyIconHelper.ShowThis(null, null);
                                    }
                                }
                            }

                        }

                    }).Start();

                }
            }

        }



        public static void ShowModEdit()
        {
            if (CurrentModEdit == null)
            {
                CurrentModEdit = new ModEdit();
                CurrentModEdit.Show();
                CurrentModEdit.ActiveThis();
            }
            else
            {
                CurrentModEdit.Show();
                CurrentModEdit.ActiveThis();
            }
        }

        public static void ExitThis()
        {
            try
            {

                NotifyIconHelper.OneNotifyIcon.Dispose();

            }
            catch { }

            System.Environment.Exit(System.Environment.ExitCode);
        }

        public class PasswordArray
        { 

            public List<PasswordItem> CurrentPassword = new List<PasswordItem>();

            public PasswordArray(List<PasswordItem> CurrentPassword)
            {
                this.CurrentPassword.AddRange(CurrentPassword);
            }

            public PasswordArray()
            { 
            
            }

            public PasswordArray(string SaveKey)
            {
                try 
                {
                CurrentPassword = JsonCore.JsonHelper.ProcessToJson<List<PasswordItem>>(SaveKey);
                }
                catch { }
            }

            public string GetJson()
            {
                return JsonCore.JsonHelper.GetJson(this.CurrentPassword);
            }

            public int AddPassword(string FileName, string FileKey)
            {
                int State = 0;
                for (int i = 0; i < CurrentPassword.Count; i++)
                {
                    if (CurrentPassword[i].FileName.Equals(FileName))
                    {
                        CurrentPassword[i].FileKey = FileKey;
                        State = 1;
                        break;
                    }
                }

                if (State == 0)
                {
                    CurrentPassword.Add(new PasswordItem(FileName,FileKey));
                    State = 2;
                }

                return State;
            }

            public string GetPassword(string FileName)
            {
                foreach (var Get in CurrentPassword)
                {
                    if (Get.FileName.Equals(FileName))
                    {
                        return Get.FileKey;
                    }
                }

                return string.Empty;
            }
        }

        public class PasswordItem
        {
            public string FileName = "";
            public string FileKey = "";
            public PasswordItem(string FileName,string FileKey)
            {
                this.FileName = FileName;
                this.FileKey = FileKey;
            }
        }

        public class GameStateAction
        {
            public int ActionID = 0;
            public Action<int> OneAction = null;
            public int MaxCallCount = 0;

            public GameStateAction(Action<int> OneAction, int MaxCallCount = 1)
            {
                this.OneAction = OneAction;
                this.MaxCallCount = MaxCallCount;
                this.ActionID = DateTime.Now.GetHashCode() + new Random(Guid.NewGuid().GetHashCode()).Next(10,100);
            }
        }


        public class SysSetting
        {
            public int Rowid = 1;
            public string KeySave = "";
            public string GamePath = "";

            public bool GetSetting()
            {
                string SqlOrder = "Select Rowid,* From SysSetting Where Rowid = {0}";
                DataTable NTable = SQLiteHelper.ExecuteQuery(string.Format(SqlOrder,this.Rowid));
               
                if (NTable.Rows.Count > 0)
                {
                    this.KeySave = PINHelper.Decrypt(ConvertHelper.ObjToStr(NTable.Rows[0]["KeySave"]));
                    this.GamePath = ConvertHelper.ObjToStr(NTable.Rows[0]["GamePath"]);
                    return true;
                }

                return false;
            }

            public bool SetSetting()
            {
                string SqlOrder = "UPDate SysSetting Set KeySave = '{1}' ,GamePath = '{2}'  Where Rowid = {0}";

                int State = SQLiteHelper.ExecuteNonQuery(string.Format(SqlOrder,this.Rowid,PINHelper.Encrypt(this.KeySave),this.GamePath));

                if (State == 0 == false)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
