internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddScoped<IWeatherStationManager, WeatherStationManager>();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.Run();
    }
}

interface IWeatherStationManager
{
    Task<double> Execute();
    string ToString();
}

class WeatherStationManager : IWeatherStationManager
{
    private double _temperature = 0;

    public override string ToString()
    {
        return $"The temperature {(_temperature == 0 ? "could not be retrieved" : $"is {_temperature:2}")}";
    }

    public Task<double> Execute()
    {
        double temperature = 0;

        List<string> weatherStationUris = new() {
                @"weatherstation-1",
                @"weatherstation-2",
                @"weatherstation-3"
            };

        var weatherStationTasks = weatherStationUris.Select(GetTemperature);

        while (temperature == 0)
        {
            temperature = weatherStationTasks.Where(x => x.IsCompleted)
                .Select(x => x.Result)
                .First();
        }

        return Task.FromResult(temperature);
    }

    private Task<double> GetTemperature(string uri)
    {
        HttpClient client = new();

        var httpResponse = client.GetAsync(uri).Result;
        httpResponse.EnsureSuccessStatusCode();

        var weatherModel = httpResponse.Content.ReadFromJsonAsync<WeatherTransformModel>().Result;

        return Task.FromResult(weatherModel.Temperature);
    }
}

record WeatherTransformModel(double Temperature);

