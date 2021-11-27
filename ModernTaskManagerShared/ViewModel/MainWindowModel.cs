using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TaskManager.Utils;

namespace TaskManager.ViewModel
{
    public class MainWindowModel : ObservableObject
    {

        public ProcessesViewModel ProcessesViewModel { get; } = new ProcessesViewModel();

        public ServicesViewModel ServicesViewModel { get; } = new ServicesViewModel();

        public ITabModel SelectedTab { get; set; }

        public string SearchQuery { get; set; } = "";

        private readonly Timer updateTimer;

        public MainWindowModel()
        {
            SelectedTab = ProcessesViewModel;
            PropertyChanged += MainWindowModel_PropertyChanged;
            updateTimer = ThreadPoolTimer.Create(SelectedTab.Update, TimeSpan.FromSeconds(10));
        }

        private async void MainWindowModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SearchQuery))
            {
                SelectedTab.Filter(SearchQuery ?? "");
            }
            if (e.PropertyName == nameof(SelectedTab))
            {
                await Task.Run(SelectedTab.Update);
                SelectedTab.Filter(SearchQuery ?? "");
            }
        }
    }
}
