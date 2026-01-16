using System.Data;
using Microsoft.Data.SqlClient;
using DenDen.Models;

namespace DenDen.Data.Database;

/// <summary>
/// SQL Server用の接続ファクトリ
/// 本番環境（中・大規模案件）で使用
/// </summary>
public class SqlServerConnectionFactory : IDbConnectionFactory
{
    private readonly DatabaseSettings _settings;

    public SqlServerConnectionFactory(DatabaseSettings settings)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    /// <summary>
    /// 設定・マスタDB用の接続を作成
    /// </summary>
    public IDbConnection CreateMasterConnection()
    {
        return new SqlConnection(_settings.SqlServer.MasterConnectionString);
    }

    /// <summary>
    /// 問合せ履歴DB用の接続を作成
    /// </summary>
    public IDbConnection CreateHistoryConnection()
    {
        return new SqlConnection(_settings.SqlServer.HistoryConnectionString);
    }

    /// <summary>
    /// 顧客マスタDB用の接続を作成
    /// </summary>
    public IDbConnection CreateCustomerConnection()
    {
        return new SqlConnection(_settings.SqlServer.CustomerConnectionString);
    }
}
