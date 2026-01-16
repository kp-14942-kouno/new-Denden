using Dapper;
using DenDen.Data.Database;
using DenDen.Models.DTOs;
using DenDen.Models.Entities;

namespace DenDen.Data.Repositories;

/// <summary>
/// 問合せ履歴リポジトリの実装
/// </summary>
public class InquiryHistoryRepository : IInquiryHistoryRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IOperatorRepository _operatorRepository;

    public InquiryHistoryRepository(IDbConnectionFactory connectionFactory, IOperatorRepository operatorRepository)
    {
        _connectionFactory = connectionFactory;
        _operatorRepository = operatorRepository;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<InquiryHistory>> SearchAsync(int projectId, InquirySearchRequest request)
    {
        using var connection = _connectionFactory.CreateHistoryConnection();
        connection.Open();

        var conditions = new List<string> { "ProjectID = @ProjectId" };
        var parameters = new DynamicParameters();
        parameters.Add("ProjectId", projectId);

        if (request.StartDate.HasValue)
        {
            conditions.Add("date(FirstReceivedDateTime) >= date(@StartDate)");
            parameters.Add("StartDate", request.StartDate.Value.ToString("yyyy-MM-dd"));
        }

        if (request.EndDate.HasValue)
        {
            conditions.Add("date(FirstReceivedDateTime) <= date(@EndDate)");
            parameters.Add("EndDate", request.EndDate.Value.ToString("yyyy-MM-dd"));
        }

        if (request.CategoryID.HasValue)
        {
            conditions.Add("CategoryID = @CategoryID");
            parameters.Add("CategoryID", request.CategoryID.Value);
        }

        if (request.StatusID.HasValue)
        {
            conditions.Add("StatusID = @StatusID");
            parameters.Add("StatusID", request.StatusID.Value);
        }

        if (request.OperatorID.HasValue)
        {
            conditions.Add("OperatorID = @OperatorID");
            parameters.Add("OperatorID", request.OperatorID.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.CustomerKey))
        {
            conditions.Add("CustomerKey = @CustomerKey");
            parameters.Add("CustomerKey", request.CustomerKey);
        }

        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            conditions.Add("(InquiryContent LIKE @Keyword OR ResponseContent LIKE @Keyword)");
            parameters.Add("Keyword", $"%{request.Keyword}%");
        }

        var whereClause = string.Join(" AND ", conditions);

        var sql = $@"
            SELECT * FROM InquiryHistory
            WHERE {whereClause}
            ORDER BY FirstReceivedDateTime DESC
            LIMIT 100";

        var results = (await connection.QueryAsync<InquiryHistory>(sql, parameters)).ToList();

        // オペレーター名を補完
        await PopulateUpdatedByNamesAsync(projectId, results);

        return results;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<InquiryHistory>> GetByCustomerKeyAsync(int projectId, string customerKey)
    {
        using var connection = _connectionFactory.CreateHistoryConnection();
        connection.Open();

        var sql = @"
            SELECT * FROM InquiryHistory
            WHERE ProjectID = @ProjectId AND CustomerKey = @CustomerKey
            ORDER BY FirstReceivedDateTime DESC";

        var results = (await connection.QueryAsync<InquiryHistory>(sql, new { ProjectId = projectId, CustomerKey = customerKey })).ToList();

        // オペレーター名を補完
        await PopulateUpdatedByNamesAsync(projectId, results);

        return results;
    }

    /// <inheritdoc/>
    public async Task<InquiryHistory?> GetByIdAsync(int inquiryId)
    {
        using var connection = _connectionFactory.CreateHistoryConnection();
        connection.Open();

        var sql = "SELECT * FROM InquiryHistory WHERE InquiryID = @InquiryId";
        var result = await connection.QueryFirstOrDefaultAsync<InquiryHistory>(sql, new { InquiryId = inquiryId });

        // オペレーター名を補完
        if (result != null)
        {
            await PopulateUpdatedByNamesAsync(result.ProjectID, new List<InquiryHistory> { result });
        }

        return result;
    }

    /// <inheritdoc/>
    public async Task<int> InsertAsync(InquiryHistory inquiry)
    {
        using var connection = _connectionFactory.CreateHistoryConnection();
        connection.Open();

        var sql = @"
            INSERT INTO InquiryHistory (
                ProjectID, CustomerKey, OperatorID, CategoryID, StatusID,
                InquiryContent, ResponseContent, FirstReceivedDateTime,
                CreatedBy, CustomCol01, CustomCol02, CustomCol03, CustomCol04, CustomCol05,
                CustomCol06, CustomCol07, CustomCol08, CustomCol09, CustomCol10
            ) VALUES (
                @ProjectID, @CustomerKey, @OperatorID, @CategoryID, @StatusID,
                @InquiryContent, @ResponseContent, @FirstReceivedDateTime,
                @CreatedBy, @CustomCol01, @CustomCol02, @CustomCol03, @CustomCol04, @CustomCol05,
                @CustomCol06, @CustomCol07, @CustomCol08, @CustomCol09, @CustomCol10
            );
            SELECT last_insert_rowid();";

        return await connection.ExecuteScalarAsync<int>(sql, inquiry);
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(InquiryHistory inquiry)
    {
        using var connection = _connectionFactory.CreateHistoryConnection();
        connection.Open();

        var sql = @"
            UPDATE InquiryHistory SET
                CustomerKey = @CustomerKey,
                CategoryID = @CategoryID,
                StatusID = @StatusID,
                InquiryContent = @InquiryContent,
                ResponseContent = @ResponseContent,
                UpdatedDateTime = @UpdatedDateTime,
                UpdatedBy = @UpdatedBy,
                CustomCol01 = @CustomCol01,
                CustomCol02 = @CustomCol02,
                CustomCol03 = @CustomCol03,
                CustomCol04 = @CustomCol04,
                CustomCol05 = @CustomCol05,
                CustomCol06 = @CustomCol06,
                CustomCol07 = @CustomCol07,
                CustomCol08 = @CustomCol08,
                CustomCol09 = @CustomCol09,
                CustomCol10 = @CustomCol10
            WHERE InquiryID = @InquiryID";

        await connection.ExecuteAsync(sql, inquiry);
    }

    /// <summary>
    /// 問合せ履歴リストにオペレーター名を補完
    /// </summary>
    private async Task PopulateUpdatedByNamesAsync(int projectId, List<InquiryHistory> histories)
    {
        // UpdatedByが設定されている履歴のみ対象
        var updatedByIds = histories
            .Where(h => h.UpdatedBy.HasValue)
            .Select(h => h.UpdatedBy!.Value)
            .Distinct()
            .ToList();

        if (!updatedByIds.Any())
            return;

        // オペレーター一覧を取得
        var operators = await _operatorRepository.GetAllByProjectIdAsync(projectId);
        var operatorDict = operators.ToDictionary(o => o.OperatorID, o => o.OperatorName);

        // オペレーター名を補完
        foreach (var history in histories)
        {
            if (history.UpdatedBy.HasValue && operatorDict.TryGetValue(history.UpdatedBy.Value, out var name))
            {
                history.UpdatedByName = name;
            }
        }
    }
}
