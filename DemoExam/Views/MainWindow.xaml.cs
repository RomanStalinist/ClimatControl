using System.Windows.Input;

namespace DemoExam.Views
{
    public partial class MainWindow
    {
        public MainWindow() => InitializeComponent();

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();
    }
}
