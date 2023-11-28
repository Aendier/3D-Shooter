using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum FireMode { Auto, Burst, Single };
    public FireMode fireMode;
    //枪口位置
    public Transform[] projectileSpwan;
    //子弹
    public Projectile projectile;
    //射击间隔时间
    public float msBetweenShots = 100;
    //子弹速度
    public float muzzleVelocity = 35;
    public int burstCount;

    //下一次射击时间
    float newShotTime;

    public Transform shell;
    public Transform shellEjection;

    private MuzzleFlash muzzleFlash;

    private bool triggerReleasedSinceLastShot;
    private int shotsRemainingInBurst;
    private void Start()
    {
        muzzleFlash = GetComponent<MuzzleFlash>();
        shotsRemainingInBurst = burstCount;
    }
    
    private void Shoot()
    {
        if(Time.time > newShotTime)
        {
            if(fireMode == FireMode.Burst)
            {
                if(shotsRemainingInBurst == 0)
                {
                    return;
                }
                shotsRemainingInBurst--;
            }
            else if(fireMode == FireMode.Single)
            {
                if(!triggerReleasedSinceLastShot)
                {
                    return;
                }
            }
            for (int i = 0; i < projectileSpwan.Length; i++)
            {
                //新的射击时间等于当前时间加上射击间隔时间
                newShotTime = Time.time + msBetweenShots / 1000;
                //生成子弹
                Projectile newProjectile = Instantiate(projectile, projectileSpwan[i].position, projectileSpwan[i].rotation);
                //设置子弹的速度
                newProjectile.SetSpeed(muzzleVelocity);
            }
            Instantiate(shell, shellEjection.position, shellEjection.rotation);
            muzzleFlash.Activate();

        }
    }

    public void OnTriggerHold()
    {
        Shoot();
        triggerReleasedSinceLastShot = false;
    }

    public void OnTriggerRelease()
    {
        triggerReleasedSinceLastShot = true;
        shotsRemainingInBurst = burstCount;
    }
}
