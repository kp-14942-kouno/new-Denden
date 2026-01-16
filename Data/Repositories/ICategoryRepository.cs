using DenDen.Models.Entities;

namespace DenDen.Data.Repositories;

/// <summary>
/// カテゴリーマスタリポジトリのインターフェース
/// </summary>
public interface ICategoryRepository
{
    /// <summary>
    /// 案件に紐づく有効なカテゴリーを全件取得
    /// </summary>
    Task<IEnumerable<CategoryMaster>> GetAllByProjectIdAsync(int projectId);
}
