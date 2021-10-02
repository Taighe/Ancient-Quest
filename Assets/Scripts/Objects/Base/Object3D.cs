﻿using Assets.Scripts.Events;
using UnityEditor;
using UnityEngine;
using System;
using System.Collections;

public class Object3D : MonoBehaviour
{
    public float DamageDelay = 0.5f;
    public bool IsInvulnerableDuringDelay = true;
    public bool FlashDuringDelay 
    {
        get
        {
            return FlashRate > 0;
        }
    }
    public float FlashRate = 0.2f;
    public bool IsInvulnerable
    {
        get
        {
            return IsInvulnerableDuringDelay && !DamageFinished;
        }
    }
    private bool _isFlashing;
    public bool DamageFinished
    {
        get
        {
            return _time == -1 || _time >= DamageDelay;
        }
    }

    private float _time = -1;

    public virtual void Awake()
    {

    }

    protected virtual void OnDamaged(int damage)
    {

    }

    protected void Instance_Damaged(object sender, DamagedEventArgs e)
    {        
        if (DamageFinished)
        {
            OnDamaged(e.Damage);
            if(DamageDelay != 0)
            {
                _time = 0;
                StartCoroutine(DamagedCoroutine());
            }
        }
    }

    private IEnumerator DamagedCoroutine()
    {
        if (FlashDuringDelay && !_isFlashing)
        {
            StartCoroutine(FlashCoroutine());
        }

        while (_time < DamageDelay)
        {
            yield return new WaitForEndOfFrame();
            _time += 1.0f * Time.deltaTime;
        }

        _isFlashing = false;
    }

    private IEnumerator FlashCoroutine()
    {
        _isFlashing = true;
        bool visible = true;
        while(_isFlashing)
        {
            Flash(!visible);
            yield return new WaitForSeconds(FlashRate);
        }
    }

    protected virtual void Flash(bool visible)
    {

    }

    // Start is called before the first frame update
    public virtual void Start()
    {

    }
}