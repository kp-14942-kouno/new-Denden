using System.Collections.ObjectModel;
using System.Windows.Input;
using DenDen.Common;
using DenDen.Models.Entities;
using DenDen.Services;
using DenDen.Views;

namespace DenDen.ViewModels;

/// <summary>
/// レポート画面のViewModel
/// </summary>
public class ReportViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;
    private readonly IExportService _exportService;

    private ObservableCollection<InquiryHistory> _inquiryHistories = new();
    private string _reportTitle = "問合せ履歴レポート";
    private string _generatedAt = string.Empty;

    public ReportViewModel(
        IDialogService dialogService,
        IExportService exportService)
    {
        _dialogService = dialogService;
        _exportService = exportService;

        PrintCommand = new RelayCommand(Print);
        PreviewCommand = new RelayCommand(Preview);
        ExportCsvCommand = new RelayCommand(async () => await ExportCsvAsync());
        CloseCommand = new RelayCommand(Close);

        GeneratedAt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
    }

    /// <summary>
    /// 問合せ履歴一覧
    /// </summary>
    public ObservableCollection<InquiryHistory> InquiryHistories
    {
        get => _inquiryHistories;
        set => SetProperty(ref _inquiryHistories, value);
    }

    /// <summary>
    /// レポートタイトル
    /// </summary>
    public string ReportTitle
    {
        get => _reportTitle;
        set => SetProperty(ref _reportTitle, value);
    }

    /// <summary>
    /// 生成日時
    /// </summary>
    public string GeneratedAt
    {
        get => _generatedAt;
        set => SetProperty(ref _generatedAt, value);
    }

    /// <summary>
    /// 件数
    /// </summary>
    public int TotalCount => InquiryHistories?.Count ?? 0;

    /// <summary>
    /// 印刷コマンド
    /// </summary>
    public ICommand PrintCommand { get; }

    /// <summary>
    /// プレビューコマンド
    /// </summary>
    public ICommand PreviewCommand { get; }

    /// <summary>
    /// CSV出力コマンド
    /// </summary>
    public ICommand ExportCsvCommand { get; }

    /// <summary>
    /// 閉じるコマンド
    /// </summary>
    public ICommand CloseCommand { get; }

    /// <summary>
    /// 閉じるリクエストイベント
    /// </summary>
    public event EventHandler? CloseRequested;

    /// <summary>
    /// 問合せ履歴をセット
    /// </summary>
    public void SetInquiryHistories(IEnumerable<InquiryHistory> inquiryHistories)
    {
        InquiryHistories = new ObservableCollection<InquiryHistory>(inquiryHistories);
        GeneratedAt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        OnPropertyChanged(nameof(TotalCount));
    }

    /// <summary>
    /// 印刷処理
    /// </summary>
    private void Print()
    {
        try
        {
            var printDialog = new System.Windows.Controls.PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                // FlowDocumentを作成して印刷
                var document = CreatePrintDocument();
                var paginator = ((System.Windows.Documents.IDocumentPaginatorSource)document).DocumentPaginator;
                printDialog.PrintDocument(paginator, "問合せ履歴レポート");
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
    private void Preview()
    {
        try
        {
            var document = CreatePrintDocument();
            var previewWindow = new PrintPreviewWindow
            {
                Title = "問合せ履歴レポート - 印刷プレビュー"
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
    /// 印刷用のFlowDocumentを作成
    /// </summary>
    private System.Windows.Documents.FlowDocument CreatePrintDocument()
    {
        var document = new System.Windows.Documents.FlowDocument
        {
            PageWidth = 793, // A4
            PageHeight = 1122,
            PagePadding = new System.Windows.Thickness(50),
            ColumnWidth = double.PositiveInfinity,
            FontFamily = new System.Windows.Media.FontFamily("Meiryo UI"),
            FontSize = 10
        };

        // タイトル
        var titlePara = new System.Windows.Documents.Paragraph(
            new System.Windows.Documents.Run(ReportTitle))
        {
            FontSize = 18,
            FontWeight = System.Windows.FontWeights.Bold,
            TextAlignment = System.Windows.TextAlignment.Center,
            Margin = new System.Windows.Thickness(0, 0, 0, 10)
        };
        document.Blocks.Add(titlePara);

        // 生成日時と件数
        var infoPara = new System.Windows.Documents.Paragraph(
            new System.Windows.Documents.Run($"出力日時: {GeneratedAt}　　件数: {TotalCount}件"))
        {
            FontSize = 10,
            Foreground = System.Windows.Media.Brushes.Gray,
            Margin = new System.Windows.Thickness(0, 0, 0, 20)
        };
        document.Blocks.Add(infoPara);

        // テーブル
        var table = new System.Windows.Documents.Table
        {
            CellSpacing = 0,
            BorderBrush = System.Windows.Media.Brushes.Black,
            BorderThickness = new System.Windows.Thickness(1)
        };

        // カラム定義
        table.Columns.Add(new System.Windows.Documents.TableColumn { Width = new System.Windows.GridLength(60) });  // ID
        table.Columns.Add(new System.Windows.Documents.TableColumn { Width = new System.Windows.GridLength(100) }); // 受電日時
        table.Columns.Add(new System.Windows.Documents.TableColumn { Width = new System.Windows.GridLength(80) });  // 顧客Key
        table.Columns.Add(new System.Windows.Documents.TableColumn { Width = new System.Windows.GridLength(200) }); // 問合せ内容
        table.Columns.Add(new System.Windows.Documents.TableColumn { Width = new System.Windows.GridLength(200) }); // 対応内容

        var rowGroup = new System.Windows.Documents.TableRowGroup();

        // ヘッダー行
        var headerRow = new System.Windows.Documents.TableRow
        {
            Background = System.Windows.Media.Brushes.LightGray
        };
        headerRow.Cells.Add(CreateCell("ID", true));
        headerRow.Cells.Add(CreateCell("受電日時", true));
        headerRow.Cells.Add(CreateCell("顧客Key", true));
        headerRow.Cells.Add(CreateCell("問合せ内容", true));
        headerRow.Cells.Add(CreateCell("対応内容", true));
        rowGroup.Rows.Add(headerRow);

        // データ行
        foreach (var inquiry in InquiryHistories)
        {
            var dataRow = new System.Windows.Documents.TableRow();
            dataRow.Cells.Add(CreateCell(inquiry.InquiryID.ToString(), false));
            dataRow.Cells.Add(CreateCell(inquiry.FirstReceivedDateTime.ToString("yyyy/MM/dd HH:mm"), false));
            dataRow.Cells.Add(CreateCell(inquiry.CustomerKey ?? "", false));
            dataRow.Cells.Add(CreateCell(TruncateText(inquiry.InquiryContent, 50), false));
            dataRow.Cells.Add(CreateCell(TruncateText(inquiry.ResponseContent ?? "", 50), false));
            rowGroup.Rows.Add(dataRow);
        }

        table.RowGroups.Add(rowGroup);
        document.Blocks.Add(table);

        return document;
    }

    /// <summary>
    /// テーブルセルを作成
    /// </summary>
    private static System.Windows.Documents.TableCell CreateCell(string text, bool isHeader)
    {
        var cell = new System.Windows.Documents.TableCell(
            new System.Windows.Documents.Paragraph(
                new System.Windows.Documents.Run(text)))
        {
            BorderBrush = System.Windows.Media.Brushes.Black,
            BorderThickness = new System.Windows.Thickness(0.5),
            Padding = new System.Windows.Thickness(4)
        };

        if (isHeader)
        {
            cell.Blocks.FirstBlock!.FontWeight = System.Windows.FontWeights.Bold;
        }

        return cell;
    }

    /// <summary>
    /// テキストを指定文字数で切り詰め
    /// </summary>
    private static string TruncateText(string text, int maxLength)
    {
        if (string.IsNullOrEmpty(text)) return "";
        return text.Length <= maxLength ? text : text.Substring(0, maxLength) + "...";
    }

    /// <summary>
    /// CSV出力処理
    /// </summary>
    private async Task ExportCsvAsync()
    {
        if (InquiryHistories == null || InquiryHistories.Count == 0)
        {
            _dialogService.ShowMessage("出力するデータがありません。");
            return;
        }

        try
        {
            var result = await _exportService.ExportInquiryHistoriesToCsvAsync(InquiryHistories);
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
    /// 閉じる処理
    /// </summary>
    private void Close()
    {
        CloseRequested?.Invoke(this, EventArgs.Empty);
    }
}
