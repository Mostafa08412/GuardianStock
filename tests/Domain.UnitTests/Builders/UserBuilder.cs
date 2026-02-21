using GuardianStock.Domain.Core.Primitives.Result;
using GuardianStock.Domain.Users;

namespace Domain.UnitTests.Builders;

/// <summary>
/// Test Data Builder for User aggregate.
/// Creates test instances with sensible defaults.
/// </summary>
public class UserBuilder
{
    private Guid _id = Guid.NewGuid();
    private string _firstName = "John";
    private string _lastName = "Doe";
    private string _username = "johndoe";
    private string _email = "john.doe@example.com";

    private UserBuilder()
    {
    }

    /// <summary>
    /// Creates a new UserBuilder with default valid values.
    /// </summary>
    public static UserBuilder Create() => new();

    /// <summary>
    /// Creates a UserBuilder with invalid values.
    /// </summary>
    public static UserBuilder CreateInvalid() => new UserBuilder
    {
        _id = Guid.Empty,
        _firstName = string.Empty,
        _lastName = string.Empty,
        _username = string.Empty,
        _email = string.Empty
    };

    public UserBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public UserBuilder WithFirstName(string firstName)
    {
        _firstName = firstName;
        return this;
    }

    public UserBuilder WithLastName(string lastName)
    {
        _lastName = lastName;
        return this;
    }

    public UserBuilder WithUsername(string username)
    {
        _username = username;
        return this;
    }

    public UserBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    /// <summary>
    /// Builds the User using the configured values.
    /// </summary>
    public Result<User> Build()
    {
        return User.Create(_id, _firstName, _lastName, _username, _email);
    }

    /// <summary>
    /// Builds and returns the User directly, throwing if creation fails.
    /// </summary>
    public User BuildSuccessfully()
    {
        var result = Build();
        if (result.IsFailure)
        {
            throw new InvalidOperationException(
                $"Failed to build user: {result.Error.Description}");
        }
        return result.Value;
    }
}
