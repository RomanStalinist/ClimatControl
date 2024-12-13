using DemoExam.Data;
using DemoExam.Models;
using DemoExam.Views;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace DemoExam.ViewModels
{
    public class CommentWindowViewModel : ViewModel
    {
        private readonly Request _request;

        private string _content;
        public string Content
        {
            get => _content;
            set => SetField(ref _content, value);
        }

        public ICommand AddCommand { get; }
        public ICommand CloseCommand { get; }

        public CommentWindowViewModel(Request request)
        {
            _request = request;
            AddCommand = new RelayCommand(Add);
            CloseCommand = new RelayCommand(Close);
        }

        private void Add(object parameter)
        {
            var specialistId = App.User.Id;

            var newComment = new Comment
            {
                Content = Content,
                RequestId = _request.Id,
                SpecialistId = specialistId
            };

            var newRequestSpecialist = new RequestSpecialist
            {
                RequestId = _request.Id,
                SpecialistId = specialistId
            };

            using (var db = new Db())
            {
                db.Insert(newComment);
                if (!db.Select<RequestSpecialist>(
                    new string[] { "RequestId", "SpecialistId" },
                    new string[] { "RequestId = @RequestId", "SpecialistId = @SpecialistId" },
                    new object[] { _request.Id, specialistId }).Any())
                    db.Insert(newRequestSpecialist);
            }

            Close(parameter);
        }

        private void Close(object parameter)
        {
            Application.Current.Windows.OfType<CommentWindow>()
                .ToList()
                .ForEach(w => w.Close());
        }
    }
}
