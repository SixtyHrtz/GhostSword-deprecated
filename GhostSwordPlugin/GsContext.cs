using GhostSword;
using GhostSword.Types;
using GhostSwordPlugin.Enums;
using GhostSwordPlugin.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace GhostSwordPlugin
{
    public class GsContext : DbContext
    {
        public DbSet<Dialogue> Dialogues { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemType> ItemTypes { get; set; }
        public DbSet<ItemDiscovery> ItemDiscoveries { get; set; }
        public DbSet<Journey> Journeys { get; set; }
        public DbSet<NpcInfo> NpcInfos { get; set; }
        public DbSet<Place> Places { get; set; }
        public DbSet<PlaceAdjacency> PlaceAdjacencies { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<PlayerItem> PlayerItems { get; set; }
        public DbSet<PlaceLink> PlaceLinks { get; set; }
        public DbSet<PlayerPlace> PlayerPlaces { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Player>(x =>
            {
                x.Property(y => y.PlaceId).HasDefaultValue(1);
                x.Property(y => y.MenuId).HasDefaultValue(MenuType.Main);
                x.Property(y => y.IsBusy).HasDefaultValue(false);
                x.HasOne(y => y.HeadItem).WithMany().OnDelete(DeleteBehavior.Restrict);
                x.HasOne(y => y.ChestItem).WithMany().OnDelete(DeleteBehavior.Restrict);
                x.HasOne(y => y.HandsItem).WithMany().OnDelete(DeleteBehavior.Restrict);
                x.HasOne(y => y.LegsItem).WithMany().OnDelete(DeleteBehavior.Restrict);
                x.HasOne(y => y.FeetsItem).WithMany().OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PlaceLink>()
                .HasIndex(x => x.Name)
                .IsUnique();

            modelBuilder.Entity<PlayerPlace>()
                .Property(x => x.Phase)
                .HasDefaultValue(1);

            modelBuilder.Entity<Item>()
                .Property(x => x.ItemTypeId)
                .HasDefaultValue(1);

            modelBuilder.Entity<PlayerItem>(x =>
            {
                x.Property(y => y.Guid).HasDefaultValueSql("newsequentialid()");
                x.HasOne(y => y.Player).WithMany().OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Place>()
                .HasOne(x => x.PlaceLink)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Dialogue>()
                .Property(x => x.Text)
                .HasDefaultValue(string.Empty);

            modelBuilder.Entity<PlaceAdjacency>(x =>
            {
                x.HasOne(y => y.Place1)
                    .WithMany()
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasForeignKey(y => y.Place1Id);

                x.HasOne(y => y.Place2)
                    .WithMany()
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasForeignKey(y => y.Place2Id);
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = DatabaseSettings.Load("database.json").GetConnectionString();
            if (!connectionString.IsValid)
                throw new Exception($"{Resources.FailedLoadDbSettings}!");

            optionsBuilder.UseSqlServer(connectionString.Value);
        }
    }
}
