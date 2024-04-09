using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public GameObject testEnemy;
    public float movementSpeed = 3f;
    protected float dashTime = 0f;
    protected float health = 50f;
    protected int itemCooldown = 0;
    private bool canUpdate;
    public Color playerColor;
    protected float hitFloat;
    protected Vector2 movementVec;
    private ArrayList weaponArray;
    protected Rigidbody2D playerRigidbody;
    protected SpriteRenderer playerSprite;
    public GameObject trailObject;
    protected TrailRenderer trailRenderer;
    protected DungeonScript owningDungeon;
    protected WeaponPlayerScript playerWeapon;
    private UIHealthScript uiHealth;
    private UIItemScript uiItem;
    private UIWeaponScript uiWeapon;

    // Awake is called before the first frame update
    virtual public void Awake()
    {
        playerRigidbody = gameObject.GetComponent<Rigidbody2D>();
        playerSprite = gameObject.GetComponent<SpriteRenderer>();
        playerSprite.color = playerColor;

        if (trailObject != null) trailRenderer = trailObject.GetComponent<TrailRenderer>();

        playerWeapon = null;
        weaponArray = new ArrayList();
        
        canUpdate = false;
    }

    public void Update()
    {
        if (canUpdate && !GetPauseState())
        {
            // movement
            movementVec.x = movementSpeed * Input.GetAxisRaw("Horizontal");
            movementVec.y = movementSpeed * Input.GetAxisRaw("Vertical");//rotation
            Vector3 mousePos = Input.mousePosition;
    
            Vector3 objectPos = Camera.main.WorldToScreenPoint(transform.position);
            mousePos.x = mousePos.x - objectPos.x;
            mousePos.y = mousePos.y - objectPos.y;
    
            float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg + 270f;
    
            // check if player can rotate
            if (playerWeapon == null || !playerWeapon.GetIsAttacking()) transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            
            if (hitFloat > 0f)
            {
                hitFloat = Mathf.Clamp(hitFloat - Time.deltaTime, 0f, 1f);
                playerSprite.color = Color.Lerp(playerColor, Color.red, hitFloat);
            }
            else if (hitFloat < 0f)
            {
                hitFloat = Mathf.Clamp(hitFloat + Time.deltaTime, -1f, 0f);
                playerSprite.color = Color.Lerp(playerColor, Color.green, -hitFloat);
            }
            
            if (dashTime != 0f) 
            {
                dashTime = Mathf.Clamp(dashTime - Time.deltaTime, 0f, 1f);
                if (dashTime <= 0.7f) trailRenderer.time = Mathf.Clamp(trailRenderer.time - Time.deltaTime, 0f, 0.5f);
                if (trailRenderer.time == 0f) trailRenderer.emitting = false;
            }
    
            // inputs
            if (Input.GetButtonDown("Equip") && weaponArray.Count > 0)
            {
                EquipWeapon();
            }
            if (Input.GetButton("Fire1") && playerWeapon != null)
            {
                playerWeapon.Attack();
            }
            if (Input.GetButtonDown("Item"))
            {
                if (itemCooldown == 0) 
                {
                    IncreaseHealth(20f);
                    itemCooldown = 3;
                    uiItem.SetSprite(itemCooldown);
                }
            }
            if (Input.GetButtonDown("Dash"))
            {
                if (dashTime == 0f) 
                {
                    dashTime = 1f;
                    trailRenderer.time = 0.1f;
                    trailRenderer.emitting = true;
                }
            }
        }
        // if (Input.GetButtonDown("Debug") && playerWeapon != null)
        // {
        //     playerWeapon.Regenerate(Random.Range(1,4));
        //     uiWeapon.SetPoints(playerWeapon.GetPointsVal());
        // }
        if (Input.GetButtonDown("Pause"))
        {
            owningDungeon.TogglePause();
        }
    }

    // Update is called once per frame
    virtual public void FixedUpdate()
    {
        if (canUpdate && !GetPauseState())
        {
            // movement
            if (playerWeapon != null) movementVec *= playerWeapon.GetWeaponMobility();
            if (dashTime >= 0.7f) movementVec *= ((dashTime + 0.3f) * 3f);
            playerRigidbody.velocity = movementVec;

        }
    }

    public void ToggleUpdate()
    {
        canUpdate = !canUpdate;
        movementVec = Vector2.zero;
    }

    public bool GetPauseState()
    {
        if (owningDungeon != null) return owningDungeon.GetPauseState();
        else return false;
    }

    void EquipWeapon()
    {
        WeaponPlayerScript tempWeapon = null;
        if (playerWeapon != null)
        {
            tempWeapon = playerWeapon;
        }

        float lowestDist = 100f;
        foreach (WeaponPlayerScript weapon in weaponArray)
        {
            Transform tempTransform = weapon.GetComponent<Transform>();
            float tempDist = Vector3.Distance(tempTransform.position, transform.position);
            if (tempDist < lowestDist)
            {
                lowestDist = tempDist;
                playerWeapon = weapon;
            }
        }

        if (tempWeapon != null)
        {
            tempWeapon.transform.SetParent(playerWeapon.transform.parent, true);
            tempWeapon.UnEquipped(transform.up);
        }

        weaponArray.Remove(playerWeapon);
        playerWeapon.GetComponent<Transform>().SetParent(transform);
        playerWeapon.Equipped();

        uiWeapon.SetPoints(playerWeapon.GetPointsVal());
    }

    public void SetOwningDungeon(DungeonScript newDungeon)
    {
        owningDungeon = newDungeon;
    }

    public void SetUIHealth(UIHealthScript newUIHealth)
    {
        uiHealth = newUIHealth;
    }
    
    public void SetUIItem(UIItemScript newUIItem)
    {
        uiItem = newUIItem;
    }

    public void SetUIWeapon(UIWeaponScript newUIWeapon)
    {
        uiWeapon = newUIWeapon;
    }

    public float GetHealth()
    {
        return health;
    }

    virtual protected void ReduceHealth(float damage)
    {
        hitFloat = 1f;
        health = Mathf.Clamp(health - damage, 0f, 50f);
        if (uiHealth != null) uiHealth.UpdateHealth(health);
        if (health == 0f && owningDungeon != null) owningDungeon.ShowOverlay(false);
    }

    virtual protected void IncreaseHealth(float change)
    {
        hitFloat = -1f;
        health = Mathf.Clamp(health + change, 0f, 50f);
        if (uiHealth != null) uiHealth.UpdateHealth(health);
    }

    public void ReduceItemCooldown(bool full)
    {
        if (full)
        {
            itemCooldown = 0;
            uiItem.SetSprite(itemCooldown);
        }
        if (itemCooldown > 0) 
        {
            itemCooldown--;
            uiItem.SetSprite(itemCooldown);
        }
    }

    public WeaponPlayerScript GetWeaponScript()
    {
        return playerWeapon;
    }

    virtual public void OnCollisionEnter2D(Collision2D collision)
    {
        // Collided with enemy
        if (collision.gameObject.tag == "Enemy" && hitFloat == 0f)
        {
            if (!collision.gameObject.GetComponent<EnemyScript>().isDead())
            {
                ReduceHealth(5f);
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        // Entered player weapon pickup area
        if (other.tag == "Weapon")
        {
            WeaponPlayerScript tempPlayerWeapon = other.GetComponent<WeaponPlayerScript>();
            if (tempPlayerWeapon != null) weaponArray.Add(tempPlayerWeapon);
            return;
        }

        // Hit by enemy weapon
        WeaponEnemyScript tempEnemyWeapon = other.GetComponent<WeaponEnemyScript>();
        if (other.tag == "Particle")
        {
            tempEnemyWeapon = other.GetComponentInParent<WeaponEnemyScript>();
            if (tempEnemyWeapon == null)
            {
                ProjectileScript tempProjectile = other.GetComponent<ProjectileScript>();
                if (tempProjectile != null && !tempProjectile.GetInDestroy())
                {
                    tempEnemyWeapon = tempProjectile.GetSpawningWeapon();
                    tempProjectile.transform.parent = transform;
                    tempProjectile.StartDestroy();
                }
            }
        }
        if (tempEnemyWeapon != null && tempEnemyWeapon.gameObject.tag == "Weapon")
        {
            ReduceHealth(tempEnemyWeapon.GetWeaponDamage());
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        WeaponPlayerScript tempWeapon = other.GetComponent<WeaponPlayerScript>();
        if (tempWeapon != null && weaponArray.Contains(tempWeapon))
        {
            weaponArray.Remove(tempWeapon);
        }
    }
   
}
