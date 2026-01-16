using System.Windows;
using DenDen.ViewModels;
using DenDen.Views;

namespace DenDen;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly LoginViewModel _loginViewModel;
    private readonly MainViewModel _mainViewModel;

    public MainWindow(LoginViewModel loginViewModel, MainViewModel mainViewModel)
    {
        InitializeComponent();

        _loginViewModel = loginViewModel;
        _mainViewModel = mainViewModel;

        // ログイン成功時のイベント購読
        _loginViewModel.LoginSucceeded += OnLoginSucceeded;

        // ログアウト要求時のイベント購読
        _mainViewModel.LogoutRequested += OnLogoutRequested;

        // 初期表示はログイン画面
        ShowLoginView();
    }

    /// <summary>
    /// ログイン画面を表示
    /// </summary>
    private void ShowLoginView()
    {
        var loginView = new LoginView
        {
            DataContext = _loginViewModel
        };
        MainContent.Content = loginView;
        Title = "コール業務管理システム - ログイン";
    }

    /// <summary>
    /// メイン画面を表示
    /// </summary>
    private async void ShowMainView()
    {
        var mainView = new MainView
        {
            DataContext = _mainViewModel
        };
        MainContent.Content = mainView;
        Title = "コール業務管理システム";

        // マスタデータの初期化
        await _mainViewModel.InitializeAsync();
    }

    /// <summary>
    /// ログイン成功時の処理
    /// </summary>
    private void OnLoginSucceeded(object? sender, EventArgs e)
    {
        ShowMainView();
    }

    /// <summary>
    /// ログアウト要求時の処理
    /// </summary>
    private void OnLogoutRequested(object? sender, EventArgs e)
    {
        // ログイン情報をクリア
        _loginViewModel.LoginId = string.Empty;
        _loginViewModel.Password = string.Empty;
        _loginViewModel.ErrorMessage = string.Empty;

        ShowLoginView();
    }
}