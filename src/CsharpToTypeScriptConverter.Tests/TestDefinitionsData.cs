namespace CsharpToTypeScriptConverter.Tests
{
    /// <summary>
    /// The interface is a generic type that defines a contract for request commands.
    /// The generic type parameter represents the return type of the command.
    /// The interface defines method signatures for handling the command, such as `Execute()`
    /// and `CanExecute()`
    /// </summary>
    public interface ICommand<T>;

    /// <summary>
    /// Simple interface for identification of request command classes
    /// It can be used for model binding in asp.net
    /// </summary>
    public interface IRequestCommand;

    /// <summary>
    /// Simple command example.
    /// </summary>
    public class ChangeUserRoleRequestCommand : IRequestCommand, ICommand<bool>
    {
        public string? UserId { get; set; }
        public UserRoles NewRole { get; set; }
    }

    /// <summary>
    /// Simple command to get users. Can be filtered and paged.
    /// </summary>
    public class GetUsersRequestCommand : IRequestCommand, ICommand<IEnumerable<FoundUsers>>
    {
        public int Skip { get; set; }
        public int Take { get; set; }
        public string? SearchFilter { get; set; }
    }

    /// <summary>
    /// Return type of found users.
    /// </summary>
    public class FoundUsers
    {
        public int UsersCount { get; set; }
        public PaginationResponse<User> Users { get; set; } = new();
    }

    public class User
    {
        public string? Name { get; set; }
        public int Age { get; set; }
        public UserRoles UserRole { get; set; }
    }

    /// <summary>
    /// simple enum example
    /// </summary>
    public enum UserRoles
    {
        User,
        Admin
    }

    /// <summary>
    /// Simple class example with generic type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PaginationResponse<T>
    {
        public int Total { get; set; }
        public IEnumerable<T> Items { get; set; } = [];
    }
}

// This enum is defined in a different namespace to test name collision handling.
namespace NameSpaceForNameCollision
{
    public enum UserRoles
    {
        User,
        Admin
    }
}

namespace CollisionFreeNamespaceWithCollisionTypeNames
{
    public enum UserRoles_1
    {
        User,
        Admin
    }

    public enum UserRoles_2
    {
        User,
        Admin
    }

    public enum UserRoles_3
    {
        User,
        Admin
    }
}

namespace ClassWithoutReferences
{
    public class ClassWithoutReferences
    {
        public string Name { get; set; }
        public string Age { get; set; }
    }
}
