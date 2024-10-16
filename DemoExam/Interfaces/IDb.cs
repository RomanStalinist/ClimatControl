using System.Collections.Generic;

namespace DemoExam.Interfaces
{
    public interface IDb
    {
        string ConnectionString { get; }
        IEnumerable<T> Select<T>(string[] columns, string[] whereClause, object[] whereArgs) where T : IEntity;
        int Insert<T>(T item) where T : IEntity;
        int Delete<T>(T item) where T : IEntity;
        int Update<T>(T item) where T : IEntity;
    }
}
