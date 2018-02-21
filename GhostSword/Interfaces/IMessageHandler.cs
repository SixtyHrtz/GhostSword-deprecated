using GhostSword.Types;
using Microsoft.EntityFrameworkCore;

namespace GhostSword.Interfaces
{
    public interface IMessageHandler
    {
        Data<Message> Invoke(DbContext context, IUser user, Command command);
    }
}
