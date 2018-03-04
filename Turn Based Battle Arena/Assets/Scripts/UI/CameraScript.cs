using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {
    public float movementSpeed;

    public float dragSpeed;

    private Vector3 dragOrigin;
    
    private Vector3 startingPos;
    private Vector3 newPosition;
    private float startingZoom;
    private float newZoom;
    private float trackingDuration;
    private float timePassed;
    private bool moving;

    private void Awake()
    {
        moving = false;
    }

    void Update () {
        if (moving)
        {
            timePassed += Time.deltaTime;
            
            transform.position = Vector3.Lerp(startingPos, newPosition, timePassed / trackingDuration);
            Camera.main.orthographicSize = Mathf.Lerp(startingZoom, newZoom, timePassed / trackingDuration);

            if (timePassed > trackingDuration)
            {
                moving = false;
            }
        } else
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                Camera.main.orthographicSize--;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                Camera.main.orthographicSize++;
            }

            if (Input.GetMouseButtonDown(1))
            {
                dragOrigin = Input.mousePosition;
                return;
            }

            if (!Input.GetMouseButton(1))
            {
                return;
            }

            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
            Vector3 move = new Vector3(pos.x * -dragSpeed, pos.y * -dragSpeed, 0);

            transform.Translate(move, Space.World);
        }
    }

    public void CameraMovement(Vector3 position, float zoom, float duration)
    {
        moving = true;
        newPosition = position;
        newPosition.z = -10;
        newZoom = zoom;
        trackingDuration = duration;
        startingPos = transform.position;
        startingZoom = Camera.main.orthographicSize;
        timePassed = 0f;
    }
}
