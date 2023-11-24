using Azure.Core;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

namespace SampleWebApp.Extensions;

public static class ConfigurationManagerExtensions
{
    public static IConfiguration AddCustomAzureAppConfiguration(
        this ConfigurationManager configuration,
        TokenCredential credential)
    {
        var azureAppConfigurationOptions = new ServiceAzureAppConfigurationOptions();

        configuration
            .GetSection(ServiceAzureAppConfigurationOptions.SectionName)
            .Bind(azureAppConfigurationOptions);

        configuration.AddAzureAppConfiguration(
            options =>
            {
                ArgumentNullException.ThrowIfNull(azureAppConfigurationOptions.AccountEndpoint);

                options.Connect(
                        azureAppConfigurationOptions.AccountEndpoint,
                        credential)
                    .ConfigureKeyVault(vaultOptions => vaultOptions.SetCredential(credential));

                foreach (var label in azureAppConfigurationOptions.Labels)
                {
                    options.Select(
                        KeyFilter.Any,
                        label);

                    options.UseFeatureFlags(
                        flagOptions =>
                        {
                            flagOptions.Label = label;
                            flagOptions.CacheExpirationInterval = azureAppConfigurationOptions.FeatureFlags.RefreshInterval;
                        });
                }

                foreach (var key in azureAppConfigurationOptions.Sentinels)
                {
                    options.ConfigureRefresh(
                        refreshOptions =>
                        {
                            refreshOptions.Register(
                                    key.Name,
                                    key.Label,
                                    true)
                                .SetCacheExpiration(key.CacheInterval);
                        });
                }
        
            });

        return configuration;
    }
}