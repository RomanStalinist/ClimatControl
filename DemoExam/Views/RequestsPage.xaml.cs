using DemoExam.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace DemoExam.Views
{
    public partial class RequestsPage
    {
        public RequestsPage()
        {
            InitializeComponent();
        }

        private void ChangeStatus_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem mi && int.TryParse(mi.Tag.ToString(), out var id) && DataContext is RequestsPageViewModel vm)
                vm.ChangeStatusCommand.Execute(id);
        }
    }
}
