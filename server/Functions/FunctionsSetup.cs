using SecureNotes.Functions.Services;

namespace SecureNotes.Functions;

public partial class Functions
{
    private readonly NoteService _noteService;
    private readonly UserService _userService;

    public Functions(UserService userService, NoteService noteService)
    {
        _userService = userService;
        _noteService = noteService;
    }
}