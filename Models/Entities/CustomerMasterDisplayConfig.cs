namespace DeDen.Models.Entities;

/// <summary>
/// 顧客マスタ表示設定グループ
/// </summary>
public class CustomerMasterDisplayConfig
{
    /// <summary>
    /// 設定ID
    /// </summary>
    public int ConfigID { get; set; }

    /// <summary>
    /// 案件ID
    /// </summary>
    public int ProjectID { get; set; }

    /// <summary>
    /// グループID（1~5）
    /// </summary>
    public int GroupID { get; set; }

    /// <summary>
    /// グループ名
    /// </summary>
    public string GroupName { get; set; } = string.Empty;

    /// <summary>
    /// 表示順
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// 作成日時
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
