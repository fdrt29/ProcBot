using Microsoft.EntityFrameworkCore;

namespace Telegram.Bot.Examples.WebHook.Models;

public sealed class BotContext : DbContext
{
    public BotContext(DbContextOptions<BotContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } // => SetUser(...);
    public DbSet<Account> Accounts { get; set; } // => SetUser(...);
    public DbSet<AccountDraft> AccountDrafts { get; set; } // => SetUser(...);
    public DbSet<Company> Companies { get; set; } // => SetUser(...);

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Company>().HasData(new Company {CompanyId = 1, CompanyName = "Ozon"});
        modelBuilder.Entity<Company>().HasData(new Company {CompanyId = 2, CompanyName = "Yandex"});
        //modelBuilder.Entity<User>()
        //    .HasOne(a => a.AccountDraft)
        //    .WithOne(a => a.User)
        //.HasForeignKey<AccountDraft>(c => c.UserId);

        // modelBuilder.Entity<AccountDraft>().HasNoKey();
    }

    public async Task<User> InsertUserIfNotExist(long id)
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


    public async void EnsureAccountDraftCreated(int id, User user)
    {
        AccountDraft? accountDraft = await AccountDrafts.FindAsync(id);
        if (accountDraft is not null) return;

        AccountDrafts.Add(new AccountDraft());
        await SaveChangesAsync();
    }
}