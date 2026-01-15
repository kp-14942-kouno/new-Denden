using DeDen.Models.DTOs;

namespace DeDen.Services;

/// <summary>
/// 認証サービスのインターフェース
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// ログイン認証を行う
    /// </summary>
    /// <param name="request">ログインリクエスト</param>
    /// <returns>ログインレスポンス</returns>
    Task<LoginResponse> LoginAsync(LoginRequest request);

    /// <summary>
    /// パスワードをハッシュ化する
    /// </summary>
    /// <param name="password">平文パスワード</param>
    /// <returns>ハッシュ化されたパスワード</returns>
    string HashPassword(string password);

    /// <summary>
    /// パスワードを検証する
    /// </summary>
    /// <param name="password">平文パスワード</param>
    /// <param name="hash">ハッシュ値</param>
    /// <returns>一致する場合true</returns>
    bool VerifyPassword(string password, string hash);
}
