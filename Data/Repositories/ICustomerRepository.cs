using DeDen.Models.DTOs;

namespace DeDen.Data.Repositories;

/// <summary>
/// 顧客マスタリポジトリのインターフェース
/// </summary>
public interface ICustomerRepository
{
    /// <summary>
    /// 顧客を検索
    /// </summary>
    Task<IEnumerable<CustomerSearchResult>> SearchAsync(string? customerId, string? customerName, string? telNo);

    /// <summary>
    /// 顧客IDで取得
    /// </summary>
    Task<CustomerSearchResult?> GetByIdAsync(string customerId);

    /// <summary>
    /// 顧客の全カラムデータを取得
    /// </summary>
    Task<Dictionary<string, object?>> GetCustomerDataAsync(string customerId);
}
