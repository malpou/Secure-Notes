using SecureNotes.Functions.Helpers;
using SecureNotes.Functions.Services;

namespace SecureNotes.Functions;

public partial class Functions
{
    private readonly JwtHelper _jwtHelper;
    private readonly NoteService _noteService;
    private readonly UserService _userService;

    public Functions(UserService userService, NoteService noteService, JwtHelper jwtHelper)
    {
        _userService = userService;
        _noteService = noteService;
        _jwtHelper = jwtHelper;
    }
}