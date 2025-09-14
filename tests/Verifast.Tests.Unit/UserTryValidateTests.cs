namespace Verifast.Tests.Unit;

public class UserTryValidateTests {
    [Fact]
    public void TryValidate_Valid_User_Has_No_Errors() {
        var user = new UserDto {
            Name = "Alice",
            Age = 30,
            Email = "alice@example.com",
            Password = "Secret123",
            Phone = "+14155552671"
        };

        var ok = user.TryValidate(user, out var result);
        Assert.True(ok);
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
        Assert.Empty(result.Warnings);
    }

    [Fact]
    public void TryValidate_Missing_Name_Is_Error() {
        var user = new UserDto {
            Name = "",
            Age = 25,
            Email = "bob@example.com",
            Password = "Abcdef12"
        };

        var ok = user.TryValidate(user, out var result);
        Assert.False(ok);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("Name"));
    }

    [Fact]
    public void TryValidate_Age_Out_Of_Range_Is_Error() {
        var user = new UserDto {
            Name = "Bob",
            Age = 10,
            Email = "bob@example.com",
            Password = "Abcdef12"
        };

        var ok = user.TryValidate(user, out var result);
        Assert.False(ok);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("Age"));
    }

    [Fact]
    public void TryValidate_Invalid_Email_Is_Error() {
        var user = new UserDto {
            Name = "Carol",
            Age = 40,
            Email = "invalid-email",
            Password = "Abcdef12"
        };

        var ok = user.TryValidate(user, out var result);
        Assert.False(ok);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("Email"));
    }

    [Fact]
    public void TryValidate_Password_Rules_Are_Validated() {
        var user = new UserDto {
            Name = "Dan",
            Age = 22,
            Email = "dan@example.com",
            Password = "short"
        };

        var ok = user.TryValidate(user, out var result);
        Assert.False(ok);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("at least 8"));
        Assert.True(result.Errors.Count >= 2);
    }

    [Fact]
    public void TryValidate_Invalid_Phone_Is_Warning_Not_Error() {
        var user = new UserDto {
            Name = "Erin",
            Age = 28,
            Email = "erin@example.com",
            Password = "Abcdef12",
            Phone = "123-456" // too few digits
        };

        var ok = user.TryValidate(user, out var result);
        Assert.True(ok);
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
        Assert.NotEmpty(result.Warnings);
        Assert.Contains(result.Warnings, w => w.Contains("Phone"));
    }
}