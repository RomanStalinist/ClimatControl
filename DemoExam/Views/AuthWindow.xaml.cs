using System.ComponentModel;
using System.Windows;

namespace DemoExam.Views
{
    public partial class AuthWindow
    {
        public bool WillAsk { get; set; } = true;

        public AuthWindow() => InitializeComponent();

        protected override void OnClosing(CancelEventArgs e)
            => e.Cancel = WillAsk && MessageBox.Show(
                "Вы уверены, что хотите выйти?", "Выход", MessageBoxButton.YesNo, MessageBoxImage.Question)
                    != MessageBoxResult.Yes;
    }
}
