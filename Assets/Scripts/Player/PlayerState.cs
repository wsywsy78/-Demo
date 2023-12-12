/// <summary>
/// ���״̬�����
/// </summary>
public class PlayerState
{
    /// <summary>
    /// ��Ҷ���
    /// </summary>
    protected Player player;
    /// <summary>
    /// ״̬�����ƻ�
    /// </summary>
    protected PlayerStateMachine playerstateMachine;
    /// <summary>
    /// ����״̬����
    /// </summary>
    protected string animBoolName;
    /// <summary>
    /// ���췽��
    /// </summary>
    /// <param name="_player"></param>
    /// <param name="_playerStateMachine"></param>
    public PlayerState(Player _player, PlayerStateMachine _playerStateMachine, string _animBoolName)
    {
        this.player = _player;
        this.playerstateMachine = _playerStateMachine;
        this.animBoolName = _animBoolName;
    }
    /// <summary>
    /// ����һ��״̬
    /// </summary>
    public virtual void EnterState() { player.anim.SetBool(animBoolName, true); }
    /// <summary>
    /// �˳�һ��״̬
    /// </summary>
    public virtual void ExitState() { player.anim.SetBool(animBoolName, false); }
    /// <summary>
    /// ÿһ֡�����¼�
    /// </summary>
    public virtual void FrameUpdate() { }
    /// <summary>
    /// ����֡�����¼�
    /// </summary>
    public virtual void PhysicsUpdate() { }
    /// <summary>
    /// ���������¼�
    /// </summary>
    public virtual void AnimationTriggerEvent() { }
}
