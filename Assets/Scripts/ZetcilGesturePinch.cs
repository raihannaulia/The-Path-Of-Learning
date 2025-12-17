using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZetcilGesturePinch : MonoBehaviour
{
    [Header("Main Settings")]
    public bool PinchEnabled;

    [Header("Movement Settings")]
    public float scaleFactor = 1;

    private float initialDistance;
    private Vector3 initialScale;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void SetTouchLookEnabled(bool aValue)
    {
        PinchEnabled = aValue;
    }

    // Update is called once per frame
    void Update()
    {
        if (PinchEnabled && Input.touchCount == 2)
        {
            var touchZero = Input.GetTouch(0);
            var touchOne = Input.GetTouch(1);

            // if any one of touchzero or touchOne is cancelled or maybe ended then do nothing
            if (touchZero.phase == TouchPhase.Ended || touchZero.phase == TouchPhase.Canceled ||
                touchOne.phase == TouchPhase.Ended || touchOne.phase == TouchPhase.Canceled)
            {
                return; // basically do nothing
            }

            if (touchZero.phase == TouchPhase.Began || touchOne.phase == TouchPhase.Began)
            {
                initialDistance = Vector2.Distance(touchZero.position, touchOne.position);
                initialScale = transform.localScale;
            }
            else // if touch is moved
            {
                var currentDistance = Vector2.Distance(touchZero.position, touchOne.position);

                //if accidentally touched or pinch movement is very very small
                if (Mathf.Approximately(initialDistance, 0))
                {
                    return; // do nothing if it can be ignored where inital distance is very close to zero
                }

                var factor = currentDistance / initialDistance * scaleFactor;
                transform.localScale = initialScale * factor; // scale multiplied by the factor we calculated
            }
        }

    }
}
