using DeDen.Models;

namespace DeDen.Services;

/// <summary>
/// 設定管理サービスのインターフェース
/// </summary>
public interface IConfigurationService
{
    /// <summary>
    /// データベース設定を取得
    /// </summary>
    DatabaseSettings DatabaseSettings { get; }

    /// <summary>
    /// 顧客マスタ設定を取得
    /// </summary>
    CustomerMasterSettings CustomerMasterSettings { get; }

    /// <summary>
    /// カスタム項目検索設定を取得
    /// </summary>
    CustomFieldSearchSettings CustomFieldSearchSettings { get; }

    /// <summary>
    /// 画像設定を取得
    /// </summary>
    ImageSettings ImageSettings { get; }

    /// <summary>
    /// 現在の環境名を取得
    /// </summary>
    string Environment { get; }

    /// <summary>
    /// 指定したセクションの設定値を取得
    /// </summary>
    /// <typeparam name="T">設定値の型</typeparam>
    /// <param name="sectionName">セクション名</param>
    /// <returns>設定値</returns>
    T? GetSection<T>(string sectionName) where T : class, new();

    /// <summary>
    /// 指定したキーの設定値を取得
    /// </summary>
    /// <param name="key">キー</param>
    /// <returns>設定値</returns>
    string? GetValue(string key);
}
