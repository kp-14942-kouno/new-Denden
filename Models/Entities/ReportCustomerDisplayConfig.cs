namespace DenDen.Models.Entities;

/// <summary>
/// レポート用顧客情報表示設定
/// </summary>
public class ReportCustomerDisplayConfig
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
    /// 表示順（1〜3）
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// カラム名（顧客マスタのカラム名）
    /// </summary>
    public string ColumnName { get; set; } = string.Empty;

    /// <summary>
    /// 表示名
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// 作成日時
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
