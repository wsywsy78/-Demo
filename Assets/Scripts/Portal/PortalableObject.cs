using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.TextCore.Text;

enum ObjectType 
{ 
    player,
    other
}

/// <summary>
/// 所有进入传送门碰撞区域的物体
/// </summary>
[Serializable]
public class PortalableObject
{
    /// <summary>
    /// 原物体
    /// </summary>
    [SerializeField] private GameObject originalObject;
    /// <summary>
    /// 克隆的物体
    /// </summary>
    public GameObject cloneObject { get; private set; }
    /// <summary>
    /// 被克隆物体的进入传送门
    /// </summary>
    public Transform inPortal { get; private set; }
    /// <summary>
    /// 被克隆物体的出传送门
    /// </summary>
    public Transform outPortal { get; private set; }
    /// <summary>
    /// 被克隆物体的Transform的引用
    /// </summary>
    private Transform originalTransform;
    /// <summary>
    /// 被克隆物体的类型
    /// </summary>
    private ObjectType objectType;
    /// <summary>
    /// 当被克隆物体是玩家的话，就要获得玩家身体的Transform
    /// </summary>
    private Transform playerBody;    

    private float intervalTime = 0;
    private static readonly Quaternion halfTurn = Quaternion.Euler(0.0f, 180.0f, 0.0f);

    public PortalableObject(GameObject _cloneObject, Transform _inPortal, Transform _outPortal)
    {
        if (_cloneObject.GetComponentInChildren<Player>() != null)
            objectType = ObjectType.player;
        else
            objectType = ObjectType.other;
        this.originalObject = _cloneObject;
        this.inPortal = _inPortal;
        this.outPortal = _outPortal;
        CloneObject(_cloneObject);
        //当被克隆物体是玩家时，获取的不是物体的Transform而是相机的Transform，为了更好的传送体验
        this.originalTransform = objectType == ObjectType.player ? 
            _cloneObject.GetComponentInChildren<Camera>().transform : originalObject.transform;
        this.playerBody = objectType == ObjectType.player ? 
            _cloneObject.GetComponentInChildren<Animator>().transform : null;
    }

    public void Update()
    {
        intervalTime -= Time.deltaTime;
        MoveOriginalObject();
        MoveCloneObject();
    }

    /// <summary>
    /// 克隆一个传送门碰撞体内的物体
    /// </summary>
    /// <param name="_cloneObject"></param>
    private void CloneObject(GameObject _cloneObject)
    {
        //如果克隆的对象是玩家，由于玩家的模型用的是SkinnedMeshRenderer，无法简单复制
        //实例化玩家子对象中挂载了Animator的对象（避免将带有控制脚本的对象复制），这个对象是玩家的模型，因为只有模型，所以没有动画...
        if (_cloneObject.GetComponent<Player>() != null)
        {
            //不继承自MonoBehaviour,要这样实例化
            this.cloneObject = UnityEngine.Object.Instantiate(_cloneObject.GetComponentInChildren<Animator>().gameObject);
            //将层级设为default，因为玩家的摄像机会剔除原本的玩家模型所在的层级
            foreach(Transform child in this.cloneObject.GetComponentsInChildren<Transform>())
            {
                child.gameObject.layer = 0;
            }
            this.cloneObject.name = _cloneObject.name;
            return;
        }

        //只克隆被复制物体的Mesh信息，提升性能
        //this.cloneObject = new GameObject();
        //var _meshFilter = this.cloneObject.AddComponent<MeshFilter>();
        //var _meshRender = this.cloneObject.AddComponent<MeshRenderer>();
        //_meshFilter.mesh = _cloneObject.GetComponent<MeshFilter>()?.mesh;
        //_meshRender.materials = _cloneObject.GetComponent<MeshRenderer>()?.materials;

        //注：在测试时发现，当克隆出来的物体同样带有刚体和碰撞体的时候，会同时触发另一个传送门的克隆，有可能会形成一种无限克隆
        //故在这里我们要取消克隆物体和传送门碰撞盒之间的碰撞
        this.cloneObject = UnityEngine.Object.Instantiate(_cloneObject);
        this.cloneObject.transform.localScale = _cloneObject.transform.lossyScale;
        this.cloneObject.name = _cloneObject.name;
        var _collider = this.cloneObject.GetComponent<Collider>();
        _collider.enabled = false;
    }

