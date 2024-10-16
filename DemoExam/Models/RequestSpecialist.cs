using DemoExam.Interfaces;
using System;

namespace DemoExam.Models
{
    public class RequestSpecialist : IEntity
    {
        public int RequestId { get; set; }
        public int SpecialistId { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
