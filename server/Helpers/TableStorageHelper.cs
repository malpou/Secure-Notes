using SecureNotes.Functions.Entities;

namespace SecureNotes.Functions.Helpers;

public class TableStorageHelper<T> where T : BaseEntity, new()
{
    private readonly TableClient _tableClient;

    public TableStorageHelper(string connectionString, string tableName)
    {
        _tableClient = new TableClient(connectionString, tableName);
    }

    public async Task AddEntityAsync(T entity)
    {
        await _tableClient.AddEntityAsync(entity);
    }

    public async Task<T?> GetEntityByColumnAsync(string columnName, string value)
    {
        var filter = $"{columnName} eq '{value}'";

        await foreach (var entity in _tableClient.QueryAsync<T>(filter)) return entity;

        return null;
    }

    public async Task<List<T>> GetAllEntitiesByColumnAsync(string columnName, string value)
    {
        var entities = new List<T>();
        var filter = $"{columnName} eq '{value}'";

        await foreach (var entity in _tableClient.QueryAsync<T>(filter)) entities.Add(entity);

        return entities;
    }

    public async Task UpdateEntityAsync(T entity, ETag eTag)
    {
        await _tableClient.UpdateEntityAsync(entity, eTag);
    }

    public async Task DeleteEntityAsync(string partitionKey, string rowKey)
    {
        await _tableClient.DeleteEntityAsync(partitionKey, rowKey);
    }

    public async Task<bool> IsValueInColumnUniqueAsync(string columnName, string value)
    {
        var entities = await GetAllEntitiesByColumnAsync(columnName, value);
        return !entities.Any();
    }
}