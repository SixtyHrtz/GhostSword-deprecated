namespace GhostSword.Interfaces
{
    public interface IUser
    {
        int Id { get; set; }
        long UserId { get; set; }
        string Username { get; set; }
    }
}
