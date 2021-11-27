using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ModernTaskManagerShared.Model
{
    public class ProcessesService
    {

        public List<ProcessModel> GetProcesses()
        {
            Stopwatch sw = Stopwatch.StartNew();
            var processes = Process.GetProcesses();
            sw.Stop();
            Debug.WriteLine("GetProcesses took " + sw.ElapsedMilliseconds + "ms");
            sw.Restart();
            var ports = GetNetStatPorts();
            sw.Stop();
            Debug.WriteLine("GetNetStatPorts took " + sw.ElapsedMilliseconds + "ms");
            sw.Restart();
            var models = processes.Select(process => new ProcessModel(process, ports)).ToList();
            sw.Stop();
            Debug.WriteLine("Mapping to models took " + sw.ElapsedMilliseconds + "ms");
            return models;
        }

        private List<(int Port, int PID)> GetNetStatPorts()
        {
            var Ports = new List<(int Port, int PID)>();

            using (Process p = new Process())
            {
                ProcessStartInfo ps = new ProcessStartInfo();
                ps.Arguments = "-a -n -o";
                ps.FileName = "netstat.exe";
                ps.UseShellExecute = false;
                ps.CreateNoWindow = true;
                ps.RedirectStandardInput = true;
                ps.RedirectStandardOutput = true;
                ps.RedirectStandardError = true;

                p.StartInfo = ps;
                p.Start();

                StreamReader stdOutput = p.StandardOutput;
                StreamReader stdError = p.StandardError;

                string content = stdOutput.ReadToEnd() + stdError.ReadToEnd();

                string exitStatus = p.ExitCode.ToString();

                if (exitStatus != "0")
                {
                    throw new Exception($"Process exited with code {exitStatus}");
                }

                string[] rows = Regex.Split(content, "\r\n");

                foreach (string row in rows)
                {
                    string[] tokens = Regex.Split(row, "\\s+");

                    if (tokens.Length > 4 && (tokens[1].Equals("UDP") || tokens[1].Equals("TCP")))
                    {
                        string localAddress = Regex.Replace(tokens[2], @"\[(.*?)\]", "1.1.1.1");
                        int portNumber = int.Parse(localAddress.Split(':')[1]);
                        int pid = tokens[1] == "UDP" ? Convert.ToInt32(tokens[4]) : Convert.ToInt32(tokens[5]);
                        Ports.Add((portNumber, pid));
                    }
                }
            }

            return Ports;
        }

        //[DllImport("ntdll.dll")]
        //private static extern int NtQueryInformationProcess(IntPtr processHandle, int processInformationClass, ref ProcessInformation processInformation, int processInformationLength, out int returnLength);

        //[StructLayout(LayoutKind.Sequential)]
        //public struct ProcessInformation
        //{
        //    // These members must match PROCESS_BASIC_INFORMATION
        //    internal IntPtr Reserved1;
        //    internal IntPtr PebBaseAddress;
        //    internal IntPtr Reserved2_0;
        //    internal IntPtr Reserved2_1;
        //    internal IntPtr UniqueProcessId;
        //    internal IntPtr InheritedFromUniqueProcessId;
        //}

        //public static int? GetParentProcess(Process process)
        //{
        //    ProcessInformation pi = new ProcessInformation();
        //    int returnLength;
        //    try
        //    {
        //        int status = NtQueryInformationProcess(process.Handle, 0, ref pi, Marshal.SizeOf(pi), out returnLength);
        //        if (status != 0)
        //        {
        //            throw new Win32Exception(status);
        //        }
        //        return pi.InheritedFromUniqueProcessId.ToInt32();
        //        // return Process.GetProcessById(pi.InheritedFromUniqueProcessId.ToInt32()).Id;
        //    }
        //    catch (ArgumentException)
        //    {
        //        // not found
        //        return null;
        //    }
        //    catch (Exception)
        //    {
        //        // TODO
        //        return null;
        //    }
        //}

    }
}
