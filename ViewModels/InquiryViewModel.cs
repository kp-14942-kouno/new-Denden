using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using DenDen.Common;
using DenDen.Data.Repositories;
using DenDen.Models.Entities;
using DenDen.Services;
using DenDen.Views;

namespace DenDen.ViewModels;

/// <summary>
/// カスタム項目の入力用モデル
/// </summary>
public class CustomFieldItem : ViewModelBase
{
    private string? _value;
    private string? _selectedOption;

    public int ColumnNumber { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string FieldType { get; set; } = "Text";
    public bool IsRequired { get; set; }
    public ObservableCollection<string> SelectOptions { get; set; } = new();

    public string? Value
    {
        get => _value;
        set => SetProperty(ref _value, value);
    }

    public string? SelectedOption
    {
        get => _selectedOption;
        set
        {
            if (SetProperty(ref _selectedOption, value))
            {
                Value = value;
            }
        }
    }
}

/// <summary>
/// 問合せ登録・更新のViewModel
/// </summary>
public class InquiryViewModel : ViewModelBase
{
    private readonly IInquiryHistoryRepository _inquiryHistoryRepository;
    private readonly IInquiryHistoryLogRepository _inquiryHistoryLogRepository;
    private readonly ICustomFieldDefinitionRepository _customFieldDefinitionRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IStatusRepository _statusRepository;
    private readonly SessionManager _sessionManager;
    private readonly IDialogService _dialogService;
    private readonly IInquiryPrintService _inquiryPrintService;

    private InquiryHistory? _currentInquiry;
    private InquiryHistory? _originalInquiry; // 更新前の状態を保持
    private bool _isEditMode;
    private bool _isSaving;
    private string _modeText = "新規登録モード";

    // 入力フィールド
    private DateTime _receivedDateTime;
    private string _operatorName = string.Empty;
    private CategoryMaster? _selectedCategory;
    private StatusMaster? _selectedStatus;
    private string _inquiryContent = string.Empty;
    private string _responseContent = string.Empty;
    private string? _customerKey;

    // マスタデータ
    private ObservableCollection<CategoryMaster> _categories = new();
    private ObservableCollection<StatusMaster> _statuses = new();
    private ObservableCollection<CustomFieldItem> _customFields = new();

    public InquiryViewModel(
        IInquiryHistoryRepository inquiryHistoryRepository,
        IInquiryHistoryLogRepository inquiryHistoryLogRepository,
        ICustomFieldDefinitionRepository customFieldDefinitionRepository,
        ICategoryRepository categoryRepository,
        IStatusRepository statusRepository,
        SessionManager sessionManager,
        IDialogService dialogService,
        IInquiryPrintService inquiryPrintService)
    {
        _inquiryHistoryRepository = inquiryHistoryRepository;
        _inquiryHistoryLogRepository = inquiryHistoryLogRepository;
        _customFieldDefinitionRepository = customFieldDefinitionRepository;
        _categoryRepository = categoryRepository;
        _statusRepository = statusRepository;
        _sessionManager = sessionManager;
        _dialogService = dialogService;
        _inquiryPrintService = inquiryPrintService;

        SaveCommand = new RelayCommand(async () => await SaveAsync(), () => !IsSaving);
        ClearCommand = new RelayCommand(Clear);
        NewCommand = new RelayCommand(PrepareNew);
        LinkCustomerCommand = new RelayCommand(RequestLinkCustomer, () => !HasCustomerKey);
        UnlinkCustomerCommand = new RelayCommand(UnlinkCustomer, () => HasCustomerKey);
        PrintCommand = new RelayCommand(async () => await PrintAsync());
        PreviewCommand = new RelayCommand(async () => await PreviewAsync());

        // 初期状態
        PrepareNew();
    }

    #region プロパティ

    public DateTime ReceivedDateTime
    {
        get => _receivedDateTime;
        set => SetProperty(ref _receivedDateTime, value);
    }

