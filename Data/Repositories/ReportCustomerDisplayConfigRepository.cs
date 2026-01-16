using Dapper;
using DenDen.Data.Database;
using DenDen.Models.Entities;

namespace DenDen.Data.Repositories;

/// <summary>
/// レポート用顧客情報表示設定リポジトリ
/// </summary>
public class ReportCustomerDisplayConfigRepository : IReportCustomerDisplayConfigRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ReportCustomerDisplayConfigRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    /// <summary>
    /// 指定案件の表示設定を取得（DisplayOrder順）
    /// </summary>
    public async Task<IEnumerable<ReportCustomerDisplayConfig>> GetByProjectIdAsync(int projectId)
    {
        const string sql = @"
            SELECT * FROM ReportCustomerDisplayConfig
            WHERE ProjectID = @ProjectID
            ORDER BY DisplayOrder";

        using var connection = _connectionFactory.CreateMasterConnection();
        return await connection.QueryAsync<ReportCustomerDisplayConfig>(sql, new { ProjectID = projectId });
    }
}
