using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private Transform playerTransform;
    private Transform roomTransform;
    public float followSpeed;
    private bool xMovement;
    private bool yMovement;

    // Start is called before the first frame update
    void Start()
    {
        xMovement = yMovement = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTransform == null || roomTransform == null) return;
        
        float xTarget = playerTransform.position.x;
        float yTarget = playerTransform.position.y;

        float xNew = transform.position.x;
        float yNew = transform.position.y;

        if (xMovement)
        {
            xNew = Mathf.Lerp(transform.position.x, xTarget, Time.deltaTime * followSpeed);
            xNew = Mathf.Clamp(xNew, roomTransform.position.x - (roomTransform.localScale.x / 2f), roomTransform.position.x + (roomTransform.localScale.x / 2f));
        }

        if (yMovement)
        {
            yNew = Mathf.Lerp(transform.position.y, yTarget, Time.deltaTime * followSpeed);
            yNew = Mathf.Clamp(yNew, roomTransform.position.y - (roomTransform.localScale.y / 2f), roomTransform.position.y + (roomTransform.localScale.y / 2f));
        }
        
        transform.position = new Vector3(xNew, yNew, transform.position.z);
    }

    public void SetPlayerTransform(Transform newTransform)
    {
        playerTransform = newTransform;
    }

    public void SetRoomTransform(Transform newTransform)
    {
        roomTransform = newTransform;
        transform.position = roomTransform.position + new Vector3(0f, 0f, -1f);
        if (roomTransform.localScale.x > 19) xMovement = true;
        if (roomTransform.localScale.y > 11) yMovement = true;
    }
}
