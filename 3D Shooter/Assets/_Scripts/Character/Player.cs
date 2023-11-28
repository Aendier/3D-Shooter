using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : LivingEntity
{
    //移动速度
    public float moveSpeed = 5;
    //玩家控制器
    PlayerController controller;
    //视图相机
    Camera viewCamera;
    //枪械控制器
    GunController gunController;
    public Crosshairs crossHair;
    protected override void Start()
    {
        base.Start();
        controller = GetComponent<PlayerController>();
        gunController = GetComponent<GunController>();
        viewCamera = Camera.main;
    }

    void Update()
    {
        //移动
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 moveVelocity = moveInput.normalized * moveSpeed;
        controller.Move(moveVelocity);
        
        //旋转
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.up * gunController.GunHeight);
        float rayDistance;

        if(groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            Debug.DrawLine(ray.origin, point, Color.red);
            controller.LookAt(point);
            //如果操作系统是windows，才会开启准星移动
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                crossHair.gameObject.SetActive(true);
                crossHair.transform.position = point;
                crossHair.DetectTargets(ray);
            }
        }

        //射击
        if (Input.GetMouseButton(0))
        {
            gunController.OnTriggerHold();
        }        
        if (Input.GetMouseButtonUp(0))
        {
            gunController.OnTriggerRelease();
        }
    }
}
