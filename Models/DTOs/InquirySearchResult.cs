namespace DenDen.Models.DTOs;

/// <summary>
/// 問合せ履歴検索結果
/// </summary>
public class InquirySearchResult
{
    /// <summary>
    /// 問合せID
    /// </summary>
    public int InquiryID { get; set; }

    /// <summary>
    /// 顧客キー
    /// </summary>
    public string? CustomerKey { get; set; }

    /// <summary>
    /// 顧客名
    /// </summary>
    public string? CustomerName { get; set; }

    /// <summary>
    /// オペレーター名
    /// </summary>
    public string OperatorName { get; set; } = string.Empty;

    /// <summary>
    /// カテゴリー名
    /// </summary>
    public string? CategoryName { get; set; }

    /// <summary>
    /// ステータス名
    /// </summary>
    public string? StatusName { get; set; }

    /// <summary>
    /// 問合せ内容
    /// </summary>
    public string InquiryContent { get; set; } = string.Empty;

    /// <summary>
    /// 対応内容
    /// </summary>
    public string? ResponseContent { get; set; }

    /// <summary>
    /// 受電日時
    /// </summary>
    public DateTime FirstReceivedDateTime { get; set; }

    /// <summary>
    /// 更新日時
    /// </summary>
    public DateTime? UpdatedDateTime { get; set; }
}
