
public interface IState<TStateId, TManager> where TManager : class
{
    TStateId StateID { get; set; }
    StateMachine<TStateId, TManager> StateMachine { get; set; }
    void OnEnter();
    void OnUpdate();
    void OnLateUpdate();
    void OnFixedUpdate();
    void OnExit();
}

