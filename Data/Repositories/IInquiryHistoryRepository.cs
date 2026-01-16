using DenDen.Models.DTOs;
using DenDen.Models.Entities;

namespace DenDen.Data.Repositories;

/// <summary>
/// 問合せ履歴リポジトリのインターフェース
/// </summary>
public interface IInquiryHistoryRepository
{
    /// <summary>
    /// 問合せ履歴を検索
    /// </summary>
    Task<IEnumerable<InquiryHistory>> SearchAsync(int projectId, InquirySearchRequest request);

    /// <summary>
    /// 顧客の問合せ履歴を取得
    /// </summary>
    Task<IEnumerable<InquiryHistory>> GetByCustomerKeyAsync(int projectId, string customerKey);

    /// <summary>
    /// 問合せIDで取得
    /// </summary>
    Task<InquiryHistory?> GetByIdAsync(int inquiryId);

    /// <summary>
    /// 新規登録
    /// </summary>
    Task<int> InsertAsync(InquiryHistory inquiry);

    /// <summary>
    /// 更新
    /// </summary>
    Task UpdateAsync(InquiryHistory inquiry);
}
