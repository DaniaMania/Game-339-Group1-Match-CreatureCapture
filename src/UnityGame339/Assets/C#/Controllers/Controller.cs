using Game.Runtime;

public abstract class Controller : ObserverMonoBehaviour
{
    protected readonly TurnEngine _turnEngine = ServiceResolver.Resolve<TurnEngine>();
}
