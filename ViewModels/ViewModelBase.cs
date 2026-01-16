using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DenDen.ViewModels;

/// <summary>
/// ViewModelの基底クラス
/// INotifyPropertyChangedを実装し、プロパティ変更通知機能を提供
/// </summary>
public abstract class ViewModelBase : INotifyPropertyChanged
{
    /// <summary>
    /// プロパティ変更通知イベント
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// プロパティ変更通知を発火
    /// </summary>
    /// <param name="propertyName">プロパティ名（省略時は呼び出し元プロパティ名）</param>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// プロパティ値を設定し、変更があれば通知を発火
    /// </summary>
    /// <typeparam name="T">プロパティの型</typeparam>
    /// <param name="field">バッキングフィールド</param>
    /// <param name="value">新しい値</param>
    /// <param name="propertyName">プロパティ名（省略時は呼び出し元プロパティ名）</param>
    /// <returns>値が変更された場合true</returns>
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