    public string OperatorName
    {
        get => _operatorName;
        set => SetProperty(ref _operatorName, value);
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

    public string InquiryContent
    {
        get => _inquiryContent;
        set => SetProperty(ref _inquiryContent, value);
    }

    public string ResponseContent
    {
        get => _responseContent;
        set => SetProperty(ref _responseContent, value);
    }

    public string? CustomerKey
    {
        get => _customerKey;
        set
        {
            if (SetProperty(ref _customerKey, value))
            {
                OnPropertyChanged(nameof(HasCustomerKey));
            }
        }
    }

    public bool HasCustomerKey => !string.IsNullOrEmpty(CustomerKey);

    public bool IsEditMode
    {
        get => _isEditMode;
        set
        {
            if (SetProperty(ref _isEditMode, value))
            {
                ModeText = value ? "編集モード" : "新規登録モード";
                OnPropertyChanged(nameof(IsNewMode));
                OnPropertyChanged(nameof(SaveButtonText));
            }
        }
    }

    public bool IsNewMode => !IsEditMode;

    public string ModeText
    {
        get => _modeText;
        set => SetProperty(ref _modeText, value);
    }

    public string SaveButtonText => IsEditMode ? "更新" : "登録";

    public bool IsSaving
    {
        get => _isSaving;
        set => SetProperty(ref _isSaving, value);
    }

    public ObservableCollection<CustomFieldItem> CustomFields
    {
        get => _customFields;
        set => SetProperty(ref _customFields, value);
    }

    public bool HasCustomFields => CustomFields.Count > 0;

    #endregion

    #region コマンド

    public ICommand SaveCommand { get; }
    public ICommand ClearCommand { get; }
    public ICommand NewCommand { get; }
    public ICommand LinkCustomerCommand { get; }
    public ICommand UnlinkCustomerCommand { get; }
    public ICommand PrintCommand { get; }
    public ICommand PreviewCommand { get; }

    #endregion

    #region イベント

    public event EventHandler? InquirySaved;
    public event EventHandler? LinkCustomerRequested;

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

        // カスタム項目定義の読み込み
        await LoadCustomFieldsAsync(projectId);

        // オペレーター名の設定
        OperatorName = _sessionManager.CurrentOperator?.OperatorName ?? string.Empty;
    }

    /// <summary>
    /// カスタム項目定義を読み込み
    /// </summary>
    private async Task LoadCustomFieldsAsync(int projectId)
    {
        var definitions = await _customFieldDefinitionRepository.GetEnabledByProjectIdAsync(projectId);
        var items = new ObservableCollection<CustomFieldItem>();

        foreach (var def in definitions)
        {
            var item = new CustomFieldItem
            {
                ColumnNumber = def.ColumnNumber,
                FieldName = def.FieldName,
                DisplayName = def.DisplayName,
                FieldType = def.FieldType,
                IsRequired = def.IsRequired
            };

            // Select型の場合、選択肢をパース
            if (def.FieldType == "Select" && !string.IsNullOrEmpty(def.SelectOptions))
            {
                try
                {
                    var options = JsonSerializer.Deserialize<List<string>>(def.SelectOptions);
                    if (options != null)
                    {
                        item.SelectOptions.Add(""); // 空の選択肢
                        foreach (var opt in options)
                        {
                            item.SelectOptions.Add(opt);
                        }
                    }
                }
                catch
                {
                    // JSON解析失敗時は無視
                }
            }

            items.Add(item);
        }

        CustomFields = items;
        OnPropertyChanged(nameof(HasCustomFields));
    }

    /// <summary>
    /// 新規登録の準備
    /// </summary>
    public void PrepareNew()
    {
        _currentInquiry = null;
        IsEditMode = false;
        ReceivedDateTime = DateTime.Now;
        OperatorName = _sessionManager.CurrentOperator?.OperatorName ?? string.Empty;
        SelectedCategory = null;
        SelectedStatus = null;
        InquiryContent = string.Empty;
        ResponseContent = string.Empty;
        CustomerKey = null;

        // カスタム項目をクリア
        foreach (var field in CustomFields)
        {
            field.Value = null;
            field.SelectedOption = null;
        }
    }

    /// <summary>
    /// 既存の問合せを編集モードで読み込み
    /// </summary>
    public async Task LoadInquiryAsync(int inquiryId)
    {
        var inquiry = await _inquiryHistoryRepository.GetByIdAsync(inquiryId);
        if (inquiry == null)
        {
            _dialogService.ShowError("問合せデータが見つかりませんでした。");
            return;
        }

        LoadInquiry(inquiry);
    }

