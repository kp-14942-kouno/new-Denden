namespace DenDen.Models.Entities;

/// <summary>
/// オペレーターマスタ
/// </summary>
public class OperatorMaster
{
    /// <summary>
    /// オペレーターID
    /// </summary>
    public int OperatorID { get; set; }

    /// <summary>
    /// 案件ID
    /// </summary>
    public int ProjectID { get; set; }

    /// <summary>
    /// ログインID
    /// </summary>
    public string LoginID { get; set; } = string.Empty;

    /// <summary>
    /// オペレーター名
    /// </summary>
    public string OperatorName { get; set; } = string.Empty;

    /// <summary>
    /// パスワードハッシュ（BCrypt）
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// 権限（General / Admin）
    /// </summary>
    public string Role { get; set; } = "General";

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
