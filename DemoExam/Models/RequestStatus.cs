using DemoExam.Interfaces;
using System;

namespace DemoExam.Models
{
    public class RequestStatus : IEntity
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
        public int StatusId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
