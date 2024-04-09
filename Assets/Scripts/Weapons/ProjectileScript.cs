using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    private WeaponEnemyScript spawningWeapon;
    private Rigidbody2D projRigidbody;
    private TrailRenderer projTrail;
    private bool inDestroy;

    // Start is called before the first frame update
    void Start()
    {
        projRigidbody = gameObject.GetComponent<Rigidbody2D>();
        projTrail = GetComponentInChildren<TrailRenderer>();

        inDestroy = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (inDestroy && projTrail != null)
        {
            projTrail.time -= Time.deltaTime * 2f;
            if (projTrail.time <= 0f) Destroy(gameObject, 1f);
        }
        else projRigidbody.velocity = transform.up * 4f; 
    }

    public void SetSpawningWeapon(WeaponEnemyScript newWeapon)
    {
        spawningWeapon = newWeapon;
        transform.rotation = spawningWeapon.transform.rotation;
    }

    public WeaponEnemyScript GetSpawningWeapon()
    {
        return spawningWeapon;
    }
    
    public void StartDestroy()
    {
        inDestroy = true;
        projRigidbody.velocity = Vector2.zero;
    }

    public bool GetInDestroy()
    {
        return inDestroy;
    }

    void OnBecameInvisible()
    {
        inDestroy = true;
    }

     public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Wall")
        {
            StartDestroy();
        }
    }
}
