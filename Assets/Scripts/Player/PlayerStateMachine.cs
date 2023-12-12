public class PlayerStateMachine
{
    /// <summary>
    /// 玩家状态
    /// </summary>
    public PlayerState currentState { get; private set; }
    /// <summary>
    /// 初始化状态
    /// </summary>
    /// <param name="_startState">初始状态</param>
    public void Initialize(PlayerState _startState)
    {
        currentState = _startState;
        currentState.EnterState();
    }
    /// <summary>
    /// 切换状态
    /// </summary>
    /// <param name="_newState">新的状态</param>
    public void ChangeState(PlayerState _newState)
    {
        //如果新状态等于当前的状态，返回（防止发生多次进入状态的状态初始化）
        if (_newState == currentState)
            return;
        currentState.ExitState();
        currentState = _newState;
        currentState.EnterState();
    }
}
