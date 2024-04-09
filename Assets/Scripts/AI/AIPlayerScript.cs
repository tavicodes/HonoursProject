using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayerScript : PlayerScript
{
    public GameObject weaponPrefab;
    public GameObject attachedRoom;
    public GameObject AIDirector;
    private bool rotationDirection;
    private int deathState;
    public override void Awake()
    {
        base.Awake();

        rotationDirection = false;
        deathState = 0;

        GameObject playerWeaponObj = Instantiate(weaponPrefab, transform);
        playerWeaponObj.transform.SetParent(transform);
        playerWeapon = playerWeaponObj.GetComponent<WeaponPlayerScript>();
        playerWeapon.Equipped();
    }

    public new void FixedUpdate()
    {
        if (deathState == 1) 
        {
            attachedRoom.GetComponent<AIRoomScript>().ClearRoom();
            deathState = 2;
        }
        else if (deathState == 0)
        {
            List<GameObject> enemies = attachedRoom.GetComponent<RoomScript>().GetEnemyArray();
            if (enemies != null)
            {    
                GameObject closestEnemy = null;
                foreach (GameObject enemy in enemies)
                {
                    if (enemy != null)
                    {
                        if (closestEnemy == null) closestEnemy = enemy;
                        else if (Vector3.Distance(transform.position, enemy.transform.position) < Vector3.Distance(transform.position, closestEnemy.transform.position)) closestEnemy = enemy;
                    }
                }

                if (closestEnemy != null)
                {
                    Vector2 vecToEnemy = playerWeapon.GetWeaponMobility() * ((Vector2)(closestEnemy.transform.position - transform.position)).normalized;

                    float distanceMultiplier = 0f;
                    float distance = Vector2.Distance(transform.position, closestEnemy.transform.position) - closestEnemy.transform.localScale.x;
                    if (distance <= playerWeapon.GetRange() * 0.75f) 
                    {
                        distanceMultiplier = -1f;
                    }
                    else if (distance >= playerWeapon.GetRange() * 0.95f) 
                    {
                        distanceMultiplier = 1f;
                    }

                    movementVec = vecToEnemy * distanceMultiplier;

                    Vector3 direction = closestEnemy.transform.position - transform.position;
                    direction.Normalize();
                    float targetAngle = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 270f) % 360f;
                    Quaternion newQuat = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0f, 0f, targetAngle), 720f * Time.fixedDeltaTime);
                    transform.rotation = newQuat;
                    
                    //Debug.Log("Distance: " + distance + "\nRange: " + playerWeapon.GetRange());

                    bool distanceCheck = (distance <= playerWeapon.GetRange());            
                    bool angleCheck = (Mathf.Abs(newQuat.eulerAngles.z - targetAngle) <= 30f);

                    if (distanceCheck && angleCheck) playerWeapon.Attack();

                    if ((closestEnemy.GetComponent<EnemyScript>().IsRanged() && distanceCheck) || Mathf.Abs(closestEnemy.transform.rotation.eulerAngles.z - targetAngle) <= 170f) 
                    {
                        Vector2 strafeVec = new Vector2(-vecToEnemy.y, vecToEnemy.x);
                        if (rotationDirection) strafeVec = -strafeVec;
                        movementVec += strafeVec * 2f;
                    }

                    playerRigidbody.velocity = movementVec;

                    if (hitFloat != 0f)
                    {
                        hitFloat = Mathf.Clamp(hitFloat - Time.fixedDeltaTime, 0f, 1f);
                        playerSprite.color = Color.Lerp(playerColor, Color.red, hitFloat);
                    }
                }
            }
            //else Debug.Log("Enemies null");
        }
    }

    public void Reset(Vector3 roomPos, int weaponRarity)
    {
        transform.SetPositionAndRotation(roomPos + new Vector3(0, -3, 0), Quaternion.identity);
        health = 30f;
        deathState = 0;
        playerWeapon.Regenerate(weaponRarity);
    }

    public void SoftReset(Vector3 roomPos)
    {
        transform.SetPositionAndRotation(roomPos + new Vector3(0, -3, 0), Quaternion.identity);
        health = 30f;
        deathState = 0;
    }

    protected override void ReduceHealth(float damage)
    {
        base.ReduceHealth(damage);
        AIDirector.GetComponent<AIDirectorScript>().AddReward((damage / 30f) * 0.5f);
        if (health <= 0f) deathState = 1;
    }

    public override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        if (collision.gameObject.tag == "Wall")
        {
            rotationDirection = !rotationDirection;
        }
    }
}
