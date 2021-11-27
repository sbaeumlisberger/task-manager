using ModernTaskManagerShared.ViewModel;
using RestoreWindowPlace;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ModernTaskManager
{
    public partial class App : Application
    {

        public WindowPlace WindowPlace { get; }

        public App()
        {
            WindowPlace = new WindowPlace("placement.config");
            ViewModelBase.DispatcherAction = Dispatcher.Invoke;
            ViewModelBase.MessageBoxAction = (title, message) => MessageBox.Show(message, title);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var window = new MainWindow();
            window.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            this.WindowPlace.Save();
        }
    }
}
