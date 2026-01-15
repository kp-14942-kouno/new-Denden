namespace DeDen.Models.DTOs;

/// <summary>
/// 顧客検索リクエスト
/// </summary>
public class CustomerSearchRequest
{
    /// <summary>
    /// 検索条件（カラム名と値のペア）
    /// </summary>
    public Dictionary<string, string> SearchConditions { get; set; } = new();

    /// <summary>
    /// 検索方式（Exact / Partial）
    /// </summary>
    public string SearchType { get; set; } = "Partial";
}
