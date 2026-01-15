using DeDen.Models.Entities;

namespace DeDen.Data.Repositories;

/// <summary>
/// ステータスマスタリポジトリのインターフェース
/// </summary>
public interface IStatusRepository
{
    /// <summary>
    /// 案件に紐づく有効なステータスを全件取得
    /// </summary>
    Task<IEnumerable<StatusMaster>> GetAllByProjectIdAsync(int projectId);
}
