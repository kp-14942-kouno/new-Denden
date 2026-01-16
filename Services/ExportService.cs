using System.IO;
using System.Text;
using Microsoft.Win32;
using DenDen.Models.Entities;

namespace DenDen.Services;

/// <summary>
/// エクスポートサービス
/// CSV出力機能を提供
/// </summary>
public class ExportService : IExportService
{
    /// <summary>
    /// 問合せ履歴をCSVファイルに出力（SaveFileDialog使用）
    /// </summary>
    /// <param name="inquiryHistories">出力する問合せ履歴のリスト</param>
    /// <returns>出力成功時はtrue、キャンセル時はfalse</returns>
    public async Task<bool> ExportInquiryHistoriesToCsvAsync(IEnumerable<InquiryHistory> inquiryHistories)
    {
        var fileName = $"問合せ履歴_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

        var saveFileDialog = new SaveFileDialog
        {
            Filter = "CSVファイル (*.csv)|*.csv",
            FileName = fileName,
            DefaultExt = ".csv",
            Title = "問合せ履歴をCSV出力"
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            await ExportInquiryHistoriesToCsvAsync(inquiryHistories, saveFileDialog.FileName);
            return true;
        }

        return false;
    }

    /// <summary>
    /// 問合せ履歴をCSVファイルに出力（ファイルパス指定）
    /// </summary>
    /// <param name="inquiryHistories">出力する問合せ履歴のリスト</param>
    /// <param name="filePath">出力先ファイルパス</param>
    public async Task ExportInquiryHistoriesToCsvAsync(IEnumerable<InquiryHistory> inquiryHistories, string filePath)
    {
        var sb = new StringBuilder();

        // ヘッダー行
        sb.AppendLine(string.Join(",", new[]
        {
            "問合せID",
            "案件ID",
            "顧客Key",
            "オペレーターID",
            "カテゴリーID",
            "ステータスID",
            "問合せ内容",
            "対応内容",
            "初回受電日時",
            "更新日時",
            "最終更新者",
            "作成日時",
            "カスタム項目1",
            "カスタム項目2",
            "カスタム項目3",
            "カスタム項目4",
            "カスタム項目5",
            "カスタム項目6",
            "カスタム項目7",
            "カスタム項目8",
            "カスタム項目9",
            "カスタム項目10"
        }));

        // データ行
        foreach (var inquiry in inquiryHistories)
        {
            sb.AppendLine(string.Join(",", new[]
            {
                inquiry.InquiryID.ToString(),
                inquiry.ProjectID.ToString(),
                EscapeCsvField(inquiry.CustomerKey),
                inquiry.OperatorID.ToString(),
                inquiry.CategoryID?.ToString() ?? "",
                inquiry.StatusID?.ToString() ?? "",
                EscapeCsvField(inquiry.InquiryContent),
                EscapeCsvField(inquiry.ResponseContent),
                inquiry.FirstReceivedDateTime.ToString("yyyy/MM/dd HH:mm:ss"),
                inquiry.UpdatedDateTime?.ToString("yyyy/MM/dd HH:mm:ss") ?? "",
                EscapeCsvField(inquiry.UpdatedByName),
                inquiry.CreatedAt.ToString("yyyy/MM/dd HH:mm:ss"),
                EscapeCsvField(inquiry.CustomCol01),
                EscapeCsvField(inquiry.CustomCol02),
                EscapeCsvField(inquiry.CustomCol03),
                EscapeCsvField(inquiry.CustomCol04),
                EscapeCsvField(inquiry.CustomCol05),
                EscapeCsvField(inquiry.CustomCol06),
                EscapeCsvField(inquiry.CustomCol07),
                EscapeCsvField(inquiry.CustomCol08),
                EscapeCsvField(inquiry.CustomCol09),
                EscapeCsvField(inquiry.CustomCol10)
            }));
        }

        // UTF-8 with BOMで出力
        var utf8WithBom = new UTF8Encoding(true);
        await File.WriteAllTextAsync(filePath, sb.ToString(), utf8WithBom);
    }

    /// <summary>
    /// CSVフィールドをエスケープ
    /// </summary>
    /// <param name="field">エスケープするフィールド</param>
    /// <returns>エスケープされたフィールド</returns>
    private static string EscapeCsvField(string? field)
    {
        if (string.IsNullOrEmpty(field))
            return "";

        // カンマ、ダブルクォート、改行を含む場合はダブルクォートで囲む
        if (field.Contains(',') || field.Contains('"') || field.Contains('\n') || field.Contains('\r'))
        {
            // ダブルクォートをエスケープ（"を""に）
            field = field.Replace("\"", "\"\"");
            return $"\"{field}\"";
        }

        return field;
    }
}
