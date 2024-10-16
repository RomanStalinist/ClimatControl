using DemoExam.Data;
using DemoExam.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace DemoExam.Converters
{
    public class RequestIdToInfoForCustomerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var sb = new StringBuilder();

            try
            {
                if (!int.TryParse(value.ToString(), out int id))
                    throw new InvalidOperationException("Ошибка: Не удалось преобразовать значение в int");

                using (var db = new Db())
                {
                    var request = db.Select<Request>(null, new string[1] { "Id = @Id" }, new object[1] { id })
                        .FirstOrDefault()
                        ?? throw new InvalidOperationException($"Ошибка: Не удалось найти запрашиваемый Request по id \"{id}\"");

                    var requestSpecialists = db.Select<RequestSpecialist>(null, new string[1] { "RequestId = @RequestId" }, new object[1] { id });

                    var requestStatus = db.Select<RequestStatus>(null, new string[1] { "RequestId = @RequestId" }, new object[1] { id })
                        .FirstOrDefault();

                    if (requestStatus is null)
                    {
                        requestStatus = new RequestStatus
                        {
                            StatusId = 1,
                            RequestId = id,
                        };

                        db.Insert(requestStatus);
                    }

                    var statusId = requestStatus.StatusId;

                    var status = db.Select<Status>(new string[1] { "Name" }, new string[1] { "Id = @Id" }, new object[1] { statusId })
                        .FirstOrDefault()
                        ?? throw new InvalidOperationException($"Ошибка: Не удалось найти статус по id \"{statusId}\"");

                    var specialists = new List<Specialist>();

                    foreach (var requestSpecialist in requestSpecialists)
                    {
                        var specialistId = requestSpecialist.SpecialistId;
                        specialists.Add(
                            db.Select<Specialist>(new string[2] { "Name", "ContactInfo" }, new string[1] { "Id = @Id" }, new object[1] { specialistId })
                            .FirstOrDefault()
                            ?? throw new InvalidOperationException($"Ошибка: Не удалось найти специалиста по id \"{specialistId}\""));
                    }

                    sb.AppendLine($"Статус запроса: {status.Name}")
                        .AppendLine($"Обновлён: {requestStatus.UpdatedAt}")
                        .AppendLine()
                        .Append($"Количество специалистов: {specialists.Count}");

                    foreach (var specialist in specialists)
                    {
                        sb.AppendLine();
                        sb.Append(specialist.Name);

                        if (specialist.ContactInfo != null)
                            sb.AppendLine()
                                .Append(", ")
                                .Append(specialist.ContactInfo);
                    }

                    return sb.ToString();
                }
            }
            catch (InvalidOperationException ioe)
            {
                sb.AppendLine("------");
                sb.AppendLine(ioe.Message);
                sb.Append("------");
            }

            return sb.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
