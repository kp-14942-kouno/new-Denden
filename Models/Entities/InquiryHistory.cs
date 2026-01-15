namespace DeDen.Models.Entities;

/// <summary>
/// 問合せ履歴
/// </summary>
public class InquiryHistory
{
    /// <summary>
    /// 問合せID
    /// </summary>
    public int InquiryID { get; set; }

    /// <summary>
    /// 案件ID
    /// </summary>
    public int ProjectID { get; set; }

    /// <summary>
    /// 顧客マスタのキー
    /// </summary>
    public string? CustomerKey { get; set; }

    /// <summary>
    /// オペレーターID
    /// </summary>
    public int OperatorID { get; set; }

    /// <summary>
    /// カテゴリーID
    /// </summary>
    public int? CategoryID { get; set; }

    /// <summary>
    /// ステータスID
    /// </summary>
    public int? StatusID { get; set; }

    /// <summary>
    /// 問合せ内容
    /// </summary>
    public string InquiryContent { get; set; } = string.Empty;

    /// <summary>
    /// 対応内容
    /// </summary>
    public string? ResponseContent { get; set; }

    /// <summary>
    /// 初回受電日時
    /// </summary>
    public DateTime FirstReceivedDateTime { get; set; }

    /// <summary>
    /// 更新日時
    /// </summary>
    public DateTime? UpdatedDateTime { get; set; }

    /// <summary>
    /// 作成日時
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 作成者（OperatorID）
    /// </summary>
    public int CreatedBy { get; set; }

    /// <summary>
    /// 最終更新者（OperatorID）
    /// </summary>
    public int? UpdatedBy { get; set; }

    /// <summary>
    /// カスタム項目1
    /// </summary>
    public string? CustomCol01 { get; set; }

    /// <summary>
    /// カスタム項目2
    /// </summary>
    public string? CustomCol02 { get; set; }

    /// <summary>
    /// カスタム項目3
    /// </summary>
    public string? CustomCol03 { get; set; }

    /// <summary>
    /// カスタム項目4
    /// </summary>
    public string? CustomCol04 { get; set; }

    /// <summary>
    /// カスタム項目5
    /// </summary>
    public string? CustomCol05 { get; set; }

    /// <summary>
    /// カスタム項目6
    /// </summary>
    public string? CustomCol06 { get; set; }

    /// <summary>
    /// カスタム項目7
    /// </summary>
    public string? CustomCol07 { get; set; }

    /// <summary>
    /// カスタム項目8
    /// </summary>
    public string? CustomCol08 { get; set; }

    /// <summary>
    /// カスタム項目9
    /// </summary>
    public string? CustomCol09 { get; set; }

    /// <summary>
    /// カスタム項目10
    /// </summary>
    public string? CustomCol10 { get; set; }
}
