using System.Collections.ObjectModel;
using DeDen.Data.Repositories;
using DeDen.Models.DTOs;
using DeDen.Models.Entities;
using DeDen.Services;

namespace DeDen.ViewModels;

/// <summary>
/// 顧客情報表示のViewModel
/// </summary>
public class CustomerInfoViewModel : ViewModelBase
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IInquiryHistoryRepository _inquiryHistoryRepository;
    private readonly SessionManager _sessionManager;

    private CustomerSearchResult? _currentCustomer;
    private Dictionary<string, object?> _customerData = new();
    private ObservableCollection<CustomerInfoItem> _customerInfoItems = new();
    private ObservableCollection<InquiryHistory> _inquiryHistories = new();
    private InquiryHistory? _selectedInquiry;
    private bool _isLoading;
    private bool _hasCustomer;

    public CustomerInfoViewModel(
        ICustomerRepository customerRepository,
        IInquiryHistoryRepository inquiryHistoryRepository,
        SessionManager sessionManager)
    {
        _customerRepository = customerRepository;
        _inquiryHistoryRepository = inquiryHistoryRepository;
        _sessionManager = sessionManager;
    }

    /// <summary>
    /// 現在の顧客
    /// </summary>
    public CustomerSearchResult? CurrentCustomer
    {
        get => _currentCustomer;
        private set
        {
            if (SetProperty(ref _currentCustomer, value))
            {
                HasCustomer = value != null;
            }
        }
    }

    /// <summary>
    /// 顧客データ（全カラム）
    /// </summary>
    public Dictionary<string, object?> CustomerData
    {
        get => _customerData;
        set => SetProperty(ref _customerData, value);
    }

    /// <summary>
    /// 顧客情報項目リスト
    /// </summary>
    public ObservableCollection<CustomerInfoItem> CustomerInfoItems
    {
        get => _customerInfoItems;
        set => SetProperty(ref _customerInfoItems, value);
    }

    /// <summary>
    /// 問合せ履歴リスト
    /// </summary>
    public ObservableCollection<InquiryHistory> InquiryHistories
    {
        get => _inquiryHistories;
        set => SetProperty(ref _inquiryHistories, value);
    }

    /// <summary>
    /// 選択された問合せ履歴
    /// </summary>
    public InquiryHistory? SelectedInquiry
    {
        get => _selectedInquiry;
        set
        {
            if (SetProperty(ref _selectedInquiry, value) && value != null)
            {
                InquirySelected?.Invoke(this, value);
            }
        }
    }

    /// <summary>
    /// 現在の顧客Key
    /// </summary>
    public string? CurrentCustomerKey => CurrentCustomer?.CustomerKey;

    /// <summary>
    /// 読み込み中かどうか
    /// </summary>
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    /// <summary>
    /// 顧客が選択されているかどうか
    /// </summary>
    public bool HasCustomer
    {
        get => _hasCustomer;
        set => SetProperty(ref _hasCustomer, value);
    }

    /// <summary>
    /// 問合せ履歴選択時のイベント
    /// </summary>
    public event EventHandler<InquiryHistory?>? InquirySelected;

    /// <summary>
    /// 顧客情報を読み込む
    /// </summary>
    public async Task LoadCustomerAsync(string customerKey)
    {
        try
        {
            IsLoading = true;

            // 顧客情報を取得
            CurrentCustomer = await _customerRepository.GetByIdAsync(customerKey);
            if (CurrentCustomer == null)
            {
                Clear();
                return;
            }

            // 全カラムデータを取得
            CustomerData = await _customerRepository.GetCustomerDataAsync(customerKey);

            // 表示用リストを作成
            var items = new ObservableCollection<CustomerInfoItem>();
            var displayNames = GetDisplayNames();

            foreach (var kvp in CustomerData)
            {
                if (kvp.Key == "CreatedAt") continue; // システム項目は除外

                var displayName = displayNames.TryGetValue(kvp.Key, out var name) ? name : kvp.Key;
                items.Add(new CustomerInfoItem
                {
                    ColumnName = kvp.Key,
                    DisplayName = displayName,
                    Value = kvp.Value?.ToString() ?? string.Empty
                });
            }
            CustomerInfoItems = items;

            // 問合せ履歴を取得
            if (_sessionManager.CurrentProject != null)
            {
                var histories = await _inquiryHistoryRepository.GetByCustomerKeyAsync(
                    _sessionManager.CurrentProject.ProjectID,
                    customerKey);
                InquiryHistories = new ObservableCollection<InquiryHistory>(histories);
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// 顧客情報をクリア
    /// </summary>
    public void Clear()
    {
        CurrentCustomer = null;
        CustomerData = new Dictionary<string, object?>();
        CustomerInfoItems.Clear();
        InquiryHistories.Clear();
        SelectedInquiry = null;
    }

    /// <summary>
    /// カラム名と表示名のマッピング
    /// </summary>
    private static Dictionary<string, string> GetDisplayNames()
    {
        return new Dictionary<string, string>
        {
            { "CustomerID", "顧客ID" },
            { "customer_name", "顧客名" },
            { "kana", "フリガナ" },
            { "tel_no", "電話番号" },
            { "email", "メールアドレス" },
            { "postal_code", "郵便番号" },
            { "addr1", "住所1" },
            { "addr2", "住所2" },
            { "contract_date", "契約日" },
            { "contract_no", "契約番号" },
            { "plan_name", "契約プラン" },
            { "member_rank", "会員ランク" },
            { "remarks", "備考" }
        };
    }
}

/// <summary>
/// 顧客情報表示用アイテム
/// </summary>
public class CustomerInfoItem
{
    public string ColumnName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
