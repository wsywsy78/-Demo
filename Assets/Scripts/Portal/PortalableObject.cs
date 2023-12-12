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
/// ���н��봫������ײ���������
/// </summary>
[Serializable]
public class PortalableObject
{
    /// <summary>
    /// ԭ����
    /// </summary>
    [SerializeField] private GameObject originalObject;
    /// <summary>
    /// ��¡������
    /// </summary>
    public GameObject cloneObject { get; private set; }
    /// <summary>
    /// ����¡����Ľ��봫����
    /// </summary>
    public Transform inPortal { get; private set; }
    /// <summary>
    /// ����¡����ĳ�������
    /// </summary>
    public Transform outPortal { get; private set; }
    /// <summary>
    /// ����¡�����Transform������
    /// </summary>
    private Transform originalTransform;
    /// <summary>
    /// ����¡���������
    /// </summary>
    private ObjectType objectType;
    /// <summary>
    /// ������¡��������ҵĻ�����Ҫ�����������Transform
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
        //������¡���������ʱ����ȡ�Ĳ��������Transform���������Transform��Ϊ�˸��õĴ�������
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
    /// ��¡һ����������ײ���ڵ�����
    /// </summary>
    /// <param name="_cloneObject"></param>
    private void CloneObject(GameObject _cloneObject)
    {
        //�����¡�Ķ�������ң�������ҵ�ģ���õ���SkinnedMeshRenderer���޷��򵥸���
        //ʵ��������Ӷ����й�����Animator�Ķ��󣨱��⽫���п��ƽű��Ķ����ƣ��������������ҵ�ģ�ͣ���Ϊֻ��ģ�ͣ�����û�ж���...
        if (_cloneObject.GetComponent<Player>() != null)
        {
            //���̳���MonoBehaviour,Ҫ����ʵ����
            this.cloneObject = UnityEngine.Object.Instantiate(_cloneObject.GetComponentInChildren<Animator>().gameObject);
            //���㼶��Ϊdefault����Ϊ��ҵ���������޳�ԭ�������ģ�����ڵĲ㼶
            foreach(Transform child in this.cloneObject.GetComponentsInChildren<Transform>())
            {
                child.gameObject.layer = 0;
            }
            this.cloneObject.name = _cloneObject.name;
            return;
        }

        //ֻ��¡�����������Mesh��Ϣ����������
        //this.cloneObject = new GameObject();
        //var _meshFilter = this.cloneObject.AddComponent<MeshFilter>();
        //var _meshRender = this.cloneObject.AddComponent<MeshRenderer>();
        //_meshFilter.mesh = _cloneObject.GetComponent<MeshFilter>()?.mesh;
        //_meshRender.materials = _cloneObject.GetComponent<MeshRenderer>()?.materials;

        //ע���ڲ���ʱ���֣�����¡����������ͬ�����и������ײ���ʱ�򣬻�ͬʱ������һ�������ŵĿ�¡���п��ܻ��γ�һ�����޿�¡
        //������������Ҫȡ����¡����ʹ�������ײ��֮�����ײ
        this.cloneObject = UnityEngine.Object.Instantiate(_cloneObject);
        this.cloneObject.transform.localScale = _cloneObject.transform.lossyScale;
        this.cloneObject.name = _cloneObject.name;
        var _collider = this.cloneObject.GetComponent<Collider>();
        _collider.enabled = false;
    }

    /// <summary>
    /// �ƶ���¡����
    /// </summary>
    /// <param name="_cloneObject"></param>
    private void MoveCloneObject()
    {
        var _transform = objectType == ObjectType.player ? playerBody : originalObject.transform;

        //ͬPortalCamera������¡����ת������һ�������ŵĺ���
        Quaternion _relativeRot = Quaternion.Inverse(inPortal.rotation) * originalObject.transform.rotation;
        _relativeRot = halfTurn * _relativeRot;
        cloneObject.transform.rotation = outPortal.rotation * _relativeRot;

        Vector3 _relativePos = inPortal.InverseTransformPoint(_transform.position);
        _relativePos = halfTurn * _relativePos;
        cloneObject.transform.position = outPortal.TransformPoint(_relativePos);

    }

