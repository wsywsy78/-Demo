using UnityEngine;

[CreateAssetMenu(fileName = "JumpSimple", menuName = "Player/Jump/Jump Simple")]
public class PlayerJumpSimple : PlayerJumpSOBase
{
    /// <summary>
    /// 跳跃的高度
    /// </summary>
    [SerializeField] private float jumpHeight = 1;
    /// <summary>
    /// 在跳跃时的同样可以进行横向输入，这是影响系数
    /// </summary>
    [SerializeField] private float moveEffect = 1f;
    public override void DoAnimationTriggerEventLogic()
    {
        base.DoAnimationTriggerEventLogic();
    }

    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
        Debug.Log("DoJump");
        //mgh = 1/2mv*2
        applyVector.y = Mathf.Sqrt(-2f * jumpHeight * virtualGravity);

        applyVector.x = m_playerController.velocity.x;
        applyVector.z = m_playerController.velocity.z;
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
    }

    public override void DoFrameUpdateLogic()
    {
        //Debug.Log("JumpUpdate");
        base.DoFrameUpdateLogic();
        //if (m_playerController.isGrounded)
        //    player.stateMachine.ChangeState(player.playerIdleState);
        //调整跳跃动画状态
        float _yVelocity = Mathf.Clamp(m_playerController.velocity.y, -1, 1);
        player.anim.SetFloat("yVelocity", _yVelocity);

        if (m_playerController.velocity.y <= 0)
            player.stateMachine.ChangeState(player.playerFallState);
    }

    public override void DoPhysicsUpdateLogic()
    {
        
        motionVector = (transform.right * player.playerMove.x + transform.forward * player.playerMove.y) * moveEffect;
        if ((applyVector.x < 0 && motionVector.x > 0) || (applyVector.x > 0 && motionVector.x < 0))
            applyVector.x = applyVector.x * 0.85f;
        else if((applyVector.x < 0 && motionVector.x < 0) || (applyVector.x > 0 && motionVector.x > 0))
            motionVector.x = 0;
        if ((applyVector.z < 0 && motionVector.z > 0) || (applyVector.z > 0 && motionVector.z < 0))
            applyVector.z = applyVector.z * 0.85f;
        else if((applyVector.z < 0 && motionVector.z < 0) || (applyVector.z > 0 && motionVector.z > 0))
            motionVector.z = 0;

        base.DoPhysicsUpdateLogic();

    }

    public override void Initialize(GameObject _gameObject, Player _player)
    {
        base.Initialize(_gameObject, _player);
    }

    public override void ResetValues()
    {
        base.ResetValues();
    }
}
