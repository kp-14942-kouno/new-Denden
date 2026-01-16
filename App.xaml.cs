using System.IO;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DenDen.Data.Database;
using DenDen.Data.Repositories;
using DenDen.Models;
using DenDen.Services;
using DenDen.ViewModels;

namespace DenDen;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private IServiceProvider? _serviceProvider;
    private IConfiguration? _configuration;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // 設定ファイルの読み込み
        _configuration = BuildConfiguration();

        // DIコンテナのセットアップ
        _serviceProvider = ConfigureServices();

        // データベース初期化
        await InitializeDatabaseAsync();

        // メインウィンドウの表示
        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    /// <summary>
    /// 設定ファイルを読み込み
    /// </summary>
    private IConfiguration BuildConfiguration()
    {
        var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development";

        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
            .Build();
    }

    /// <summary>
    /// DIコンテナの設定
    /// </summary>
    private IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        // 設定の登録
        var databaseSettings = _configuration!.GetSection("DatabaseSettings").Get<DatabaseSettings>()
            ?? new DatabaseSettings();
        services.AddSingleton(databaseSettings);

        // データベース接続ファクトリの登録（環境に応じて切り替え）
        switch (databaseSettings.DatabaseType)
        {
            case "SQLite":
                services.AddSingleton<IDbConnectionFactory, SqliteConnectionFactory>();
                break;
            case "SqlServer":
                services.AddSingleton<IDbConnectionFactory, SqlServerConnectionFactory>();
                break;
            case "Access":
                services.AddSingleton<IDbConnectionFactory, AccessConnectionFactory>();
                break;
            default:
                throw new InvalidOperationException($"Unsupported database type: {databaseSettings.DatabaseType}");
        }

        // データベース初期化サービス
        services.AddSingleton<DatabaseInitializer>();

        // Repositoryの登録
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IOperatorRepository, OperatorRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IStatusRepository, StatusRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IInquiryHistoryRepository, InquiryHistoryRepository>();
        services.AddScoped<IInquiryHistoryLogRepository, InquiryHistoryLogRepository>();
        services.AddScoped<ICustomFieldDefinitionRepository, CustomFieldDefinitionRepository>();
        services.AddScoped<IReportCustomerDisplayConfigRepository, ReportCustomerDisplayConfigRepository>();

        // IConfigurationの登録
        services.AddSingleton<IConfiguration>(_configuration!);

        // サービスの登録
        services.AddSingleton<SessionManager>();
        services.AddSingleton<IConfigurationService, ConfigurationService>();
        services.AddSingleton<IDialogService, DialogService>();
        services.AddSingleton<IExportService, ExportService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IInquiryPrintService, InquiryPrintService>();

        // ナビゲーションサービスの登録（ServiceProviderを必要とするため、後で登録）
        services.AddSingleton<INavigationService>(sp => new NavigationService(sp));

        // ViewModelの登録
        services.AddTransient<LoginViewModel>();
        services.AddTransient<SearchPanelViewModel>();
        services.AddTransient<CustomerInfoViewModel>();
        services.AddTransient<InquiryViewModel>();
        services.AddTransient<MainViewModel>();
        services.AddTransient<ReportViewModel>();

        // Windowの登録
        services.AddTransient<MainWindow>();

        return services.BuildServiceProvider();
    }

    /// <summary>
    /// データベースの初期化
    /// </summary>
    private async Task InitializeDatabaseAsync()
    {
        var initializer = _serviceProvider!.GetRequiredService<DatabaseInitializer>();
        await initializer.InitializeAsync();
    }

    /// <summary>
    /// サービスプロバイダを取得
    /// </summary>
    public IServiceProvider? ServiceProvider => _serviceProvider;
}
