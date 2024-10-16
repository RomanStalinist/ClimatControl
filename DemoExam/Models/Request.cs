using DemoExam.Interfaces;

namespace DemoExam.Models
{
    public class Request : IEntity
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string EquipmentType { get; set; }
        public string DeviceModel { get; set; }
        public string ProblemDescription { get; set; }

        public override int GetHashCode() => Id.GetHashCode();

        public override string ToString() => ProblemDescription;

        public override bool Equals(object obj) => obj is Request r && r.GetHashCode() == GetHashCode();
    }
}
