using DemoExam.Interfaces;

namespace DemoExam.ViewModels
{
    public class HomePageViewModel : ViewModel
    {
        public IUser User => App.User;
    }
}
