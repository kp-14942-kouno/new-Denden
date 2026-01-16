using System.Data;
using Dapper;
using DenDen.Data.Database;
using DenDen.Models.Entities;

namespace DenDen.Data.Repositories;

/// <summary>
/// オペレーターマスタリポジトリ
/// </summary>
public class OperatorRepository : IOperatorRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public OperatorRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    /// <summary>
    /// ログインIDとProjectIDで検索
    /// </summary>
    public async Task<OperatorMaster?> GetByLoginIdAsync(int projectId, string loginId)
    {
        const string sql = @"
            SELECT * FROM OperatorMaster
            WHERE ProjectID = @ProjectID
              AND LoginID = @LoginID";

        using var connection = _connectionFactory.CreateMasterConnection();
        return await connection.QueryFirstOrDefaultAsync<OperatorMaster>(sql, new { ProjectID = projectId, LoginID = loginId });
    }

    /// <summary>
    /// オペレーターIDで取得
    /// </summary>
    public async Task<OperatorMaster?> GetByIdAsync(int operatorId)
    {
        const string sql = @"
            SELECT * FROM OperatorMaster
            WHERE OperatorID = @OperatorID";

        using var connection = _connectionFactory.CreateMasterConnection();
        return await connection.QueryFirstOrDefaultAsync<OperatorMaster>(sql, new { OperatorID = operatorId });
    }

    /// <summary>
    /// 案件に紐づくオペレーターを全件取得
    /// </summary>
    public async Task<IEnumerable<OperatorMaster>> GetAllByProjectIdAsync(int projectId)
    {
        const string sql = @"
            SELECT * FROM OperatorMaster
            WHERE ProjectID = @ProjectID
              AND IsActive = 1
            ORDER BY OperatorName";

        using var connection = _connectionFactory.CreateMasterConnection();
        return await connection.QueryAsync<OperatorMaster>(sql, new { ProjectID = projectId });
    }
}
