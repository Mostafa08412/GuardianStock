using Domain.UnitTests.Builders;
using GuardianStock.Domain.Users;
using GuardianStock.Domain.Core.Errors;

namespace Domain.UnitTests.Users;

/// <summary>
/// Unit tests for User aggregate.
/// Tests validation of Create factory method.
/// </summary>
public class UserTests
{
    #region Create Factory Method Tests

    [Fact]
    public void Create_WithValidInputs_ShouldReturnSuccessResult()
    {
        // Arrange
        var id = Guid.NewGuid();
        var firstName = "Jane";
        var lastName = "Smith";
        var username = "janesmith";
        var email = "jane.smith@example.com";

        // Act
        var result = User.Create(id, firstName, lastName, username, email);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(id);
        result.Value.FirstName.Should().Be(firstName);
        result.Value.LastName.Should().Be(lastName);
        result.Value.Username.Should().Be(username);
        result.Value.Email.Should().Be(email);
    }

    [Fact]
    public void Create_WithValidInputsUsingBuilder_ShouldReturnSuccessResult()
    {
        // Arrange & Act
        var result = UserBuilder.Create()
            .WithFirstName("Alice")
            .WithLastName("Johnson")
            .WithEmail("alice@example.com")
            .Build();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.FirstName.Should().Be("Alice");
        result.Value.LastName.Should().Be("Johnson");
        result.Value.Email.Should().Be("alice@example.com");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    public void Create_WithInvalidId_ShouldReturnFailureResult(Guid invalidId)
    {
        // Arrange & Act
        var result = UserBuilder.Create()
            .WithId(invalidId)
            .Build();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Errors.UserErrors.IdIsRequired);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidFirstName_ShouldReturnFailureResult(string invalidFirstName)
    {
        // Arrange & Act
        var result = UserBuilder.Create()
            .WithFirstName(invalidFirstName)
            .Build();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Errors.UserErrors.FirstNameIsRequired);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidLastName_ShouldReturnFailureResult(string invalidLastName)
    {
        // Arrange & Act
        var result = UserBuilder.Create()
            .WithLastName(invalidLastName)
            .Build();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Errors.UserErrors.LastNameIsRequired);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidUsername_ShouldReturnFailureResult(string invalidUsername)
    {
        // Arrange & Act
        var result = UserBuilder.Create()
            .WithUsername(invalidUsername)
            .Build();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Errors.UserErrors.UsernameIsRequired);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidEmail_ShouldReturnFailureResult(string invalidEmail)
    {
        // Arrange & Act
        var result = UserBuilder.Create()
            .WithEmail(invalidEmail)
            .Build();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Errors.UserErrors.EmailIsRequired);
    }

    #endregion

    #region User Properties Tests

    [Fact]
    public void User_AfterCreation_ShouldHaveAllPropertiesSet()
    {

        var userId = Guid.NewGuid();
        // Arrange & Act
        var user = UserBuilder.Create()
            .WithId(userId)
            .WithFirstName("Test")
            .WithLastName("User")
            .WithUsername("testuser")
            .WithEmail("test@example.com")
            .BuildSuccessfully();

        // Assert
        user.Id.Should().Be(userId);
        user.FirstName.Should().Be("Test");
        user.LastName.Should().Be("User");
        user.Username.Should().Be("testuser");
        user.Email.Should().Be("test@example.com");
    }

    #endregion
}
