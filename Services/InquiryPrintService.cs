using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using DeDen.Data.Repositories;

namespace DeDen.Services;

/// <summary>
/// 問合せ印刷サービス
/// </summary>
public class InquiryPrintService : IInquiryPrintService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IReportCustomerDisplayConfigRepository _reportConfigRepository;

    public InquiryPrintService(
        ICustomerRepository customerRepository,
        IReportCustomerDisplayConfigRepository reportConfigRepository)
    {
        _customerRepository = customerRepository;
        _reportConfigRepository = reportConfigRepository;
    }

    /// <summary>
    /// 問合せ印刷用のFlowDocumentを生成
    /// </summary>
    public FlowDocument CreateInquiryDocument(InquiryPrintData printData, IEnumerable<CustomerPrintData>? customerData)
    {
        var document = new FlowDocument
        {
            PageWidth = 793,  // A4
            PageHeight = 1122,
            PagePadding = new Thickness(50),
            ColumnWidth = double.PositiveInfinity,
            FontFamily = new FontFamily("Meiryo UI"),
            FontSize = 11
        };

        // タイトル
        var titlePara = new Paragraph(new Run("問合せ票"))
        {
            FontSize = 22,
            FontWeight = FontWeights.Bold,
            TextAlignment = TextAlignment.Center,
            Margin = new Thickness(0, 0, 0, 20)
        };
        document.Blocks.Add(titlePara);

        // 基本情報テーブル
        var basicTable = CreateTable();
        AddTableRow(basicTable, "受電日時", printData.ReceivedDateTime.ToString("yyyy/MM/dd HH:mm:ss"));
        AddTableRow(basicTable, "オペレーター", printData.OperatorName);
        AddTableRow(basicTable, "カテゴリー", printData.CategoryName ?? "-");
        AddTableRow(basicTable, "ステータス", printData.StatusName ?? "-");
        document.Blocks.Add(basicTable);

        // 顧客情報セクション（顧客キーがある場合）
        if (!string.IsNullOrEmpty(printData.CustomerKey))
        {
            var customerSection = new Paragraph(new Run("顧客情報"))
            {
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 20, 0, 8),
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(0, 0, 0, 1),
                Padding = new Thickness(0, 0, 0, 4)
            };
            document.Blocks.Add(customerSection);

            var customerTable = CreateTable();
            AddTableRow(customerTable, "顧客Key", printData.CustomerKey);

            if (customerData != null)
            {
                foreach (var item in customerData)
                {
                    AddTableRow(customerTable, item.DisplayName, item.Value ?? "-");
                }
            }
            document.Blocks.Add(customerTable);
        }

        // 問合せ内容セクション
        var inquirySection = new Paragraph(new Run("問合せ内容"))
        {
            FontSize = 14,
            FontWeight = FontWeights.Bold,
            Margin = new Thickness(0, 20, 0, 8),
            BorderBrush = Brushes.Gray,
            BorderThickness = new Thickness(0, 0, 0, 1),
            Padding = new Thickness(0, 0, 0, 4)
        };
        document.Blocks.Add(inquirySection);

        var inquiryContent = new Paragraph(new Run(printData.InquiryContent ?? "-"))
        {
            Margin = new Thickness(0, 0, 0, 10),
            Background = new SolidColorBrush(Color.FromRgb(248, 248, 248)),
            Padding = new Thickness(8)
        };
        document.Blocks.Add(inquiryContent);

        // 対応内容セクション
        var responseSection = new Paragraph(new Run("対応内容"))
        {
            FontSize = 14,
            FontWeight = FontWeights.Bold,
            Margin = new Thickness(0, 10, 0, 8),
            BorderBrush = Brushes.Gray,
            BorderThickness = new Thickness(0, 0, 0, 1),
            Padding = new Thickness(0, 0, 0, 4)
        };
        document.Blocks.Add(responseSection);

        var responseContent = new Paragraph(new Run(printData.ResponseContent ?? "-"))
        {
            Margin = new Thickness(0, 0, 0, 10),
            Background = new SolidColorBrush(Color.FromRgb(248, 248, 248)),
            Padding = new Thickness(8)
        };
        document.Blocks.Add(responseContent);

        // カスタム項目セクション（項目がある場合）
        if (printData.CustomFields.Count > 0)
        {
            var customSection = new Paragraph(new Run("カスタム項目"))
            {
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 10, 0, 8),
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(0, 0, 0, 1),
                Padding = new Thickness(0, 0, 0, 4)
            };
            document.Blocks.Add(customSection);

            var customTable = CreateTable();
            foreach (var field in printData.CustomFields)
            {
                AddTableRow(customTable, field.DisplayName, field.Value ?? "-");
            }
            document.Blocks.Add(customTable);
        }

        // フッター（出力日時）
        var footerPara = new Paragraph(new Run($"出力日時: {DateTime.Now:yyyy/MM/dd HH:mm:ss}"))
        {
            FontSize = 9,
            Foreground = Brushes.Gray,
            TextAlignment = TextAlignment.Right,
            Margin = new Thickness(0, 30, 0, 0)
        };
        document.Blocks.Add(footerPara);

        return document;
    }

    /// <summary>
    /// 顧客情報を取得（レポート設定に基づく）
    /// </summary>
    public async Task<IEnumerable<CustomerPrintData>> GetCustomerPrintDataAsync(string customerKey, int projectId)
    {
        var result = new List<CustomerPrintData>();

        // レポート用表示設定を取得
        var configs = await _reportConfigRepository.GetByProjectIdAsync(projectId);
        if (!configs.Any())
        {
            return result;
        }

        // 顧客データを取得
        var customerData = await _customerRepository.GetCustomerDataAsync(customerKey);
        if (customerData == null)
        {
            return result;
        }

        // 設定された項目の値を取得
        foreach (var config in configs.OrderBy(c => c.DisplayOrder))
        {
            string? value = null;
            if (customerData.TryGetValue(config.ColumnName, out var obj) && obj != null)
            {
                value = obj.ToString();
            }

            result.Add(new CustomerPrintData
            {
                DisplayName = config.DisplayName,
                Value = value
            });
        }

        return result;
    }

    /// <summary>
    /// テーブルを作成
    /// </summary>
    private static Table CreateTable()
    {
        var table = new Table
        {
            CellSpacing = 0,
            Margin = new Thickness(0, 0, 0, 10)
        };

        table.Columns.Add(new TableColumn { Width = new GridLength(120) });
        table.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });

        table.RowGroups.Add(new TableRowGroup());

        return table;
    }

    /// <summary>
    /// テーブルに行を追加
    /// </summary>
    private static void AddTableRow(Table table, string label, string? value)
    {
        var row = new TableRow();

        var labelCell = new TableCell(new Paragraph(new Run(label))
        {
            FontWeight = FontWeights.SemiBold,
            Margin = new Thickness(0)
        })
        {
            Background = new SolidColorBrush(Color.FromRgb(240, 240, 240)),
            BorderBrush = Brushes.LightGray,
            BorderThickness = new Thickness(1),
            Padding = new Thickness(8, 4, 8, 4)
        };

        var valueCell = new TableCell(new Paragraph(new Run(value ?? "-"))
        {
            Margin = new Thickness(0)
        })
        {
            BorderBrush = Brushes.LightGray,
            BorderThickness = new Thickness(0, 1, 1, 1),
            Padding = new Thickness(8, 4, 8, 4)
        };

        row.Cells.Add(labelCell);
        row.Cells.Add(valueCell);

        table.RowGroups[0].Rows.Add(row);
    }
}
