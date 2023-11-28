using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    Rigidbody rb;
    //移动速度
    Vector3 velocity;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    //移动
    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
    }

    private void FixedUpdate()
    {
        //移动
        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
    }

    //看向point点
    public void LookAt(Vector3 point)
    {
        //y轴不变
        Vector3 heightCorrectedPoint = new Vector3(point.x, transform.position.y, point.z);
         transform.LookAt(heightCorrectedPoint);
    }
}
