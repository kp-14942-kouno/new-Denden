using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace DenDen.Views;

/// <summary>
/// PrintPreviewWindow.xaml の相互作用ロジック
/// </summary>
public partial class PrintPreviewWindow : Window
{
    private FlowDocument? _document;

    public PrintPreviewWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// FlowDocumentを設定
    /// </summary>
    public void SetDocument(FlowDocument document)
    {
        _document = document;
        documentReader.Document = document;
    }

    /// <summary>
    /// 印刷ボタンクリック
    /// </summary>
    private void PrintButton_Click(object sender, RoutedEventArgs e)
    {
        if (_document == null) return;

        var printDialog = new PrintDialog();
        if (printDialog.ShowDialog() == true)
        {
            var paginator = ((IDocumentPaginatorSource)_document).DocumentPaginator;
            printDialog.PrintDocument(paginator, Title);
        }
    }

    /// <summary>
    /// 閉じるボタンクリック
    /// </summary>
    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
