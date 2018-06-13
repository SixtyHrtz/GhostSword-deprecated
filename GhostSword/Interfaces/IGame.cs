using GhostSword.Types;
using System;
using System.Collections.Generic;

namespace GhostSword.Interfaces
{
    public interface IGame
    {
        Tuple<IUser, Data<AnswerMessage>> GetAnswer(IncomeMessage message);
        List<AnswerMessage> GetEventsResults();
    }
}
