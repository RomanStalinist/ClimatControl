using DemoExam.Interfaces;

namespace DemoExam.Data
{
    public class TableInfo
    {
        public string Name { get; set; }

        private TableInfo(string name)
        {
            Name = name;
        }

        public static TableInfo FromEntityType<T>() where T : IEntity
        {
            var typeName = typeof(T).Name;
            if (typeName.EndsWith("Status")) typeName += "e";
            return new TableInfo(typeName + "s");
        }

        public static implicit operator TableInfo(string value) => new TableInfo(value);

        public static implicit operator string(TableInfo tableName) => tableName.Name;
    }
}
