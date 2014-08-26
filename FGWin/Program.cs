using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace FGWin
{
    class Program
    {
        /// <summary>The GetForegroundWindow function returns a handle to the foreground window.</summary>
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();


        static void Main(string[] args)
        {
            string Current = "";

            while (true)
            {
                Process[] processCollection = Process.GetProcesses();
                foreach (Process prc in processCollection)
                {
                    if (prc.MainWindowHandle == GetForegroundWindow() && prc.ProcessName!="" &&prc.ProcessName!=Current)
                    {
                        
                        Console.WriteLine("PRCName=\""+prc.ProcessName+"\"");
                        Console.WriteLine("$SYS_READY?BROADCAST(" + prc.ProcessName + " activated)");
                        if (prc.MainWindowTitle != "")
                        {
                            Console.WriteLine("PRCTitle=\"" + prc.MainWindowTitle + "\"");
                        }

                        Current = prc.ProcessName;
                    }
                }
                System.Threading.Thread.Sleep(500);
            }
        }
    }
}
