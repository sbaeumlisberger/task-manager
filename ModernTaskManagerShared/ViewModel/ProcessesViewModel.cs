using CommunityToolkit.Mvvm.ComponentModel;
using ModernTaskManagerShared.Model;
using ModernTaskManagerShared.Utils;
using ModernTaskManagerShared.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TaskManager.ViewModel;

namespace TaskManager.ViewModel
{
    public class ProcessesViewModel : ViewModelBase, ITabModel
    {
        public ObservableCollection<ProcessViewModel> Processes { get; set; } = new ObservableCollection<ProcessViewModel>();

        private Dictionary<int, ProcessViewModel> processViewModelsByID = new Dictionary<int, ProcessViewModel>();

        private string searchQuery = "";

        private readonly ProcessesService processesService = new ProcessesService();

        public void Update()
        {
            Stopwatch sw = Stopwatch.StartNew();

            var processModels = processesService.GetProcesses();

            foreach (var processViewModel in processViewModelsByID.Values)
            {
                if (!processModels.Any(processModel => processModel.ID == processViewModel.ID))
                {
                    processViewModelsByID.Remove(processViewModel.ID);
                }
            }

            Dispatch(() =>
            {
                foreach (var processModel in processModels)
                {
                    if (processViewModelsByID.TryGetValue(processModel.ID, out var processesViewModel))
                    {
                        processesViewModel.Update(processModel);
                    }
                    else
                    {
                        processViewModelsByID.Add(processModel.ID, CreateProcessViewModel(processModel));
                    }
                }

                UpdateObservableProcessesCollection();

                sw.Stop();
                Debug.WriteLine("Udpate took " + sw.ElapsedMilliseconds + "ms");
            });
        }

        private ProcessViewModel CreateProcessViewModel(ProcessModel process)
        {
            ProcessViewModel processViewModel = new ProcessViewModel(process);
            processViewModel.Killed += Process_Killed;
            return processViewModel;
        }

        private void Process_Killed(object? sender, EventArgs e)
        {
            var processViewModel = (ProcessViewModel)sender!;
            processViewModel.Killed -= Process_Killed;
            processViewModelsByID.Remove(processViewModel.ID);
            Processes.Remove(processViewModel);
        }

        public void Filter(string searchQuery)
        {
            this.searchQuery = searchQuery;
            UpdateObservableProcessesCollection();
        }

        public void UpdateObservableProcessesCollection()
        {
            List<ProcessViewModel> viewModels;
            if (searchQuery == "")
            {
                viewModels = processViewModelsByID.Values.ToList();
            }
            else
            {
                viewModels = processViewModelsByID.Values.Where(process =>
                    process.Name.Contains(searchQuery, StringComparison.CurrentCultureIgnoreCase)
                    || process.Ports.Contains(searchQuery)
                    || process.ID.ToString().Contains(searchQuery)
               ).ToList();
            }

            viewModels = viewModels.OrderBy(vm => vm.Type).ThenBy(vm => vm.Name).ToList();

            if (!Processes.Any())
            {
                Task.Run(async () =>
                {
                    foreach (var vm in viewModels)
                    {
                        await Task.Delay(1);
                        Dispatch(() => Processes.Add(vm));
                    }
                });
                //Processes = new ObservableCollection<ProcessViewModel>(viewModels);
            }
            else
            {
                Processes.MatchTo(viewModels);
            }
        }

    }
}
