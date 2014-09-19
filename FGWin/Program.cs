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
        static bool EXITING = false;


        static void Main(string[] args)
        {

            new Thread(new ThreadStart(PrcMon)).Start();

            Console.WriteLine("LinkRID(WEBMON,true)");
            string input;
            string url="";

            while (true)
            {
                input = Console.ReadLine();
                if (input.StartsWith("http") && input!=url)
                {
                    url = input;
                    try
                    {
                        new Thread(new ThreadStart(delegate(){WebMon(url);})).Start();
                    }
                    catch { }
                }
            }
        }

        static void WebMon(string url)
        {
            string tmp;
            string current;
            WebClient wc = new WebClient();
            current=wc.DownloadString(url);
            tmp=current;

            while(tmp==current){
                current = wc.DownloadString(url);
                Thread.Sleep(1000);
            }

            Console.WriteLine("BROADCAST([WEB_UPDATE]" + url + ")");

        }

        static void PrcMon()
        {
            List<string> Current = new List<string>();
            Process[] processCollection = Process.GetProcesses();
            foreach (Process prc in processCollection)
            {
                Current.Add(prc.ProcessName);
            }
            while (!EXITING)
            {
                processCollection = Process.GetProcesses();
                foreach (Process prc in processCollection)
                {
                    if (prc.ProcessName != "" && !Current.Contains(prc.ProcessName))
                    {
                        try { Console.WriteLine("PRCPath=\"" + prc.MainModule.FileName + "\""); }
                        catch { }
                        Console.WriteLine("PRCName=\"" + prc.ProcessName + "\"");
                        Console.WriteLine("BROADCAST([PRC_START]" + prc.ProcessName + ")");
                        if (prc.MainWindowTitle != "")
                        {
                            Console.WriteLine("PRCTitle=\"" + prc.MainWindowTitle + "\"");
                        }

                    }
                }

                foreach (string prc in Current)
                {
                    if (Process.GetProcessesByName(prc).Count() == 0)
                    {
                        Console.WriteLine("BROADCAST([PRC_END]" + prc + ")");
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
