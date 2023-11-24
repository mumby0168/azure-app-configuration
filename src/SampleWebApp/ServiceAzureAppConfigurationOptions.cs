namespace SampleWebApp;

public class ServiceAzureAppConfigurationOptions
{
    public const string SectionName = "AzureAppConfiguration";
    public Uri? AccountEndpoint { get; set; }

    public string[] Labels { get; set; } = Array.Empty<string>();

    public SentinelKey[] Sentinels { get; set; } = Array.Empty<SentinelKey>();

    public FeatureConfig FeatureFlags { get; set; } = new(30);
    
    public record SentinelKey(string Name, string Label = "\0", int CacheIntervalInSeconds = 30)
    {
        public TimeSpan CacheInterval => TimeSpan.FromSeconds(CacheIntervalInSeconds);
    }

    public record FeatureConfig(int RefreshIntervalInSeconds)
    {
        public TimeSpan RefreshInterval => TimeSpan.FromSeconds(RefreshIntervalInSeconds);
    }
}

