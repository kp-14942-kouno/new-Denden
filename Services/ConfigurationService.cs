using Microsoft.Extensions.Configuration;
using DenDen.Models;

namespace DenDen.Services;

/// <summary>
/// 設定管理サービスの実装
/// appsettings.jsonから設定を読み込む
/// </summary>
public class ConfigurationService : IConfigurationService
{
    private readonly IConfiguration _configuration;
    private readonly Lazy<DatabaseSettings> _databaseSettings;
    private readonly Lazy<CustomerMasterSettings> _customerMasterSettings;
    private readonly Lazy<CustomFieldSearchSettings> _customFieldSearchSettings;
    private readonly Lazy<ImageSettings> _imageSettings;

    public ConfigurationService(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        // 遅延初期化で設定を読み込む
        _databaseSettings = new Lazy<DatabaseSettings>(() =>
            GetSection<DatabaseSettings>("DatabaseSettings") ?? new DatabaseSettings());

        _customerMasterSettings = new Lazy<CustomerMasterSettings>(() =>
            GetSection<CustomerMasterSettings>("CustomerMasterSettings") ?? new CustomerMasterSettings());

        _customFieldSearchSettings = new Lazy<CustomFieldSearchSettings>(() =>
            GetSection<CustomFieldSearchSettings>("CustomFieldSearchSettings") ?? new CustomFieldSearchSettings());

        _imageSettings = new Lazy<ImageSettings>(() =>
            GetSection<ImageSettings>("ImageSettings") ?? new ImageSettings());
    }

    /// <inheritdoc/>
    public DatabaseSettings DatabaseSettings => _databaseSettings.Value;

    /// <inheritdoc/>
    public CustomerMasterSettings CustomerMasterSettings => _customerMasterSettings.Value;

    /// <inheritdoc/>
    public CustomFieldSearchSettings CustomFieldSearchSettings => _customFieldSearchSettings.Value;

    /// <inheritdoc/>
    public ImageSettings ImageSettings => _imageSettings.Value;

    /// <inheritdoc/>
    public string Environment =>
        System.Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development";

    /// <inheritdoc/>
    public T? GetSection<T>(string sectionName) where T : class, new()
    {
        return _configuration.GetSection(sectionName).Get<T>();
    }

    /// <inheritdoc/>
    public string? GetValue(string key)
    {
        return _configuration[key];
    }
}
