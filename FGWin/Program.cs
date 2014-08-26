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
        


        static void Main(string[] args)
        {
            List<string> Current = new List<string>();
            Process[] processCollection = Process.GetProcesses();
            foreach (Process prc in processCollection)
            {
                Current.Add(prc.ProcessName);
            }

            while (true)
            {
                processCollection = Process.GetProcesses();
                foreach (Process prc in processCollection)
                {
                    if (prc.ProcessName!="" && !Current.Contains(prc.ProcessName))
                    {
                        try { Console.WriteLine("PRCPath=\"" + prc.MainModule.FileName + "\""); }
                        catch { }
                        Console.WriteLine("PRCName=\""+prc.ProcessName+"\"");
                        Console.WriteLine("BROADCAST(PRCName+\" is activated\")");
                        if (prc.MainWindowTitle != "")
                        {
                            Console.WriteLine("PRCTitle=\"" + prc.MainWindowTitle + "\"");
                        }
                        
                    }
                }

                Current.Clear();

                foreach (Process prc in processCollection)
                {
                    Current.Add(prc.ProcessName);
                }

                System.Threading.Thread.Sleep(500);
            }
        }
    }
}
