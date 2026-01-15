namespace DeDen.Models.Entities;

/// <summary>
/// ステータスマスタ
/// </summary>
public class StatusMaster
{
    /// <summary>
    /// ステータスID
    /// </summary>
    public int StatusID { get; set; }

    /// <summary>
    /// 案件ID
    /// </summary>
    public int ProjectID { get; set; }

    /// <summary>
    /// ステータス名
    /// </summary>
    public string StatusName { get; set; } = string.Empty;

    /// <summary>
    /// 表示順
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// 有効/無効
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// 作成日時
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