    /// <summary>
    /// 移动克隆物体
    /// </summary>
    /// <param name="_cloneObject"></param>
    private void MoveCloneObject()
    {
        var _transform = objectType == ObjectType.player ? playerBody : originalObject.transform;

        //同PortalCamera，将克隆物体转换到另一个传送门的后面
        Quaternion _relativeRot = Quaternion.Inverse(inPortal.rotation) * originalObject.transform.rotation;
        _relativeRot = halfTurn * _relativeRot;
        cloneObject.transform.rotation = outPortal.rotation * _relativeRot;

        Vector3 _relativePos = inPortal.InverseTransformPoint(_transform.position);
        _relativePos = halfTurn * _relativePos;
        cloneObject.transform.position = outPortal.TransformPoint(_relativePos);

    }

    /// <summary>
    /// 检查原物体是否穿过传送门，以将物体传送至另一个传送门
    /// </summary>
    private void MoveOriginalObject()
    {
        //如果克隆物体是传送枪，返回（因为玩家的旋转和传送已经会将传送枪一并传送）
        if (originalObject.CompareTag("PortalGun"))
            return;
        //检测物体相对传送门的位置，如果相对z轴的距离大于零（传送门的z轴在反面），将物体传送
        Vector3 _pos = inPortal.InverseTransformPoint(originalTransform.position);
        if (_pos.z > 0 && intervalTime < 0)
        {
            //Debug.Log($"tag:{originalObject.tag},id:{originalObject.GetInstanceID()}");
            intervalTime = 0.04f;

            KeepMomentum();
            ConvertRotation();
            ConvertPosition();
            //Time.timeScale = 0;
            //Debug.Log($"Trigger, {inPortal.name}, {originalObject.transform.position}, pos:{_pos}");

            var _temPortal = inPortal;
            inPortal = outPortal;
            outPortal = _temPortal;

            //检查是否要对玩家吸附物体进行更改
            IsChangePickObject();           
        }
    }

    /// <summary>
    /// 调整物体位置
    /// </summary>
    private void ConvertPosition()
    {

        //多次测试发现，当物体上挂载了CharacterController，当此时调用了Move的同时，将物体传送到另一个传送门后马上会回来，
        //推测与Unity的物理引擎行为有关，为了能正确传送，只能暂时禁用这些组件
        //暂时没有更好的方法
        if (objectType == ObjectType.player)
        {
            //当人物是从两个都垂直于水平面的传送门进出时，只需要简单的计算相对位置即可
            //当一方的传送门不是垂直于水平面时，就不能简单计算相对位置，因为人物的特殊旋转使得人物始终都是竖直的，而摄像机是人物的子物体
            //简单的计算相对位置可能会导致各种各样的视角穿模，
            //而且人物的传送是通过人物的相机位置和传送门的距离关系决定的，因为人物特殊的旋转，可能会导致误判，使得人物再次传送回去
            //必须通过不同的传送门出口对人物的位置进行一定的偏移，但是又不能太过破坏传送的流畅性
            //所以，这里的想法是通过人物的摄像机的位置反过来推算人物的位置偏移，这样能够最大程度的保持传送的流畅性，而且能够避免上述提到的问题
            Vector3 _relativePos = inPortal.InverseTransformPoint(originalTransform.position);
            _relativePos = halfTurn * _relativePos;
            _relativePos = outPortal.TransformPoint(_relativePos);
            //相机与玩家的相对位置
            Vector3 _cameraToPlayer = originalTransform.localPosition;
            _relativePos = _relativePos - _cameraToPlayer;

            //测试发现，当禁用CharacterController时，由于碰撞体失效，会导致传送门的OntriggerExit不触发，使得克隆体保留，故使用Move直接将人物移动
            //var _player = originalObject.GetComponent<CharacterController>();
            //_player.enableOverlapRecovery = false;
            //_player.Move(_cameraVec);
            //_player.enableOverlapRecovery = true;

            //多次测试发现，当使用CharacterController的Move方法移动人物时，由于Move受物理引擎影响
            //在碰到碰撞体时会停下来，尝试了多种无视碰撞的方法，都有问题，故还是使用一开始的方法
            //在传送后主动调用离开传送门的事件方法，以清除克隆体
            var _character = originalObject.GetComponent<CharacterController>();
            //由于Portal的OntriggerEnter触发晚于传送，在出传送门平面与玩家会有重叠时，有可能将玩家挤出
            //所以此处要手动取消和出传送平面所附加碰撞体的碰撞，在传送完成后调整回来
            Physics.IgnoreCollision(_character, outPortal.GetComponent<Portal>().attachCollider);
            _character.enabled = false;
            originalObject.transform.position = _relativePos;
            _character.enabled = true;

            this.TriggerEvent(EventName.GetOutPortal, new EventParameter { eventGetOutPortal = originalObject.GetComponent<Collider>().GetInstanceID() });
            //Debug.Log($"afterportalVec:{_character.velocity}");

            AddVelocity();
            Physics.IgnoreCollision(_character, outPortal.GetComponent<Portal>().attachCollider, false);
        }
        //当传送的物体不是人物时，通过物体的刚体组件移动物体，避免物理错误。（暂时没有考虑像激光一样的可能不带有刚体的物体）
        else
        {
            Vector3 _relativePos = inPortal.InverseTransformPoint(originalTransform.position);
            _relativePos = halfTurn * _relativePos;
            Rigidbody rb = originalObject.GetComponent<Rigidbody>();
            originalObject.transform.position = outPortal.TransformPoint(_relativePos);
            //rb.MovePosition(outPortal.TransformPoint(_relativePos));       
        }
    }

