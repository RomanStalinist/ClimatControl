using DemoExam.Data;
using DemoExam.Models;
using DemoExam.Views;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace DemoExam.ViewModels
{
    public class RequestsPageViewModel : ViewModel
    {
        private Request _request;
        public Request Request
        {
            get => _request;
            set => SetField(ref _request, value);
        }

        private ObservableCollection<Request> _requests;
        public ObservableCollection<Request> Requests
        {
            get => _requests;
            set => SetField(ref _requests, value);
        }

        public ICommand AnswerCommand { get; }

        public RequestsPageViewModel()
        {
            using (var db = new Db())
                Requests = new ObservableCollection<Request>(
                    db.Select<Request>(null, null, null));

            AnswerCommand = new RelayCommand(Answer);
        }

        private void Answer(object parameter)
        {
            new CommentWindow(parameter as Request).ShowDialog();
        }
    }
}
