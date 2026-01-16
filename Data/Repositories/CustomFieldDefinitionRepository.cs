using Dapper;
using DenDen.Data.Database;
using DenDen.Models.Entities;

namespace DenDen.Data.Repositories;

/// <summary>
/// カスタム項目定義リポジトリの実装
/// </summary>
public class CustomFieldDefinitionRepository : ICustomFieldDefinitionRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public CustomFieldDefinitionRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<CustomFieldDefinition>> GetEnabledByProjectIdAsync(int projectId)
    {
        using var connection = _connectionFactory.CreateMasterConnection();
        connection.Open();

        var sql = @"
            SELECT * FROM CustomFieldDefinition
            WHERE ProjectID = @ProjectId AND IsEnabled = 1
            ORDER BY DisplayOrder";

        return await connection.QueryAsync<CustomFieldDefinition>(sql, new { ProjectId = projectId });
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<CustomFieldDefinition>> GetSearchableByProjectIdAsync(int projectId)
    {
        using var connection = _connectionFactory.CreateMasterConnection();
        connection.Open();

        var sql = @"
            SELECT * FROM CustomFieldDefinition
            WHERE ProjectID = @ProjectId AND IsEnabled = 1 AND IsSearchable = 1
            ORDER BY DisplayOrder";

        return await connection.QueryAsync<CustomFieldDefinition>(sql, new { ProjectId = projectId });
    }

    /// <inheritdoc/>
    public async Task<CustomFieldDefinition?> GetByIdAsync(int fieldId)
    {
        using var connection = _connectionFactory.CreateMasterConnection();
        connection.Open();

        var sql = "SELECT * FROM CustomFieldDefinition WHERE FieldID = @FieldId";
        return await connection.QueryFirstOrDefaultAsync<CustomFieldDefinition>(sql, new { FieldId = fieldId });
    }

    /// <inheritdoc/>
    public async Task<int> InsertAsync(CustomFieldDefinition definition)
    {
        using var connection = _connectionFactory.CreateMasterConnection();
        connection.Open();

        var sql = @"
            INSERT INTO CustomFieldDefinition (
                ProjectID, ColumnNumber, FieldName, DisplayName, FieldType,
                IsRequired, IsEnabled, DisplayOrder, SelectOptions, IsSearchable, CreatedAt
            ) VALUES (
                @ProjectID, @ColumnNumber, @FieldName, @DisplayName, @FieldType,
                @IsRequired, @IsEnabled, @DisplayOrder, @SelectOptions, @IsSearchable, @CreatedAt
            );
            SELECT last_insert_rowid();";

        return await connection.ExecuteScalarAsync<int>(sql, definition);
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(CustomFieldDefinition definition)
    {
        using var connection = _connectionFactory.CreateMasterConnection();
        connection.Open();

        var sql = @"
            UPDATE CustomFieldDefinition SET
                FieldName = @FieldName,
                DisplayName = @DisplayName,
                FieldType = @FieldType,
                IsRequired = @IsRequired,
                IsEnabled = @IsEnabled,
                DisplayOrder = @DisplayOrder,
                SelectOptions = @SelectOptions,
                IsSearchable = @IsSearchable
            WHERE FieldID = @FieldID";

        await connection.ExecuteAsync(sql, definition);
    }
}
