/// <summary>
/// 玩家状态类基类
/// </summary>
public class PlayerState
{
    /// <summary>
    /// 玩家对象
    /// </summary>
    protected Player player;
    /// <summary>
    /// 状态机控制机
    /// </summary>
    protected PlayerStateMachine playerstateMachine;
    /// <summary>
    /// 动画状态名称
    /// </summary>
    protected string animBoolName;
    /// <summary>
    /// 构造方法
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
    /// 进入一个状态
    /// </summary>
    public virtual void EnterState() { player.anim.SetBool(animBoolName, true); }
    /// <summary>
    /// 退出一个状态
    /// </summary>
    public virtual void ExitState() { player.anim.SetBool(animBoolName, false); }
    /// <summary>
    /// 每一帧更新事件
    /// </summary>
    public virtual void FrameUpdate() { }
    /// <summary>
    /// 物理帧更新事件
    /// </summary>
    public virtual void PhysicsUpdate() { }
    /// <summary>
    /// 动画触发事件
    /// </summary>
    public virtual void AnimationTriggerEvent() { }
}
