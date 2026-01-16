using DenDen.ViewModels;

namespace DenDen.Services;

/// <summary>
/// ナビゲーションサービスのインターフェース
/// 画面遷移を管理する
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// 現在のViewModel
    /// </summary>
    ViewModelBase? CurrentViewModel { get; }

    /// <summary>
    /// ナビゲーション変更時に発火するイベント
    /// </summary>
    event EventHandler<ViewModelBase?>? CurrentViewModelChanged;

    /// <summary>
    /// 指定したViewModelに遷移する
    /// </summary>
    /// <typeparam name="TViewModel">ViewModelの型</typeparam>
    void NavigateTo<TViewModel>() where TViewModel : ViewModelBase;

    /// <summary>
    /// 指定したViewModelのインスタンスに遷移する
    /// </summary>
    /// <param name="viewModel">ViewModelインスタンス</param>
    void NavigateTo(ViewModelBase viewModel);

    /// <summary>
    /// ナビゲーション履歴をクリアする
    /// </summary>
    void Clear();
}
