namespace DenDen.Models;

/// <summary>
/// 顧客マスタ設定
/// </summary>
public class CustomerMasterSettings
{
    /// <summary>
    /// 有効/無効
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// テーブル名
    /// </summary>
    public string TableName { get; set; } = "T_Customer";

    /// <summary>
    /// キーカラム名
    /// </summary>
    public string KeyColumn { get; set; } = "CustomerID";

    /// <summary>
    /// 検索カラム設定
    /// </summary>
    public List<SearchColumnConfig> SearchColumns { get; set; } = new();
}

/// <summary>
/// 検索カラム設定
/// </summary>
public class SearchColumnConfig
{
    /// <summary>
    /// カラム名
    /// </summary>
    public string ColumnName { get; set; } = string.Empty;

    /// <summary>
    /// 表示名
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// 検索タイプ（Exact: 完全一致, Partial: 部分一致）
    /// </summary>
    public string SearchType { get; set; } = "Partial";
}
