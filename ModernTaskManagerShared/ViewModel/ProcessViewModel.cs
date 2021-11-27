using CommunityToolkit.Mvvm.Input;
using ModernTaskManagerShared.Model;
using ModernTaskManagerShared.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Utils;

namespace TaskManager.ViewModel
{
    public class ProcessViewModel : ViewModelBase
    {
        public event EventHandler<EventArgs>? Killed;

        public string Name { get; private set; } = "";

        public int ID { get; private set; }

        public TimeSpan Uptime { get; private set; } = TimeSpan.Zero;

        public string UptimeString { get; private set; } = "";

        public string Ports { get; private set; } = "";

        public int? ParentID { get; private set; }

        public ProcessType Type { get; private set; }

        public string? Path { get; private set; }

        public RelayCommand KillCommand { get; }

        public RelayCommand ShowInExplorerCommand { get; }

        private ProcessModel processModel;

        public ProcessViewModel(ProcessModel process)
        {
            KillCommand = new RelayCommand(Kill);
            ShowInExplorerCommand = new RelayCommand(ShowInExplorer);
            Update(process);
        }

        [MemberNotNull(nameof(processModel))]
        public void Update(ProcessModel processModel)
        {
            this.processModel = processModel;

            Name = processModel.Name;

            ID = processModel.ID;

            Ports = string.Join(", ", processModel.Ports);
            Uptime = processModel.StartTime != default ? DateTime.Now - processModel.StartTime : TimeSpan.Zero;
            UptimeString = processModel.StartTime != default ? TimeSpanFormatter.ToReadableString(Uptime) : "";

            Type = processModel.ProcessType;

            Path = processModel.Path;
        }

        private void Kill()
        {
            try
            {
                processModel.Process.Kill();
                Killed?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                ShowMessageBox("Could not terminate process", e.Message);
            }
        }

        private void ShowInExplorer()
        {
            try
            {
                Process.Start("explorer.exe", "/select, \"" + processModel.Path + "\"");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                ShowMessageBox("Could not show in explorer", e.Message);
            }
        }

    }
}
