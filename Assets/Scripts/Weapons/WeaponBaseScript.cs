using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PointsStruct
{
    private int rarity;
    private int mobility;
    public void IncMobility() { mobility++; }
    private int attackSpeed;
    public void IncAttackSpeed() { attackSpeed++; }
    private int range;
    public void IncRange() { range++; }
    private int damage;
    public void IncDamage() { damage++; }
    private int cooldown;
    public void IncCooldown() { cooldown++; }

    public void Init(int newRarity)
    {
        rarity = newRarity;
        mobility = 0;
        range = 0;
        damage = 0;
        attackSpeed = 0;
        cooldown = 0;
    }

    public void IncAtPos(int pos)
    {
        switch (pos)
        {
            case 0:
                mobility++;
                break;
            case 1:
                attackSpeed++;
                break;
            case 2:
                range++;
                break;
            case 3:
                damage++;
                break;
            case 4:
                cooldown++;
                break;
            default:
                break;
        }
    }

    public int GetAtPos(int pos)
    {
        switch (pos)
        {
            case 0:
                return mobility;
            case 1:
                return attackSpeed;
            case 2:
                return range;
            case 3:
                return damage;
            case 4:
                return cooldown;
            default:
                break;
        }
        return rarity;
    }

    public int GetMaxPoints()
    {
        switch (rarity)
        {
            default:
            case 1:
                return 8;
            case 2:
                return 16;
            case 3:
                return 24;
        }
    }

    public int GetHighestAttribute()
    {
        int highestAttribute = 0;
        for (int i = 1; i < 5; i++)
        {
            if (GetAtPos(highestAttribute) < GetAtPos(i)) highestAttribute = i;
        }
        return highestAttribute;
    }
    
    public int GetLowestAttribute()
    {
        int lowestAttribute = 0;
        for (int i = 1; i < 5; i++)
        {
            if (GetAtPos(lowestAttribute) >= GetAtPos(i)) lowestAttribute = i;
        }
        return lowestAttribute;
    }
};

public class WeaponStatistics
{
    private int rarity;
    private PointsStruct pointsVal;
    private float mobility;
    private float attackSpeed;
    private float range;
    private float damage;
    private float cooldown;
    private bool attackJab;

    public void GenerateValues(int newRarity)
    {
        rarity = newRarity;
        pointsVal.Init(newRarity);

        int maxPoints = 8;
        if (rarity == 2)
        {
            maxPoints = 16;
        }
        else if (rarity == 3)
        {
            maxPoints = 24;
        }

        for (int point = 0; point < maxPoints; point++)
        {
            pointsVal.IncAtPos(Random.Range(0, 5));
        }

        mobility = 0.8f + (0.3f * Mathf.Pow(pointsVal.GetAtPos(0), 0.5f));
        attackSpeed = 5f + (2f * Mathf.Pow(pointsVal.GetAtPos(1), 0.5f));
        range = 0.2f + (0.15f * Mathf.Pow(pointsVal.GetAtPos(2), 0.4f));
        damage = 6f + (1.5f * Mathf.Pow(pointsVal.GetAtPos(3), 0.75f));
        cooldown = 1.2f - (0.1f * Mathf.Pow(pointsVal.GetAtPos(4), 0.8f));

        if (pointsVal.GetAtPos(1) >= (1 + (rarity * 2))) 
        {
            attackSpeed *= 1.5f;
            cooldown = cooldown / 3f;
            attackJab = true;
        }
        else attackJab = false;
    }

    public void LoadEnemyValues(EnemyStatistics.EnemyEnum enemyType, float newRange, float newDamage)
    {
        rarity = 0;
        mobility = 1f;
        range = newRange;
        damage = newDamage;
        switch (enemyType)
        {
            default:
            case EnemyStatistics.EnemyEnum.Standard:
                attackSpeed = 7f;
                cooldown = 1f;
                attackJab = false;
                break;
            case EnemyStatistics.EnemyEnum.Tank:
                attackSpeed = 5f;
                cooldown = 1.2f;
                attackJab = false;
                break;
            case EnemyStatistics.EnemyEnum.Swarm:
                attackSpeed = 15f;
                cooldown = 0.6f;
                attackJab = true;
                break;
            case EnemyStatistics.EnemyEnum.Sniper:
                attackSpeed = 10f;
                cooldown = 1f;
                attackJab = false;
            break;
            case EnemyStatistics.EnemyEnum.Turret:
                attackSpeed = 5f;
                cooldown = 1f;
                attackJab = false;
            break;
        }
    }

