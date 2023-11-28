using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable
{
    //开始生命值
    public float startingHealth;
    //生命值
    protected float health;
    //是否死亡
    protected bool dead = false;

    //死亡事件
    public event Action OnDeath;


    protected virtual void Start()
    {
        //初始化生命值
        health = startingHealth;
    }

    //受到攻击
    public virtual void TakeHit(float damage, Vector3 hitPoint,Vector3 hitDirection)
    {
        TakeDamage(damage);
    }

    //造成伤害
    public virtual void TakeDamage(float damage)
    {
        //生命值减少
        health -= damage;
        Debug.Log("生命值减少");
        //如果生命值小于等于0并且没有死亡
        Debug.Log(health);
        if (health <= 0 && !dead)
        {
            Debug.Log("Dead");
            //死亡
            Die();
        }
    }

    [ContextMenu("Self Destruct")]
    protected void Die()
    {
        dead = true;
        //判断死亡间是否为空，不为空则调用
        OnDeath?.Invoke();
        Destroy(gameObject);
    }
}
