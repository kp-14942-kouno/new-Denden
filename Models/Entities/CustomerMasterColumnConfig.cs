namespace DeDen.Models.Entities;

/// <summary>
/// 顧客マスタ項目設定
/// </summary>
public class CustomerMasterColumnConfig
{
    /// <summary>
    /// 項目設定ID
    /// </summary>
    public int ColumnConfigID { get; set; }

    /// <summary>
    /// 設定ID
    /// </summary>
    public int ConfigID { get; set; }

    /// <summary>
    /// DBのカラム名
    /// </summary>
    public string ColumnName { get; set; } = string.Empty;

    /// <summary>
    /// 表示名
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// グループ内の表示順
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// 作成日時
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
