using PackFileManager.FormManager;
using PackFileManager.PFMCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PackFileManager
{
    /// <summary>
    /// HookWin.xaml 的交互逻辑
    /// </summary>
    public partial class HookWin : Window
    {
        public HookWin()
        {
            InitializeComponent();
        }

        public void ShowMsg(string Msg)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                CurrentMsg.Content = Msg;
            }));
        }

        public string SourcePath = "";
        public void SetSourcePath(string NewPath)
        {
            SourcePath = NewPath;
            this.Show();
        }
        public MyProcessBar ThisBar = null;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string GetSourceName = SourcePath.Substring(SourcePath.LastIndexOf(@"\"));
            string GetSourcePath = SourcePath.Substring(0, SourcePath.LastIndexOf(@"\") + @"\".Length);

            ProcessControl.SetWorkingForm(this);

            ThisBar = ProcessControl.CreatProcessBarControl("ProcessBar", NameHelper.GetNewControl(), 300, 15, 5);
            ThisBar.Start(1600, Color.FromRgb(255, 255, 255), 10);

            Thread ThreadUI = new Thread(
                   new ThreadStart(
                   () =>
                   {
                       HookHelper.InstallState = false;

                       Thread.Sleep(1000);

                       ShowMsg("Install Hook Please Wait!");

                       HookHelper.InstallHook(GetSourcePath);

                       Thread.Sleep(1000);

                       HookHelper.RunGame(GetSourcePath);

                       Thread.Sleep(1000);

                       ShowMsg("Get Instance Hwnd PleaseWait");

                       bool CanHook = false;

                       while (!CanHook)
                       {
                           IntPtr GetHwnd = IntPtr.Zero;
                           GetHwnd = WinApiHelper.FindWindowExByDimStrIntoWindow("They Are Billions");

                           if (((int)GetHwnd) > 0)
                           {
                               ShowMsg(string.Format("Windows Hwnd Is {0}", GetHwnd));
                               Thread.Sleep(2000);
                               CanHook = true;
                           }

                           Thread.Sleep(100);
                       }

                       Thread.Sleep(1000);

                       int MaxTry = 120;

                       NextGet:

                       ShowMsg(string.Format("Wait Target Program Return!({0})", MaxTry));

                       List<string> CurrentPasswords = new List<string>();
                       CurrentPasswords = HookHelper.GetFilePassword(GetSourcePath);

                       if (CurrentPasswords.Count >= 20)
                       {
                           foreach (var GetItem in CurrentPasswords)
                           {
                               string GetPasswordName = GetItem;

                               if (GetPasswordName.Contains("OnePas"))
                               {
                                   GetPasswordName = GetPasswordName.Substring(0, GetPasswordName.IndexOf("OnePas"));
                               }
                               else
                               if (GetPasswordName.Contains("NoPas"))
                               {
                                   GetPasswordName = GetPasswordName.Substring(0, GetPasswordName.IndexOf("NoPas"));
                               }

                               string GetPasswordKey = GetItem.Split('>')[1].Replace("]", "");

                               DeFine.CurrentPassword.AddPassword(GetPasswordName, GetPasswordKey);

                               Thread.Sleep(100);

                               ShowMsg(string.Format("NewKey File:{0},Key:{1}", GetPasswordName, GetPasswordKey));
                           }
                       }
                       else
                       {
                           Thread.Sleep(1000);

                           if (MaxTry > 0)
                           {
                               MaxTry--;

                               goto NextGet;
                           }
                           else
                           {
                               foreach (var GetItem in CurrentPasswords)
                               {
                                   string GetPasswordName = GetItem;

                                   if (GetPasswordName.Contains("OnePas"))
                                   {
                                       GetPasswordName = GetPasswordName.Substring(0, GetPasswordName.IndexOf("OnePas"));
                                   }
                                   else
                                   if (GetPasswordName.Contains("NoPas"))
                                   {
                                       GetPasswordName = GetPasswordName.Substring(0, GetPasswordName.IndexOf("NoPas"));
                                   }

                                   string GetPasswordKey = GetItem.Split('>')[1].Replace("]", "");

                                   DeFine.CurrentPassword.AddPassword(GetPasswordName, GetPasswordKey);

                                   Thread.Sleep(100);

                                   ShowMsg(string.Format("NewKey File:{0},Key:{1}", GetPasswordName, GetPasswordKey));
                               }
                           }
                       }

                       ShowMsg("Get Key Sucess!");

                       Thread.Sleep(1000);

                       HookHelper.KillGame();

                       ThisBar.Stop();

                       this.Dispatcher.Invoke(new Action(() =>
                       {

                           this.Close();

                       }));
                   }));

            ThreadUI.Start();
            ThreadUI.IsBackground = true;
        }
    }
}
