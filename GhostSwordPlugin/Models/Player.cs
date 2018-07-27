using GhostSword.Interfaces;
using GhostSwordPlugin.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GhostSwordPlugin.Models
{
    public class Player : IUser
    {
        public int Id { get; set; }
        public long UserId { get; set; }
        [Required]
        public string Username { get; set; }
        public bool IsBusy { get; set; }
        public int ClassId { get; set; }
        public int PlaceId { get; set; }
        public MenuType MenuId { get; set; }

        public uint Experience { get; set; }
        public uint BaseHealth { get; set; }
        public uint Health { get; set; }
        public uint BaseStamina { get; set; }
        public uint Stamina { get; set; }
        public DateTime? StartRecoveryTime { get; set; }
        public uint BaseEnergy { get; set; }
        public uint Energy { get; set; }

        public uint BaseMeleeAttack { get; set; }
        public uint BaseRangeAttack { get; set; }
        public uint BaseMagicAttack { get; set; }
        public uint BaseMeleeDefence { get; set; }
        public uint BaseRangeDefence { get; set; }
        public uint BaseMagicDefence { get; set; }

        public Guid? HeadItemGuid { get; set; }
        public Guid? ChestItemGuid { get; set; }
        public Guid? HandsItemGuid { get; set; }
        public Guid? LegsItemGuid { get; set; }
        public Guid? FeetsItemGuid { get; set; }

        public Class Class { get; set; }
        public Place Place { get; set; }

        public PlayerItem HeadItem { get; set; }
        public PlayerItem ChestItem { get; set; }
        public PlayerItem HandsItem { get; set; }
        public PlayerItem LegsItem { get; set; }
        public PlayerItem FeetsItem { get; set; }

        public IEnumerable<PlayerPlace> PlayerPlaces { get; set; }
        public IEnumerable<PlayerNpc> PlayerNpcs { get; set; }
        public IEnumerable<PlayerItem> PlayerItems { get; set; }
        public IEnumerable<Journey> Journeys { get; set; }

        [NotMapped]
        public uint Level { get { return GetLevelInfo(Experience).Item1; } }
        [NotMapped]
        public uint ExperienceToNextLevel { get { return GetLevelInfo(Experience).Item2; } }
        [NotMapped]
        public uint TotalHealth { get { return BaseHealth; } }
        [NotMapped]
        public uint TotalStamina { get { return BaseStamina; } }
        [NotMapped]
        public uint TotalEnergy { get { return BaseEnergy; } }

        [NotMapped]
        public uint TotalMeleeAttack { get { return BaseMeleeAttack; } }
        [NotMapped]
        public uint TotalRangeAttack { get { return BaseRangeAttack; } }
        [NotMapped]
        public uint TotalMagicAttack { get { return BaseMagicAttack; } }
        [NotMapped]
        public uint TotalMeleeDefence { get { return BaseMeleeDefence; } }
        [NotMapped]
        public uint TotalRangeDefence { get { return BaseRangeDefence; } }
        [NotMapped]
        public uint TotalMagicDefence { get { return BaseMagicDefence; } }

        public Player() { }

        public Player(long id, string username) : this()
        {
            UserId = id;
            Username = username;
        }

        private Tuple<uint, uint> GetLevelInfo(uint experience)
        {
            uint expToNext = 0;
            uint level = 0;

            while (expToNext < experience)
                expToNext += (++level + 1) * 100;

            return new Tuple<uint, uint>(Math.Max(1, level), Math.Max(expToNext, 200));
        }

        public int GetStaminaRecoveryTime()
        {
            if (StartRecoveryTime == null)
                return 0;
            return (int)(30 - (DateTime.Now - StartRecoveryTime.Value).TotalMinutes);
        }
    }
}
