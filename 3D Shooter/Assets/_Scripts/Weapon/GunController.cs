using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    //武器的位置
    public Transform weaponHold;
    //开始的武器
    public Gun startingGun;
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
        //如果开始的武器不为空
        if(startingGun != null)
        {
            //装备设置好的开始武器
            EquipGun(startingGun);
        }
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

    //射击
    public void OnTriggerHold() 
    {         
        //如果当前武器不为空
        if(equippedGun != null)
        {
            //调用武器的射击函数
            equippedGun.OnTriggerHold();
        }
    }    //射击
    public void OnTriggerRelease() 
    {         
        //如果当前武器不为空
        if(equippedGun != null)
        {
            //调用武器的射击函数
            equippedGun.OnTriggerRelease();
        }
    }


}
