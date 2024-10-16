using DemoExam.Models;
using DemoExam.ViewModels;
using System.Windows.Input;

namespace DemoExam.Views
{
    public partial class RequestWindow
    {
        public RequestWindow(Request request = null)
        {
            InitializeComponent();
            DataContext = new RequestWindowViewModel(request);

        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();
    }
}
