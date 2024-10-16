using DemoExam.Interfaces;

namespace DemoExam.Models
{
    public class Customer : IUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }

        public static string GetRole() => "Пользователь";
    }
}
