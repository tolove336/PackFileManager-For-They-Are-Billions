using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using System.Threading;

using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;

namespace PackFileManager.FormManager
{
    public class NotifyIconHelper
    {
        public static NotifyIcon OneNotifyIcon = null;
        public static MainWindow CurrentGui = null;
        public static void Init(MainWindow Gui,string Tittle)
        {
            Gui.Title = Tittle;
            CurrentGui = Gui;

            OneNotifyIcon = new NotifyIcon();

            OneNotifyIcon.BalloonTipText = Tittle;
            OneNotifyIcon.Text = Tittle;
            OneNotifyIcon.Visible = true;
            OneNotifyIcon.MouseClick += ShowThis;

            string IcoPath = System.Environment.CurrentDirectory + @"\" + "logo.ico";

            if (File.Exists(IcoPath))
            {
                OneNotifyIcon.Icon = new System.Drawing.Icon(IcoPath);
            }
            else
            {
             
            }

            System.Windows.Forms.MenuItem ExitNow = new System.Windows.Forms.MenuItem("退出");

            ExitNow.Click += new EventHandler(QuickExit);

            OneNotifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(ShowThis);


            System.Windows.Forms.MenuItem[] Childen = new System.Windows.Forms.MenuItem[] { ExitNow };

            OneNotifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu(Childen);
            
        }
        public static void ShowThis(object sender, MouseEventArgs e)
        {
            if (CurrentGui == null == false)
            {
                CurrentGui.Dispatcher.Invoke(new Action(()=> {

                    CurrentGui.WindowState = WindowState.Normal;

                    CurrentGui.Show();

                }));
            }
        }
      
        public static void ShowMsgInNotifyIcon(string ActionType, string ActionMessage, int MsgType, int TimeOut = 1000)
        {
            OneNotifyIcon.ShowBalloonTip(TimeOut, ActionType, ActionMessage.Replace("_", "\r\n"), (ToolTipIcon)MsgType);
        }
      
        public static void QuickExit(object sender, EventArgs e)
        {
            DeFine.ExitThis();
        }
    }
}
