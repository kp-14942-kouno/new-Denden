namespace DeDen.Models.DTOs;

/// <summary>
/// ログインリクエスト
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// 案件ID
    /// </summary>
    public int ProjectId { get; set; }

    /// <summary>
    /// ログインID
    /// </summary>
    public string LoginId { get; set; } = string.Empty;

    /// <summary>
    /// パスワード（平文）
    /// </summary>
    public string Password { get; set; } = string.Empty;
}
