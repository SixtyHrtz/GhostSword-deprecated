using GhostSword;
using GhostSword.Types;
using GhostSwordPlugin.Enums;
using GhostSwordPlugin.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace GhostSwordPlugin
{
    public class GsContext : DbContext
    {
        public DbSet<Player> Players { get; set; }
        public DbSet<PlayerPlace> PlayerPlaces { get; set; }
        public DbSet<PlayerNpc> PlayerNpcs { get; set; }
        public DbSet<PlayerItem> PlayerItems { get; set; }

        public DbSet<Place> Places { get; set; }
        public DbSet<PlaceLink> PlaceLinks { get; set; }
        public DbSet<PlaceAdjacency> PlaceAdjacencies { get; set; }
        public DbSet<Journey> Journeys { get; set; }

        public DbSet<Npc> Npcs { get; set; }
        public DbSet<NpcLink> NpcLinks { get; set; }
        public DbSet<NpcInfo> NpcInfos { get; set; }
        public DbSet<NpcDialogue> NpcDialogues { get; set; }

        public DbSet<Dialogue> Dialogues { get; set; }

        public DbSet<Item> Items { get; set; }
        public DbSet<ItemType> ItemTypes { get; set; }
        public DbSet<ItemDiscovery> ItemDiscoveries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var relationships = modelBuilder.Model.GetEntityTypes().SelectMany(et => et.GetForeignKeys());
            foreach (var relationship in relationships)
                relationship.DeleteBehavior = DeleteBehavior.Restrict;

            modelBuilder.Entity<Player>(p =>
            {
                p.Property(p1 => p1.PlaceId).HasDefaultValue(1);
                p.Property(p1 => p1.MenuId).HasDefaultValue(MenuType.Main);
                p.Property(p1 => p1.IsBusy).HasDefaultValue(false);
                p.HasOne(p1 => p1.HeadItem).WithMany().OnDelete(DeleteBehavior.Restrict);
                p.HasOne(p1 => p1.ChestItem).WithMany().OnDelete(DeleteBehavior.Restrict);
                p.HasOne(p1 => p1.HandsItem).WithMany().OnDelete(DeleteBehavior.Restrict);
                p.HasOne(p1 => p1.LegsItem).WithMany().OnDelete(DeleteBehavior.Restrict);
                p.HasOne(p1 => p1.FeetsItem).WithMany().OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PlayerPlace>(pp =>
            {
                pp.HasIndex(pp1 => new { pp1.PlayerId, pp1.PlaceLinkId, pp1.Phase }).IsUnique();
                pp.Property(pp1 => pp1.Phase).HasDefaultValue(1);
                pp.HasOne(pp1 => pp1.Player)
                    .WithMany(p => p.PlayerPlaces)
                    .HasForeignKey(pp1 => pp1.PlayerId);
                pp.HasOne(pp1 => pp1.PlaceLink)
                    .WithMany(pl => pl.PlayerPlaces)
                    .HasForeignKey(pp1 => pp1.PlaceLinkId);
            });

            modelBuilder.Entity<PlayerNpc>(pn =>
            {
                pn.HasIndex(pn1 => new { pn1.PlayerId, pn1.NpcLinkId, pn1.Phase }).IsUnique();
                pn.Property(pn1 => pn1.Phase).HasDefaultValue(1);
                pn.HasOne(pn1 => pn1.Player)
                    .WithMany(p => p.PlayerNpcs)
                    .HasForeignKey(pn1 => pn1.PlayerId);
                pn.HasOne(pn1 => pn1.NpcLink)
                    .WithMany(pl => pl.PlayerNpcs)
                    .HasForeignKey(pn1 => pn1.NpcLinkId);
            });

            modelBuilder.Entity<PlayerItem>(pi =>
            {
                pi.HasIndex(pi1 => new { pi1.Guid, pi1.PlayerId, pi1.ItemId }).IsUnique();
                pi.Property(pi1 => pi1.Guid).HasDefaultValueSql("newid()");
                pi.HasOne(pi1 => pi1.Player)
                    .WithMany(p => p.PlayerItems)
                    .HasForeignKey(pi1 => pi1.PlayerId);
                pi.HasOne(pi1 => pi1.Item)
                    .WithMany(i => i.PlayerItems)
                    .HasForeignKey(pi1 => pi1.ItemId);
            });

            modelBuilder.Entity<Place>(p =>
            {
                p.HasIndex(p1 => new { p1.PlaceLinkId, p1.Phase }).IsUnique();
                p.HasOne(p1 => p1.PlaceLink)
                    .WithMany(pl => pl.Places)
                    .HasForeignKey(p1 => p1.PlaceLinkId);
            });

            modelBuilder.Entity<PlaceLink>().HasIndex(pl => pl.Name).IsUnique();

            modelBuilder.Entity<PlaceAdjacency>(pa =>
            {
                pa.HasOne(pa1 => pa1.Place1)
                    .WithMany()
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasForeignKey(pa1 => pa1.Place1Id);

                pa.HasOne(pa1 => pa1.Place2)
                    .WithMany()
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasForeignKey(pa1 => pa1.Place2Id);
            });

            modelBuilder.Entity<Journey>(j =>
            {
                j.HasIndex(j1 => new { j1.Guid, j1.PlayerId, j1.PlaceAdjacencyId }).IsUnique();
                j.Property(j1 => j1.Guid).HasDefaultValueSql("newid()");
                j.HasOne(j1 => j1.Player)
                    .WithMany(p => p.Journeys)
                    .HasForeignKey(j1 => j1.PlayerId);
                j.HasOne(j1 => j1.PlaceAdjacency)
                    .WithMany(pa => pa.Journeys)
                    .HasForeignKey(j1 => j1.PlaceAdjacencyId);
            });

            modelBuilder.Entity<Npc>(n =>
            {
                n.HasIndex(n1 => new { n1.NpcLinkId, n1.Phase }).IsUnique();
                n.HasOne(n1 => n1.NpcLink)
                    .WithMany(nl => nl.Npcs)
                    .HasForeignKey(n1 => n1.NpcLinkId);
            });

            modelBuilder.Entity<NpcLink>().HasIndex(nl => nl.Name).IsUnique();

            modelBuilder.Entity<Dialogue>().Property(d => d.Text).HasDefaultValue(string.Empty);

            modelBuilder.Entity<Item>().Property(i => i.ItemTypeId).HasDefaultValue(1);
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
