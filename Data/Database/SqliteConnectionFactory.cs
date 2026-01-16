using System.Data;
using System.IO;
using Microsoft.Data.Sqlite;
using DenDen.Models;

namespace DenDen.Data.Database;

/// <summary>
/// SQLite用の接続ファクトリ
/// 開発環境で使用
/// </summary>
public class SqliteConnectionFactory : IDbConnectionFactory
{
    private readonly DatabaseSettings _settings;

    public SqliteConnectionFactory(DatabaseSettings settings)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    /// <summary>
    /// 設定・マスタDB用の接続を作成
    /// </summary>
    public IDbConnection CreateMasterConnection()
    {
        var path = GetAbsolutePath(_settings.SQLite.MasterDbPath);
        var connectionString = $"Data Source={path};";
        return new SqliteConnection(connectionString);
    }

    /// <summary>
    /// 問合せ履歴DB用の接続を作成
    /// </summary>
    public IDbConnection CreateHistoryConnection()
    {
        var path = GetAbsolutePath(_settings.SQLite.HistoryDbPath);
        var connectionString = $"Data Source={path};";
        return new SqliteConnection(connectionString);
    }

    /// <summary>
    /// 顧客マスタDB用の接続を作成
    /// </summary>
    public IDbConnection CreateCustomerConnection()
    {
        var path = GetAbsolutePath(_settings.SQLite.CustomerDbPath);
        var connectionString = $"Data Source={path};";
        return new SqliteConnection(connectionString);
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

        // 相対パスの場合、アプリケーションのベースディレクトリを基準に変換
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        return Path.Combine(baseDirectory, path);
    }
}
