namespace Verifast.Tests.Unit;

public class AsyncUserValidatorTests {
    [Fact]
    public async Task Valid_User_Has_No_Errors_Async() {
        var user = new AsyncUserDto {
            Name = "Alice",
            Age = 30,
            Email = "alice@example.com",
            Password = "Secret123",
            Phone = "+14155552671"
        };

        var result = await user.ValidateAsync(user, TestContext.Current.CancellationToken);
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
        Assert.Empty(result.Warnings);
    }

    [Fact]
    public async Task Missing_Name_Is_Error_Async() {
        var user = new AsyncUserDto {
            Name = "",
            Age = 25,
            Email = "bob@example.com",
            Password = "Abcdef12"
        };

        var result = await user.ValidateAsync(user, TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("Name"));
    }

    [Fact]
    public async Task Age_Out_Of_Range_Is_Error_Async() {
        var user = new AsyncUserDto {
            Name = "Bob",
            Age = 10,
            Email = "bob@example.com",
            Password = "Abcdef12"
        };

        var result = await user.ValidateAsync(user, TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("Age"));
    }

    [Fact]
    public async Task Invalid_Email_Is_Error_Async() {
        var user = new AsyncUserDto {
            Name = "Carol",
            Age = 40,
            Email = "invalid-email",
            Password = "Abcdef12"
        };

        var result = await user.ValidateAsync(user, TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("Email"));
    }

    [Fact]
    public async Task Password_Rules_Are_Validated_Async() {
        var user = new AsyncUserDto {
            Name = "Dan",
            Age = 22,
            Email = "dan@example.com",
            Password = "short"
        };

        var result = await user.ValidateAsync(user, TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("at least 8"));
        Assert.True(result.Errors.Count >= 2);
    }

    [Fact]
    public async Task Invalid_Phone_Is_Warning_Not_Error_Async() {
        var user = new AsyncUserDto {
            Name = "Erin",
            Age = 28,
            Email = "erin@example.com",
            Password = "Abcdef12",
            Phone = "123-456" // too few digits
        };

        var result = await user.ValidateAsync(user, TestContext.Current.CancellationToken);
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
        Assert.NotEmpty(result.Warnings);
        Assert.Contains(result.Warnings, w => w.Contains("Phone"));
    }
}