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

    [Header("Gun Info")]
    [Tooltip("射击间隔")]
    public float msBetweenShots = 100;    
    [Tooltip("子弹速度")]
    public float muzzleVelocity = 35;
    [Tooltip("爆发数量")]
    public int burstCount;
    [Tooltip("弹匣数量")]
    public int projectilesPerMag;
    [Tooltip("装填速度")]
    public float reloadTime = 0.5f;


    [Header("Recoil")]
    public Vector2 kickMinMax = new Vector2(0.05f, 0.2f);
    public Vector2 recoilAngleMinMax = new Vector2(3, 5);
    public float recoilMoveSettleTime = 0.1f;
    public float recoilRotationSettleTime = 0.1f;

    [Header("Effects")]
    public Transform shell;
    public Transform shellEjection;
    public AudioClip shootAudio;
    public AudioClip reloadAudio;

    private MuzzleFlash muzzleFlash;
    //下一次射击时间
    private float newShotTime;
    private bool triggerReleasedSinceLastShot;
    private int shotsRemainingInBurst;

    private int projectilesRemainingInMag;
    private bool isReloading;

    private Vector3 recoilSmoothDampVelocity;
    private float recoilRotationSmoothDampVelocity;
    private float recoilAngle;
    private void Start()
    {
        muzzleFlash = GetComponent<MuzzleFlash>();
        shotsRemainingInBurst = burstCount;
        projectilesRemainingInMag = projectilesPerMag;
    }

    private void Update()
    {
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref recoilSmoothDampVelocity, recoilMoveSettleTime);
        recoilAngle = Mathf.SmoothDamp(recoilAngle, 0, ref recoilRotationSmoothDampVelocity, recoilRotationSettleTime);
        transform.localEulerAngles = Vector3.left * recoilAngle;
        if(!isReloading && projectilesRemainingInMag == 0)
        {
            Reload();
        }
    }
    private void Shoot()
    {
        
        if (!isReloading && Time.time > newShotTime && projectilesRemainingInMag > 0)
        {
            Debug.Log("Shoot");
            if (fireMode == FireMode.Burst)
            {
                if (shotsRemainingInBurst == 0)
                {
                    return;
                }
                shotsRemainingInBurst--;
            }
            else if (fireMode == FireMode.Single)
            {
                if (!triggerReleasedSinceLastShot)
                {
                    return;
                }
            }

            for (int i = 0; i < projectileSpwan.Length; i++)
            {
                if (projectilesRemainingInMag == 0)
                {
                    break;
                }
                projectilesRemainingInMag--;
                //新的射击时间等于当前时间加上射击间隔时间
                newShotTime = Time.time + msBetweenShots / 1000;
                //生成子弹
                Projectile newProjectile = Instantiate(projectile, projectileSpwan[i].position, projectileSpwan[i].rotation);
                //设置子弹的速度
                newProjectile.SetSpeed(muzzleVelocity);
            }
            Instantiate(shell, shellEjection.position, shellEjection.rotation);
            muzzleFlash.Activate();

            transform.localPosition -= Vector3.forward * Random.Range(kickMinMax.x, kickMinMax.y);
            recoilAngle += Random.Range(recoilAngleMinMax.x, recoilAngleMinMax.y);
            recoilAngle = Mathf.Clamp(recoilAngle, 0, 30);
            AudioManager.instance.PlaySound(shootAudio, transform.position);
        }
    }

    public void Reload()
    {
        if (!isReloading && projectilesRemainingInMag != projectilesPerMag)
        {
            AudioManager.instance.PlaySound(reloadAudio, transform.position);
            StartCoroutine(AnimateReload());
        }
    }

    private IEnumerator AnimateReload()
    {
        isReloading = true;
        yield return new WaitForSeconds(0.2f);
        Debug.Log("Reloading");
        float reloadSpeed = 1f / reloadTime;
        float percent = 0;
        Vector3 initialRot = transform.localEulerAngles;
        float maxReloadAngle = 30;
        while (percent < 1)
        {
            percent += Time.deltaTime * reloadSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            float reloadAngle = Mathf.Lerp(0, maxReloadAngle, interpolation);
            transform.localEulerAngles = initialRot + Vector3.left * reloadAngle;
            yield return null; 
        }
        isReloading = false;
        projectilesRemainingInMag = projectilesPerMag;      
    }

    public void Aim(Vector3 aimPoint)
    {
        if(!isReloading)
        {
            transform.LookAt(aimPoint);
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
