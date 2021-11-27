using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Utils;

namespace ModernTaskManagerShared.Model
{
    public class ProcessModel
    {
        public int ID { get; }

        public string Name { get; }

        public string? Path { get; }

        public DateTime StartTime { get; }

        public IList<int> Ports { get; }

        public ProcessType ProcessType { get; }

        public Process Process { get; }

        public ProcessModel(Process process, List<(int Port, int PID)> ports)
        {
            Process = process;

            if (string.IsNullOrEmpty(process.MainWindowTitle))
            {
                Name = process.ProcessName;
            }
            else
            {
                Name = process.MainWindowTitle + " (" + process.ProcessName + ")";
            }

            try
            {
                Path = process.MainModule?.FileName;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Could not get path for process {Name}: {e.Message}");
            }

            ID = process.Id;

            Ports = ports.Where(port => port.PID == ID).Select(port => port.Port).Distinct().ToList();

            try
            {
                StartTime = process.StartTime;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Could not get start time for process {Name}: {e.Message}");
            }

            ProcessType = process.MainWindowHandle != IntPtr.Zero ? ProcessType.Application : ProcessType.BackgroundProcess;
        }

    }

}
