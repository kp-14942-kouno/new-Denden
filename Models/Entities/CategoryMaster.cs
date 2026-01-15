namespace DeDen.Models.Entities;

/// <summary>
/// カテゴリーマスタ
/// </summary>
public class CategoryMaster
{
    /// <summary>
    /// カテゴリーID
    /// </summary>
    public int CategoryID { get; set; }

    /// <summary>
    /// 案件ID
    /// </summary>
    public int ProjectID { get; set; }

    /// <summary>
    /// カテゴリー名
    /// </summary>
    public string CategoryName { get; set; } = string.Empty;

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
