using System.Data;
using Dapper;
using DenDen.Data.Database;
using DenDen.Models.Entities;

namespace DenDen.Data.Repositories;

/// <summary>
/// カテゴリーマスタリポジトリ
/// </summary>
public class CategoryRepository : ICategoryRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public CategoryRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    /// <summary>
    /// 案件に紐づく有効なカテゴリーを全件取得
    /// </summary>
    public async Task<IEnumerable<CategoryMaster>> GetAllByProjectIdAsync(int projectId)
    {
        const string sql = @"
            SELECT * FROM CategoryMaster
            WHERE ProjectID = @ProjectID
              AND IsActive = 1
            ORDER BY DisplayOrder";

        using var connection = _connectionFactory.CreateMasterConnection();
        return await connection.QueryAsync<CategoryMaster>(sql, new { ProjectID = projectId });
    }
}