    /// <summary>
    /// 调整物体旋转朝向
    /// </summary>
    private void ConvertRotation()
    {
        //对于玩家来说，只需要调整人物y轴的旋转，因为无论如何不能让人物“歪”过来从而产生身体或是头朝地的情况
        //对于玩家的摄像机来说，要同时调整x，z的旋转
        if(objectType == ObjectType.player)
        {
            Quaternion _relativeRot = Quaternion.Inverse(inPortal.rotation) * originalTransform.rotation;
            _relativeRot = halfTurn * _relativeRot;
            _relativeRot = outPortal.rotation * _relativeRot;
            //Debug.Log($"Rot3: {_relativeRot.eulerAngles}");
            originalObject.transform.rotation = Quaternion.Euler(0, _relativeRot.eulerAngles.y, 0);
            originalTransform.localRotation = Quaternion.Euler(_relativeRot.eulerAngles.x, 0, _relativeRot.eulerAngles.z);
            //Debug.Log($"camera: {originalTransform.localEulerAngles}");
            this.TriggerEvent(EventName.ChangeCameraMode);
        }
        //对于物体来说，就按照旋转虚拟摄像机的方法来旋转就行了
        else
        {
            //Debug.Log($"{originalObject.GetInstanceID()} rot");
            Quaternion _relativeRot = Quaternion.Inverse(inPortal.rotation) * originalObject.transform.rotation;
            _relativeRot = halfTurn * _relativeRot;
            originalObject.transform.rotation = outPortal.rotation * _relativeRot;
        }
    }
    
    /// <summary>
    /// 调整摄像机的旋转（人物的旋转会将摄像机一起旋转，为了保证传送的流畅性，必须单独对摄像机的旋转进行调整）
    /// </summary>
    private void AdjustCamera()
    {
        //只有在玩家传送的时候菜肴旋转摄像机
        if(objectType == ObjectType.player)
        {
            ////计算相机和进传送门z轴的夹角
            //float _inRot = Vector3.Angle(inPortal.forward, originalTransform.forward);
            ////再计算出传送门的z轴和世界下y轴的夹角
            //float _outRot = Vector3.Angle(outPortal.forward, Vector3.up);
            ////Debug.Log($"{_temRot}");
            ////计算传送后摄像机的x夹角
            //float _xRot = _outRot + originalTransform.localRotation.eulerAngles.x - 90;
            //Debug.Log($"portal:{_temRot}, resulr:{_xRot}");

            //这里将摄像机的旋转相对于传送门翻转
            Quaternion _relativeRot = Quaternion.Inverse(inPortal.rotation) * originalTransform.rotation;
            _relativeRot = halfTurn* _relativeRot;
            _relativeRot = outPortal.rotation * _relativeRot;
            Debug.Log($"{_relativeRot.eulerAngles}");
            //只需要旋转x轴，因为相机的y轴旋转是由玩家的旋转进行控制的
            originalTransform.localRotation = Quaternion.Euler(_relativeRot.eulerAngles.x, 0, 0);
            Debug.Log($"Camera: {originalTransform.rotation.eulerAngles}");
            //originalTransform.localRotation = Quaternion.Euler(_xRot, 0, 0);
            this.TriggerEvent(EventName.ChangeCameraMode);
        }
    }

