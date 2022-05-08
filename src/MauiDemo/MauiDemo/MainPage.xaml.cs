namespace MauiDemo;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
		_timer = new System.Timers.Timer(TimeSpan.FromSeconds(15).TotalMilliseconds);
        _timer.Elapsed += _timer_Elapsed;
	}

    private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
		GetLocationAsync().GetAwaiter().GetResult();
	}

    private async void OnStartClicked(object sender, EventArgs e)
    {
		_timer.Start();
		await GetLocationAsync();
	}

	private async Task GetLocationAsync()
    {
		var req = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
		_cancellationTokenSource = new CancellationTokenSource();
		var location = await Geolocation.GetLocationAsync(req);

		if (location == null)
			return;

		LatLabel.Text = $"Lat: {location.Latitude}";
		LongLabel.Text = $"Long: {location.Longitude}";
		location.Course;
		location.Altitude;
		location.Speed;
		location.Timestamp;
	}

	private void OnStopClicked(object sender, EventArgs e)
    {
		_timer.Stop();
		_cancellationTokenSource?.Cancel();
    }

	private System.Timers.Timer _timer;

	//private async void OnCounterClicked(object sender, EventArgs e)
	//{
	//	//var location = await Geolocation.GetLastKnownLocationAsync();
	//	var req = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
	//	_cancellationTokenSource = new CancellationTokenSource();
	//	var location = await Geolocation.GetLocationAsync(req);

	//	count++;
	//	CounterLabel.Text = $"Current count: {count}";

	//	LatLabel.Text = $"Lat: {location.Latitude}";
	//	LongLabel.Text = $"Long: {location.Longitude}";

	//	SemanticScreenReader.Announce(CounterLabel.Text);
	//}

	private CancellationTokenSource _cancellationTokenSource;
}

