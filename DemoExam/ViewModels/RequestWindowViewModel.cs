using DemoExam.Data;
using DemoExam.Models;
using DemoExam.Views;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace DemoExam.ViewModels
{
    public class RequestWindowViewModel : ViewModel
    {
        private readonly bool IsNewRequest;

        private string _btnContent = "Изменить";
        public string BtnContent
        {
            get => _btnContent;
            set => SetField(ref _btnContent, value);
        }

        private Request _request;
        public Request Request
        {
            get => _request;
            set => SetField(ref _request, value);
        }

        private string _equipmentType;
        public string EquipmentType
        {
            get => _equipmentType;
            set
            {
                SetField(ref _equipmentType, value);
                Request.EquipmentType = value;
            }
        }

        private string _deviceModel;
        public string DeviceModel
        {
            get => _deviceModel;
            set
            {
                SetField(ref _deviceModel, value);
                Request.DeviceModel = value;
            }
        }

        private string _problemDescription;
        public string ProblemDescription
        {
            get => _problemDescription;
            set
            {
                SetField(ref _problemDescription, value);
                Request.ProblemDescription = value;
            }
        }

        public ICommand CloseCommand { get; }
        public ICommand ChangeCommand { get; }

        public RequestWindowViewModel(Request request)
        {
            IsNewRequest = request is null;

            CloseCommand = new RelayCommand(Close);
            ChangeCommand = new RelayCommand(Change);

            if (IsNewRequest)
            {
                Request = new Request();
                BtnContent = "Добавить";
                return;
            }
            
            Request = request;
            DeviceModel = request.DeviceModel;
            EquipmentType = request.EquipmentType;
            ProblemDescription = request.ProblemDescription;
        }

        private void Close(object parameter)
        {
            Application.Current.Windows.OfType<RequestWindow>()
                .ToList()
                .ForEach(w => w.Close());
        }

        private void Change(object parameter)
        {
            var myRequestsVm = GetMyRequestsPageViewModel();

            if (myRequestsVm is null) return;

            using (var db = new Db())
                if (!IsNewRequest)
                {
                    db.Update(Request);


                    Request.Id = db.Select<Request>(new string[1] { "Id" }, null, null).Last().Id;
                    var foundRequest = myRequestsVm.Requests.FirstOrDefault(req => req.Id == Request.Id);
                    myRequestsVm.Requests = new ObservableCollection<Request>(myRequestsVm.Requests.OrderBy(req => req.Id));
                }
                else
                {
                    Request.CustomerId = App.User.Id;
                    db.Insert(Request);

                    var lastRequestId = db.Select<Request>(new string[1] { "Id" }, null, null).Last().Id;

                    var requestStatus = new RequestStatus
                    {
                        StatusId = 1,
                        RequestId = lastRequestId
                    };

                    db.Insert(requestStatus);

                    Request.Id = db.Select<Request>(new string[1] { "Id" }, null, null).Last().Id;
                    myRequestsVm.Requests.Add(Request);
                }

            Close(parameter);
        }

        private MyRequestsPageViewModel GetMyRequestsPageViewModel()
        {
            if (Application.Current.Windows.OfType<MainWindow>().FirstOrDefault().DataContext is MainWindowViewModel mainWindowVm
                && mainWindowVm.CurrentPage.DataContext is MyRequestsPageViewModel myRequestsVm)
                return myRequestsVm;

            return null;
        }
    }
}
