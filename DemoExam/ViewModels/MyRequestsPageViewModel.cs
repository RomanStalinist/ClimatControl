using DemoExam.Data;
using DemoExam.Models;
using DemoExam.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace DemoExam.ViewModels
{
    public class MyRequestsPageViewModel : ViewModel
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

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        public MyRequestsPageViewModel()
        {
            using (var db = new Db())
                Requests = new ObservableCollection<Request>(
                    db.Select<Request>(null, db.MyRequestsWhereClause, db.MyRequestsWhereArgs));
            
            Request = Requests.Count > 0 ? Requests[0] : null;

            AddCommand = new RelayCommand(Add);
            EditCommand = new RelayCommand(Edit);
            DeleteCommand = new RelayCommand(Delete);
        }

        private void Add(object parameter) => new RequestWindow().ShowDialog();

        private void Edit(object parameter) => new RequestWindow((Request)parameter).ShowDialog();

        private void Delete(object parameter)
        {
            if (MessageBox.Show(
                "Вы уверены, что хотите удалить эту заявку?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) != MessageBoxResult.Yes) return;

            var request = (Request)parameter;

            try
            {
                using (var db = new Db()) db.Delete(request);
                Requests.Remove(Requests.FirstOrDefault(req => req.Id == request.Id));
            }
            catch (Exception ex)
            {
                switch (Db.TraceLevel)
                {
                    case TraceLevel.Off:
                        break;

                    case TraceLevel.Error:
                        Debug.WriteLine(ex.Message);
                        break;

                    default:
                        Debug.WriteLine(ex.ToString());
                        break;
                }
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
