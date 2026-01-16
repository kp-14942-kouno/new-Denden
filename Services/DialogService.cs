using System.Windows;

namespace DenDen.Services;

/// <summary>
/// ダイアログサービスの実装
/// WPFのMessageBoxを使用してダイアログを表示
/// </summary>
public class DialogService : IDialogService
{
    /// <inheritdoc/>
    public void ShowMessage(string message, string title = "情報")
    {
        MessageBox.Show(
            message,
            title,
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    /// <inheritdoc/>
    public void ShowWarning(string message, string title = "警告")
    {
        MessageBox.Show(
            message,
            title,
            MessageBoxButton.OK,
            MessageBoxImage.Warning);
    }

    /// <inheritdoc/>
    public void ShowError(string message, string title = "エラー")
    {
        MessageBox.Show(
            message,
            title,
            MessageBoxButton.OK,
            MessageBoxImage.Error);
    }

    /// <inheritdoc/>
    public bool ShowConfirm(string message, string title = "確認")
    {
        var result = MessageBox.Show(
            message,
            title,
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        return result == MessageBoxResult.Yes;
    }

    /// <inheritdoc/>
    public bool? ShowConfirmWithCancel(string message, string title = "確認")
    {
        var result = MessageBox.Show(
            message,
            title,
            MessageBoxButton.YesNoCancel,
            MessageBoxImage.Question);

        return result switch
        {
            MessageBoxResult.Yes => true,
            MessageBoxResult.No => false,
            _ => null
        };
    }
}
