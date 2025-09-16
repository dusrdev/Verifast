using Bogus;
using Bogus.DataSets;

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
    private static readonly string[] TagPool = ["pro", "beta", "vip"];

    private static readonly Faker<Address> AddressFaker = new Faker<Address>()
        .StrictMode(true)
        .RuleFor(a => a.Street, f => f.Address.StreetAddress())
        .RuleFor(a => a.City, f => f.Address.City())
        .RuleFor(a => a.State, f => f.Address.StateAbbr())
        .RuleFor(a => a.PostalCode, f => f.Address.ZipCode())
        .RuleFor(a => a.Country, f => f.Address.CountryCode(Iso3166Format.Alpha2));

    private static readonly Faker<Address> InvalidAddressFaker = new Faker<Address>()
        .StrictMode(true)
        .RuleFor(a => a.Street, _ => string.Empty)
        .RuleFor(a => a.City, _ => string.Empty)
        .RuleFor(a => a.State, _ => string.Empty)
        .RuleFor(a => a.PostalCode, _ => "1")
        .RuleFor(a => a.Country, _ => string.Empty);

    private static readonly Faker<OrderItem> OrderItemFaker = new Faker<OrderItem>()
        .StrictMode(true)
        .RuleFor(i => i.Name, f => f.Commerce.ProductName())
        .RuleFor(i => i.Quantity, f => f.Random.Int(1, 5))
        .RuleFor(i => i.Price, f => f.Random.Decimal(5m, 200m));

    private static readonly Faker<Order> OrderFaker = new Faker<Order>()
        .StrictMode(true)
        .RuleFor(o => o.OrderId, f => f.Random.Guid())
        .RuleFor(o => o.Items, f => OrderItemFaker.Generate(f.Random.Int(1, 3)))
        .RuleFor(o => o.Total, (f, o) => o.Items.Sum(i => i.Price * i.Quantity));

    // Static faker instances keep data generation consistent across benchmark runs.
    private static readonly Faker<UserProfile> ValidProfileFaker = new Faker<UserProfile>()
        .StrictMode(true)
        .RuleFor(p => p.Id, f => f.Random.Guid())
        .RuleFor(p => p.FirstName, f => f.Name.FirstName())
        .RuleFor(p => p.LastName, f => f.Name.LastName())
        .RuleFor(p => p.Email, (f, p) => f.Internet.Email(p.FirstName, p.LastName))
        .RuleFor(p => p.Age, f => f.Random.Int(21, 70))
        .RuleFor(p => p.Address, _ => AddressFaker.Generate())
        .RuleFor(p => p.PhoneNumbers, f => f.Make(f.Random.Int(1, 3), () => f.Phone.PhoneNumber("+1-###-###-####")))
        .RuleFor(p => p.Preferences, f => new Preferences {
            ReceiveNewsletter = f.Random.Bool(),
            MarketingOptIn = f.Random.Bool(),
            PreferredLanguage = f.PickRandom("en", "es", "fr", "de"),
            Timezone = f.PickRandom("UTC", "America/New_York", "Europe/London", "Asia/Tokyo"),
            Tags = f.Random.Shuffle(TagPool).Take(f.Random.Int(1, TagPool.Length)).ToArray()
        })
        .RuleFor(p => p.RegisteredAt, f => f.Date.PastOffset(2))
        .RuleFor(p => p.LastLoginAt, (f, p) => f.Date.BetweenOffset(p.RegisteredAt, DateTimeOffset.UtcNow))
        .RuleFor(p => p.Orders, f => OrderFaker.Generate(f.Random.Int(1, 3)));

    private static readonly Faker<UserProfile> InvalidProfileFaker = new Faker<UserProfile>()
        .StrictMode(true)
        .RuleFor(p => p.Id, _ => Guid.Empty)
        .RuleFor(p => p.FirstName, _ => string.Empty)
        .RuleFor(p => p.LastName, _ => " ")
        .RuleFor(p => p.Email, f => f.Internet.Email())
        .RuleFor(p => p.Age, f => f.Random.Int(1, 12))
        .RuleFor(p => p.Address, _ => InvalidAddressFaker.Generate())
        .RuleFor(p => p.PhoneNumbers, f => new List<string> { f.Random.Word(), f.Random.Replace("###") })
        .RuleFor(p => p.Preferences, f => new Preferences {
            ReceiveNewsletter = true,
            MarketingOptIn = true,
            PreferredLanguage = f.Random.String2(2),
            Timezone = string.Empty,
            Tags = Array.Empty<string>()
        })
        .RuleFor(p => p.RegisteredAt, _ => default)
        .RuleFor(p => p.LastLoginAt, f => f.Date.PastOffset(5))
        .RuleFor(p => p.Orders, _ => new List<Order> {
            new() {
                OrderId = Guid.Empty,
                Total = 1000m,
                Items = new List<OrderItem> {
                    new() { Name = string.Empty, Quantity = 0, Price = -1m }
                }
            }
        });

    public static UserProfile CreateValid(string? email = null) {
        var profile = ValidProfileFaker.Generate();
        if (!string.IsNullOrWhiteSpace(email)) {
            profile.Email = email;
        }
        return profile;
    }

    public static UserProfile CreateInvalid(string? email = null) {
        var profile = InvalidProfileFaker.Generate();
        profile.Email = string.IsNullOrWhiteSpace(email) ? "taken@spam.com" : email;
        return profile;
    }
}