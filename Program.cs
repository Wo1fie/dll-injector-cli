using System;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace dll_injector
{
    internal class Program
    {
        static Version version = new Version(1,0);
       static int Main(string[] args)
        {
            Process process = null;
            FileInfo dllFile = null;
            for(int x =0;x < args.Length; x++)
            {
                string arg = args[x];
                string value;
                switch (arg)
                {
                    case "-pid":
                        value = args.Length > x + 1 ? args[++x] : null;
                        int pid = 0;
                        if (int.TryParse(value, out pid))
                        {
                            process = Process.GetProcessById(pid);
                        }
                        if(process == null)
                        {
                            Console.Error.WriteLine($"Process not found using pid {value}");
                            return -1;
                        }
                        break;
                    case "-pname":
                        value = args.Length > x + 1 ? args[++x] : null;
                        if(value != null)
                        {
                            Process[] processScan = Process.GetProcessesByName(value);
                            if(processScan.Length > 0)
                            {
                                process = processScan[0];
                            }
                        }
                        if(process == null)
                        {
                            Console.Error.WriteLine($"Process not found using process name {value}");
                        }
                        break;
                    case "-dll":
                        value = args.Length > x + 1 ? args[++x] : null;
                        dllFile = new FileInfo(value);
                        if (!dllFile.Exists)
                        {
                            Console.Error.WriteLine($"DLL File {value} does not exist");
                            return -2;
                        }
                        break;
                    case "/?":
                    case "-help":
                        DisplayHelp();
                        break;
                    default:
                        Console.WriteLine($"Unknown command line argument {arg}");
                        break;
                }
            }
            if(process == null || dllFile == null)
            {
                DisplayHelp();
                return -3;
            }
            else
            {
                return (int)DllInjector.InjectDll(process, dllFile);
            }
        }
        static void DisplayHelp()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Simple DLL Injector CLI");
            sb.AppendLine($"Author: Wo1fie");
            sb.AppendLine($"Version: {version}");
            sb.AppendLine();

            sb.AppendLine("Usage: dll-injector.exe [-pid <process Id> | -pname <process name>] -dll \"<file path>\"");
            sb.AppendLine("Examples:");
            sb.AppendLine("dll-injector.exe -pid 9999 -dll \"injectable.dll\"");
            sb.AppendLine("dll-injector.exe -pname \"notepad.exe\" -dll \"injectable.dll\"");
            sb.AppendLine();
            sb.AppendLine("\"-help\" or \"/?\": Displays this help message");
            sb.AppendLine();
            sb.AppendLine("Return values:");
            sb.AppendLine("-1 Process not found");
            sb.AppendLine("-2 DLL File not found");
            sb.AppendLine("-3 Command line arguments missing");
            sb.AppendLine("0 Success");
            for(int x = 1;x <= (int)DllInjector.InjectReturnStatus.VirtualFreeEx; x++)
            {
                DllInjector.InjectReturnStatus status = (DllInjector.InjectReturnStatus)x;
                sb.AppendLine($"{x} {status} failed");
            }
            Console.WriteLine(sb.ToString());
        }
    }
}
