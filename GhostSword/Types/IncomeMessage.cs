namespace GhostSword.Types
{
    public class IncomeMessage
    {
        public long Id { get; private set; }
        public string Username { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Text { get; private set; }

        public IncomeMessage(long id, string username, string firstname, string lastname, string text)
        {
            Id = id;
            Username = username;
            FirstName = firstname;
            LastName = lastname;
            Text = text;
        }
    }
}
