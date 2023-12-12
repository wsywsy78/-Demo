public class PlayerStateMachine
{
    /// <summary>
    /// ���״̬
    /// </summary>
    public PlayerState currentState { get; private set; }
    /// <summary>
    /// ��ʼ��״̬
    /// </summary>
    /// <param name="_startState">��ʼ״̬</param>
    public void Initialize(PlayerState _startState)
    {
        currentState = _startState;
        currentState.EnterState();
    }
    /// <summary>
    /// �л�״̬
    /// </summary>
    /// <param name="_newState">�µ�״̬</param>
    public void ChangeState(PlayerState _newState)
    {
        //�����״̬���ڵ�ǰ��״̬�����أ���ֹ������ν���״̬��״̬��ʼ����
        if (_newState == currentState)
            return;
        currentState.ExitState();
        currentState = _newState;
        currentState.EnterState();
    }
}
