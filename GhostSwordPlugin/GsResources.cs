using GhostSword;
using GhostSword.Types;

namespace GhostSwordPlugin
{
    public static class GsResources
    {
        public static string BotExecutionFailed = "Возникла ошибка при запуске бота";
        public static string BotStarted = "Бот запущен";
        public static string BotStopped = "Бот остановлен";
        public static string BotStoppingFailed = "Возникла ошибка при остановке бота";

        public static string BackpackContent = "В инвентаре у тебя";
        public static string BackpackIsEmpty = "Рюкзак пуст...";
        public static string BackpackItemNotExists = "Такого предмета в рюкзаке нет!";
        public static string BackpackItemsCountOverflow = "Столько предметов в рюкзаке нет!";
        public static string CurrentDayTime = "Текущее время суток";
        public static string DialogNotExists = "Такого диалога не существует!";
        public static string Dropped = "Выброшено";
        public static string Equiped = "Надето";
        public static string ItemSlotIsEmpty = "На этом месте нет предмета!";
        public static string ItemSlotNotExists = "Такого места не существует!";
        public static string ItemsToDrop = "Эти предметы можно выкинуть";
        public static string Nearby = "Рядом находятся";
        public static string NothingToTalkAbout = "С этим персонажем не о чем поговорить...";
        public static string NpcNotExists = "Такого персонажа не существует!";
        public static string NpcTooFar = "Этот персонаж слишком далеко!";
        public static string PlaceTooFar = "Это место слишком далеко!";
        public static string PlaceNotExists = "Такого места не существует!";
        public static string Removed = "Снято";

        public static string LookAround = $"{Emoji.Eye} Осмотреться";
        public static string Backpack = $"{Emoji.SchoolBackpack} Инвентарь";
        public static string Drop = $"{Emoji.Wastebasket} Выбросить";
        public static string Back = $"{Emoji.BackArrow} Назад";

        public static Message PlayerIsBusy = new Message("Вам не до этого...");
    }
}
