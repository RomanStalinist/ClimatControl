using DemoExam.Interfaces;
using System;

namespace DemoExam.Models
{
    public class Comment : IEntity
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
        public int SpecialistId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
