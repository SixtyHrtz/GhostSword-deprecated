namespace GhostSword
{
    public class BaseMessageHandler : BaseCommandHandler
    {
        protected override int DefaultParamCount { get { return 2; } }
        protected override void RegisterCommands() { }
    }
}
