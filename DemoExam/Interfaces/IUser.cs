namespace DemoExam.Interfaces
{
    public interface IUser : IEntity
    {
        int Id { get; set; }
        string Name { get; set; }
        string Phone { get; set; }
        string Password { get; set; }
    }
}