    public EnemyStatistics.EnemyEnum GetCounterType()
    {
        int highestAttribute = pointsVal.GetHighestAttribute();
        int lowestAttribute = pointsVal.GetLowestAttribute();

        switch (lowestAttribute)
        {
            case 0:
                return EnemyStatistics.EnemyEnum.Swarm;
            case 1:
                return EnemyStatistics.EnemyEnum.Tank;
            case 2:
                switch (highestAttribute)
                {
                    case 0:
                        return EnemyStatistics.EnemyEnum.Standard;
                    case 1:
                    case 3:
                    case 4:
                        return EnemyStatistics.EnemyEnum.Sniper;
                }
            break;
            case 3:
                return EnemyStatistics.EnemyEnum.Tank;
            case 4:
                switch (highestAttribute)
                {
                    case 0:
                        return EnemyStatistics.EnemyEnum.Standard;
                    case 1:
                        return EnemyStatistics.EnemyEnum.Sniper;
                    case 2:
                    case 3:
                        return EnemyStatistics.EnemyEnum.Swarm;
                }
            break;
        }

        return EnemyStatistics.EnemyEnum.Standard;
    }

    public void PrintValues()
    {
        Debug.Log(string.Format("Rarity: {0}\nMobility: {1}, Attack Speed: {2}, Range: {3}, Damage: {4}, Cooldown: {5}\nCounter: {6}", 
        pointsVal.GetAtPos(-1), pointsVal.GetAtPos(0), pointsVal.GetAtPos(1), pointsVal.GetAtPos(2), pointsVal.GetAtPos(3), pointsVal.GetAtPos(4), GetCounterType()));
    }

    public PointsStruct GetPointsVal()
    {
        return pointsVal;
    }

    public int GetRarity()
    {
        return rarity;
    }

    public float GetMobility()
    {
        return mobility;
    }
    
    public float GetAttackSpeed()
    {
        return attackSpeed;
    }
    
    public float GetRange()
    {
        return range;
    }
    
    public float GetDamage()
    {
        return damage;
    }

    public float GetCooldown()
    {
        return cooldown;
    }

    public bool IsAttackJab()
    {
        return attackJab;
    }
}

public class WeaponBaseScript : MonoBehaviour
{
    protected bool isAttacking;
    protected float attackPercentage;
    protected float attackTimer;
    protected Vector3 prevPlayerPosition;
    protected bool particleBuffer;
    public Transform spriteTransform;
    public GameObject particleObject;
    public Sprite[] spriteTextureArray;
    protected Rigidbody2D weaponRigidbody;
    protected BoxCollider2D weaponHitbox;
    protected WeaponStatistics weaponStatistics;
    // Start is called before the first frame update
    public virtual void Awake()
    {
        isAttacking = false;
        attackPercentage = 0f;
        attackTimer = 0f;
        particleBuffer = false;

        spriteTransform.gameObject.GetComponent<SpriteRenderer>().sprite = spriteTextureArray[0];
        weaponRigidbody = gameObject.GetComponent<Rigidbody2D>();
        weaponHitbox = gameObject.GetComponent<BoxCollider2D>();
                weaponHitbox.enabled = false;

        particleObject.transform.localPosition = Vector3.zero;
        particleObject.GetComponent<TrailRenderer>().emitting = false;
        particleObject.GetComponent<CircleCollider2D>().enabled = false;

        weaponStatistics = new WeaponStatistics();
    }

