using System.Data;
using Dapper;
using DenDen.Data.Database;
using DenDen.Models.Entities;

namespace DenDen.Data.Repositories;

/// <summary>
/// ステータスマスタリポジトリ
/// </summary>
public class StatusRepository : IStatusRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public StatusRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    /// <summary>
    /// 案件に紐づく有効なステータスを全件取得
    /// </summary>
    public async Task<IEnumerable<StatusMaster>> GetAllByProjectIdAsync(int projectId)
    {
        const string sql = @"
            SELECT * FROM StatusMaster
            WHERE ProjectID = @ProjectID
              AND IsActive = 1
            ORDER BY DisplayOrder";

        using var connection = _connectionFactory.CreateMasterConnection();
        return await connection.QueryAsync<StatusMaster>(sql, new { ProjectID = projectId });
    }
}
