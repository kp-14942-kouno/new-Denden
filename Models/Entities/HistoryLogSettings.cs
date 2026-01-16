namespace DenDen.Models.Entities;

/// <summary>
/// 履歴保存設定
/// </summary>
public class HistoryLogSettings
{
    /// <summary>
    /// 設定ID
    /// </summary>
    public int SettingID { get; set; }

    /// <summary>
    /// 案件ID
    /// </summary>
    public int ProjectID { get; set; }

    /// <summary>
    /// 履歴保存有効
    /// </summary>
    public bool EnableHistoryLog { get; set; }

    /// <summary>
    /// 作成日時
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新日時
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
