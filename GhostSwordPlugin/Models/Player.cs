using GhostSword.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace GhostSwordPlugin.Models
{
    public class Player : IUser
    {
        public int Id { get; set; }
        public long UserId { get; set; }
        [Required]
        public string Username { get; set; }
        public bool IsBusy { get; set; }
        public int PlaceId { get; set; }
        public uint MenuId { get; set; }

        public Place Place { get; set; }

        public Player() { }

        public Player(long id, string username) : this()
        {
            UserId = id;
            Username = username;
        }
    }
}
