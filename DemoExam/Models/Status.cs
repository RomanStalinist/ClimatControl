using DemoExam.Interfaces;

namespace DemoExam.Models
{
    public class Status : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
