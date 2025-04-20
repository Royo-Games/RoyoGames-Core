public interface IState : IState<string> { }

public interface IState<TStateId>
{
    TStateId StateID { get; set; }
    StateMachine<TStateId> StateMachine { get; set; }
    void OnEnter();
    void OnUpdate();
    void OnLateUpdate();
    void OnFixedUpdate();
    void OnExit();
}

