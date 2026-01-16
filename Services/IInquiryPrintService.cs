using System.Windows.Documents;
using DeDen.Models.Entities;

namespace DeDen.Services;

/// <summary>
/// 問合せ印刷データ
/// </summary>
public class InquiryPrintData
{
    public DateTime ReceivedDateTime { get; set; }
    public string OperatorName { get; set; } = string.Empty;
    public string? CustomerKey { get; set; }
    public string? CategoryName { get; set; }
    public string? StatusName { get; set; }
    public string InquiryContent { get; set; } = string.Empty;
    public string? ResponseContent { get; set; }
    public List<CustomFieldPrintData> CustomFields { get; set; } = new();
}

/// <summary>
/// カスタム項目印刷データ
/// </summary>
public class CustomFieldPrintData
{
    public string DisplayName { get; set; } = string.Empty;
    public string? Value { get; set; }
}

/// <summary>
/// 顧客情報印刷データ
/// </summary>
public class CustomerPrintData
{
    public string DisplayName { get; set; } = string.Empty;
    public string? Value { get; set; }
}

/// <summary>
/// 問合せ印刷サービスのインターフェース
/// </summary>
public interface IInquiryPrintService
{
    /// <summary>
    /// 問合せ印刷用のFlowDocumentを生成
    /// </summary>
    /// <param name="printData">問合せ印刷データ</param>
    /// <param name="customerData">顧客情報（設定された項目のみ）</param>
    /// <returns>FlowDocument</returns>
    FlowDocument CreateInquiryDocument(InquiryPrintData printData, IEnumerable<CustomerPrintData>? customerData);

    /// <summary>
    /// 顧客情報を取得（レポート設定に基づく）
    /// </summary>
    /// <param name="customerKey">顧客キー</param>
    /// <param name="projectId">案件ID</param>
    /// <returns>顧客情報リスト</returns>
    Task<IEnumerable<CustomerPrintData>> GetCustomerPrintDataAsync(string customerKey, int projectId);
}
