using DeDen.Models.Entities;

namespace DeDen.Data.Repositories;

/// <summary>
/// 問合せ履歴更新ログリポジトリのインターフェース
/// </summary>
public interface IInquiryHistoryLogRepository
{
    /// <summary>
    /// 更新ログを追加
    /// </summary>
    Task<int> InsertAsync(InquiryHistoryLog log);

    /// <summary>
    /// 問合せIDに紐づく更新ログを取得
    /// </summary>
    Task<IEnumerable<InquiryHistoryLog>> GetByInquiryIdAsync(int inquiryId);
}
