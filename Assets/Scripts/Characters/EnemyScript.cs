using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatistics
{
    public enum EnemyEnum
    {
        Standard,
        Tank,
        Swarm,
        Sniper,
        Turret,
        Empty
    }
    private int maxPoints;
    private EnemyEnum enemyType;
    private float health;
    private float mobility;
    private float range;
    private float damage;

    public void GenerateValues(int floorCount)
    {
        enemyType = EnemyEnum.Empty;
        health = 0;
        mobility = 0;
        range = 0;
        damage = 0;

        GetPointCount(floorCount);

        for (int point = 0; point < maxPoints; point++)
        {
            int category = Random.Range(1, 5);
            switch (category)
            {
                case 1:
                    health++;
                    break;
                case 2:
                    mobility++;
                    break;
                case 3:
                    range++;
                    break;
                case 4:
                    damage++;
                    break;
                default:
                    break;
            }
        }

        AssignValues();
    }

    public void AssignValues()
    {
        if (enemyType == EnemyEnum.Empty)
        {
            if (Mathf.FloorToInt(range) >= Mathf.FloorToInt(maxPoints * 0.4f))
            {
                if (Mathf.FloorToInt(mobility) >= Mathf.FloorToInt(maxPoints * 0.2f))
                {
                    enemyType = EnemyEnum.Sniper;
                }
                else
                {
                    enemyType = EnemyEnum.Turret;
                }
            }
            else if (Mathf.FloorToInt(health) >= Mathf.FloorToInt(maxPoints * 0.4f)) enemyType = EnemyEnum.Tank;
            else if (Mathf.FloorToInt(mobility) >= Mathf.FloorToInt(maxPoints * 0.4f)) enemyType = EnemyEnum.Swarm;
            else enemyType = EnemyEnum.Standard;
        }

        //PrintValues();

        switch (enemyType)
        {
            default:
            case EnemyEnum.Standard:
                health = 24 + (health * 4);
                mobility = 0.8f + 0.1f * Mathf.Pow(mobility, 0.4f);
                range = 0.5f + 0.1f * Mathf.Pow(range, 0.5f);
                damage = 2f + 0.65f * Mathf.Pow(damage, 0.8f);
            break;
            case EnemyEnum.Tank:
                health = 35 + (health * 5);
                mobility = 0.4f + 0.1f * Mathf.Pow(mobility, 0.4f);
                range = 0.4f + 0.1f * Mathf.Pow(range, 0.4f);
                damage = 4f + 0.7f * Mathf.Pow(damage, 0.7f);
            break;
            case EnemyEnum.Swarm:
                health = 10 + (health * 3);
                mobility = 1 + 0.15f * Mathf.Pow(mobility, 0.4f);
                range = 0.5f + 0.1f * Mathf.Pow(range, 0.4f);
                damage = 1f + 0.4f * Mathf.Pow(damage, 0.8f);
            break;
            case EnemyEnum.Sniper:
                health = 15 + (health * 3);
                mobility = 0.8f + 0.1f * Mathf.Pow(mobility, 0.4f);
                range = 4f + 4f * Mathf.Pow(range, 0.4f);
                damage = 2f + 0.65f * Mathf.Pow(damage, 0.8f);
                //damage = 1.5f + 0.65f * Mathf.Pow(damage, 0.8f);
            break;
            case EnemyEnum.Turret:
                health = 25 + (health * 3);
                mobility = 0;
                range = 6f + 4f * Mathf.Pow(range, 0.4f);
                damage = 1.6f + 0.65f * Mathf.Pow(damage, 0.8f);
                //damage = 1.2f + 0.65f * Mathf.Pow(damage, 0.8f);
            break;
        }
    }

    public void UpdateToBoss()
    {
        health *= 3f;
        mobility *= 1.5f;
        range *= 1.5f;
        damage *= 2f;
    }

    public void LoadType(int newType)
    {
        switch (newType)
        {
            default:
            case -1:
                enemyType = EnemyEnum.Empty;
                break;
            case 0:
                enemyType = EnemyEnum.Standard;
                break;
            case 1:
                enemyType = EnemyEnum.Tank;
                break;
            case 2:
                enemyType = EnemyEnum.Swarm;
                break;
            case 3:
                enemyType = EnemyEnum.Sniper;
                break;
            case 4:
                enemyType = EnemyEnum.Turret;
                break;
        }

        AssignValues();
    }

    public void LoadAtPos(int pos, int value)
    {
        switch (pos)
        {
            case 0:
                health = value;
                break;
            case 1:
                mobility = value;
                break;
            case 2:
                range = value;
                break;
            case 3:
                damage = value;
                break;
            default:
            break;
        }
    }

    public int GetPointCount(int floorCount)
    {
        maxPoints = (12 + 4 * floorCount);
        return maxPoints;
    }

    public void PrintValues()
    {
        Debug.Log(string.Format("EnemyType: {0}, Limit Points: {5}\nHealth: {1}, Mobility: {2}, Range: {3}, Damage: {4}", enemyType, health, mobility, range, damage, (maxPoints * 0.4f)));
    }

    public bool TakeDamage(float DamageReceived)
    {
        health -= DamageReceived;
        if (health <= 0f) return true;
        return false;
    }

    public EnemyEnum GetEnemyType()
    {
        return enemyType;
    }

    public float GetHealth()
    {
        return health;
    }

    public float GetMobility()
    {
        return mobility;
    }
    
    public float GetRange()
    {
        return range;
    }
    
    public float GetDamage()
    {
        return damage;
    }
}

