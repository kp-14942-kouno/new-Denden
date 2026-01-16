using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using DeDen.Common;
using DeDen.Models.DTOs;
using DeDen.Models.Entities;
using DeDen.Services;
using DeDen.Views;

namespace DeDen.ViewModels;

/// <summary>
/// メイン画面のViewModel
/// </summary>
public class MainViewModel : ViewModelBase
{
    private readonly SessionManager _sessionManager;
    private readonly IDialogService _dialogService;
    private readonly IExportService _exportService;
    private readonly IServiceProvider _serviceProvider;

    private string _headerTitle = string.Empty;
    private string _operatorName = string.Empty;
    private string _projectName = string.Empty;
    private string _loginTime = string.Empty;

    public MainViewModel(
        SessionManager sessionManager,
        IDialogService dialogService,
        IExportService exportService,
        IServiceProvider serviceProvider,
        SearchPanelViewModel searchPanelViewModel,
        CustomerInfoViewModel customerInfoViewModel,
        InquiryViewModel inquiryViewModel)
    {
        _sessionManager = sessionManager;
        _dialogService = dialogService;
        _exportService = exportService;
        _serviceProvider = serviceProvider;
        SearchPanel = searchPanelViewModel;
        CustomerInfo = customerInfoViewModel;
        Inquiry = inquiryViewModel;

        LogoutCommand = new RelayCommand(Logout);
        ExportCsvCommand = new RelayCommand(async () => await ExportCsvAsync());
        ShowReportCommand = new RelayCommand(ShowReport);

        // セッション情報の初期化
        UpdateSessionInfo();

        // セッション変更イベントの購読
        _sessionManager.SessionChanged += OnSessionChanged;

        // 検索結果選択時のイベント購読
        SearchPanel.CustomerSelected += OnCustomerSelected;
        SearchPanel.InquirySelected += OnInquirySelected;
        SearchPanel.SearchCleared += OnSearchCleared;

        // 顧客情報の問合せ選択イベント購読
        CustomerInfo.InquirySelected += OnCustomerInquirySelected;

        // 問合せ保存時のイベント購読
        Inquiry.InquirySaved += OnInquirySaved;

        // 顧客紐付けリクエストのイベント購読
        Inquiry.LinkCustomerRequested += OnLinkCustomerRequested;
    }

    /// <summary>
    /// 検索パネルViewModel
    /// </summary>
    public SearchPanelViewModel SearchPanel { get; }

    /// <summary>
    /// 顧客情報ViewModel
    /// </summary>
    public CustomerInfoViewModel CustomerInfo { get; }

    /// <summary>
    /// 問合せ登録ViewModel
    /// </summary>
    public InquiryViewModel Inquiry { get; }

    /// <summary>
    /// 初期化処理
    /// </summary>
    public async Task InitializeAsync()
    {
        await SearchPanel.LoadMasterDataAsync();
        await Inquiry.LoadMasterDataAsync();
    }

    /// <summary>
    /// 顧客選択時の処理
    /// </summary>
    private async void OnCustomerSelected(object? sender, CustomerSearchResult customer)
    {
        await CustomerInfo.LoadCustomerAsync(customer.CustomerKey);
    }

    /// <summary>
    /// 問合せ履歴選択時の処理（検索結果から）
    /// </summary>
    private async void OnInquirySelected(object? sender, InquiryHistory inquiry)
    {
        if (!string.IsNullOrEmpty(inquiry.CustomerKey))
        {
            await CustomerInfo.LoadCustomerAsync(inquiry.CustomerKey);
        }
        else
        {
            CustomerInfo.Clear();
        }
        Inquiry.LoadInquiry(inquiry);
    }

    /// <summary>
    /// 顧客情報の過去履歴から問合せ選択時の処理
    /// </summary>
    private void OnCustomerInquirySelected(object? sender, InquiryHistory inquiry)
    {
        Inquiry.LoadInquiry(inquiry);
    }

    /// <summary>
    /// 検索クリア時の処理
    /// </summary>
    private void OnSearchCleared(object? sender, EventArgs e)
    {
        CustomerInfo.Clear();
        Inquiry.PrepareNew();
    }

    /// <summary>
    /// 問合せ保存時の処理
    /// </summary>
    private async void OnInquirySaved(object? sender, EventArgs e)
    {
        // 顧客情報の履歴を再読み込み
        if (!string.IsNullOrEmpty(CustomerInfo.CurrentCustomerKey))
        {
            await CustomerInfo.LoadCustomerAsync(CustomerInfo.CurrentCustomerKey);
        }
    }

