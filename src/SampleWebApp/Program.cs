using Azure.Identity;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using SampleWebApp.ApplicationOptions;
using SampleWebApp.Extensions;

var builder = WebApplication.CreateBuilder(args);

var credential = new DefaultAzureCredential();

builder.Configuration.AddCustomAzureAppConfiguration(credential);
// Required for dynamic configuration refresh
builder.Services.AddAzureAppConfiguration();

builder.Services.AddOptions<SampleWebAppOptions>()
    .Configure<IConfiguration>((options, configuration) => 
        configuration
            .GetSection(SampleWebAppOptions.SectionName)
            .Bind(options));

builder.Services.AddFeatureManagement();

var app = builder.Build();

// Also required for dynamic configuration refresh
app.UseAzureAppConfiguration();

app.MapGet(
    "/",
    (IOptionsSnapshot<SampleWebAppOptions> monitor) => monitor.Value.ApplicationName);

app.MapGet(
    "/features",
    async (
        IFeatureManager features) =>
    {
        var dict = new Dictionary<string, bool>();

        await foreach (var flag in features.GetFeatureNamesAsync())
        {
            if (!dict.TryGetValue(flag, out _))
            {
                dict[flag] = await features.IsEnabledAsync(flag);
            }
        }
        
        return dict;
    });

app.Run();