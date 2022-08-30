using Microsoft.EntityFrameworkCore;

namespace Telegram.Bot.Examples.WebHook.Models;

public sealed class BotContext : DbContext
{
    public BotContext(DbContextOptions<BotContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<AccountDraft> AccountDrafts { get; set; }
    public DbSet<Company> Companies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Company>().HasData(new Company {CompanyId = 1, CompanyName = "Ozon"});
        modelBuilder.Entity<Company>().HasData(new Company {CompanyId = 2, CompanyName = "Yandex"});
    }

    public async Task<User> GetOrInsertUser(long id)
    {
        User? user = await Users.FindAsync(id);
        if (user is null)
        {
            user = new User {UserId = id};
            await Users.AddAsync(user);
        }

        await SaveChangesAsync();
        return user;
    }

    public bool TryGetCompanyById(int id, out Company? company)
    {
        company = Companies.Find(id);
        return id >= 0 && company != null;
    }

    public bool TryGetDraftForUser(User user, out AccountDraft? draft)
    {
        draft = AccountDrafts.FirstOrDefault(accountDraft => accountDraft.UserId == user.UserId);
        return draft is not null;
    }
}