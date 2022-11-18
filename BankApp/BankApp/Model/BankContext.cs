using Microsoft.EntityFrameworkCore;

using PRBD_Framework;

using System;

namespace BankApp.Model
{
    public class BankContext : DbContextBase
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=BankApp")
                .EnableSensitiveDataLogging()
                .UseLazyLoadingProxies(true);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClientInternalAccount>()
                .HasOne(x => x.Client)
                .WithMany(y => y.ClientInternalAccounts)
                .OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder.Entity<ClientInternalAccount>()
                .HasOne(x => x.InternalAccount)
                .WithMany(y => y.ClientInternalAccounts)
                .OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder.Entity<ClientInternalAccount>()
                .HasKey(x => new { x.ClientId, x.InternalAccountId });

            base.OnModelCreating(modelBuilder);

        }

        public void SeedData()
        {
            Database.BeginTransaction();

            var agency1 = new Agency("Agency1");
            var agency2 = new Agency("Agency2");
            var agency3 = new Agency("Agency3");

            var bruno = new Manager("Bruno", "Lacroix", "bruno", "bruno@test.com", "bruno");
            var benoit = new Manager("Benoît", "Penelle", "ben", "ben@test.com", "ben");

            var admin = new Admin("Admin", "Istrator", "admin", "admin@test.com", "admin");

            var bob = new Client("Bob", "Marley", "bob", "bob@test.com", "bob", agency1);
            var caro = new Client("Caroline", "de Monaco", "caro", "caro@test.com", "caro", agency1);
            var louise = new Client("Louise", "TheCross", "louise", "louise@test.com", "louise", agency2);
            var jules = new Client("Jules", "TheCross", "jules", "jules@test.com", "jules", agency2);

            Users.AddRange(bruno, benoit, admin, bob, caro, louise, jules);

            bruno.Agencies.Add(agency3);
            benoit.Agencies.Add(agency1);
            benoit.Agencies.Add(agency2);

            var currentA = new CurrentAccount("BE02 9999 1017 8207", "AAA", -50);
            var currentB = new CurrentAccount("BE14 9996 1669 4306", "BBB", -10);
            var currentD = new CurrentAccount("BE55 9999 6717 9982", "DDD", -100);

            var savingC = new SavingAccount("BE71 9991 5987 4787", "CCC");

            var externalE = new ExternalAccount("BE23 0081 6870 0358", "EEE");

            var cia1 = new ClientInternalAccount(bob, currentA, ClientRole.Holder);
            var cia2 = new ClientInternalAccount(bob, currentB, ClientRole.Holder);
            var cia3 = new ClientInternalAccount(bob, savingC, ClientRole.Agent);
            var cia4 = new ClientInternalAccount(caro, currentA, ClientRole.Agent);
            var cia5 = new ClientInternalAccount(caro, currentD, ClientRole.Holder);
            var cia6 = new ClientInternalAccount(caro, savingC, ClientRole.Holder);

            ClientInternalAccounts.AddRange(cia1, cia2, cia3, cia4, cia5, cia6);

            var cat1 = new Category("Category1");
            var cat2 = new Category("Category2");
            var cat3 = new Category("Category3");
            var cat4 = new Category("Category4");
            var cat5 = new Category("Category5");

            Categories.AddRange(cat1, cat2, cat3, cat4, cat5);

            //currentA:-50/// , currentB:-10---, currentD:-50***, savingC:0+++ 
            var transfer14 = new BankTransfer(currentA, currentB, 10, "Tx #001", bob, new DateTime(2022, 01, 01)); // ///, ---
            var transfer12 = new BankTransfer(currentB, savingC, 15, "Tx #004", bob, new DateTime(2022, 01, 02), new DateTime(2022, 01, 03));// ---, +++
            var transfer6 = new BankTransfer(currentA, currentB, 50, "Tx #005", bob, new DateTime(2022, 01, 02), new DateTime(2022, 01, 04)); // /// refusé, ---
            var transfer7 = new BankTransfer(currentA, savingC, 5, "Tx #002", caro, new DateTime(2022, 01, 01), new DateTime(2022, 01, 05)); // ///, +++
            var transfer9 = new BankTransfer(savingC, currentB, 100, "Tx #008", caro, new DateTime(2022, 01, 06), null); // +++ refusé, --- 
            var transfer5 = new BankTransfer(currentB, currentA, 20, "Tx #006", bob, new DateTime(2022, 01, 03), new DateTime(2022, 01, 07)); // --- refusé, ///
            var transfer2 = new BankTransfer(externalE, savingC, 5, "Tx #007", null, new DateTime(2022, 01, 04), new DateTime(2022, 01, 08)); // ..., +++
            var transfer8 = new BankTransfer(currentA, currentB, 35, "Tx #003", caro, new DateTime(2022, 01, 01), new DateTime(2022, 01, 09)); // ///, ---
            var transfer13 = new BankTransfer(savingC, currentA, 15, "Tx #010", bob, new DateTime(2022, 01, 10), null); // +++, ///
            var transfer4 = new BankTransfer(savingC, externalE, 10, "Tx #009", bob, new DateTime(2022, 01, 07), new DateTime(2022, 01, 11)); // +++, ... => pas bon car pas vers un compte courant 
            var transfer11 = new BankTransfer(currentA, savingC, 100, "Tx #013", caro, new DateTime(2022, 01, 13), null);// ///refusé, +++
            var transfer3 = new BankTransfer(currentB, savingC, 35, "Tx #012", bob, new DateTime(2022, 01, 12), new DateTime(2022, 01, 14));// ---, +++
            var transfer1 = new BankTransfer(currentA, currentB, 15, "Tx #014", benoit, new DateTime(2022, 01, 15), null);// ///, ---
            var transfer10 = new BankTransfer(currentA, savingC, 15, "Tx #011", caro, new DateTime(2022, 01, 11), new DateTime(2022, 01, 16));// ///refusé, +++

            BankTransfers.AddRange(transfer1, transfer2, transfer3, transfer4, transfer5, transfer6, transfer7, transfer8,
                transfer9, transfer10, transfer11, transfer12, transfer13, transfer14);

            SaveChanges();

            Database.CommitTransaction();
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Agency> Agencies { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<ClientInternalAccount> ClientInternalAccounts { get; set; }
        public DbSet<InternalAccount> InternalAccounts { get; set; }
        public DbSet<ExternalAccount> ExternalAccounts { get; set; }
        public DbSet<CurrentAccount> CurrentAccounts { get; set; }
        public DbSet<SavingAccount> SavingAccounts { get; set; }
        public DbSet<BankTransfer> BankTransfers { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}
