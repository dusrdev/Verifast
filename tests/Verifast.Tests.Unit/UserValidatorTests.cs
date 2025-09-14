namespace Verifast.Tests.Unit;

public class UserValidatorTests {
    [Fact]
    public void Valid_User_Has_No_Errors() {
        var user = new UserDto {
            Name = "Alice",
            Age = 30,
            Email = "alice@example.com",
            Password = "Secret123",
            Phone = "+14155552671"
        };

        var result = user.Validate(user);
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Missing_Name_Is_Error() {
        var user = new UserDto {
            Name = "",
            Age = 5,
            Email = "bob@example.com",
            Password = "Abcdef12"
        };

        var result = user.Validate(user);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("Name"));
    }

    [Fact]
    public void Age_Out_Of_Range_Is_Error() {
        var user = new UserDto {
            Name = "Bob",
            Age = 10,
            Email = "bob@example.com",
            Password = "Abcdef12"
        };

        var result = user.Validate(user);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("Age"));
    }

    [Fact]
    public void Invalid_Email_Is_Error() {
        var user = new UserDto {
            Name = "Carol",
            Age = 40,
            Email = "invalid-email",
            Password = "Abcdef12"
        };

        var result = user.Validate(user);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("Email"));
    }

    [Fact]
    public void Password_Rules_Are_Validated() {
        var user = new UserDto {
            Name = "Dan",
            Age = 22,
            Email = "dan@example.com",
            Password = "short"
        };

        var result = user.Validate(user);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("at least 8"));
        // Missing upper/lower/digit should trigger multiple errors
        Assert.True(result.Errors.Count >= 2);
    }

    [Fact]
    public void Invalid_Phone_Is_Warning_Not_Error() {
        var user = new UserDto {
            Name = "Erin",
            Age = 28,
            Email = "erin@example.com",
            Password = "Abcdef12",
            Phone = "123-456" // too few digits
        };

        var result = user.Validate(user);
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
        Assert.NotEmpty(result.Warnings);
        Assert.Contains(result.Warnings, w => w.Contains("Phone"));
    }
}
