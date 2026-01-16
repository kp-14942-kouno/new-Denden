namespace DenDen.Models.DTOs;

/// <summary>
/// 顧客検索結果
/// </summary>
public class CustomerSearchResult
{
    /// <summary>
    /// 顧客キー
    /// </summary>
    public string CustomerKey { get; set; } = string.Empty;

    /// <summary>
    /// 顧客名
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// 電話番号
    /// </summary>
    public string? TelNo { get; set; }

    /// <summary>
    /// メールアドレス
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 顧客情報（動的）
    /// </summary>
    public Dictionary<string, object?> CustomerData { get; set; } = new();
}
