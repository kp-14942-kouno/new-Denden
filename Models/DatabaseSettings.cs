namespace DeDen.Models;

/// <summary>
/// データベース設定
/// appsettings.jsonのDatabaseSettingsセクションにマップ
/// </summary>
public class DatabaseSettings
{
    /// <summary>
    /// 環境種別（Development / Staging / Production）
    /// </summary>
    public string Environment { get; set; } = "Development";

    /// <summary>
    /// データベース種別（SQLite / SqlServer / Access）
    /// </summary>
    public string DatabaseType { get; set; } = "SQLite";

    /// <summary>
    /// SQLite設定
    /// </summary>
    public SqliteSettings SQLite { get; set; } = new();

    /// <summary>
    /// SQL Server設定
    /// </summary>
    public SqlServerSettings SqlServer { get; set; } = new();

    /// <summary>
    /// Access設定
    /// </summary>
    public AccessSettings Access { get; set; } = new();
}

/// <summary>
/// SQLite設定
/// </summary>
public class SqliteSettings
{
    /// <summary>
    /// マスタDBパス
    /// </summary>
    public string MasterDbPath { get; set; } = "./Data/Master.db";

    /// <summary>
    /// 履歴DBパス
    /// </summary>
    public string HistoryDbPath { get; set; } = "./Data/History.db";

    /// <summary>
    /// 顧客マスタDBパス
    /// </summary>
    public string CustomerDbPath { get; set; } = "./Data/Customer.db";
}

/// <summary>
/// SQL Server設定
/// </summary>
public class SqlServerSettings
{
    /// <summary>
    /// マスタDB接続文字列
    /// </summary>
    public string MasterConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// 履歴DB接続文字列
    /// </summary>
    public string HistoryConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// 顧客マスタDB接続文字列
    /// </summary>
    public string CustomerConnectionString { get; set; } = string.Empty;
}

/// <summary>
/// Access設定
/// </summary>
public class AccessSettings
{
    /// <summary>
    /// マスタDBパス
    /// </summary>
    public string MasterDbPath { get; set; } = string.Empty;

    /// <summary>
    /// 履歴DBパス
    /// </summary>
    public string HistoryDbPath { get; set; } = string.Empty;

    /// <summary>
    /// 顧客マスタDBパス
    /// </summary>
    public string CustomerDbPath { get; set; } = string.Empty;
}
