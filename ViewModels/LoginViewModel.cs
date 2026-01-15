using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using DeDen.Common;
using DeDen.Data.Repositories;
using DeDen.Models.DTOs;
using DeDen.Models.Entities;
using DeDen.Services;

namespace DeDen.ViewModels;

/// <summary>
/// ログイン画面のViewModel
/// </summary>
public class LoginViewModel : ViewModelBase
{
    private readonly IProjectRepository _projectRepository;
    private readonly IAuthenticationService _authenticationService;
    private readonly SessionManager _sessionManager;
    private readonly IDialogService _dialogService;

    private ObservableCollection<ProjectMaster> _projects = new();
    private ProjectMaster? _selectedProject;
    private string _loginId = string.Empty;
    private string _password = string.Empty;
    private bool _isLoading;
    private string _errorMessage = string.Empty;

    public LoginViewModel(
        IProjectRepository projectRepository,
        IAuthenticationService authenticationService,
        SessionManager sessionManager,
        IDialogService dialogService)
    {
        _projectRepository = projectRepository;
        _authenticationService = authenticationService;
        _sessionManager = sessionManager;
        _dialogService = dialogService;

        LoginCommand = new RelayCommand(async () => await LoginAsync(), CanLogin);
        ExitCommand = new RelayCommand(Exit);
    }

    /// <summary>
    /// 案件リスト
    /// </summary>
    public ObservableCollection<ProjectMaster> Projects
    {
        get => _projects;
        set => SetProperty(ref _projects, value);
    }

    /// <summary>
    /// 選択された案件
    /// </summary>
    public ProjectMaster? SelectedProject
    {
        get => _selectedProject;
        set
        {
            if (SetProperty(ref _selectedProject, value))
            {
                ErrorMessage = string.Empty;
            }
        }
    }

    /// <summary>
    /// ログインID
    /// </summary>
    public string LoginId
    {
        get => _loginId;
        set
        {
            if (SetProperty(ref _loginId, value))
            {
                ErrorMessage = string.Empty;
            }
        }
    }

    /// <summary>
    /// パスワード
    /// </summary>
    public string Password
    {
        get => _password;
        set
        {
            if (SetProperty(ref _password, value))
            {
                ErrorMessage = string.Empty;
            }
        }
    }

    /// <summary>
    /// ローディング中かどうか
    /// </summary>
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    /// <summary>
    /// エラーメッセージ
    /// </summary>
    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    /// <summary>
    /// ログインコマンド
    /// </summary>
    public ICommand LoginCommand { get; }

    /// <summary>
    /// 終了コマンド
    /// </summary>
    public ICommand ExitCommand { get; }

    /// <summary>
    /// ログイン成功時に発火するイベント
    /// </summary>
    public event EventHandler? LoginSucceeded;

    /// <summary>
    /// 案件リストを読み込む
    /// </summary>
    public async Task LoadProjectsAsync()
    {
        try
        {
            IsLoading = true;
            var projects = await _projectRepository.GetAllActiveAsync();
            Projects = new ObservableCollection<ProjectMaster>(projects);

            if (Projects.Count > 0)
            {
                SelectedProject = Projects[0];
            }
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"案件リストの読み込みに失敗しました。\n{ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// ログイン処理
    /// </summary>
    private async Task LoginAsync()
    {
        if (!CanLogin())
        {
            return;
        }

        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            var request = new LoginRequest
            {
                ProjectId = SelectedProject!.ProjectID,
                LoginId = LoginId,
                Password = Password
            };

            var response = await _authenticationService.LoginAsync(request);

            if (response.IsSuccess)
            {
                _sessionManager.StartSession(response.Operator!, response.Project!);
                LoginSucceeded?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                ErrorMessage = response.ErrorMessage ?? "ログインに失敗しました。";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"エラーが発生しました: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// ログインが実行可能かどうか
    /// </summary>
    private bool CanLogin()
    {
        return !IsLoading
            && SelectedProject != null
            && !string.IsNullOrWhiteSpace(LoginId)
            && !string.IsNullOrWhiteSpace(Password);
    }

    /// <summary>
    /// アプリケーション終了
    /// </summary>
    private void Exit()
    {
        Application.Current.Shutdown();
    }
}
