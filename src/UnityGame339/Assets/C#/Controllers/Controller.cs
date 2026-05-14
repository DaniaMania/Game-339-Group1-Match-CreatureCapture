using Game.Runtime;
using Game339.Shared.Diagnostics;

public abstract class Controller : ObserverMonoBehaviour
{
    protected readonly TurnEngine _turnEngine = ServiceResolver.Resolve<TurnEngine>();
    protected readonly IGameLog _logger = ServiceResolver.Resolve<IGameLog>();
}
