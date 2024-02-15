internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddEndpointsApiExplorer();

        var app = builder.Build();

        app.Run();

        List<string> weatherStationUris = new() {
            @"weatherstation-1",
            @"weatherstation-2",
            @"weatherstation-3"
        };

        var weatherStationTasks = weatherStationUris.Select(GetTemperature);

        double temperature = 0;

        while (temperature == 0)
        {
            temperature = weatherStationTasks.Where(x => x.IsCompleted)
                .Select(x => x.Result)
                .First();
        }

        Console.WriteLine($"The temperature {(temperature == 0 ? "could not be retrieved" : $"is {temperature:2}")}");

        static Task<double> GetTemperature(string uri)
        {
            HttpClient client = new();

            var httpResponse = client.GetAsync(uri).Result;
            httpResponse.EnsureSuccessStatusCode();

            var weatherModel = httpResponse.Content.ReadFromJsonAsync<WeatherTransformModel>().Result;

            return Task.FromResult(weatherModel.Temperature);
        };
    }
}

record WeatherTransformModel(double Temperature);

