using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponEnemyScript : WeaponBaseScript
{
    public GameObject projectilePrefab;
    public GameObject projectileObject;
    public override void Awake()
    {
        base.Awake();
    }
    public void SetValues(EnemyStatistics.EnemyEnum enemyType, float newRange, float newDamage)
    {
        weaponStatistics.LoadEnemyValues(enemyType, newRange, newDamage);
        weaponRigidbody.bodyType = RigidbodyType2D.Kinematic;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        spriteTransform.localRotation = Quaternion.Euler(0f, 0f, 80f);

        switch (enemyType)
        {
            default:
            case EnemyStatistics.EnemyEnum.Standard:
                spriteTransform.localScale = Vector3.one;
                particleObject.transform.localScale = Vector3.one * 0.1f;
                particleObject.GetComponent<TrailRenderer>().widthMultiplier = 0.1f;
                break;
            case EnemyStatistics.EnemyEnum.Tank:
                spriteTransform.localScale = new Vector3(1f, 1.5f);
                particleObject.transform.localScale = Vector3.one * 0.5f;
                particleObject.GetComponent<TrailRenderer>().widthMultiplier = 0.5f;
                break;
            case EnemyStatistics.EnemyEnum.Swarm:
                spriteTransform.localScale = new Vector3(0.6f, 1f);
                particleObject.transform.localScale = Vector3.one * 0.1f;
                particleObject.GetComponent<TrailRenderer>().widthMultiplier = 0.1f;
                break;
        }
        
        spriteTransform.localPosition = new Vector3(
            transform.parent.lossyScale.x * 0.6f,
            transform.parent.lossyScale.x * 0.2f);
            
        if (enemyType == EnemyStatistics.EnemyEnum.Sniper || enemyType == EnemyStatistics.EnemyEnum.Turret)
        {
            particleObject.SetActive(false);
            spriteTransform.gameObject.GetComponent<SpriteRenderer>().sprite = spriteTextureArray[1];
            spriteTransform.localRotation = Quaternion.Euler(0f, 0f, 90f);
            spriteTransform.localPosition = new Vector3(
                transform.parent.lossyScale.x * 0f,
                transform.parent.lossyScale.x * 0.65f);
        }
    }

    public override void FixedUpdate()
    {
        if (gameObject.GetComponentInParent<EnemyScript>().IsRanged())
        {
            // Bow animation
            if (attackTimer != 0f)
            {
                attackTimer = Mathf.Clamp(attackTimer -= (Time.fixedDeltaTime * 0.8f), 0f, weaponStatistics.GetCooldown());
                //attackTimer = Mathf.Clamp(attackTimer -= (Time.fixedDeltaTime * 0.3f), 0f, weaponStatistics.GetCooldown());
            }
        }
        else
        {
            base.FixedUpdate();
        }
    }

    public override void Attack()
    {
        if (gameObject.GetComponentInParent<EnemyScript>().IsRanged())
        {
            if (attackTimer == 0f)
            {
                // Spawn projectile
                // isAttacking = true;
                attackPercentage = 0f;
                attackTimer = weaponStatistics.GetCooldown();

                projectileObject = Instantiate(projectilePrefab, transform.position, Quaternion.identity, gameObject.GetComponentInParent<RoomScript>().transform);
                projectileObject.GetComponentInParent<ProjectileScript>().SetSpawningWeapon(this);
            }
        }
        else
        {
            base.Attack();
        }
    }

    public bool HideWeapon()
    {
        if (attackTimer == 0f && spriteTransform.gameObject.activeSelf)
        {
            spriteTransform.gameObject.SetActive(false);
            return true;
        }
        return false;
    }
}
