using Bogus;

namespace Verifast.Benchmarks.User;

public sealed class FakeUserRepository {
    private static readonly Faker MyFaker = new();

    private static readonly string[] DefaultBlacklistedDomains = [
        "spam.com",
        "malware.test"
    ];

    private readonly HashSet<string> _emails;
    private readonly HashSet<string> _blacklistedDomains;
    private readonly TimeSpan _delay;

    public FakeUserRepository(IEnumerable<string>? seedEmails = null, IEnumerable<string>? blacklistedDomains = null, TimeSpan? opDelay = null) {
        _emails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (seedEmails is null) {
            for (var i = 0; i < 10; i++) {
                _emails.Add(MyFaker.Internet.Email());
            }
        } else {
            foreach (var email in seedEmails) {
                _emails.Add(email);
            }
        }

        _blacklistedDomains = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        _blacklistedDomains.UnionWith(DefaultBlacklistedDomains);
        if (blacklistedDomains is null) {
            for (var i = 0; i < 3; i++) {
                _blacklistedDomains.Add(MyFaker.Internet.DomainName());
            }
        } else {
            foreach (var domain in blacklistedDomains) {
                _blacklistedDomains.Add(domain);
            }
        }
        _delay = opDelay ?? TimeSpan.FromMilliseconds(0.25);
    }

    public Task<bool> IsEmailUniqueAsync(string email, CancellationToken ct = default) {
        return Simulate(async () => {
            await Task.Yield();
            return !_emails.Contains(email);
        }, ct);
    }

    public Task<bool> IsDomainAllowedAsync(string email, CancellationToken ct = default) {
        return Simulate(async () => {
            await Task.Yield();
            var at = email.AsSpan().IndexOf('@');
            if (at <= 0 || at + 1 >= email.Length) {
                return false;
            }
            var domain = email[(at + 1)..];
            return !_blacklistedDomains.Contains(domain);
        }, ct);
    }

    public Task AddAsync(string email, CancellationToken ct = default) {
        return Simulate(async () => {
            await Task.Yield();
            _emails.Add(email);
        }, ct);
    }

    private async Task<T> Simulate<T>(Func<Task<T>> body, CancellationToken ct) {
        if (_delay > TimeSpan.Zero) {
            await Task.Delay(_delay, ct);
        }
        return await body();
    }

    private async Task Simulate(Func<Task> body, CancellationToken ct) {
        if (_delay > TimeSpan.Zero) {
            await Task.Delay(_delay, ct);
        }
        await body();
    }
}