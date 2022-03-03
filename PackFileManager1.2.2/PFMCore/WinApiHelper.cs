using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PackFileManager.PFMCore
{
    public class WinApiHelper
    {
        const int WM_GETTEXT = 0x000D;
        const int WM_GETTEXTLENGTH = 0x000E;

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(int hwnd, int wMsg, int wParam, Byte[] lParam);


        [DllImport("user32.dll", EntryPoint = "SendMessageA")]
        private static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, StringBuilder lParam);


        public const int KeyUp = 2;
        public const int KeyDown = 0;
        public const byte vbKeyTab = 0x9;        // TAB 键
        public const byte vbKeyClear = 0xC;      // CLEAR 键
        public const byte vbKeyReturn = 0xD;     // ENTER 键
        public const byte vbKeyShift = 0x10;     // SHIFT 键
        public const byte vbKeyControl = 0x11;   // CTRL 键
        public const byte vbKeyAlt = 18;         // Alt 键  (键码18)
        public const byte vbKeyMenu = 0x12;      // MENU 键
        public const byte vbKeyPause = 0x13;     // PAUSE 键
        public const byte vbKeyCapital = 0x14;   // CAPS LOCK 键
        public const byte vbKeyEscape = 0x1B;    // ESC 键
        public const byte vbKeySpace = 0x20;     // SPACEBAR 键
        public const byte vbKeyPageUp = 0x21;    // PAGE UP 键

        public class POINTAPI
        {
            public Int32 x;
            public Int32 y;
        }

        public enum MouseEventFlag : uint
        {
            Move = 0x0001,
            LeftDown = 0x0002,
            LeftUp = 0x0004,
            RightDown = 0x0008,
            RightUp = 0x0010,
            MiddleDown = 0x0020,
            MiddleUp = 0x0040,
            XDown = 0x0080,
            XUp = 0x0100,
            Wheel = 0x0800,
            VirtualDesk = 0x4000,
            Absolute = 0x8000
        }

        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern void mouse_event(MouseEventFlag flags, int dx, int dy, uint data, UIntPtr extraInfo);

        [DllImport("user32.dll")]
        public static extern int SetCursorPos(int x, int y);

        [DllImport("User32.dll")]
        private static extern Int32 GetCursorPos(POINTAPI lpPoint);

        [DllImport("user32.dll")]
        private static extern int EnumWindows(deleWindowsProc hWnd,int lParam);

        public delegate bool deleWindowsProc(IntPtr hWnd,int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpText, int nCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpText, int nCount);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, uint hwndChildAfter, string lpszClass, string lpszWindow);


        [DllImport("user32.dll", EntryPoint = "FindWindow", CharSet = CharSet.Auto)]
        public extern static IntPtr FindWindow(string classname, string captionName);

        public static deleWindowsProc EnumFunction;

        public static List<IntPtr> AllWindows = new List<IntPtr>();

        public static IntPtr FindWindowExByDimStrIntoWindow(string dimStr)
        {
            IntPtr iResult = IntPtr.Zero;

            string controlTitle = ""; //控件完全标题

            // 枚举子窗体，查找控件句柄
            int i = EnumWindows(
            (h, l) =>
            {
                int cTxtLen;
                if (IsWindowVisible(h))
                {
                    //对每一个枚举窗口的处理
                    cTxtLen = SendMessage(h, WM_GETTEXTLENGTH, 0, 0); //获取内容长度
                    Byte[] byt = new Byte[cTxtLen];
                    SendMessage((int)h, WM_GETTEXT, cTxtLen + 1, byt); //获取内容
                    string str = Encoding.Default.GetString(byt);
                    if (str.ToString().Contains(dimStr))
                    {
                        iResult = h;
                        controlTitle = str.ToString();
                        return false;
                    }
                    else
                        return true;
                }
                else
                    return true;

            },
            0);

            // 返回查找结果
            return iResult;
        }

        public static bool FEnumWindows(IntPtr Hwnd,int lParam)
        {
            try 
            { 
            StringBuilder WindowText = new StringBuilder(255);
            GetWindowText(Hwnd, WindowText, 255);

            if (WindowText.ToString().Contains(FEnumFindName))
            {
                AllWindows.Add(Hwnd);
                return false;
            }

            return true;
            }
            catch { return false; }
        }

        public static string FEnumFindName = "";
        public static IntPtr FindWindowsByTittle(string Tittle)
        {
            FEnumFindName = Tittle;
            AllWindows.Clear();
            EnumFunction += FEnumWindows;

            IntPtr Result = IntPtr.Zero;

            EnumWindows(EnumFunction,0);

            if (AllWindows.Count > 0)
            {
                return AllWindows[0];
            }
         
            return IntPtr.Zero;
        }

    }
}
