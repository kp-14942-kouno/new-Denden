using System.Data;
using Dapper;
using DenDen.Data.Database;
using DenDen.Models.Entities;

namespace DenDen.Data.Repositories;

/// <summary>
/// 案件マスタリポジトリ
/// </summary>
public class ProjectRepository : IProjectRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ProjectRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    /// <summary>
    /// 有効な案件を全件取得
    /// </summary>
    public async Task<IEnumerable<ProjectMaster>> GetAllActiveAsync()
    {
        const string sql = @"
            SELECT * FROM ProjectMaster
            WHERE IsActive = 1
            ORDER BY ProjectName";

        using var connection = _connectionFactory.CreateMasterConnection();
        return await connection.QueryAsync<ProjectMaster>(sql);
    }

    /// <summary>
    /// 案件IDで取得
    /// </summary>
    public async Task<ProjectMaster?> GetByIdAsync(int projectId)
    {
        const string sql = @"
            SELECT * FROM ProjectMaster
            WHERE ProjectID = @ProjectID";

        using var connection = _connectionFactory.CreateMasterConnection();
        return await connection.QueryFirstOrDefaultAsync<ProjectMaster>(sql, new { ProjectID = projectId });
    }

    /// <summary>
    /// 案件コードで取得
    /// </summary>
    public async Task<ProjectMaster?> GetByCodeAsync(string projectCode)
    {
        const string sql = @"
            SELECT * FROM ProjectMaster
            WHERE ProjectCode = @ProjectCode";

        using var connection = _connectionFactory.CreateMasterConnection();
        return await connection.QueryFirstOrDefaultAsync<ProjectMaster>(sql, new { ProjectCode = projectCode });
    }
}
