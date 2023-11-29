using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    //武器的位置
    public Transform weaponHold;
    //武器库
    public Gun[] allGuns;
    //当前武器
    Gun equippedGun;

    public float GunHeight
    {
        get
        {
            return weaponHold.position.y;
        }
    }

    void Start()
    {

    }

    //装备武器
    public void EquipGun(Gun gunToEquip)
    {
        //如果当前武器不为空
        if(equippedGun != null)
        {
            //销毁当前武器
            Destroy(equippedGun.gameObject);
        }
        //生成武器
        equippedGun = Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation);
        //设置武器的父物体为weaponHold
        equippedGun.transform.parent = weaponHold;
    }    
    public void EquipGun(int gunNumber)
    {
        EquipGun(allGuns[gunNumber]);
    }

    //射击
    public void OnTriggerHold() 
    {         
        //如果当前武器不为空
        if(equippedGun != null)
        {
            //调用武器的按下扳机函数
            equippedGun.OnTriggerHold();
        }
    }    //射击
    public void OnTriggerRelease() 
    {         
        //如果当前武器不为空
        if(equippedGun != null)
        {
            //调用武器的松开扳机函数
            equippedGun.OnTriggerRelease();
        }
    }

    public void Aim(Vector3 aimPoint)
    {
        //如果当前武器不为空
        if(equippedGun != null)
        {
            //调用武器的瞄准函数
            equippedGun.Aim(aimPoint);
        }
    }

    public void Reload()
    {
        if (equippedGun != null)
        {
            equippedGun.Reload();
        }
    }

}
