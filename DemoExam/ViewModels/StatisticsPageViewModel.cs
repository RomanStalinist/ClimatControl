using DemoExam.Models;
using LiveCharts.Wpf;
using LiveCharts;
using System.Collections.ObjectModel;
using System;
using System.Collections.Generic; // Для использования Dictionary
using System.Linq;
using DemoExam.Data;
using System.Windows;
using System.Windows.Media;

namespace DemoExam.ViewModels
{
    public class StatisticsPageViewModel : ViewModel
    {
        private ObservableCollection<RequestAndStatus> _requestsAndStatuses;

        public ObservableCollection<RequestAndStatus> RequestAndStatuses
        {
            get => _requestsAndStatuses;
            set
            {
                _requestsAndStatuses = value;
                OnPropertyChanged(nameof(RequestAndStatuses));
            }
        }

        private Statistics _statistics;
        public Statistics Statistics
        {
            get => _statistics;
            set
            {
                _statistics = value;
                OnPropertyChanged(nameof(Statistics));
            }
        }

        public SeriesCollection TotalRequestsChart { get; set; }

        public StatisticsPageViewModel()
        {
            Statistics = new Statistics();
            RequestAndStatuses = new ObservableCollection<RequestAndStatus>();

            // Пример данных
            LoadSampleData();

            // Инициализация графиков
            TotalRequestsChart = new SeriesCollection();

            UpdateStatistics();
        }

        private void LoadSampleData()
        {
            using (var db = new Db())
                RequestAndStatuses = new ObservableCollection<RequestAndStatus>(
                    db.RawSelect<RequestAndStatus>(
                        "SELECT r.Id, r.CustomerId, r.EquipmentType, r.DeviceModel, r.ProblemDescription, rs.RequestId, rs.StatusId, rs.CreatedAt, rs.UpdatedAt " +
                        "FROM Requests r " +
                        "JOIN RequestStatuses rs ON r.Id = rs.RequestId " +
                        "WHERE rs.StatusId = 2"
                    ));
        }

        private void UpdateStatistics()
        {
            Statistics.CompletedRequestsCount = RequestAndStatuses.Count();

            if (Statistics.CompletedRequestsCount > 0)
            {
                Statistics.AverageCompletionTime = TimeSpan.FromTicks(
                    (long)RequestAndStatuses.Average(
                        r => (r.UpdatedAt - r.CreatedAt).Ticks
                    )
                );

                TotalRequestsChart.Clear();
                var deviceModelCounts = new Dictionary<string, int>();

                foreach (var request in RequestAndStatuses)
                {
                    var deviceModel = request.DeviceModel;

                    if (deviceModelCounts.ContainsKey(deviceModel))
                        deviceModelCounts[deviceModel]++;
                    else
                        deviceModelCounts[deviceModel] = 1;
                }

                // Создаем серии для каждой модели устройства
                foreach (var entry in deviceModelCounts)
                {
                    TotalRequestsChart.Add(new ColumnSeries
                    {
                        Title = entry.Key,
                        ColumnPadding = 20,
                        Margin = new Thickness(4),
                        MaxWidth = double.MaxValue,
                        MaxColumnWidth = double.MaxValue,
                        Values = new ChartValues<int> { entry.Value },
                        Foreground = new SolidColorBrush((Color)(ColorConverter.ConvertFromString("#DFDFDF"))),
                    });
                }
            }
            else
            {
                Statistics.AverageCompletionTime = TimeSpan.Zero;
            }

            Statistics = new Statistics
            {
                CompletedRequestsCount = CompletedRequestsCount,
                AverageCompletionTime = AverageCompletionTime,
                FaultTypeStatistics = Statistics.FaultTypeStatistics
            };
        }

        public int CompletedRequestsCount => Statistics.CompletedRequestsCount;
        public TimeSpan AverageCompletionTime => Statistics.AverageCompletionTime;
    }
}