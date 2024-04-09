using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPlayerScript : WeaponBaseScript
{
    public float throwForce;
    private bool isEquipped;
    private float idleRotation;
    // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();
        isEquipped = false;
        idleRotation = -90f;
        transform.Rotate(new Vector3(0f, 0f, Random.Range(0f, 360f)));
    }

    // Update is called once per frame
    public override void FixedUpdate()
    {        
        // idle spin
        if (!isEquipped)
        {
            transform.Rotate(new Vector3(0f, 0f, idleRotation * weaponStatistics.GetRarity() * Time.fixedDeltaTime));
        }
        // main loop
        else
        {
            base.FixedUpdate();
        }
    }
    
    public void Equipped()
    {
        isEquipped = true;
        weaponRigidbody.bodyType = RigidbodyType2D.Kinematic;
        gameObject.GetComponent<CircleCollider2D>().enabled = false;

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(0, 0, 0);
        spriteTransform.localRotation = Quaternion.Euler(0f, 0f, 80f);
        //spriteTransform.localPosition = new Vector3(
            // transform.GetComponentInParent<Transform>().localScale.x * 0.6f + (weaponStatistics.GetPointsVal().GetAtPos(2)), 
            // transform.GetComponentInParent<Transform>().localScale.x * 0.2f + ((weaponStatistics.GetPointsVal().GetAtPos(2)) *  0.5f), 
            // 0f);
        spriteTransform.localPosition = new Vector3(
            transform.GetComponentInParent<Transform>().localScale.x * 0.7f, 
            transform.GetComponentInParent<Transform>().localScale.x * 0.2f, 
            0f);
    }

    public void UnEquipped(Vector3 forwardVector)
    {
        isEquipped = false;
        weaponRigidbody.bodyType = RigidbodyType2D.Dynamic;
        gameObject.GetComponent<CircleCollider2D>().enabled = true;
        spriteTransform.gameObject.SetActive(true);
        Transform playerTransform = transform.parent;
        
        transform.localRotation = Quaternion.Euler(0f, 0f, 180f);
        spriteTransform.localRotation = Quaternion.Euler(0, 0, 0);
        spriteTransform.localPosition = Vector3.zero;
        
        particleObject.transform.localPosition = Vector3.zero;
        particleObject.GetComponent<TrailRenderer>().emitting = false;
        isAttacking = false;
        attackTimer = 0f;

        weaponRigidbody.AddForce(forwardVector * throwForce, ForceMode2D.Impulse);
    }

    public bool GetIsEquipped()
    {
        return isEquipped;
    }

    public void Regenerate(int newRarity)
    {
        weaponStatistics.GenerateValues(newRarity);      

        spriteTransform.localScale = new Vector3(0.8f + (weaponStatistics.GetPointsVal().GetAtPos(2) * 0.05f), 1.2f + (weaponStatistics.GetPointsVal().GetAtPos(3) * 0.1f), 1);
        particleObject.transform.localRotation = Quaternion.identity;
        particleObject.transform.localScale = Vector3.one * (0.1f + (weaponStatistics.GetPointsVal().GetAtPos(3) * 0.01f));
        particleObject.GetComponent<TrailRenderer>().widthMultiplier = 0.1f + (weaponStatistics.GetPointsVal().GetAtPos(3) * 0.01f);
        
        //weaponStatistics.PrintValues();    
    }
}
