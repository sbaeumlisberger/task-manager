using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.ViewModel
{
    public class ServiceViewModel
    {
        public string Name { get; }

        public string Description { get; private set; } = "";

        public string Status { get; private set; } = "";

        public int? PID { get; private set; }

        public RelayCommand StartCommand { get; }

        public RelayCommand StopCommand { get; }

        public RelayCommand RestartCommand { get; }

        public RelayCommand KillCommand { get; }

        private readonly ServiceController service;

        public ServiceViewModel(ServiceController service)
        {
            this.service = service;

            StartCommand = new RelayCommand(Start, () => service.Status == ServiceControllerStatus.Stopped);
            StopCommand = new RelayCommand(Stop, () => service.Status == ServiceControllerStatus.Running);
            RestartCommand = new RelayCommand(Restart, () => service.Status == ServiceControllerStatus.Running);
            KillCommand = new RelayCommand(Kill, () => PID is not null);

            Name = service.ServiceName;    

            LoadProperties();
        }

        private void LoadProperties()
        {
            Status = service.Status.ToString();

            string objPath = string.Format("Win32_Service.Name='{0}'", service.ServiceName);
            using (ManagementObject service = new ManagementObject(new ManagementPath(objPath)))
            {
                Description = (string)service["Description"];
                int pid = Convert.ToInt32(service["ProcessId"]);
                PID = pid != 0 ? pid : null;
            }
        }

        private void Start()
        {
            service.Start();
            Update();
        }

        private void Stop()
        {
            service.Stop();
            Update();
        }

        private void Restart()
        {
            service.Stop();
            service.Start();
            Update();
        }

        private void Kill()
        {
            if (PID is int pid)
            {
                Process.GetProcessById(pid).Kill(entireProcessTree: true);
                Update();
            }
        }

        private void Update()
        {
            service.Refresh();

            LoadProperties();

            StartCommand.NotifyCanExecuteChanged();
            StopCommand.NotifyCanExecuteChanged();
            RestartCommand.NotifyCanExecuteChanged();
            KillCommand.NotifyCanExecuteChanged();
        }

    }
}