    /// <summary>
    /// 問合せを編集モードで読み込み
    /// </summary>
    public void LoadInquiry(InquiryHistory inquiry)
    {
        _currentInquiry = inquiry;
        // 更新前の状態を保存（履歴ログ用）
        _originalInquiry = CloneInquiry(inquiry);
        IsEditMode = true;
        ReceivedDateTime = inquiry.FirstReceivedDateTime;
        CustomerKey = inquiry.CustomerKey;
        InquiryContent = inquiry.InquiryContent;
        ResponseContent = inquiry.ResponseContent ?? string.Empty;

        // カテゴリの選択
        if (inquiry.CategoryID.HasValue)
        {
            SelectedCategory = Categories.FirstOrDefault(c => c.CategoryID == inquiry.CategoryID.Value);
        }
        else
        {
            SelectedCategory = null;
        }

        // ステータスの選択
        if (inquiry.StatusID.HasValue)
        {
            SelectedStatus = Statuses.FirstOrDefault(s => s.StatusID == inquiry.StatusID.Value);
        }
        else
        {
            SelectedStatus = null;
        }

        // カスタム項目の読み込み
        LoadCustomFieldValues(inquiry);
    }

    /// <summary>
    /// カスタム項目の値を読み込み
    /// </summary>
    private void LoadCustomFieldValues(InquiryHistory inquiry)
    {
        foreach (var field in CustomFields)
        {
            var value = field.ColumnNumber switch
            {
                1 => inquiry.CustomCol01,
                2 => inquiry.CustomCol02,
                3 => inquiry.CustomCol03,
                4 => inquiry.CustomCol04,
                5 => inquiry.CustomCol05,
                6 => inquiry.CustomCol06,
                7 => inquiry.CustomCol07,
                8 => inquiry.CustomCol08,
                9 => inquiry.CustomCol09,
                10 => inquiry.CustomCol10,
                _ => null
            };

            field.Value = value;
            if (field.FieldType == "Select")
            {
                field.SelectedOption = value;
            }
        }
    }

    /// <summary>
    /// 顧客Keyを設定（顧客選択時）
    /// </summary>
    public void SetCustomerKey(string? customerKey)
    {
        CustomerKey = customerKey;
    }

    /// <summary>
    /// 顧客紐付けをリクエスト
    /// </summary>
    private void RequestLinkCustomer()
    {
        LinkCustomerRequested?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// 顧客紐付けを解除
    /// </summary>
    private void UnlinkCustomer()
    {
        CustomerKey = null;
    }

    /// <summary>
    /// 保存処理
    /// </summary>
    private async Task SaveAsync()
    {
        if (_sessionManager.CurrentProject == null || _sessionManager.CurrentOperator == null)
        {
            _dialogService.ShowError("セッション情報が不正です。再ログインしてください。");
            return;
        }

        // バリデーション
        if (!Validate())
        {
            return;
        }

        try
        {
            IsSaving = true;

            if (IsEditMode && _currentInquiry != null)
            {
                await UpdateInquiryAsync();
            }
            else
            {
                await InsertInquiryAsync();
            }

            InquirySaved?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"保存中にエラーが発生しました。\n{ex.Message}");
        }
        finally
        {
            IsSaving = false;
        }
    }

