public interface IState : IState<string> { }

public interface IState<TStateId>
{
    TStateId StateID { get; }
    void OnEnter();
    void OnUpdate();
    void OnLateUpdate();
    void OnFixedUpdate();
    void OnExit();
}

