using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace MauiDemo
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public MainPageViewModel()
        {
            _timer = new System.Timers.Timer(TimeSpan.FromSeconds(15).TotalMilliseconds);
            _timer.Elapsed += _timer_Elapsed;

            Positions = new ObservableCollection<LocationLog>();
            StartCommand = new Command(async () => await StartAsync());
            StopCommand = new Command(() => Stop());
        }

        public string TotalDistance
        {
            get => _totalDistStr;
            set
            {
                _totalDistStr = value;
                OnPropertyChanged(nameof(TotalDistance));
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand StartCommand { get; private set; }

        public ICommand StopCommand { get; private set; }

        public ObservableCollection<LocationLog> Positions { get; set; }

        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            UpdateLocationAsync().GetAwaiter().GetResult();
        }

        private void Stop()
        {
            _timer.Stop();
            _cancellationTokenSource?.Cancel();
        }

        private async Task StartAsync()
        {
            _timer.Start();
            await UpdateLocationAsync();
        }

        private async Task<Location> GetLocationAsync()
        {
            try
            {
                var req = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                _cancellationTokenSource = new CancellationTokenSource();
                return await Geolocation.GetLocationAsync(req, _cancellationTokenSource.Token);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private async Task UpdateLocationAsync()
        {
            var location = await GetLocationAsync();

            if (location == null)
                return;

            double distance = 0;
            if(_lastLocation != null)
                distance = location.CalculateDistance(_lastLocation, DistanceUnits.Miles);
        
            Positions.Add(new LocationLog()
            {
                Timestamp = location.Timestamp.ToString("HH:mm:ss"),
                Latitude = Math.Round(location.Latitude, 4).ToString(),
                Longitude = Math.Round(location.Longitude, 4).ToString(),
                Distance = Math.Round(distance, 4).ToString()
            });

            _totalDistance += distance;
            _lastLocation = location;

            TotalDistance = _totalDistance.ToString();
        }

        private Location _lastLocation;
        private double _totalDistance;
        private string _totalDistStr;
        private System.Timers.Timer _timer;
        private CancellationTokenSource _cancellationTokenSource;
    }
}
