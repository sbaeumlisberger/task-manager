using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernTaskManagerShared.ViewModel
{
    public class ViewModelBase : ObservableObject
    {
        public static Action<Action>? DispatcherAction { get; set; }

        public static Action<string, string>? MessageBoxAction { get; set; }

        public void Dispatch(Action action)
        {
            DispatcherAction!(action);
        }

        public void ShowMessageBox(string title, string message)
        {
            MessageBoxAction!(title, message);
        }

    }
}
