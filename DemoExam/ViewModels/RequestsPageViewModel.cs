using DemoExam.Data;
using DemoExam.Models;
using DemoExam.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
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
        public ICommand ChangeStatusCommand { get; }

        public RequestsPageViewModel()
        {
            using (var db = new Db())
                Requests = new ObservableCollection<Request>(
                    db.Select<Request>(null, null, null));

            AnswerCommand = new RelayCommand(Answer);
            ChangeStatusCommand = new RelayCommand(ChangeStatus);
        }

        private void Answer(object parameter) => new CommentWindow(parameter as Request).ShowDialog();

        private void ChangeStatus(object parameter)
        {
            using (var db = new Db())
            {
                try
                {
                    if (!int.TryParse(parameter.ToString(), out var statusId))
                        throw new InvalidOperationException($"Ошибка: Не удалось преобразовать параметр в int");

                    if (statusId < 1 || statusId > 3)
                        throw new InvalidOperationException($"Ошибка: Id статуса должен соблюдать правило: 1 <= id <= 3 (id = \"{statusId}\")");

                    var requestStatus = db.Select<RequestStatus>(
                        null, new string[] { "RequestId = @RequestId" }, new object[] { Request.Id })
                        .FirstOrDefault()
                        ?? throw new InvalidOperationException(
                            $"Ошибка: Не найдена связь между запросом и статусом по id запроса \"{ Request.Id }\"");

                    var status = db.Select<Status>(new string[1] { "Name" }, new string[1] { "Id = @Id" }, new object[1] { statusId })
                        .FirstOrDefault()
                        ?? throw new InvalidOperationException(
                            $"Ошибка: Не найден статус по id \"{statusId}\"");

                    db.Delete(requestStatus);

                    requestStatus.UpdatedAt = DateTime.Now;
                    requestStatus.StatusId = statusId;

                    db.Insert(requestStatus);

                    MessageBox.Show($"Статус изменён на \"{status.Name}\".\r\nПожалуйста, обновите страницу.");
                }
                catch (InvalidOperationException ioe)
                {
                    MessageBox.Show(ioe.Message);
                    Debug.WriteLine(ioe.ToString());
                }
            }
        }
    }
}
