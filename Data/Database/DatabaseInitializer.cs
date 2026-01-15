using System.IO;
using System.Reflection;
using Dapper;
using DeDen.Models;

namespace DeDen.Data.Database;

/// <summary>
/// データベース初期化サービス
/// SQLiteデータベースの自動生成と初期データ投入を行う
/// </summary>
public class DatabaseInitializer
{
    private readonly DatabaseSettings _settings;
    private readonly IDbConnectionFactory _connectionFactory;

    public DatabaseInitializer(DatabaseSettings settings, IDbConnectionFactory connectionFactory)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    /// <summary>
    /// データベースを初期化
    /// SQLiteの場合のみ実行
    /// </summary>
    public async Task InitializeAsync()
    {
        // SQLite以外の場合は何もしない
        if (_settings.DatabaseType != "SQLite")
        {
            return;
        }

        await InitializeMasterDbAsync();
        await InitializeHistoryDbAsync();
        await InitializeCustomerDbAsync();

        // パスワードハッシュの更新
        await UpdatePasswordHashesAsync();
    }

    /// <summary>
    /// プレースホルダーのパスワードハッシュを正しいハッシュに更新
    /// </summary>
    private async Task UpdatePasswordHashesAsync()
    {
        using var connection = _connectionFactory.CreateMasterConnection();
        connection.Open();

        // プレースホルダーパスワードを持つオペレーターを検索
        var operators = await connection.QueryAsync<(int OperatorID, string LoginID, string PasswordHash)>(
            "SELECT OperatorID, LoginID, PasswordHash FROM OperatorMaster WHERE PasswordHash LIKE '$2a$11$placeholder%'");

        foreach (var op in operators)
        {
            string newHash;
            if (op.LoginID == "admin")
            {
                // admin123のハッシュ
                newHash = BCrypt.Net.BCrypt.HashPassword("admin123", workFactor: 12);
            }
            else if (op.LoginID == "operator001")
            {
                // operator123のハッシュ
                newHash = BCrypt.Net.BCrypt.HashPassword("operator123", workFactor: 12);
            }
            else
            {
                continue;
            }

            await connection.ExecuteAsync(
                "UPDATE OperatorMaster SET PasswordHash = @Hash WHERE OperatorID = @Id",
                new { Hash = newHash, Id = op.OperatorID });
        }
    }

    /// <summary>
    /// 設定・マスタDBを初期化
    /// </summary>
    private async Task InitializeMasterDbAsync()
    {
        var dbPath = GetAbsolutePath(_settings.SQLite.MasterDbPath);

        // DBファイルが既に存在する場合はスキップ
        if (File.Exists(dbPath))
        {
            return;
        }

        // ディレクトリが存在しない場合は作成
        var directory = Path.GetDirectoryName(dbPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // SQLスクリプトを読み込んで実行
        var script = await ReadEmbeddedScriptAsync("Master.sql");

        using var connection = _connectionFactory.CreateMasterConnection();
        connection.Open();
        await connection.ExecuteAsync(script);
    }

    /// <summary>
    /// 問合せ履歴DBを初期化
    /// </summary>
    private async Task InitializeHistoryDbAsync()
    {
        var dbPath = GetAbsolutePath(_settings.SQLite.HistoryDbPath);

        if (File.Exists(dbPath))
        {
            return;
        }

        var directory = Path.GetDirectoryName(dbPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var script = await ReadEmbeddedScriptAsync("History.sql");

        using var connection = _connectionFactory.CreateHistoryConnection();
        connection.Open();
        await connection.ExecuteAsync(script);
    }

    /// <summary>
    /// 顧客マスタDBを初期化
    /// </summary>
    private async Task InitializeCustomerDbAsync()
    {
        var dbPath = GetAbsolutePath(_settings.SQLite.CustomerDbPath);

        if (File.Exists(dbPath))
        {
            return;
        }

        var directory = Path.GetDirectoryName(dbPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var script = await ReadEmbeddedScriptAsync("Customer.sql");

        using var connection = _connectionFactory.CreateCustomerConnection();
        connection.Open();
        await connection.ExecuteAsync(script);
    }

    /// <summary>
    /// 埋め込みリソースからSQLスクリプトを読み込み
    /// </summary>
    private static async Task<string> ReadEmbeddedScriptAsync(string scriptName)
    {
        // スクリプトファイルから直接読み込み
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var scriptPath = Path.Combine(baseDirectory, "Data", "Database", "Scripts", scriptName);

        if (File.Exists(scriptPath))
        {
            return await File.ReadAllTextAsync(scriptPath);
        }

        throw new FileNotFoundException($"SQLスクリプトが見つかりません: {scriptPath}");
    }

    /// <summary>
    /// 相対パスを絶対パスに変換
    /// </summary>
    private static string GetAbsolutePath(string path)
    {
        if (Path.IsPathRooted(path))
        {
            return path;
        }

        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        return Path.Combine(baseDirectory, path);
    }
}