    /// <summary>
    /// バリデーション
    /// </summary>
    private bool Validate()
    {
        if (string.IsNullOrWhiteSpace(InquiryContent))
        {
            _dialogService.ShowError("問合せ内容を入力してください。");
            return false;
        }

        // 必須カスタム項目のチェック
        foreach (var field in CustomFields.Where(f => f.IsRequired))
        {
            if (string.IsNullOrWhiteSpace(field.Value))
            {
                _dialogService.ShowError($"「{field.DisplayName}」を入力してください。");
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 新規登録
    /// </summary>
    private async Task InsertInquiryAsync()
    {
        var inquiry = new InquiryHistory
        {
            ProjectID = _sessionManager.CurrentProject!.ProjectID,
            CustomerKey = CustomerKey,
            OperatorID = _sessionManager.CurrentOperator!.OperatorID,
            CategoryID = SelectedCategory?.CategoryID > 0 ? SelectedCategory.CategoryID : null,
            StatusID = SelectedStatus?.StatusID > 0 ? SelectedStatus.StatusID : null,
            InquiryContent = InquiryContent,
            ResponseContent = string.IsNullOrWhiteSpace(ResponseContent) ? null : ResponseContent,
            FirstReceivedDateTime = ReceivedDateTime,
            CreatedAt = DateTime.Now,
            CreatedBy = _sessionManager.CurrentOperator.OperatorID
        };

        // カスタム項目の設定
        SetCustomFieldValues(inquiry);

        await _inquiryHistoryRepository.InsertAsync(inquiry);
        _dialogService.ShowMessage("問合せを登録しました。");

        // 新規登録モードに戻す
        PrepareNew();
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    private async Task UpdateInquiryAsync()
    {
        if (_currentInquiry == null) return;

        // 更新前の状態を履歴ログに保存
        if (_originalInquiry != null)
        {
            await SaveHistoryLogAsync(_originalInquiry);
        }

        _currentInquiry.CustomerKey = CustomerKey;
        _currentInquiry.CategoryID = SelectedCategory?.CategoryID > 0 ? SelectedCategory.CategoryID : null;
        _currentInquiry.StatusID = SelectedStatus?.StatusID > 0 ? SelectedStatus.StatusID : null;
        _currentInquiry.InquiryContent = InquiryContent;
        _currentInquiry.ResponseContent = string.IsNullOrWhiteSpace(ResponseContent) ? null : ResponseContent;
        _currentInquiry.UpdatedDateTime = DateTime.Now;
        _currentInquiry.UpdatedBy = _sessionManager.CurrentOperator!.OperatorID;

        // カスタム項目の設定
        SetCustomFieldValues(_currentInquiry);

        await _inquiryHistoryRepository.UpdateAsync(_currentInquiry);
        _dialogService.ShowMessage("問合せを更新しました。");
    }

    /// <summary>
    /// 履歴ログを保存
    /// </summary>
    private async Task SaveHistoryLogAsync(InquiryHistory original)
    {
        var log = new InquiryHistoryLog
        {
            InquiryID = original.InquiryID,
            ProjectID = original.ProjectID,
            CustomerKey = original.CustomerKey,
            OperatorID = original.OperatorID,
            CategoryID = original.CategoryID,
            StatusID = original.StatusID,
            InquiryContent = original.InquiryContent,
            ResponseContent = original.ResponseContent,
            FirstReceivedDateTime = original.FirstReceivedDateTime,
            UpdatedDateTime = original.UpdatedDateTime,
            CustomCol01 = original.CustomCol01,
            CustomCol02 = original.CustomCol02,
            CustomCol03 = original.CustomCol03,
            CustomCol04 = original.CustomCol04,
            CustomCol05 = original.CustomCol05,
            CustomCol06 = original.CustomCol06,
            CustomCol07 = original.CustomCol07,
            CustomCol08 = original.CustomCol08,
            CustomCol09 = original.CustomCol09,
            CustomCol10 = original.CustomCol10,
            UpdatedBy = _sessionManager.CurrentOperator!.OperatorID,
            LoggedAt = DateTime.Now
        };

        await _inquiryHistoryLogRepository.InsertAsync(log);
    }

    /// <summary>
    /// InquiryHistoryのクローンを作成
    /// </summary>
    private static InquiryHistory CloneInquiry(InquiryHistory source)
    {
        return new InquiryHistory
        {
            InquiryID = source.InquiryID,
            ProjectID = source.ProjectID,
            CustomerKey = source.CustomerKey,
            OperatorID = source.OperatorID,
            CategoryID = source.CategoryID,
            StatusID = source.StatusID,
            InquiryContent = source.InquiryContent,
            ResponseContent = source.ResponseContent,
            FirstReceivedDateTime = source.FirstReceivedDateTime,
            UpdatedDateTime = source.UpdatedDateTime,
            CreatedAt = source.CreatedAt,
            CreatedBy = source.CreatedBy,
            UpdatedBy = source.UpdatedBy,
            CustomCol01 = source.CustomCol01,
            CustomCol02 = source.CustomCol02,
            CustomCol03 = source.CustomCol03,
            CustomCol04 = source.CustomCol04,
            CustomCol05 = source.CustomCol05,
            CustomCol06 = source.CustomCol06,
            CustomCol07 = source.CustomCol07,
            CustomCol08 = source.CustomCol08,
            CustomCol09 = source.CustomCol09,
            CustomCol10 = source.CustomCol10
        };
    }

    /// <summary>
    /// カスタム項目の値をInquiryHistoryに設定
    /// </summary>
    private void SetCustomFieldValues(InquiryHistory inquiry)
    {
        foreach (var field in CustomFields)
        {
            switch (field.ColumnNumber)
            {
                case 1: inquiry.CustomCol01 = field.Value; break;
                case 2: inquiry.CustomCol02 = field.Value; break;
                case 3: inquiry.CustomCol03 = field.Value; break;
                case 4: inquiry.CustomCol04 = field.Value; break;
                case 5: inquiry.CustomCol05 = field.Value; break;
                case 6: inquiry.CustomCol06 = field.Value; break;
                case 7: inquiry.CustomCol07 = field.Value; break;
                case 8: inquiry.CustomCol08 = field.Value; break;
                case 9: inquiry.CustomCol09 = field.Value; break;
                case 10: inquiry.CustomCol10 = field.Value; break;
            }
        }
    }

    /// <summary>
    /// クリア処理
    /// </summary>
    private void Clear()
    {
        if (IsEditMode)
        {
            var result = _dialogService.ShowConfirm("編集内容を破棄して新規登録モードに戻りますか？");
            if (!result) return;
        }

        PrepareNew();
    }

    /// <summary>
    /// 印刷処理
    /// </summary>
    private async Task PrintAsync()
    {
        try
        {
            var document = await CreatePrintDocumentAsync();
            if (document == null) return;

            var printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                var paginator = ((IDocumentPaginatorSource)document).DocumentPaginator;
                printDialog.PrintDocument(paginator, "問合せ票");
                _dialogService.ShowMessage("印刷が完了しました。");
            }
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"印刷中にエラーが発生しました。\n{ex.Message}");
        }
    }

    /// <summary>
    /// プレビュー処理
    /// </summary>
    private async Task PreviewAsync()
    {
        try
        {
            var document = await CreatePrintDocumentAsync();
            if (document == null) return;

            var previewWindow = new PrintPreviewWindow
            {
                Title = "問合せ票 - 印刷プレビュー"
            };
            previewWindow.SetDocument(document);
            previewWindow.ShowDialog();
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"プレビュー表示中にエラーが発生しました。\n{ex.Message}");
        }
    }

    /// <summary>
    /// 印刷用ドキュメントを作成
    /// </summary>
    private async Task<FlowDocument?> CreatePrintDocumentAsync()
    {
        if (string.IsNullOrWhiteSpace(InquiryContent))
        {
            _dialogService.ShowMessage("問合せ内容を入力してから印刷してください。");
            return null;
        }

        // 印刷データを作成
        var printData = new InquiryPrintData
        {
            ReceivedDateTime = ReceivedDateTime,
            OperatorName = OperatorName,
            CustomerKey = CustomerKey,
            CategoryName = SelectedCategory?.CategoryName,
            StatusName = SelectedStatus?.StatusName,
            InquiryContent = InquiryContent,
            ResponseContent = ResponseContent
        };

        // カスタム項目を追加
        foreach (var field in CustomFields.Where(f => !string.IsNullOrEmpty(f.Value)))
        {
            printData.CustomFields.Add(new CustomFieldPrintData
            {
                DisplayName = field.DisplayName,
                Value = field.Value
            });
        }

        // 顧客情報を取得
        IEnumerable<CustomerPrintData>? customerData = null;
        if (!string.IsNullOrEmpty(CustomerKey) && _sessionManager.CurrentProject != null)
        {
            customerData = await _inquiryPrintService.GetCustomerPrintDataAsync(
                CustomerKey, _sessionManager.CurrentProject.ProjectID);
        }

        return _inquiryPrintService.CreateInquiryDocument(printData, customerData);
    }
}
