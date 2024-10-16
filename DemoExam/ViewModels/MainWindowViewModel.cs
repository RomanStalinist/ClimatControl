using DemoExam.Interfaces;
using DemoExam.Models;
using DemoExam.Views;
using System.Windows.Controls;

namespace DemoExam.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        public IUser User => App.User;

        private Page _currentPage = new HomePage();
        public Page CurrentPage
        {
            get => _currentPage;
            set => SetField(ref _currentPage, value);
        }

        private ListViewItem[] _menuItems;
        public ListViewItem[] MenuItems
        {
            get => _menuItems;
            set => SetField(ref _menuItems, value);
        }

        private ListViewItem _menuItem;
        public ListViewItem MenuItem
        {
            get => _menuItem;
            set
            {
                SetField(ref _menuItem, value);
                switch (_menuItem.Content)
                {
                    case "Домой":
                        CurrentPage = new HomePage();
                        break;

                    case "Мои заявки":
                        CurrentPage = new MyRequestsPage();
                        break;

                    case "Комментарии":
                        CurrentPage = new CommentsPage();
                        break;

                    case "Все заявки":
                        CurrentPage = new RequestsPage();
                        break;

                    case "Назначение":
                        CurrentPage = new AssignmentsPage();
                        break;

                    case "Статистика":
                        CurrentPage = new StatisticsPage();
                        break;

                    case "Выход":
                        CurrentPage = new QuitPage();
                        break;
                }
            }
        }

        public MainWindowViewModel()
        {
            switch (User)
            {
                case Customer _:
                    MenuItems = new ListViewItem[]
                    {
                        new ListViewItem()
                        {
                            Content = "Домой",
                        },
                        new ListViewItem()
                        {
                            Content = "Мои заявки"
                        },
                        new ListViewItem()
                        {
                            Content = "Комментарии"
                        },
                        new ListViewItem()
                        {
                            Content = "Выход"
                        }
                    };
                    break;

                case Specialist _:
                    MenuItems = new ListViewItem[]
                    {
                        new ListViewItem()
                        {
                            Content = "Домой"
                        },
                        new ListViewItem()
                        {
                            Content = "Все заявки"
                        },
                        new ListViewItem()
                        {
                            Content = "Выход"
                        }
                    };
                    break;

                case Administrator _:
                    MenuItems = new ListViewItem[]
                    {
                        new ListViewItem()
                        {
                            Content = "Домой"
                        },
                        new ListViewItem()
                        {
                            Content = "Назначение"
                        },
                        new ListViewItem()
                        {
                            Content = "Статистика"
                        },
                        new ListViewItem()
                        {
                            Content = "Выход"
                        }
                    };
                    break;
            }
        }
    }
}
