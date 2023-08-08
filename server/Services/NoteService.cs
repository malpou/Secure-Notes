using SecureNotes.Functions.Entities;
using SecureNotes.Functions.Helpers;
using SecureNotes.Functions.Requests;

namespace SecureNotes.Functions.Services;

public class NoteService
{
    private readonly CryptographyHelper _cryptoHelper;
    private readonly TableStorageHelper<Note> _tableStorageHelper;

    public NoteService(TableStorageHelper<Note> tableStorageHelper, CryptographyHelper cryptoHelper)
    {
        _tableStorageHelper = tableStorageHelper;
        _cryptoHelper = cryptoHelper;
    }

    public async Task<Note> CreateAsync(NoteRequest newNote, string userId)
    {
        var encryptedContent = await _cryptoHelper.EncryptData(Encoding.UTF8.GetBytes(newNote.Content), userId);

        var encryptedNote = new Note
        {
            RowKey = Guid.NewGuid().ToString(),
            Title = newNote.Title,
            Content = encryptedContent,
            PartitionKey = userId
        };

        await _tableStorageHelper.AddEntityAsync(encryptedNote);

        return encryptedNote;
    }

    public async Task<IEnumerable<Note>> GetAllAsync(string userId, int page, int pageSize)
    {
        var notes = (await _tableStorageHelper.GetAllEntitiesAsync())
            .OrderByDescending(GetNoteComparisonDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var cleanedNotes = new List<Note>();

        if (!string.IsNullOrEmpty(userId))
        {
            var decryptedNotes = await Task.WhenAll(notes
                .Where(note => note.PartitionKey == userId)
                .Select(note => DecryptNoteAsync(note, userId)));

            cleanedNotes.AddRange(decryptedNotes);
        }

        foreach (var note in notes.Where(note => note.PartitionKey != userId))
        {
            var contentWithoutKey = note.Content.Split(":")[1];
            note.Content = contentWithoutKey;

            cleanedNotes.Add(note);
        }

        return cleanedNotes.OrderByDescending(GetNoteComparisonDate);
    }

    public async Task<Note?> UpdateAsync(string userId, string noteId, NoteRequest updateRequest)
    {
        var noteToUpdate = await _tableStorageHelper.GetEntityAsync(userId, noteId);
        if (noteToUpdate == null) return null;

        noteToUpdate.Title = updateRequest.Title;
        noteToUpdate.Content = await _cryptoHelper.EncryptData(Encoding.UTF8.GetBytes(updateRequest.Content), userId);
        noteToUpdate.LastUpdatedTime = DateTimeOffset.UtcNow;

        await _tableStorageHelper.UpdateEntityAsync(noteToUpdate, noteToUpdate.ETag);

        return noteToUpdate;
    }

    public async Task<bool> DeleteAsync(string userId, string noteId)
    {
        try
        {
            var noteToDelete = await _tableStorageHelper.GetEntityAsync(userId, noteId);
            if (noteToDelete == null) return false;

            await _tableStorageHelper.DeleteEntityAsync(userId, noteId);
            return true;
        }
        catch (RequestFailedException ex) when (ex.ErrorCode == "ResourceNotFound")
        {
            return false;
        }
    }

    private async Task<Note> DecryptNoteAsync(Note note, string userId)
    {
        var decryptedContent = await _cryptoHelper.DecryptData(note.Content, userId);
        note.Content = decryptedContent;
        return note;
    }

    private static DateTimeOffset GetNoteComparisonDate(Note note)
    {
        return note.LastUpdatedTime != DateTimeOffset.MinValue ? note.LastUpdatedTime : note.CreatedTime;
    }
}