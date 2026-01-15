using DeDen.Models.Entities;

namespace DeDen.Data.Repositories;

/// <summary>
/// 案件マスタリポジトリのインターフェース
/// </summary>
public interface IProjectRepository
{
    /// <summary>
    /// 有効な案件を全件取得
    /// </summary>
    Task<IEnumerable<ProjectMaster>> GetAllActiveAsync();

    /// <summary>
    /// 案件IDで取得
    /// </summary>
    Task<ProjectMaster?> GetByIdAsync(int projectId);

    /// <summary>
    /// 案件コードで取得
    /// </summary>
    Task<ProjectMaster?> GetByCodeAsync(string projectCode);
}
