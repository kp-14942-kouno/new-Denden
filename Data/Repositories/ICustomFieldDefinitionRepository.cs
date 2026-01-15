using DeDen.Models.Entities;

namespace DeDen.Data.Repositories;

/// <summary>
/// カスタム項目定義リポジトリのインターフェース
/// </summary>
public interface ICustomFieldDefinitionRepository
{
    /// <summary>
    /// 案件IDに紐づく有効なカスタム項目定義を取得
    /// </summary>
    Task<IEnumerable<CustomFieldDefinition>> GetEnabledByProjectIdAsync(int projectId);

    /// <summary>
    /// 案件IDに紐づく検索可能なカスタム項目定義を取得
    /// </summary>
    Task<IEnumerable<CustomFieldDefinition>> GetSearchableByProjectIdAsync(int projectId);

    /// <summary>
    /// カスタム項目定義を取得
    /// </summary>
    Task<CustomFieldDefinition?> GetByIdAsync(int fieldId);

    /// <summary>
    /// カスタム項目定義を追加
    /// </summary>
    Task<int> InsertAsync(CustomFieldDefinition definition);

    /// <summary>
    /// カスタム項目定義を更新
    /// </summary>
    Task UpdateAsync(CustomFieldDefinition definition);
}