    /// <summary>
    /// ���ԭ�����Ƿ񴩹������ţ��Խ����崫������һ��������
    /// </summary>
    private void MoveOriginalObject()
    {
        //�����¡�����Ǵ���ǹ�����أ���Ϊ��ҵ���ת�ʹ����Ѿ��Ὣ����ǹһ�����ͣ�
        if (originalObject.CompareTag("PortalGun"))
            return;
        //���������Դ����ŵ�λ�ã�������z��ľ�������㣨�����ŵ�z���ڷ��棩�������崫��
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

            //����Ƿ�Ҫ���������������и���
            IsChangePickObject();           
        }
    }

    /// <summary>
    /// ��������λ��
    /// </summary>
    private void ConvertPosition()
    {

        //��β��Է��֣��������Ϲ�����CharacterController������ʱ������Move��ͬʱ�������崫�͵���һ�������ź����ϻ������
        //�Ʋ���Unity������������Ϊ�йأ�Ϊ������ȷ���ͣ�ֻ����ʱ������Щ���
        //��ʱû�и��õķ���
        if (objectType == ObjectType.player)
        {
            //�������Ǵ���������ֱ��ˮƽ��Ĵ����Ž���ʱ��ֻ��Ҫ�򵥵ļ������λ�ü���
            //��һ���Ĵ����Ų��Ǵ�ֱ��ˮƽ��ʱ���Ͳ��ܼ򵥼������λ�ã���Ϊ�����������תʹ������ʼ�ն�����ֱ�ģ���������������������
            //�򵥵ļ������λ�ÿ��ܻᵼ�¸��ָ������ӽǴ�ģ��
            //��������Ĵ�����ͨ����������λ�úʹ����ŵľ����ϵ�����ģ���Ϊ�����������ת�����ܻᵼ�����У�ʹ�������ٴδ��ͻ�ȥ
            //����ͨ����ͬ�Ĵ����ų��ڶ������λ�ý���һ����ƫ�ƣ������ֲ���̫���ƻ����͵�������
            //���ԣ�������뷨��ͨ��������������λ�÷��������������λ��ƫ�ƣ������ܹ����̶ȵı��ִ��͵������ԣ������ܹ����������ᵽ������
            Vector3 _relativePos = inPortal.InverseTransformPoint(originalTransform.position);
            _relativePos = halfTurn * _relativePos;
            _relativePos = outPortal.TransformPoint(_relativePos);
            //�������ҵ����λ��
            Vector3 _cameraToPlayer = originalTransform.localPosition;
            _relativePos = _relativePos - _cameraToPlayer;

            //���Է��֣�������CharacterControllerʱ��������ײ��ʧЧ���ᵼ�´����ŵ�OntriggerExit��������ʹ�ÿ�¡�屣������ʹ��Moveֱ�ӽ������ƶ�
            //var _player = originalObject.GetComponent<CharacterController>();
            //_player.enableOverlapRecovery = false;
            //_player.Move(_cameraVec);
            //_player.enableOverlapRecovery = true;

            //��β��Է��֣���ʹ��CharacterController��Move�����ƶ�����ʱ������Move����������Ӱ��
            //��������ײ��ʱ��ͣ�����������˶���������ײ�ķ������������⣬�ʻ���ʹ��һ��ʼ�ķ���
            //�ڴ��ͺ����������뿪�����ŵ��¼��������������¡��
            var _character = originalObject.GetComponent<CharacterController>();
            //����Portal��OntriggerEnter�������ڴ��ͣ��ڳ�������ƽ������һ����ص�ʱ���п��ܽ���Ҽ���
            //���Դ˴�Ҫ�ֶ�ȡ���ͳ�����ƽ����������ײ�����ײ���ڴ�����ɺ��������
            Physics.IgnoreCollision(_character, outPortal.GetComponent<Portal>().attachCollider);
            _character.enabled = false;
            originalObject.transform.position = _relativePos;
            _character.enabled = true;

            this.TriggerEvent(EventName.GetOutPortal, new EventParameter { eventGetOutPortal = originalObject.GetComponent<Collider>().GetInstanceID() });
            //Debug.Log($"afterportalVec:{_character.velocity}");

            AddVelocity();
            Physics.IgnoreCollision(_character, outPortal.GetComponent<Portal>().attachCollider, false);
        }
        //�����͵����岻������ʱ��ͨ������ĸ�������ƶ����壬����������󡣣���ʱû�п����񼤹�һ���Ŀ��ܲ����и�������壩
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
    /// ����������ת����
    /// </summary>
    private void ConvertRotation()
    {
        //���������˵��ֻ��Ҫ��������y�����ת����Ϊ������β���������ᡱ�����Ӷ������������ͷ���ص����
        //������ҵ��������˵��Ҫͬʱ����x��z����ת
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
        //����������˵���Ͱ�����ת����������ķ�������ת������
        else
        {
            //Debug.Log($"{originalObject.GetInstanceID()} rot");
            Quaternion _relativeRot = Quaternion.Inverse(inPortal.rotation) * originalObject.transform.rotation;
            _relativeRot = halfTurn * _relativeRot;
            originalObject.transform.rotation = outPortal.rotation * _relativeRot;
        }
    }
    
    /// <summary>
    /// �������������ת���������ת�Ὣ�����һ����ת��Ϊ�˱�֤���͵������ԣ����뵥�������������ת���е�����
    /// </summary>
    private void AdjustCamera()
    {
        //ֻ������Ҵ��͵�ʱ�������ת�����
        if(objectType == ObjectType.player)
        {
            ////��������ͽ�������z��ļн�
            //float _inRot = Vector3.Angle(inPortal.forward, originalTransform.forward);
            ////�ټ���������ŵ�z���������y��ļн�
            //float _outRot = Vector3.Angle(outPortal.forward, Vector3.up);
            ////Debug.Log($"{_temRot}");
            ////���㴫�ͺ��������x�н�
            //float _xRot = _outRot + originalTransform.localRotation.eulerAngles.x - 90;
            //Debug.Log($"portal:{_temRot}, resulr:{_xRot}");

            //���ｫ���������ת����ڴ����ŷ�ת
            Quaternion _relativeRot = Quaternion.Inverse(inPortal.rotation) * originalTransform.rotation;
            _relativeRot = halfTurn* _relativeRot;
            _relativeRot = outPortal.rotation * _relativeRot;
            Debug.Log($"{_relativeRot.eulerAngles}");
            //ֻ��Ҫ��תx�ᣬ��Ϊ�����y����ת������ҵ���ת���п��Ƶ�
            originalTransform.localRotation = Quaternion.Euler(_relativeRot.eulerAngles.x, 0, 0);
            Debug.Log($"Camera: {originalTransform.rotation.eulerAngles}");
            //originalTransform.localRotation = Quaternion.Euler(_xRot, 0, 0);
            this.TriggerEvent(EventName.ChangeCameraMode);
        }
    }

    /// <summary>
    /// �ж��Ƿ�Ҫ�����������������ʽ���и���
    /// </summary>
    private void IsChangePickObject()
    {
        //������������������壬��ָ�����
        if(PickObjectController.Instance.pickedObject == originalObject)
        {
            //����������Ѿ�������һ�Σ���ô���˳�����ǰ������ζ��޷����ĵڶ��Σ����Խ���ָ�
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
            //������ڴ��������ŵ�ʱ�������ʱ�������Ѿ�����ģʽ�Ѿ����ģ���Ӧ��Ҫ���Ļ���
            if (PickObjectController.Instance.pickedObject != null && PickObjectController.Instance.isInPortal)
            {
                this.TriggerEvent(EventName.PickGetInfo, new EventParameter { eventTransforms = null });
                //���Է��֣���ʱ�����صĻ������ϴ�����һ������
                return;
            }
            //��֮������ʱδ���ģ���Ҫ���и���
            else if (PickObjectController.Instance.pickedObject != null && !PickObjectController.Instance.isInPortal)
            {
                this.TriggerEvent(EventName.PickGetInfo, new EventParameter
                { eventTransforms = new Transform[2] { outPortal, inPortal } });
            }
        }
    }

    /// <summary>
    /// �ڴ���ʱΪ��������һ�����ٶ�
    /// </summary>
    private void AddVelocity()
    {
        //�ڴ����ŵ�forward�ǳ���xozƽ�����ϵ�ʱ��Ϊ����˲�����������һ�����ٶȣ����ƴ������飨��ʱ���ڴ��ͺ������������Ὣ������������
        var _player = originalObject.GetComponent<CharacterController>();
        //����������ŵ�������y�᷽���ϵ�ͶӰ
        Vector3 _projectVec = Vector3.Project(-outPortal.forward, Vector3.up);
        //ֻ���ڴ������ǳ��ϵ�ʱ�������ٶȳ�ʼֵ
        if (_projectVec.y <= 0)
            return;
        //����ͶӰ��ģ��(���ģ����Ӧ�������ٶȵ�ǿ�ȴ�С)
        float _upMagnitude = _projectVec.magnitude;
        //�����ٶȵ�ʸ��
        Vector3 _addVelocity = -outPortal.forward * _upMagnitude;
        //������ڳ������ŵ�forward�����Ͼ���һ�����ٶȣ�����ٶ���Ҫ������������ܹ�˳��ͨ��һЩ���мнǵĳ�������
        _player.Move(_addVelocity * 3);
        //Debug.Log($"trigger, velocity: {_addVelocity}");
    }

    /// <summary>
    /// �̳д���ǰ��Ķ���
    /// </summary>
    private void KeepMomentum()
    {
        Vector3 _momentum;
        if(objectType == ObjectType.player)
        {
            //_momentum = originalObject.GetComponent<CharacterController>().velocity;
            _momentum = GetPhysicsProperty.prevFrameApplyVec + GetPhysicsProperty.prevFrameMaintainVec;
            //Debug.Log($"ԭʼ�ٶȣ�{_momentum}");
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