    // Update is called once per frame
    public virtual void FixedUpdate()
    {     
        // make trail local space
        TrailRenderer tempTrail = particleObject.GetComponent<TrailRenderer>();
        Vector3[] positionArray = new Vector3[tempTrail.positionCount];
        //Debug.Log(tempTrail.positionCount); //25
        int positionAmount = tempTrail.GetPositions(positionArray);

        for (int trailpos = 0; trailpos < positionAmount; trailpos++)
        {
            positionArray[trailpos] += (transform.parent.position - prevPlayerPosition);
        }
        tempTrail.SetPositions(positionArray);
        
        prevPlayerPosition = transform.parent.position;

        // prevent last point
        if (particleBuffer)
        {
            particleBuffer = false;
            if (positionAmount > 1) positionArray[positionAmount - 1] = positionArray[positionAmount - 2];
            tempTrail.SetPositions(positionArray);
            isAttacking = false;
        }
        // attack movement
        else if (isAttacking) 
        {
            tempTrail.emitting = true;

            // attack speed below 15 is a swing
            if (!weaponStatistics.IsAttackJab())
            {
                particleObject.transform.RotateAround(transform.parent.position, new Vector3(0, 0, 1), 90f * Time.fixedDeltaTime * weaponStatistics.GetAttackSpeed());
                attackPercentage += Time.fixedDeltaTime * weaponStatistics.GetAttackSpeed();
                weaponHitbox.size = new Vector2(0.2f + attackPercentage * 0.8f, weaponHitbox.size.y);
                weaponHitbox.offset = new Vector2((transform.GetComponentInParent<Transform>().localScale.x *  0.5f) - (weaponHitbox.size.x *  0.5f), weaponHitbox.offset.y);
            }
            // else its a jab
            else
            {
                Vector3 attackVector = new Vector3(0, 1, 0) * Time.fixedDeltaTime * weaponStatistics.GetAttackSpeed();
                if (attackPercentage < 0.5f)
                {
                    particleObject.transform.Translate(attackVector);
                }
                else
                {
                    tempTrail.emitting = false;
                    particleObject.transform.Translate(-attackVector);
                }
                attackPercentage += Time.fixedDeltaTime * ((1 / weaponStatistics.GetRange()) * 2.5f);
            }

            if (attackPercentage >= 1f)
            {
                tempTrail.emitting = false;
                particleBuffer = true;
            }
        }
        else 
        {
            if (attackTimer != 0f)
            {
                attackTimer = Mathf.Clamp(attackTimer -= Time.fixedDeltaTime, 0f, weaponStatistics.GetCooldown());
            
                if (attackTimer < 0.2f)
                {
                    spriteTransform.gameObject.SetActive(true);   
                    tempTrail.Clear();
                    particleObject.GetComponent<CircleCollider2D>().enabled = false;                 
                }
                else if (attackTimer < weaponStatistics.GetCooldown() - 0.3f)
                {
                    particleObject.transform.localPosition = new Vector3(
                        transform.GetComponentInParent<Transform>().localScale.x * 0.5f + weaponStatistics.GetRange(), 
                        transform.GetComponentInParent<Transform>().localScale.x * 0.5f + weaponStatistics.GetRange(), 
                        0f);
                    weaponHitbox.enabled = false;
                    particleObject.GetComponent<CircleCollider2D>().enabled = false;
                }
            }
        }
    }

    public virtual void Attack()
    {
        if (attackTimer == 0f)
        {
            spriteTransform.gameObject.SetActive(false);
            particleObject.GetComponent<CircleCollider2D>().enabled = true;
            isAttacking = true;
            attackPercentage = 0f;
            attackTimer = weaponStatistics.GetCooldown();

            // place attack object at start of attack
            if (weaponStatistics.IsAttackJab())
            {
                particleObject.transform.localPosition = new Vector3(
                    transform.GetComponentInParent<Transform>().localScale.x * 0.3f, 
                    transform.GetComponentInParent<Transform>().localScale.y * 0.6f, 
                    0f);
            }
            else
            {
                weaponHitbox.enabled = true;
                particleObject.transform.localPosition = new Vector3(
                    transform.GetComponentInParent<Transform>().localScale.x * 0.5f + weaponStatistics.GetRange(), 
                    transform.GetComponentInParent<Transform>().localScale.y * 0.5f + weaponStatistics.GetRange(), 
                    0f);
                weaponHitbox.size = new Vector2(0.2f, weaponStatistics.GetRange());
                weaponHitbox.offset = new Vector2(
                    transform.GetComponentInParent<Transform>().localScale.x * 0.5f - 0.1f, 
                    transform.GetComponentInParent<Transform>().localScale.y * 0.6f + (weaponStatistics.GetRange() *  0.5f)
                );
            }
        }
    }

    public PointsStruct GetPointsVal()
    {
        return weaponStatistics.GetPointsVal();
    }

    public virtual float GetWeaponMobility()
    {
        if (isAttacking) 
        {
            if (weaponStatistics.IsAttackJab()) return weaponStatistics.GetMobility() * 0.75f;
            else return weaponStatistics.GetMobility() * 0.5f;
        }
        else return weaponStatistics.GetMobility();
    }

    public virtual float GetWeaponDamage()
    {
        return weaponStatistics.GetDamage();
    }

    public virtual float GetRange()
    {
        return weaponStatistics.GetRange();
    }

    public virtual bool GetIsAttacking()
    {
        return isAttacking;
    }

    
    public EnemyStatistics.EnemyEnum GetCounterType()
    {
        return weaponStatistics.GetCounterType();
    }
}
