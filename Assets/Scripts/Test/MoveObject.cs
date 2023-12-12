using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private bool start;
    [SerializeField] private float force;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(start)
        {
            rb.AddForce(transform.right * -force);
        }
        //Debug.Log($"id:{gameObject.GetInstanceID()},origin: {transform.rotation.eulerAngles}");
    }
}
