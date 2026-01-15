namespace DeDen.Models;

/// <summary>
/// 画像設定
/// </summary>
public class ImageSettings
{
    /// <summary>
    /// 有効/無効
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// テーブル名
    /// </summary>
    public string TableName { get; set; } = "T_CustomerImages";

    /// <summary>
    /// 顧客キーカラム名
    /// </summary>
    public string CustomerKeyColumn { get; set; } = "CustomerID";

    /// <summary>
    /// 画像パスカラム名
    /// </summary>
    public string ImagePathColumn { get; set; } = "ImagePath";
}