    /// <summary>
    /// 判断是否要对吸附物体的吸附方式进行更改
    /// </summary>
    private void IsChangePickObject()
    {
        //如果此物体是吸附物体，则恢复更改
        if(PickObjectController.Instance.pickedObject == originalObject)
        {
            //如果吸附体已经更改了一次，那么在退出更改前无论如何都无法更改第二次，所以将其恢复
            if(PickObjectController.Instance.isInPortal)
            {
                this.TriggerEvent(EventName.PickGetInfo, new EventParameter { eventTransforms = null });
            }
            else
            {
                this.TriggerEvent(EventName.PickGetInfo, new EventParameter
                { eventTransforms = new Transform[2] { outPortal, inPortal } });
            }
        }
    
        else if (originalObject.CompareTag("Player"))
        {
            //当玩家在穿过传送门的时候，如果此时吸附物已经吸附模式已经更改，相应的要更改回来
            if (PickObjectController.Instance.pickedObject != null && PickObjectController.Instance.isInPortal)
            {
                this.TriggerEvent(EventName.PickGetInfo, new EventParameter { eventTransforms = null });
                //测试发现，此时不返回的话会马上触发下一个条件
                return;
            }
            //反之，当此时未更改，则要进行更改
            else if (PickObjectController.Instance.pickedObject != null && !PickObjectController.Instance.isInPortal)
            {
                this.TriggerEvent(EventName.PickGetInfo, new EventParameter
                { eventTransforms = new Transform[2] { outPortal, inPortal } });
            }
        }
    }

    /// <summary>
    /// 在传送时为物体增加一个初速度
    /// </summary>
    private void AddVelocity()
    {
        //在传送门的forward是朝着xoz平面向上的时候，为传送瞬间的物体增加一个初速度，改善传送体验（有时由于传送后的人物的重力会将人物拉回来）
        var _player = originalObject.GetComponent<CharacterController>();
        //计算出传送门的向量在y轴方向上的投影
        Vector3 _projectVec = Vector3.Project(-outPortal.forward, Vector3.up);
        //只有在传送门是朝上的时候增加速度初始值
        if (_projectVec.y <= 0)
            return;
        //计算投影的模长(这个模场对应着增加速度的强度大小)
        float _upMagnitude = _projectVec.magnitude;
        //增加速度的矢量
        Vector3 _addVelocity = -outPortal.forward * _upMagnitude;
        //令玩家在出传送门的forward方向上具有一定的速度，这个速度主要是用于让玩家能够顺利通过一些具有夹角的出传送门
        _player.Move(_addVelocity * 3);
        //Debug.Log($"trigger, velocity: {_addVelocity}");
    }

    /// <summary>
    /// 继承传送前后的动量
    /// </summary>
    private void KeepMomentum()
    {
        Vector3 _momentum;
        if(objectType == ObjectType.player)
        {
            //_momentum = originalObject.GetComponent<CharacterController>().velocity;
            _momentum = GetPhysicsProperty.prevFrameApplyVec + GetPhysicsProperty.prevFrameMaintainVec;
            //Debug.Log($"原始速度：{_momentum}");
            _momentum = Vector3.Project(_momentum, inPortal.forward);
            this.TriggerEvent(EventName.MaintainMomentum, new EventParameter 
            { maintainVec = _momentum.magnitude * -outPortal.forward });
            Debug.Log($"KeepVec: {_momentum}, outVec: {_momentum.magnitude * -outPortal.forward}");
        }
        else
        {
            var _rb = originalObject.GetComponent<Rigidbody>();
            if (_rb == null)
                return;
            _momentum = inPortal.InverseTransformDirection(_rb.velocity);
            _momentum = halfTurn * _momentum;
            _rb.velocity = outPortal.TransformDirection(_momentum);
        }
    }
}
