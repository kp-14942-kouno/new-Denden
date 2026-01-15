namespace DeDen.Models.DTOs;

/// <summary>
/// 問合せ履歴検索リクエスト
/// </summary>
public class InquirySearchRequest
{
    /// <summary>
    /// 受電日（開始）
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// 受電日（終了）
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// カテゴリーID
    /// </summary>
    public int? CategoryID { get; set; }

    /// <summary>
    /// ステータスID
    /// </summary>
    public int? StatusID { get; set; }

    /// <summary>
    /// オペレーターID
    /// </summary>
    public int? OperatorID { get; set; }

    /// <summary>
    /// 顧客キー
    /// </summary>
    public string? CustomerKey { get; set; }

    /// <summary>
    /// キーワード（問合せ内容、対応内容から検索）
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// カスタム項目検索条件
    /// </summary>
    public Dictionary<string, string>? CustomFieldConditions { get; set; }
}
