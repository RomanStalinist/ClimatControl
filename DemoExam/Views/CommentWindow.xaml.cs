using DemoExam.Models;
using DemoExam.ViewModels;
using System.Windows.Input;

namespace DemoExam.Views
{
    public partial class CommentWindow
    {
        public CommentWindow(Request request)
        {
            InitializeComponent();
            DataContext = new CommentWindowViewModel(request);
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();
    }
}
