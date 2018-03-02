using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {
    public float movementSpeed;

    public float dragSpeed;
    public float zoom;

    private Vector3 dragOrigin;

    private Vector3 playerPos;
    private Vector3 originalPos;
    private float originalZoom;
    private float trackingDuration;
    private float timePassed;
    private bool tracking;

    private void Awake()
    {
        tracking = false;
    }

    void Update () {
        if (tracking)
        {
            timePassed += Time.deltaTime;
            
            transform.position = Vector3.Lerp(originalPos, playerPos, timePassed / trackingDuration);
            Camera.main.orthographicSize = Mathf.Lerp(originalZoom, zoom, timePassed / trackingDuration);

            if (timePassed > trackingDuration)
            {
                tracking = false;
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

    public void CameraTracking(Vector3 position, float duration)
    {
        tracking = true;
        playerPos = position;
        playerPos.z = -10;
        trackingDuration = duration;
        originalPos = transform.position;
        originalZoom = Camera.main.orthographicSize;
        timePassed = 0f;
    }
}
