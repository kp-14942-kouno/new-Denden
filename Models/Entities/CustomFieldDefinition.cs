namespace DenDen.Models.Entities;

/// <summary>
/// カスタム項目定義
/// </summary>
public class CustomFieldDefinition
{
    /// <summary>
    /// フィールドID
    /// </summary>
    public int FieldID { get; set; }

    /// <summary>
    /// 案件ID
    /// </summary>
    public int ProjectID { get; set; }

    /// <summary>
    /// カラム番号（1~10）
    /// </summary>
    public int ColumnNumber { get; set; }

    /// <summary>
    /// 項目名（内部用）
    /// </summary>
    public string FieldName { get; set; } = string.Empty;

    /// <summary>
    /// 表示名
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// 項目タイプ（Text/Number/Date/Select）
    /// </summary>
    public string FieldType { get; set; } = "Text";

    /// <summary>
    /// 必須
    /// </summary>
    public bool IsRequired { get; set; }

    /// <summary>
    /// 使用する
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// 表示順
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// 選択肢（JSON形式）
    /// </summary>
    public string? SelectOptions { get; set; }

    /// <summary>
    /// 検索可能
    /// </summary>
    public bool IsSearchable { get; set; }

    /// <summary>
    /// 作成日時
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