    /// <summary>
    /// 顧客紐付けリクエスト時の処理
    /// </summary>
    private void OnLinkCustomerRequested(object? sender, EventArgs e)
    {
        // 検索結果で選択中の顧客を取得
        if (SearchPanel.SelectedSearchResult is CustomerSearchResult customer)
        {
            Inquiry.SetCustomerKey(customer.CustomerKey);
        }
        else if (!string.IsNullOrEmpty(CustomerInfo.CurrentCustomerKey))
        {
            // 顧客情報パネルに表示中の顧客を使用
            Inquiry.SetCustomerKey(CustomerInfo.CurrentCustomerKey);
        }
        else
        {
            _dialogService.ShowMessage("紐付ける顧客を検索結果から選択してください。");
        }
    }

    /// <summary>
    /// ヘッダータイトル
    /// </summary>
    public string HeaderTitle
    {
        get => _headerTitle;
        set => SetProperty(ref _headerTitle, value);
    }

    /// <summary>
    /// オペレーター名
    /// </summary>
    public string OperatorName
    {
        get => _operatorName;
        set => SetProperty(ref _operatorName, value);
    }

    /// <summary>
    /// 案件名
    /// </summary>
    public string ProjectName
    {
        get => _projectName;
        set => SetProperty(ref _projectName, value);
    }

    /// <summary>
    /// ログイン時刻
    /// </summary>
    public string LoginTime
    {
        get => _loginTime;
        set => SetProperty(ref _loginTime, value);
    }

    /// <summary>
    /// 管理者かどうか
    /// </summary>
    public bool IsAdmin => _sessionManager.IsAdmin;

    /// <summary>
    /// ログアウトコマンド
    /// </summary>
    public ICommand LogoutCommand { get; }

    /// <summary>
    /// CSV出力コマンド
    /// </summary>
    public ICommand ExportCsvCommand { get; }

    /// <summary>
    /// レポート表示コマンド
    /// </summary>
    public ICommand ShowReportCommand { get; }

    /// <summary>
    /// ログアウト要求時に発火するイベント
    /// </summary>
    public event EventHandler? LogoutRequested;

    /// <summary>
    /// ログアウト処理
    /// </summary>
    private void Logout()
    {
        var result = _dialogService.ShowConfirm("ログアウトしますか？");
        if (result)
        {
            _sessionManager.EndSession();
            LogoutRequested?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// CSV出力処理
    /// </summary>
    private async Task ExportCsvAsync()
    {
        // 問合せ履歴検索結果を取得
        var inquiryHistories = SearchPanel.InquirySearchResults;

        if (inquiryHistories == null || inquiryHistories.Count == 0)
        {
            _dialogService.ShowMessage("出力する問合せ履歴がありません。\n問合せ履歴検索を実行してから、CSV出力を行ってください。");
            return;
        }

        try
        {
            var result = await _exportService.ExportInquiryHistoriesToCsvAsync(inquiryHistories);
            if (result)
            {
                _dialogService.ShowMessage("CSV出力が完了しました。");
            }
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"CSV出力中にエラーが発生しました。\n{ex.Message}");
        }
    }

    /// <summary>
    /// レポート表示処理
    /// </summary>
    private void ShowReport()
    {
        // 問合せ履歴検索結果を取得
        var inquiryHistories = SearchPanel.InquirySearchResults;

        if (inquiryHistories == null || inquiryHistories.Count == 0)
        {
            _dialogService.ShowMessage("表示する問合せ履歴がありません。\n問合せ履歴検索を実行してから、レポートを表示してください。");
            return;
        }

        try
        {
            var reportViewModel = _serviceProvider.GetRequiredService<ReportViewModel>();
            reportViewModel.SetInquiryHistories(inquiryHistories);

            var reportView = new ReportView(reportViewModel);
            reportView.ShowDialog();
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"レポート表示中にエラーが発生しました。\n{ex.Message}");
        }
    }

    /// <summary>
    /// セッション情報を更新
    /// </summary>
    private void UpdateSessionInfo()
    {
        if (_sessionManager.IsLoggedIn)
        {
            HeaderTitle = "コール業務管理システム";
            OperatorName = _sessionManager.CurrentOperator?.OperatorName ?? string.Empty;
            ProjectName = _sessionManager.CurrentProject?.ProjectName ?? string.Empty;
            LoginTime = _sessionManager.LoginTime?.ToString("yyyy/MM/dd HH:mm") ?? string.Empty;
        }
        else
        {
            HeaderTitle = string.Empty;
            OperatorName = string.Empty;
            ProjectName = string.Empty;
            LoginTime = string.Empty;
        }

        OnPropertyChanged(nameof(IsAdmin));
    }

    /// <summary>
    /// セッション変更時の処理
    /// </summary>
    private void OnSessionChanged(object? sender, EventArgs e)
    {
        UpdateSessionInfo();
    }
}
