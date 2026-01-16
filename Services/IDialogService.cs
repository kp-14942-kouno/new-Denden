namespace DenDen.Services;

/// <summary>
/// ダイアログサービスのインターフェース
/// </summary>
public interface IDialogService
{
    /// <summary>
    /// 情報メッセージを表示
    /// </summary>
    /// <param name="message">メッセージ</param>
    /// <param name="title">タイトル（省略時は「情報」）</param>
    void ShowMessage(string message, string title = "情報");

    /// <summary>
    /// 警告メッセージを表示
    /// </summary>
    /// <param name="message">メッセージ</param>
    /// <param name="title">タイトル（省略時は「警告」）</param>
    void ShowWarning(string message, string title = "警告");

    /// <summary>
    /// エラーメッセージを表示
    /// </summary>
    /// <param name="message">メッセージ</param>
    /// <param name="title">タイトル（省略時は「エラー」）</param>
    void ShowError(string message, string title = "エラー");

    /// <summary>
    /// 確認ダイアログを表示
    /// </summary>
    /// <param name="message">メッセージ</param>
    /// <param name="title">タイトル（省略時は「確認」）</param>
    /// <returns>「はい」を選択した場合true</returns>
    bool ShowConfirm(string message, string title = "確認");

    /// <summary>
    /// 3択の確認ダイアログを表示
    /// </summary>
    /// <param name="message">メッセージ</param>
    /// <param name="title">タイトル</param>
    /// <returns>Yes=true, No=false, Cancel=null</returns>
    bool? ShowConfirmWithCancel(string message, string title = "確認");
}
