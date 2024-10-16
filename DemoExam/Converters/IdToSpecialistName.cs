using DemoExam.Data;
using DemoExam.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace DemoExam.Converters
{
    public class IdToSpecialistName : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!int.TryParse(value.ToString(), out int id)) return null;

            using (var db = new Db())
                return db.Select<Specialist>(new string[1] { "Name" }, new string[1] { "Id = @Id" }, new object[1] { id })
                    .FirstOrDefault()
                    .Name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
