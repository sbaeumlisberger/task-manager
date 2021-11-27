using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TaskManager.ViewModel;

namespace ModernTaskManager
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = new MainWindowModel();
            InitializeComponent();
            ((App)Application.Current).WindowPlace.Register(this);
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ((MainWindowModel)DataContext).SelectedTab = (ITabModel)((FrameworkElement)((TabItem)((TabControl)sender).SelectedItem).Content).DataContext;
        }
    }
}
