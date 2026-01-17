using System.Collections.ObjectModel;
using System.Windows.Input;
using DenDen.Common;
using DenDen.Data.Repositories;
using DenDen.Models.DTOs;
using DenDen.Models.Entities;
using DenDen.Services;

namespace DenDen.ViewModels;

/// <summary>
/// 検索タイプの列挙型（将来のカスタム項目検索にも対応）
/// </summary>
public enum SearchType
{
    None,
    CustomerMaster,    // 顧客マスタ
    InquiryHistory     // 問合せ履歴
    // CustomField     // 将来追加予定
}

/// <summary>
/// 検索パネルのViewModel
/// 顧客検索と問合せ履歴検索を管理
/// </summary>
public class SearchPanelViewModel : ViewModelBase
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IInquiryHistoryRepository _inquiryHistoryRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IStatusRepository _statusRepository;
    private readonly IOperatorRepository _operatorRepository;
    private readonly SessionManager _sessionManager;
    private readonly IDialogService _dialogService;

    // 顧客検索
    private string _searchCustomerId = string.Empty;
    private string _searchCustomerName = string.Empty;
    private string _searchTelNo = string.Empty;

    // 履歴検索
    private DateTime? _searchStartDate;
    private DateTime? _searchEndDate;
    private CategoryMaster? _selectedCategory;
    private StatusMaster? _selectedStatus;
    private OperatorMaster? _selectedOperator;

    // 検索結果
    private ObservableCollection<CustomerSearchResult> _customerSearchResults = new();
    private ObservableCollection<InquiryHistory> _inquirySearchResults = new();
    private object? _selectedSearchResult;
    private bool _isSearching;
    private int _selectedTabIndex;
    private SearchType _currentSearchType = SearchType.None;

    // マスタデータ
    private ObservableCollection<CategoryMaster> _categories = new();
    private ObservableCollection<StatusMaster> _statuses = new();
    private ObservableCollection<OperatorMaster> _operators = new();

    public SearchPanelViewModel(
        ICustomerRepository customerRepository,
        IInquiryHistoryRepository inquiryHistoryRepository,
        ICategoryRepository categoryRepository,
        IStatusRepository statusRepository,
        IOperatorRepository operatorRepository,
        SessionManager sessionManager,
        IDialogService dialogService)
    {
        _customerRepository = customerRepository;
        _inquiryHistoryRepository = inquiryHistoryRepository;
        _categoryRepository = categoryRepository;
        _statusRepository = statusRepository;
        _operatorRepository = operatorRepository;
        _sessionManager = sessionManager;
        _dialogService = dialogService;

        SearchCustomerCommand = new RelayCommand(async () => await SearchCustomerAsync());
        SearchInquiryCommand = new RelayCommand(async () => await SearchInquiryAsync());
        ClearSearchCommand = new RelayCommand(ClearSearch);
    }

    #region 顧客検索プロパティ

    public string SearchCustomerId
    {
        get => _searchCustomerId;
        set => SetProperty(ref _searchCustomerId, value);
    }

    public string SearchCustomerName
    {
        get => _searchCustomerName;
        set => SetProperty(ref _searchCustomerName, value);
    }

    public string SearchTelNo
    {
        get => _searchTelNo;
        set => SetProperty(ref _searchTelNo, value);
    }

    #endregion

    #region 履歴検索プロパティ

    public DateTime? SearchStartDate
    {
        get => _searchStartDate;
        set => SetProperty(ref _searchStartDate, value);
    }

    public DateTime? SearchEndDate
    {
        get => _searchEndDate;
        set => SetProperty(ref _searchEndDate, value);
    }

    public ObservableCollection<CategoryMaster> Categories
    {
        get => _categories;
        set => SetProperty(ref _categories, value);
    }

    public CategoryMaster? SelectedCategory
    {
        get => _selectedCategory;
        set => SetProperty(ref _selectedCategory, value);
    }

    public ObservableCollection<StatusMaster> Statuses
    {
        get => _statuses;
        set => SetProperty(ref _statuses, value);
    }

    public StatusMaster? SelectedStatus
    {
        get => _selectedStatus;
        set => SetProperty(ref _selectedStatus, value);
    }

    public ObservableCollection<OperatorMaster> Operators
    {
        get => _operators;
        set => SetProperty(ref _operators, value);
    }

    public OperatorMaster? SelectedOperator
    {
        get => _selectedOperator;
        set => SetProperty(ref _selectedOperator, value);
    }

    #endregion

    #region 検索結果プロパティ

    public ObservableCollection<CustomerSearchResult> CustomerSearchResults
    {
        get => _customerSearchResults;
        set => SetProperty(ref _customerSearchResults, value);
    }

    public ObservableCollection<InquiryHistory> InquirySearchResults
    {
        get => _inquirySearchResults;
        set => SetProperty(ref _inquirySearchResults, value);
    }

    public object? SelectedSearchResult
    {
        get => _selectedSearchResult;
        set
        {
            if (SetProperty(ref _selectedSearchResult, value))
            {
                OnSearchResultSelected();
            }
        }
    }

    public bool IsSearching
    {
        get => _isSearching;
        set => SetProperty(ref _isSearching, value);
    }

    public int SelectedTabIndex
    {
        get => _selectedTabIndex;
        set => SetProperty(ref _selectedTabIndex, value);
    }

    public SearchType CurrentSearchType
    {
        get => _currentSearchType;
        set
        {
            if (SetProperty(ref _currentSearchType, value))
            {
                OnPropertyChanged(nameof(SearchResultLabel));
            }
        }
    }

    public string SearchResultLabel => CurrentSearchType switch
    {
        SearchType.CustomerMaster => "[顧客マスタ]検索結果",
        SearchType.InquiryHistory => "[問合せ履歴]検索結果",
        // SearchType.CustomField => "[カスタム項目]検索結果",
        _ => "検索結果"
    };

    #endregion

    #region コマンド

    public ICommand SearchCustomerCommand { get; }
    public ICommand SearchInquiryCommand { get; }
    public ICommand ClearSearchCommand { get; }

    #endregion

    #region イベント

    public event EventHandler<CustomerSearchResult>? CustomerSelected;
    public event EventHandler<InquiryHistory>? InquirySelected;
    public event EventHandler? SearchCleared;

    #endregion

    /// <summary>
    /// マスタデータを読み込み
    /// </summary>
    public async Task LoadMasterDataAsync()
    {
        if (_sessionManager.CurrentProject == null) return;

        var projectId = _sessionManager.CurrentProject.ProjectID;

        // カテゴリ（先頭に空の選択肢を追加）
        var categories = await _categoryRepository.GetAllByProjectIdAsync(projectId);
        var categoryList = new List<CategoryMaster> { new CategoryMaster { CategoryID = 0, CategoryName = "" } };
        categoryList.AddRange(categories);
        Categories = new ObservableCollection<CategoryMaster>(categoryList);

        // ステータス（先頭に空の選択肢を追加）
        var statuses = await _statusRepository.GetAllByProjectIdAsync(projectId);
        var statusList = new List<StatusMaster> { new StatusMaster { StatusID = 0, StatusName = "" } };
        statusList.AddRange(statuses);
        Statuses = new ObservableCollection<StatusMaster>(statusList);

        // オペレータ（先頭に空の選択肢を追加）
        var operators = await _operatorRepository.GetAllByProjectIdAsync(projectId);
        var operatorList = new List<OperatorMaster> { new OperatorMaster { OperatorID = 0, OperatorName = "" } };
        operatorList.AddRange(operators);
        Operators = new ObservableCollection<OperatorMaster>(operatorList);
    }

    /// <summary>
    /// 顧客検索
    /// </summary>
    private async Task SearchCustomerAsync()
    {
        try
        {
            IsSearching = true;

            var results = await _customerRepository.SearchAsync(
                SearchCustomerId,
                SearchCustomerName,
                SearchTelNo);

            CustomerSearchResults = new ObservableCollection<CustomerSearchResult>(results);
            InquirySearchResults.Clear();
            CurrentSearchType = SearchType.CustomerMaster;
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"検索中にエラーが発生しました。\n{ex.Message}");
        }
        finally
        {
            IsSearching = false;
        }
    }

    /// <summary>
    /// 問合せ履歴検索
    /// </summary>
    private async Task SearchInquiryAsync()
    {
        if (_sessionManager.CurrentProject == null) return;

        try
        {
            IsSearching = true;

            var request = new InquirySearchRequest
            {
                StartDate = SearchStartDate,
                EndDate = SearchEndDate,
                CategoryID = SelectedCategory?.CategoryID > 0 ? SelectedCategory.CategoryID : null,
                StatusID = SelectedStatus?.StatusID > 0 ? SelectedStatus.StatusID : null,
                OperatorID = SelectedOperator?.OperatorID > 0 ? SelectedOperator.OperatorID : null
            };

            var results = await _inquiryHistoryRepository.SearchAsync(
                _sessionManager.CurrentProject.ProjectID,
                request);

            InquirySearchResults = new ObservableCollection<InquiryHistory>(results);
            CustomerSearchResults.Clear();
            CurrentSearchType = SearchType.InquiryHistory;
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"検索中にエラーが発生しました。\n{ex.Message}");
        }
        finally
        {
            IsSearching = false;
        }
    }

    /// <summary>
    /// 検索条件をクリア
    /// </summary>
    private void ClearSearch()
    {
        SearchCustomerId = string.Empty;
        SearchCustomerName = string.Empty;
        SearchTelNo = string.Empty;
        SearchStartDate = null;
        SearchEndDate = null;
        SelectedCategory = null;
        SelectedStatus = null;
        SelectedOperator = null;
        CustomerSearchResults.Clear();
        InquirySearchResults.Clear();
        SelectedSearchResult = null;

        // クリアイベントを発火
        SearchCleared?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// 検索結果選択時の処理
    /// </summary>
    private void OnSearchResultSelected()
    {
        if (SelectedSearchResult is CustomerSearchResult customer)
        {
            CustomerSelected?.Invoke(this, customer);
        }
        else if (SelectedSearchResult is InquiryHistory inquiry)
        {
            InquirySelected?.Invoke(this, inquiry);
        }
    }
}
