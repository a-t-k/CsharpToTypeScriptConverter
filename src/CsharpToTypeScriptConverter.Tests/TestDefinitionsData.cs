/// <summary>
/// Base interface for handler search and execution.
/// </summary>
public interface ICommand<T>;

/// <summary>
/// Interface for request command identification model binding.
/// </summary>
public interface IRequestCommand;

/// <summary>
/// Command example
/// </summary>
public class ChangeUserRoleRequestCommand: IRequestCommand, ICommand<bool>{
    public string UserId { get; set; }
    public UserRoles NewRole { get; set; }
}

/// <summary>
/// Used enum in command example
/// </summary>
public enum UserRoles
{
    User,
    Admin
}