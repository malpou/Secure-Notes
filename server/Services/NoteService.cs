using SecureNotes.Functions.Entities;
using SecureNotes.Functions.Helpers;
using SecureNotes.Functions.Requests;

namespace SecureNotes.Functions.Services;

public class NoteService
{
    private readonly CryptographyHelper _cryptoHelper;
    private readonly TableStorageHelper<Note> _tableStorageHelper;
    private readonly UserService _userService;

    public NoteService(TableStorageHelper<Note> tableStorageHelper, UserService userService,
        CryptographyHelper cryptoHelper)
    {
        _tableStorageHelper = tableStorageHelper;
        _userService = userService;
        _cryptoHelper = cryptoHelper;
    }

    public async Task<Note> CreateAsync(NoteRequest newNote, string userId)
    {
        var encryptedContent = await _cryptoHelper.EncryptData(Encoding.UTF8.GetBytes(newNote.Content), userId);
        var encryptedTitle = await _cryptoHelper.EncryptData(Encoding.UTF8.GetBytes(newNote.Title), userId);

        var encryptedNote = new Note
        {
            RowKey = Guid.NewGuid().ToString(),
            Title = encryptedTitle,
            Content = encryptedContent,
            PartitionKey = userId
        };

        await _tableStorageHelper.AddEntityAsync(encryptedNote);

        var user = await _userService.GetUserByRowKey(userId);
        if (user != null)
        {
            encryptedNote.Author = user.PartitionKey;
        }

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

        foreach (var note in cleanedNotes)
        {
            note.Author = user.PartitionKey;
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

        var user = await _userService.GetUserByRowKey(userId);
        if (user != null)
        {
            noteToUpdate.Author = user.PartitionKey;
        }

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