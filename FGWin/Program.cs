using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
using System.Net;
using System.IO;


namespace FGWin
{

    class Program
    {
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }

        static bool EXITING = false;
        static int AZUSAPid = -1;

        static void Main(string[] args)
        {

            while (AZUSAPid == -1)
            {
                WriteLine("GetAzusaPid()");
                try
                {
                    AZUSAPid=Convert.ToInt32(Console.ReadLine());
                    break;
                }
                catch
                {
                }
            }

            new Thread(new ThreadStart(PrcMon)).Start();

            WriteLine("LinkRID(WEBMON,true)");
            string input;
            string url = "";

            while (true)
            {
                input = System.Web.HttpUtility.UrlDecode(Console.ReadLine());
                if (input.StartsWith("http") && input != url)
                {
                    url = input;
                    try
                    {
                        new Thread(new ThreadStart(delegate() { WebMon(url); })).Start();
                    }
                    catch { }
                }
            }
        }

        static void WriteLine(string msg)
        {
            Console.WriteLine(System.Web.HttpUtility.UrlEncode(msg));
        }

        static void WebMon(string url)
        {
            string tmp;
            string current;
            WebClient wc = new WebClient();
            current = wc.DownloadString(url);
            tmp = current;

            while (true)
            {
                current = wc.DownloadString(url);
                if (tmp == current)
                {
                    Thread.Sleep(1000);
                }
                else
                {
                    WriteLine("EVENT([WEB_UPDATE]" + url + ")");
                    tmp = current;
                }
            }
        }

        static void PrcMon()
        {
            RECT rect;
            List<string> Current = new List<string>();
            Process[] processCollection = Process.GetProcesses();
            foreach (Process prc in processCollection)
            {
                Current.Add(prc.ProcessName);
            }
            while (!EXITING)
            {
                try { Process.GetProcessById(AZUSAPid); }
                catch
                {
                    Environment.Exit(0);
                    break;
                }

                processCollection = Process.GetProcesses();
                foreach (Process prc in processCollection)
                {
                    if (prc.ProcessName != "" && !Current.Contains(prc.ProcessName))
                    {

                        try
                        {
                            GetWindowRect(prc.MainWindowHandle, out rect);
                            if (rect.Top != rect.Bottom && rect.Left != rect.Right)
                            {

                                try { WriteLine("PRCPath=\"" + prc.MainModule.FileName + "\""); }
                                catch { }
                                WriteLine("PRCName=\"" + prc.ProcessName + "\"");
                                WriteLine("EVENT([PRC_START]" + prc.ProcessName + ")");
                                if (prc.MainWindowTitle != "")
                                {
                                    WriteLine("PRCTitle=\"" + prc.MainWindowTitle + "\"");
                                }
                            }
                        }
                        catch
                        {
                        }

                    }
                }

                foreach (string prc in Current)
                {
                    if (Process.GetProcessesByName(prc).Count() == 0)
                    {
                        WriteLine("EVENT([PRC_END]" + prc + ")");
                    }
                }

                Current.Clear();

                foreach (Process prc in processCollection)
                {
                    Current.Add(prc.ProcessName);
                }

                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}
