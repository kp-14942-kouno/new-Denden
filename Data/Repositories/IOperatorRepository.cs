using DenDen.Models.Entities;

namespace DenDen.Data.Repositories;

/// <summary>
/// オペレーターマスタリポジトリのインターフェース
/// </summary>
public interface IOperatorRepository
{
    /// <summary>
    /// ログインIDとProjectIDで検索
    /// </summary>
    Task<OperatorMaster?> GetByLoginIdAsync(int projectId, string loginId);

    /// <summary>
    /// オペレーターIDで取得
    /// </summary>
    Task<OperatorMaster?> GetByIdAsync(int operatorId);

    /// <summary>
    /// 案件に紐づくオペレーターを全件取得
    /// </summary>
    Task<IEnumerable<OperatorMaster>> GetAllByProjectIdAsync(int projectId);
}
