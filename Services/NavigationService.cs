using DenDen.ViewModels;

namespace DenDen.Services;

/// <summary>
/// ナビゲーションサービスの実装
/// DIコンテナを使用してViewModelを解決し、画面遷移を管理する
/// </summary>
public class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;
    private ViewModelBase? _currentViewModel;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <inheritdoc/>
    public ViewModelBase? CurrentViewModel
    {
        get => _currentViewModel;
        private set
        {
            if (_currentViewModel != value)
            {
                _currentViewModel = value;
                OnCurrentViewModelChanged();
            }
        }
    }

    /// <inheritdoc/>
    public event EventHandler<ViewModelBase?>? CurrentViewModelChanged;

    /// <inheritdoc/>
    public void NavigateTo<TViewModel>() where TViewModel : ViewModelBase
    {
        var viewModel = _serviceProvider.GetService(typeof(TViewModel)) as TViewModel;
        if (viewModel != null)
        {
            CurrentViewModel = viewModel;
        }
    }

    /// <inheritdoc/>
    public void NavigateTo(ViewModelBase viewModel)
    {
        CurrentViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
    }

    /// <inheritdoc/>
    public void Clear()
    {
        CurrentViewModel = null;
    }

    /// <summary>
    /// CurrentViewModelChanged イベントを発火
    /// </summary>
    protected virtual void OnCurrentViewModelChanged()
    {
        CurrentViewModelChanged?.Invoke(this, CurrentViewModel);
    }
}
