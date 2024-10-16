using DemoExam.Views;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace DemoExam.ViewModels
{
    public class QuitPageViewModel : ViewModel
    {
        public ICommand QuitCommand { get; }
        
        public QuitPageViewModel()
        {
            QuitCommand = new RelayCommand(Quit);
        }

        private void Quit(object parameter)
        {
            App.User = null;
            var aWindow = new AuthWindow();
            aWindow.Show();
            Application.Current.MainWindow = aWindow;
            Application.Current.Windows.OfType<MainWindow>().ToList().ForEach(w => w.Close());
        }
    }
}
