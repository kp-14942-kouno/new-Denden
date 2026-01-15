using DeDen.Models.Entities;

namespace DeDen.Models.DTOs;

/// <summary>
/// ログインレスポンス
/// </summary>
public class LoginResponse
{
    /// <summary>
    /// 認証成功
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// エラーメッセージ
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// オペレーター情報
    /// </summary>
    public OperatorMaster? Operator { get; set; }

    /// <summary>
    /// 案件情報
    /// </summary>
    public ProjectMaster? Project { get; set; }
}
