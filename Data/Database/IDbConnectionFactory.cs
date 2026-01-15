using System.Data;

namespace DeDen.Data.Database;

/// <summary>
/// データベース接続ファクトリのインターフェース
/// SQLite、SQL Server、Accessの切り替えを抽象化
/// </summary>
public interface IDbConnectionFactory
{
    /// <summary>
    /// 設定・マスタDB用の接続を作成
    /// </summary>
    IDbConnection CreateMasterConnection();

    /// <summary>
    /// 問合せ履歴DB用の接続を作成
    /// </summary>
    IDbConnection CreateHistoryConnection();

    /// <summary>
    /// 顧客マスタDB用の接続を作成
    /// </summary>
    IDbConnection CreateCustomerConnection();
}
