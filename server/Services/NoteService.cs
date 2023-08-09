using SecureNotes.Functions.Entities;
using SecureNotes.Functions.Helpers;
using SecureNotes.Functions.Requests;

namespace SecureNotes.Functions.Services;

public class NoteService
{
    private readonly CryptographyHelper _cryptoHelper;
    private readonly TableStorageHelper<Note> _tableStorageHelper;

    public NoteService(TableStorageHelper<Note> tableStorageHelper,
        CryptographyHelper cryptoHelper)
    {
        _tableStorageHelper = tableStorageHelper;
        _cryptoHelper = cryptoHelper;
    }

    public async Task<Note> CreateAsync(NoteRequest newNote, User user)
    {
        var encryptedContent = await _cryptoHelper.EncryptData(Encoding.UTF8.GetBytes(newNote.Content), user.RowKey);
        var encryptedTitle = await _cryptoHelper.EncryptData(Encoding.UTF8.GetBytes(newNote.Title), user.RowKey);

        var hashedContent = HashingHelper.HashData(newNote.Content);
        var hashedTitle = HashingHelper.HashData(newNote.Title);

        var encryptedNote = new Note
        {
            RowKey = Guid.NewGuid().ToString(),
            Title = encryptedTitle,
            Content = encryptedContent,
            HashedTitle = hashedTitle,
            HashedContent = hashedContent,
            PartitionKey = user.RowKey
        };

        await _tableStorageHelper.AddEntityAsync(encryptedNote);

        return encryptedNote;
    }


    public async Task<IEnumerable<Note>> GetAllAsync(User user, int page, int pageSize)
    {
        var notes = (await _tableStorageHelper.GetAllEntitiesByColumnAsync("PartitionKey", user.RowKey))
            .OrderByDescending(GetNoteComparisonDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var cleanedNotes = await Task.WhenAll(notes
            .Select(note => DecryptNoteAsync(note, user.RowKey)));

        return cleanedNotes.OrderByDescending(GetNoteComparisonDate);
    }

    public async Task<Note?> UpdateAsync(User user, string noteId, NoteRequest updateRequest)
    {
        var noteToUpdate = await _tableStorageHelper.GetEntityByColumnAsync("RowKey", noteId);
        if (noteToUpdate == null || noteToUpdate.PartitionKey != user.RowKey) return null;

        var contentChanged = !HashingHelper.ValidateHash(updateRequest.Content, noteToUpdate.HashedContent, out var newHashedContent);
        var titleChanged = !HashingHelper.ValidateHash(updateRequest.Title, noteToUpdate.HashedTitle, out var newHashedTitle);

        if (!contentChanged && !titleChanged) return noteToUpdate;

        if (contentChanged)
        {
            var encryptedContent =
                await _cryptoHelper.EncryptData(Encoding.UTF8.GetBytes(updateRequest.Content), user.RowKey);

            noteToUpdate.Content = encryptedContent;
            noteToUpdate.HashedContent = newHashedContent;
        }

        if (titleChanged)
        {
            var encryptedTitle =
                await _cryptoHelper.EncryptData(Encoding.UTF8.GetBytes(updateRequest.Title), user.RowKey);

            noteToUpdate.Title = encryptedTitle;
            noteToUpdate.HashedTitle = newHashedTitle;
        }

        noteToUpdate.LastUpdatedTime = DateTimeOffset.UtcNow;

        await _tableStorageHelper.UpdateEntityAsync(noteToUpdate, noteToUpdate.ETag);

        return noteToUpdate;
    }

    public async Task<bool> DeleteAsync(string userId, string noteId)
    {
        try
        {
            var noteToDelete = await _tableStorageHelper.GetEntityByColumnAsync("RowKey", noteId);
            if (noteToDelete == null || noteToDelete.PartitionKey != userId) return false;

            await _tableStorageHelper.DeleteEntityAsync(userId, noteId);
            return true;
        }
        catch (RequestFailedException ex) when (ex.ErrorCode == "ResourceNotFound")
        {
            return false;
        }
    }

    public async Task<bool> DeleteAllNotes(User user)
    {
        var notes = await _tableStorageHelper.GetAllEntitiesByColumnAsync("PartitionKey", user.RowKey);

        if (!notes.Any()) return true;

        await Task.WhenAll(notes.Select(note => _tableStorageHelper.DeleteEntityAsync(note.PartitionKey, note.RowKey)));

        return true;
    }

    private async Task<Note> DecryptNoteAsync(Note note, string userId)
    {
        var decryptedContent = await _cryptoHelper.DecryptData(note.Content, userId);
        var decryptedTitle = await _cryptoHelper.DecryptData(note.Title, userId);
        note.Content = decryptedContent;
        note.Title = decryptedTitle;
        return note;
    }

    private static DateTimeOffset GetNoteComparisonDate(Note note)
    {
        return note.LastUpdatedTime != DateTimeOffset.MinValue ? note.LastUpdatedTime : note.CreatedTime;
    }
}