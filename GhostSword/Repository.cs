using GhostSword.Interfaces;
using GhostSword.Types;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace GhostSword
{
    public abstract class Repository
    {
        public bool Locked { get; private set; }

        public abstract Data<IUser> GetUser(DbContext context, IncomeMessage message);
        public abstract Data<Keyboard> GetKeyboard(DbContext context, IUser user);
        public abstract List<AnswerMessage> GetEventsResults(DbContext context);

        public void Lock() => Locked = true;
        public void Unlock() => Locked = false;
    }
}
