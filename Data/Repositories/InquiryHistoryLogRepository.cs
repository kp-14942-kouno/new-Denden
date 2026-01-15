using Dapper;
using DeDen.Data.Database;
using DeDen.Models.Entities;

namespace DeDen.Data.Repositories;

/// <summary>
/// 問合せ履歴更新ログリポジトリの実装
/// </summary>
public class InquiryHistoryLogRepository : IInquiryHistoryLogRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public InquiryHistoryLogRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    /// <inheritdoc/>
    public async Task<int> InsertAsync(InquiryHistoryLog log)
    {
        using var connection = _connectionFactory.CreateHistoryConnection();
        connection.Open();

        var sql = @"
            INSERT INTO InquiryHistoryLog (
                InquiryID, ProjectID, CustomerKey, OperatorID, CategoryID, StatusID,
                InquiryContent, ResponseContent, FirstReceivedDateTime, UpdatedDateTime,
                CustomCol01, CustomCol02, CustomCol03, CustomCol04, CustomCol05,
                CustomCol06, CustomCol07, CustomCol08, CustomCol09, CustomCol10,
                UpdatedBy, LoggedAt
            ) VALUES (
                @InquiryID, @ProjectID, @CustomerKey, @OperatorID, @CategoryID, @StatusID,
                @InquiryContent, @ResponseContent, @FirstReceivedDateTime, @UpdatedDateTime,
                @CustomCol01, @CustomCol02, @CustomCol03, @CustomCol04, @CustomCol05,
                @CustomCol06, @CustomCol07, @CustomCol08, @CustomCol09, @CustomCol10,
                @UpdatedBy, @LoggedAt
            );
            SELECT last_insert_rowid();";

        return await connection.ExecuteScalarAsync<int>(sql, log);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<InquiryHistoryLog>> GetByInquiryIdAsync(int inquiryId)
    {
        using var connection = _connectionFactory.CreateHistoryConnection();
        connection.Open();

        var sql = @"
            SELECT * FROM InquiryHistoryLog
            WHERE InquiryID = @InquiryId
            ORDER BY LoggedAt DESC";

        return await connection.QueryAsync<InquiryHistoryLog>(sql, new { InquiryId = inquiryId });
    }
}
