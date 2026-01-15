using DeDen.Models.Entities;

namespace DeDen.Services;

/// <summary>
/// セッション管理クラス
/// ログイン中のオペレーター情報と案件情報を保持する
/// </summary>
public class SessionManager
{
    /// <summary>
    /// 現在ログイン中のオペレーター
    /// </summary>
    public OperatorMaster? CurrentOperator { get; private set; }

    /// <summary>
    /// 現在選択中の案件
    /// </summary>
    public ProjectMaster? CurrentProject { get; private set; }

    /// <summary>
    /// ログイン日時
    /// </summary>
    public DateTime? LoginTime { get; private set; }

    /// <summary>
    /// ログイン済みかどうか
    /// </summary>
    public bool IsLoggedIn => CurrentOperator != null && CurrentProject != null;

    /// <summary>
    /// 現在のオペレーターが管理者かどうか
    /// </summary>
    public bool IsAdmin => CurrentOperator?.Role == "Admin";

    /// <summary>
    /// セッション変更時に発火するイベント
    /// </summary>
    public event EventHandler? SessionChanged;

    /// <summary>
    /// ログインセッションを開始する
    /// </summary>
    /// <param name="operatorMaster">ログインしたオペレーター</param>
    /// <param name="project">選択された案件</param>
    public void StartSession(OperatorMaster operatorMaster, ProjectMaster project)
    {
        CurrentOperator = operatorMaster ?? throw new ArgumentNullException(nameof(operatorMaster));
        CurrentProject = project ?? throw new ArgumentNullException(nameof(project));
        LoginTime = DateTime.Now;

        OnSessionChanged();
    }

    /// <summary>
    /// ログアウトしてセッションをクリアする
    /// </summary>
    public void EndSession()
    {
        CurrentOperator = null;
        CurrentProject = null;
        LoginTime = null;

        OnSessionChanged();
    }

    /// <summary>
    /// セッション変更イベントを発火
    /// </summary>
    protected virtual void OnSessionChanged()
    {
        SessionChanged?.Invoke(this, EventArgs.Empty);
    }
}