public class EnemyScript : MonoBehaviour
{
    // magMult 135
    public float magnitudeMult;
    public Color enemyColor;
    public Sprite[] enemySpriteArray;
    private float hitFloat;
    private bool canUpdate;
    private bool dead;
    private float movementSpeed = 1.5f;
    private float rotAdd;
    private float attackWindup;
    private List<EnemyScript> swarmClones;
    private Vector2 movementVec;
    public GameObject enemyWeaponPrefab;
    public GameObject enemyWeaponObj;
    private GameObject playerObj;
    private EnemyStatistics enemyStatistics;
    private Rigidbody2D enemyRigidbody;
    private SpriteRenderer enemySprite;
    private WeaponEnemyScript enemyWeapon;
    
    // Awake is called before the first frame update
    void Awake()
    {
        hitFloat = 0f;
        attackWindup = 0f;
        movementVec = Vector2.zero;
        dead = false;
        canUpdate = false;
        swarmClones = new List<EnemyScript>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (canUpdate && playerObj != null)
        {
            if (!playerObj.GetComponent<PlayerScript>().GetPauseState())
            {
                if (!dead)
                {
                    movementVec = movementSpeed * enemyStatistics.GetMobility() * (Vector2)((playerObj.transform.position - transform.position)).normalized;

                    float distance = Vector2.Distance(transform.position, playerObj.transform.position) - (playerObj.transform.localScale.x);
                    if (enemyStatistics.GetEnemyType() == EnemyStatistics.EnemyEnum.Sniper)
                    {
                        float distanceMultiplier = 0f;
                        if (distance <= enemyStatistics.GetRange() * 0.8f) 
                        {
                            distanceMultiplier = Mathf.Clamp(distance - (enemyStatistics.GetRange() * 0.8f), -1f, 0f);
                        }
                        if (distance >= enemyStatistics.GetRange() * 0.9f) 
                        {
                            distanceMultiplier = Mathf.Clamp(distance - (enemyStatistics.GetRange() * 0.9f), 0f, 1f);
                        }
                        movementVec *= distanceMultiplier;
                    }

                    if (enemyWeapon.GetIsAttacking()) movementVec *= 0.5f;
                    else
                    {
                        Vector3 direction = playerObj.transform.position - transform.position;
                        direction.Normalize();
                        float targetAngle = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 270f) % 360f;
                        Quaternion newQuat = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0f, 0f, targetAngle), (magnitudeMult + rotAdd) * Time.fixedDeltaTime);
                        transform.rotation = newQuat;
                        
                        bool distanceCheck = (distance <= enemyStatistics.GetRange());
                        
                        bool rangedCheck = true;
                        if (enemyStatistics.GetEnemyType() == EnemyStatistics.EnemyEnum.Sniper) 
                        {
                            rangedCheck = (Vector3.Distance(transform.position, playerObj.transform.position) > (transform.localScale.x * 0.5f + playerObj.transform.localScale.x * 1.25f));
                            if (!rangedCheck) enemyWeapon.HideWeapon();
                        }

                        bool angleCheck;
                        if (IsRanged()) angleCheck = (Mathf.Abs(newQuat.eulerAngles.z - targetAngle) <= 15f);
                        else angleCheck = (Mathf.Abs(newQuat.eulerAngles.z - targetAngle) <= 30f);

                        if (attackWindup > 0f)
                        {
                            attackWindup = Mathf.Clamp(attackWindup - Time.fixedDeltaTime, 0f, 1f);
                            if (attackWindup == 0f) enemyWeapon.Attack();
                        }
                        else if (distanceCheck && rangedCheck && angleCheck) StartAttack();
                    }

                    if (enemyStatistics.GetEnemyType() != EnemyStatistics.EnemyEnum.Turret) enemyRigidbody.velocity = movementVec;
                }

