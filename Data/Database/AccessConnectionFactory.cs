using System.Data;
using System.Data.OleDb;
using DenDen.Models;

namespace DenDen.Data.Database;

/// <summary>
/// Access用の接続ファクトリ
/// 本番環境（小規模案件）で使用
/// </summary>
public class AccessConnectionFactory : IDbConnectionFactory
{
    private readonly DatabaseSettings _settings;

    public AccessConnectionFactory(DatabaseSettings settings)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    /// <summary>
    /// 設定・マスタDB用の接続を作成
    /// </summary>
    public IDbConnection CreateMasterConnection()
    {
        var connectionString = BuildAccessConnectionString(_settings.Access.MasterDbPath);
        return new OleDbConnection(connectionString);
    }

    /// <summary>
    /// 問合せ履歴DB用の接続を作成
    /// </summary>
    public IDbConnection CreateHistoryConnection()
    {
        var connectionString = BuildAccessConnectionString(_settings.Access.HistoryDbPath);
        return new OleDbConnection(connectionString);
    }

    /// <summary>
    /// 顧客マスタDB用の接続を作成
    /// </summary>
    public IDbConnection CreateCustomerConnection()
    {
        var connectionString = BuildAccessConnectionString(_settings.Access.CustomerDbPath);
        return new OleDbConnection(connectionString);
    }

    /// <summary>
    /// Access接続文字列を構築
    /// </summary>
    private static string BuildAccessConnectionString(string dbPath)
    {
        // Access 2007以降（.accdb形式）
        return $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbPath};Persist Security Info=False;";
    }
}
