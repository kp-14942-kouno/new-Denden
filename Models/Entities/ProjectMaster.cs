namespace DeDen.Models.Entities;

/// <summary>
/// 案件マスタ
/// </summary>
public class ProjectMaster
{
    /// <summary>
    /// 案件ID
    /// </summary>
    public int ProjectID { get; set; }

    /// <summary>
    /// 案件コード
    /// </summary>
    public string ProjectCode { get; set; } = string.Empty;

    /// <summary>
    /// 案件名
    /// </summary>
    public string ProjectName { get; set; } = string.Empty;

    /// <summary>
    /// 説明
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 有効/無効
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// 作成日時
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新日時
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