                if (hitFloat != 0f)
                {
                    hitFloat = Mathf.Clamp(hitFloat - Time.fixedDeltaTime, 0f, 1f);
                    enemySprite.color = Color.Lerp(enemyColor, Color.red, hitFloat);
                }
            }
        }
    }

    private void StartAttack()
    {
        if (IsRanged()) enemyWeapon.Attack();
        else if (attackWindup == 0f) 
        {
            if (enemyWeapon.HideWeapon()) attackWindup = 0.5f;
        }
    }

    public void SetValues(EnemyStatistics newStatistics, bool isBoss = false, bool isClone = false)
    {
        enemyRigidbody = gameObject.GetComponent<Rigidbody2D>();
        enemySprite = gameObject.GetComponent<SpriteRenderer>();
        enemySprite.color = enemyColor;
        
        enemyStatistics = newStatistics;
        
        enemyWeaponObj = Instantiate(enemyWeaponPrefab, transform.position, Quaternion.identity);
        enemyWeaponObj.transform.SetParent(transform);
        enemyWeapon = enemyWeaponObj.GetComponent<WeaponEnemyScript>();
        enemyWeapon.SetValues(enemyStatistics.GetEnemyType(), enemyStatistics.GetRange(), enemyStatistics.GetDamage());

        if (isBoss) enemyStatistics.UpdateToBoss();

        enemySprite.sprite = enemySpriteArray[(int)enemyStatistics.GetEnemyType()];
        switch (enemyStatistics.GetEnemyType())
        {
            default:
            case EnemyStatistics.EnemyEnum.Standard:
            transform.localScale = Vector3.one * 1.0f;
            rotAdd = 0f;
            break;
            case EnemyStatistics.EnemyEnum.Swarm:
            transform.localScale = Vector3.one * 0.8f;
            rotAdd = 20f;
            if(!isClone)
            {
                GameObject swarmClone;
                for (int i = 0; i < 2; i++)
                {
                    swarmClone = Instantiate(gameObject, (transform.position + new Vector3(transform.localScale.x * (i + 1), transform.localScale.y * (i + 1))),
                     transform.rotation, gameObject.GetComponentInParent<RoomScript>().transform);
                    swarmClones.Add(swarmClone.GetComponent<EnemyScript>());
                    gameObject.GetComponentInParent<RoomScript>().AddToEnemyArray(swarmClone);

                    swarmClones[swarmClones.Count - 1].SetValues(newStatistics, false, true);
                    swarmClones[swarmClones.Count - 1].SetPlayer(playerObj);
                }
            }
            break;
            case EnemyStatistics.EnemyEnum.Sniper:
            transform.localScale = Vector3.one * 0.9f;
            rotAdd = 10f;
            break;
            case EnemyStatistics.EnemyEnum.Tank:
            transform.localScale = Vector3.one * 1.2f;
            rotAdd = -10f;
            break;
            case EnemyStatistics.EnemyEnum.Turret:
            transform.localScale = Vector3.one * 1.2f;
            enemyRigidbody.bodyType = RigidbodyType2D.Static;
            rotAdd = 0f;
            break;
        }
    }

    public void SetPlayer(GameObject newPlayer)
    {
        playerObj = newPlayer;
    }

    public void Activate()
    {
        canUpdate = true;
        foreach (EnemyScript swarm in swarmClones)
        {
            swarm.Activate();
        }
    }

    public bool IsRanged()
    {
        return ((enemyStatistics.GetEnemyType() == EnemyStatistics.EnemyEnum.Sniper) || (enemyStatistics.GetEnemyType() == EnemyStatistics.EnemyEnum.Turret));
    }

    public EnemyStatistics.EnemyEnum GetEnemyType()
    {
        return enemyStatistics.GetEnemyType();
    }

    public void PrintStats()
    {
        enemyStatistics.PrintValues();
    }

    public void KillEnemy()
    {
        dead = true;
        Physics2D.IgnoreCollision(gameObject.GetComponent<CircleCollider2D>(), playerObj.GetComponent<CircleCollider2D>());
        for (int childIndex = 0; childIndex < transform.childCount; childIndex++)
        {
            Destroy(transform.GetChild(childIndex).gameObject);
        }
        transform.DetachChildren();
        Destroy(gameObject, 0.5f);        
    }

    public bool isDead()
    {
        return dead;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        WeaponPlayerScript tempWeapon = other.GetComponent<WeaponPlayerScript>();
        if (tempWeapon == null)
        {
            tempWeapon = other.GetComponentInParent<WeaponPlayerScript>();
        }
        if (tempWeapon != null && tempWeapon.gameObject.tag == "Weapon" && tempWeapon.GetIsEquipped() && !dead)
        {
            if (enemyStatistics.TakeDamage(tempWeapon.GetWeaponDamage())) KillEnemy();
            hitFloat = 1f;
        }
    }
}
