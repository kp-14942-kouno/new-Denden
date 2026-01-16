using System.Windows.Input;

namespace DenDen.Common;

/// <summary>
/// ICommandの実装（パラメータなし）
/// </summary>
public class RelayCommand : ICommand
{
    private readonly Action _execute;
    private readonly Func<bool>? _canExecute;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="execute">実行するアクション</param>
    /// <param name="canExecute">実行可能かどうかを判定する関数（省略可）</param>
    public RelayCommand(Action execute, Func<bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    /// <summary>
    /// コマンドが実行可能かどうかを判定
    /// </summary>
    public bool CanExecute(object? parameter)
    {
        return _canExecute?.Invoke() ?? true;
    }

    /// <summary>
    /// コマンドを実行
    /// </summary>
    public void Execute(object? parameter)
    {
        _execute();
    }

    /// <summary>
    /// CanExecuteの変更を通知
    /// </summary>
    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    /// <summary>
    /// CanExecuteの再評価を強制
    /// </summary>
    public void RaiseCanExecuteChanged()
    {
        CommandManager.InvalidateRequerySuggested();
    }
}

/// <summary>
/// ICommandの実装（パラメータあり）
/// </summary>
/// <typeparam name="T">パラメータの型</typeparam>
public class RelayCommand<T> : ICommand
{
    private readonly Action<T?> _execute;
    private readonly Func<T?, bool>? _canExecute;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="execute">実行するアクション</param>
    /// <param name="canExecute">実行可能かどうかを判定する関数（省略可）</param>
    public RelayCommand(Action<T?> execute, Func<T?, bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    /// <summary>
    /// コマンドが実行可能かどうかを判定
    /// </summary>
    public bool CanExecute(object? parameter)
    {
        return _canExecute?.Invoke((T?)parameter) ?? true;
    }

    /// <summary>
    /// コマンドを実行
    /// </summary>
    public void Execute(object? parameter)
    {
        _execute((T?)parameter);
    }

    /// <summary>
    /// CanExecuteの変更を通知
    /// </summary>
    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    /// <summary>
    /// CanExecuteの再評価を強制
    /// </summary>
    public void RaiseCanExecuteChanged()
    {
        CommandManager.InvalidateRequerySuggested();
    }
}
