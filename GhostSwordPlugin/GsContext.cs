using GhostSword;
using GhostSword.Types;
using GhostSwordPlugin.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace GhostSwordPlugin
{
    public class GsContext : DbContext
    {
        public DbSet<Dialogue> Dialogues { get; set; }
        public DbSet<Item> Items { get; set; }
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
            modelBuilder.Entity<Player>()
                .Property(x => x.PlaceId)
                .HasDefaultValue(1);

            modelBuilder.Entity<Player>()
                .Property(x => x.MenuId)
                .HasDefaultValue(1);

            modelBuilder.Entity<Player>()
                .Property(x => x.IsBusy)
                .HasDefaultValue(false);

            modelBuilder.Entity<PlaceLink>()
                .HasIndex(x => x.Name)
                .IsUnique();

            modelBuilder.Entity<PlayerPlace>()
                .Property(x => x.Phase)
                .HasDefaultValue(1);

            modelBuilder.Entity<Place>()
                .HasOne(x => x.PlaceLink)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Dialogue>()
                .Property(x => x.Text)
                .HasDefaultValue(string.Empty);

            modelBuilder.Entity<PlaceAdjacency>()
                .HasOne(x => x.Place1)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict)
                .HasForeignKey(x => x.Place1Id);

            modelBuilder.Entity<PlaceAdjacency>()
                .HasOne(x => x.Place2)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict)
                .HasForeignKey(x => x.Place2Id);
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
