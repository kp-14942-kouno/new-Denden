namespace DenDen.Models.Entities;

/// <summary>
/// 顧客メモ
/// </summary>
public class CustomerMemo
{
    /// <summary>
    /// メモID
    /// </summary>
    public int MemoID { get; set; }

    /// <summary>
    /// 案件ID
    /// </summary>
    public int ProjectID { get; set; }

    /// <summary>
    /// 顧客マスタのキー
    /// </summary>
    public string CustomerKey { get; set; } = string.Empty;

    /// <summary>
    /// メモ内容
    /// </summary>
    public string MemoContent { get; set; } = string.Empty;

    /// <summary>
    /// 作成者（OperatorID）
    /// </summary>
    public int CreatedBy { get; set; }

    /// <summary>
    /// 作成日時
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新者（OperatorID）
    /// </summary>
    public int? UpdatedBy { get; set; }

    /// <summary>
    /// 更新日時
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
