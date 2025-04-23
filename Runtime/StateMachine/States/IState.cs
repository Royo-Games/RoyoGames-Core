
public interface IState : IState<string> { }
public interface IState<TStateId> : IState<TStateId, Empty> { }
public interface IState<TStateId, TBlackBoard> where TBlackBoard : class
{
    TStateId StateID { get; set; }
    StateMachine<TStateId, TBlackBoard> StateMachine { get; set; }
    void OnEnter();
    void OnUpdate();
    void OnLateUpdate();
    void OnFixedUpdate();
    void OnExit();
}

