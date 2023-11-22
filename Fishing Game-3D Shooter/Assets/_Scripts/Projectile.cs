using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    //子弹速度
    private float speed = 10;
    //子弹的碰撞层
    public LayerMask collisionMask;
    //子弹的伤害
    public float damage = 1;

    //子弹的存活时间
    private float lifttIme = 3;
    //子弹的皮肤宽度
    private float skinWidth = 0.1f;

    private void Start()
    {
        //设置子弹的存活时间
        Destroy(gameObject, lifttIme);

        //获取子弹刚生成时接触到的所有collisionMask层下的碰撞体
        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, 0.1f, collisionMask);
        //当碰撞体数量大于0时，说明接触到了敌人，调用OnHitObject函数
        if(initialCollisions.Length > 0)
        {
            //对第一个碰撞体调用OnHitObject函数
            OnHitObject(initialCollisions[0]);
        }
    }
    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    void Update()
    {
        //子弹每帧移动的距离
        float moveDistance = speed * Time.deltaTime;
        //检测碰撞
        CheckeCollisions(moveDistance);
        //子弹移动
        transform.Translate(Vector3.forward * moveDistance);
        
    }

    //检测碰撞
    private void CheckeCollisions(float moveDistance)
    {
        //子弹的射线
        Ray ray = new Ray(transform.position, transform.forward);
        //子弹的射线碰撞信息
        RaycastHit hit;
        //如果子弹射线在moveDistance + skinWidth的距离内碰撞到了碰撞层的碰撞体
        if (Physics.Raycast(ray, out hit, moveDistance + skinWidth, collisionMask, QueryTriggerInteraction.Collide)) 
        {
            OnHitObject(hit);
        }
    }

    //当子弹碰撞到碰撞体时调用
    private void OnHitObject(RaycastHit hit)
    {
        //获取碰撞体上的IDamageable组件
        IDamageable damageableObject = hit.collider.GetComponent<IDamageable>();
        //如果碰撞体上有IDamageable组件
        if (damageableObject != null)
        {
            //调用TakeHit函数
            damageableObject.TakeHit(damage,hit);
        }
        //销毁子弹
        Destroy(gameObject);
    }

    //当子弹碰撞到碰撞体时调用
    private void OnHitObject(Collider c)
    {
        //获取碰撞体上的IDamageable组件
        IDamageable damageableObject = c.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            damageableObject.TakeDamage(damage);
        }
        Destroy(gameObject);
    }
}
 