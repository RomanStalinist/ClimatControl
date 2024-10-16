using DemoExam.Data;
using DemoExam.Interfaces;
using DemoExam.Models;
using DemoExam.Views;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace DemoExam.ViewModels
{
    public class AuthWindowViewModel : ViewModel
    {
        // Fields
        private readonly string[] authColumns = { "Phone = @Phone", "Password = @Password" };

        // Properties
        private string _phone;

        public string Phone
        {
            get => _phone;
            set => SetField(ref _phone, value);
        }

        private string _password;

        public string Password
        {
            get => _password;
            set => SetField(ref _password, value);
        }

        private string _message;

        public string Message
        {
            get => _message;
            set => SetField(ref _message, value);
        }

        private string _role;

        public string Role
        {
            get => _role;
            set => SetField(ref _role, value);
        }

        public string[] Roles => new string[3]
        {
            Customer.GetRole(), Specialist.GetRole(), Administrator.GetRole()
        };

        // Commands
        public ICommand AuthCommand { get; }
        public ICommand QuitCommand { get; }

        public AuthWindowViewModel()
        {
            Role = Roles[0];
            AuthCommand = new RelayCommand(Auth);
            QuitCommand = new RelayCommand(Quit);
        }

        private void Auth(object parameter)
        {
            Message = string.Empty;
            IUser user = null;

            if (string.IsNullOrEmpty(Phone) || string.IsNullOrEmpty(Password))
            {
                Message = "Заполните все поля";
                return;
            }

            var authArgs = new object[2] { Phone, Password };

            using (var db = new Db())
            {
                switch (Role)
                {
                    case "Специалист":
                        user = db.Select<Specialist>(null, authColumns, authArgs).FirstOrDefault();
                        break;

                    case "Администратор":
                        user = db.Select<Administrator>(null, authColumns, authArgs).FirstOrDefault();
                        break;

                    default:
                        user = db.Select<Customer>(null, authColumns, authArgs).FirstOrDefault();
                        break;
                }
            }

            if (user is null)
            {
                Message = "Неверный логин или пароль";
                return;
            }

            App.User = user;

            var mWindow = new MainWindow();
            mWindow.Show();

            Application.Current.MainWindow = mWindow;
            Application.Current.Windows.OfType<AuthWindow>()
                .ToList()
                .ForEach(w =>
                {
                    w.WillAsk = false;
                    w.Close();
                });
        }

        private void Quit(object parameter)
        {
            Application.Current.Windows.OfType<AuthWindow>()
                .ToList()
                .ForEach(w => w.Close());
        }
    }
}
