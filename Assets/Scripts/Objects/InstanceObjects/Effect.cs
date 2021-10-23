﻿using Assets.Scripts.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Effect : MonoBehaviour, IInstanceObject
{
    public int EffectMaxInstances;
    public int MaxInstancesAlive { get => EffectMaxInstances; set => EffectMaxInstances = value; }
    public int Strength { get => 0; set => new int(); }
    public float EffectSpawnRate;
    int _ownerID;
    IInstanceObject _prefab;
    private Vector3 _positionOrigin;
    private Quaternion _rotationOrigin;
    private bool _resetTriggered;

    public float SpawnRate { get => EffectSpawnRate; set => EffectSpawnRate = value; }

    public GameObject GameObject => gameObject;

    public IInstanceObject Prefab => _prefab;

    public int OwnerID => _ownerID;

    public float ResetDelay = 1;

    public void Death()
    {
        if (Prefab != null)
        {
            GameEvents.Instance.InstanceManager_OnDeath(new DeathEventArgs(this));
        }
    }

    public void DeSpawn()
    {
        if (Prefab != null)
        {
            gameObject.SetActive(false);
        }
    }

    public IInstanceObject SetInstance(int ownerID)
    {
        _ownerID = ownerID;
        var inst = Instantiate(gameObject, transform.position, Quaternion.identity).GetComponent<Effect>();
        inst._prefab = this;
        inst._ownerID = ownerID;
        return inst;
    }
    public void ResetEffect()
    {
        _resetTriggered = false;
        StopAllCoroutines();
        transform.position = _positionOrigin;
        transform.rotation = _rotationOrigin;
    }

    IEnumerator TriggerResetDelay()
    {
        yield return new WaitForSeconds(ResetDelay);
        DeSpawn();
    }

    public void TriggerReset()
    {
        if (_resetTriggered == false)
        {
            _resetTriggered = true;
            StartCoroutine(TriggerResetDelay());
        }
    }

    public void Spawn(Vector3 position, Vector3 dir)
    {
        gameObject.SetActive(true);
        _positionOrigin = position;
        _rotationOrigin = Quaternion.identity;
        ResetEffect();
        TriggerReset();
    }
}
