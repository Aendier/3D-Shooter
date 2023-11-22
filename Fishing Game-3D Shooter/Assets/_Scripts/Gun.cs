using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    //枪口位置
    public Transform muzzle;
    //子弹
    public Projectile projectile;
    //射击间隔时间
    public float msBetweenShots = 100;
    //子弹速度
    public float muzzleVelocity = 35;

    //下一次射击时间
    float newShotTime;
    public void Shoot()
    {
        if(Time.time > newShotTime)
        {
            //新的射击时间等于当前时间加上射击间隔时间
            newShotTime = Time.time + msBetweenShots / 1000;
            //生成子弹
            Projectile newProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation);
            //设置子弹的速度
            newProjectile.SetSpeed(muzzleVelocity);
        }
    }
}
