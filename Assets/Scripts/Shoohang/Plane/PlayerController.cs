using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : BasicPlane
{
    [SerializeField] private Text speedText;
    [SerializeField] private Text ultimateText;
    
    [SerializeField] private Transform[] firePositions;

    private float _rotXAmount = 10.0f;
    private float _rotZAmount = 7.5f;

    private float _currentFireTime = 0.0f;
    private float _fireDelay = 0.25f;

    private int _attackLevel = 1;
    private int _speedLevel = 0;
    private float _speedUpValue => moveSpeed * 0.1f;

    private bool _isReadyUltimate = true;

    private void Update()
    {
        RotXValue = Input.GetAxis("Mouse Y") * _rotXAmount;
        RotZValue = Input.GetAxis("Mouse X") * _rotZAmount;

        speedText.text = $"{moveSpeed} km/p";
        ultimateText.text = $"궁극기 {(_isReadyUltimate ? "준비됨" : "준비 중")}";

        Fire();
    }

    private void Fire()
    {
        _currentFireTime += Time.deltaTime;

        if (_currentFireTime < _fireDelay)
        {
            return;
        }
        
        if (Input.GetButton("Fire1"))
        {
            for (int i = 0; i < _attackLevel; i++)
            {
                GameObject bullet = ObjectPoolManager.Instance.GetBullet();

                bullet.transform.position = firePositions[i].position;
                bullet.transform.rotation = firePositions[i].rotation;
                
                bullet.SetActive(true);
            }

            _currentFireTime = 0;
        }

        if (_isReadyUltimate && Input.GetButtonDown("Fire2"))
        {
            UseUltimate();
        }
    }

    public void GetItem(Item.ItemType itemType)
    {
        switch (itemType)
        {
            case Item.ItemType.Attack :
                GetAttackItem();
                break;
            
            case Item.ItemType.Speed :
                GetSpeedItem();
                break;
            
            case Item.ItemType.Ultimate :
                GetUltimateItem();
                break;
        }
    }

    private void GetAttackItem()
    {
        if (_attackLevel < 7)
        {
            _attackLevel += 2;
        }
    }
    
    private void GetSpeedItem()
    {
        if (_speedLevel++ < 3)
        {
            moveSpeed += _speedUpValue;
            Debug.Log(moveSpeed);
        }
    }
    
    private void GetUltimateItem()
    {
        _isReadyUltimate = true;
    }
    
    private void UseUltimate()
    {
        _isReadyUltimate = false;

        var damagables = FindObjectsOfType<MonoBehaviour>().OfType<IDamagable>();

        foreach (var damagable in damagables)
        {
            damagable.DamageAction();
        }
    }
    
    private void OnCollisionEnter(Collision other)
    {
        ObjectPoolManager.Instance.SpawnExplosion(transform.position);
        
        IDamagable damagable = other.gameObject.GetComponent<IDamagable>();

        if (damagable is not null)
        {
            damagable.DamageAction();
        }
        
        gameObject.SetActive(false);
    }
}
