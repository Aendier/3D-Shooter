﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{
    //状态
    private enum State { Idle, Chasing, Attacking };
    private State currentState;

    //寻路AI组件
    private NavMeshAgent pathFinder;
    //目标Transform组件
    private Transform target;

    //皮肤材质
    private Material skinMaterial;
    //皮肤原始颜色
    private Color originalColour;

    //攻击距离
    private float attackDistanceThreshold = 0.5f;
    //攻击间隔
    private float timeBetweenAttacks = 1;
    //攻击伤害
    public float damage;

    //下次攻击时间
    private float nextAttackTime;
    //自身半径
    private float myCollsionRadius;
    //目标半径
    private float targetCollisionRadius;

    //目标实体
    private LivingEntity targetEntity;

    //是否有目标
    private bool hasTarget;
    protected override void Start()
    {
        base.Start();
        pathFinder = GetComponent<NavMeshAgent>();
        skinMaterial = GetComponent<Renderer>().material;
        originalColour = skinMaterial.color;

        //如果有目标
        if(GameObject.FindGameObjectWithTag("Player") != null)
        {
            currentState = State.Chasing;
            hasTarget = true;
            target = GameObject.FindGameObjectWithTag("Player").transform;

            myCollsionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;
            //开始更新路径
            StartCoroutine(UpdatePath());

            targetEntity = target.GetComponent<LivingEntity>();
            targetEntity.OnDeath += OnTargetDeath;
        }
    }

    void Update()
    {
        //如果有目标
        if (hasTarget)
        {
            //如果距离小于攻击距离
            if (Time.time > nextAttackTime)
            {
                //自身与目标的距离
                float distanceToTarget = (target.position - transform.position).sqrMagnitude;
                //如果距离小于攻击距离
                if (distanceToTarget < Mathf.Pow(attackDistanceThreshold + myCollsionRadius + targetCollisionRadius, 2))
                {
                    nextAttackTime = Time.time + timeBetweenAttacks;
                    StartCoroutine(Attack());
                }
            }
        }
    }

    //当目标死亡时调用
    private void OnTargetDeath()
    {
        hasTarget = false;
        currentState = State.Idle;
    }

    //攻击
    IEnumerator Attack()
    {
        //攻击状态
        currentState = State.Attacking;
        //停止寻路
        pathFinder.enabled = false;

        //攻击前的位置
        Vector3 originalPosition = transform.position;
        //攻击目标的方向
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        //攻击目标的位置 = 目标位置 - 目标方向 * 自身半径
        Vector3 attackPosition = target.position - dirToTarget * myCollsionRadius;  

        //攻击速度
        float attackSpeed = 3;
        //攻击百分比
        float percent = 0;

        //攻击时的颜色 = 红色
        skinMaterial.color = Color.red;
        //是否已经造成伤害
        bool hasAppliedDamage = false;

        //当攻击百分比小于等于1
        while (percent <= 1)
        {
            //如果攻击百分比大于等于0.5且还没有造成伤害
            if(percent >= 0.5f && !hasAppliedDamage)
            {
                //造成伤害
                Debug.Log("攻击");
                hasAppliedDamage = true;
                targetEntity.TakeDamage(damage);
            }

            //攻击百分比增加
            percent += Time.deltaTime * attackSpeed;

            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);

            yield return null;
        }
        skinMaterial.color = originalColour;
        currentState = State.Chasing;
        pathFinder.enabled = true;
    }

    //更新路径
    IEnumerator UpdatePath()
    {
        //当有目标时
        while (hasTarget)
        {
            //刷新路径的时间间隔
            float refreshRate = 0.25f;
            //当目标不为空
            while (target != null)
            {
                //如果当前状态是追逐
                if (currentState == State.Chasing)
                {
                    //目标方向 = (目标位置 - 自身位置).归一化
                    Vector3 dirToTarget = (target.position - transform.position).normalized;
                    //目标位置 = 目标位置 - 目标方向 * (自身半径 + 目标半径 + 攻击距离 / 2)
                    Vector3 targetPosition = target.position - dirToTarget * (myCollsionRadius + targetCollisionRadius + attackDistanceThreshold / 2);
                    //如果没有死亡
                    if (!dead)
                        //设置目标位置
                        pathFinder.SetDestination(targetPosition);
                }
                yield return new WaitForSeconds(refreshRate);
            }
        }
    }
}
