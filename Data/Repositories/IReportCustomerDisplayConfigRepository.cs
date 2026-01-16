using DenDen.Models.Entities;

namespace DenDen.Data.Repositories;

/// <summary>
/// レポート用顧客情報表示設定リポジトリのインターフェース
/// </summary>
public interface IReportCustomerDisplayConfigRepository
{
    /// <summary>
    /// 指定案件の表示設定を取得（DisplayOrder順）
    /// </summary>
    /// <param name="projectId">案件ID</param>
    /// <returns>表示設定リスト（最大3件）</returns>
    Task<IEnumerable<ReportCustomerDisplayConfig>> GetByProjectIdAsync(int projectId);
}
