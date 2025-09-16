namespace Verifast.Benchmarks.User;

public class UserProfile {
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Age { get; set; }
    public Address Address { get; set; } = new();
    public List<string> PhoneNumbers { get; set; } = new();
    public Preferences Preferences { get; set; } = new();
    public DateTimeOffset RegisteredAt { get; set; }
    public DateTimeOffset? LastLoginAt { get; set; }
    public List<Order> Orders { get; set; } = new();
}

public class Address {
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}

public class Preferences {
    public bool ReceiveNewsletter { get; set; }
    public bool MarketingOptIn { get; set; }
    public string PreferredLanguage { get; set; } = "en";
    public string Timezone { get; set; } = "UTC";
    public string[] Tags { get; set; } = Array.Empty<string>();
}

public class Order {
    public Guid OrderId { get; set; }
    public decimal Total { get; set; }
    public List<OrderItem> Items { get; set; } = new();
}

public class OrderItem {
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

public static class UserProfileFactory {
    private static readonly string[] Tags = ["pro", "beta", "vip"];

    public static UserProfile CreateValid(string email = "john.doe@example.com") {
        var profile = new UserProfile {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Email = email,
            Age = 34,
            Address = new Address {
                Street = "123 Main St",
                City = "Metropolis",
                State = "NY",
                PostalCode = "10001",
                Country = "US"
            },
            PhoneNumbers = new List<string> { "+1-212-555-0101", "+1-212-555-0199" },
            Preferences = new Preferences {
                ReceiveNewsletter = true,
                MarketingOptIn = false,
                PreferredLanguage = "en",
                Timezone = "America/New_York",
                Tags = Tags
            },
            RegisteredAt = DateTimeOffset.UtcNow.AddDays(-30),
            LastLoginAt = DateTimeOffset.UtcNow.AddMinutes(-15),
            Orders = new List<Order>()
        };
        profile.Orders = ComputeTotals(new List<Order> {
            new() { OrderId = Guid.NewGuid(), Items = new List<OrderItem>{ new() { Name = "Widget", Quantity=2, Price=19.99m } } },
            new() { OrderId = Guid.NewGuid(), Items = new List<OrderItem>{ new() { Name = "Gadget", Quantity=1, Price=49.99m } } },
        });
        return profile;
    }

    public static UserProfile CreateInvalid(string email = "taken@spam.com") {
        // Multiple issues: empty names, invalid email, underage, weak address, bad phones, bad totals
        var profile = new UserProfile {
            Id = Guid.Empty,
            FirstName = "",
            LastName = " ",
            Email = email,
            Age = 10,
            Address = new Address {
                Street = "",
                City = "",
                State = "",
                PostalCode = "1",
                Country = ""
            },
            PhoneNumbers = new List<string> { "not-a-number", "123" },
            Preferences = new Preferences {
                ReceiveNewsletter = true,
                MarketingOptIn = true,
                PreferredLanguage = "zz",
                Timezone = "",
                Tags = Array.Empty<string>()
            },
            RegisteredAt = default,
            LastLoginAt = DateTimeOffset.UtcNow.AddYears(-1),
            Orders = new List<Order> {
                new() { OrderId = Guid.Empty, Total = 1000m, Items = new List<OrderItem>{ new() { Name = "", Quantity=0, Price=-1m } } },
            }
        };
        return profile;
    }

    private static List<Order> ComputeTotals(List<Order> orders) {
        foreach (var o in orders) {
            decimal sum = 0m;
            foreach (var i in o.Items) {
                sum += i.Price * i.Quantity;
            }
            o.Total = sum;
        }
        return orders;
    }
}
