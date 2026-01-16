using DenDen.Models.Entities;

namespace DenDen.Services;

/// <summary>
/// エクスポートサービスのインターフェース
/// </summary>
public interface IExportService
{
    /// <summary>
    /// 問合せ履歴をCSVファイルに出力
    /// </summary>
    /// <param name="inquiryHistories">出力する問合せ履歴のリスト</param>
    /// <returns>出力成功時はtrue、キャンセル時はfalse</returns>
    Task<bool> ExportInquiryHistoriesToCsvAsync(IEnumerable<InquiryHistory> inquiryHistories);

    /// <summary>
    /// 問合せ履歴をCSVファイルに出力（ファイルパス指定）
    /// </summary>
    /// <param name="inquiryHistories">出力する問合せ履歴のリスト</param>
    /// <param name="filePath">出力先ファイルパス</param>
    Task ExportInquiryHistoriesToCsvAsync(IEnumerable<InquiryHistory> inquiryHistories, string filePath);
}
