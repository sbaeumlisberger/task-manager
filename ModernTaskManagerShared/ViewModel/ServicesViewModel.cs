using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ModernTaskManagerShared.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using TaskManager.ViewModel;

namespace TaskManager.ViewModel
{
    public class ServicesViewModel : ViewModelBase, ITabModel
    {
        public IList<ServiceViewModel> Services { get; set; } = new List<ServiceViewModel>(0);

        private List<ServiceViewModel> services = new List<ServiceViewModel>(0);

        public void Update()
        {
            services = ServiceController.GetServices().Select(service => new ServiceViewModel(service)).ToList();
            Dispatch(() =>
            {
                Services = services;
            });
        }

        public void Filter(string searchQuery)
        {
            Services = services.Where(service => service.Name.Contains(searchQuery)).ToList(); ;
        }

    }
}
