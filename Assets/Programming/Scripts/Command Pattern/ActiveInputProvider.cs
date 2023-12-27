using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ActiveInputProvider
{
    [Header("Active input")]
    [SerializeField, Tooltip("The threshold the slide movement of the finger has to exceed to detect the actual slide")] float _slideThreshold = 80;

    public static Action<Vector3, Vector2> OnDrag; //FIXME: Da rimuovere.

    Touch touch;

    Vector2 startTPos = Vector2.zero;
    Vector2 endTPos = Vector2.zero;

    Vector3 worldPos = Vector3.zero;

    bool _slided = false;
    bool canSlide = false;

    public Drag GetInput()
    {
        if (Input.touchCount <= 0 || Input.touchCount >= 2) return null;

        for (int i = 0; i < Input.touchCount; i++)
        {
            touch = Input.GetTouch(i);

            switch (touch.phase)
            {
                case TouchPhase.Began:

                    _slided = false;

                    startTPos = touch.position;
                    endTPos = touch.position;

                    worldPos = new Vector3(startTPos.x, startTPos.y, 20);
                    //converts the touch position on the screen to world units
                    Ray ray = Camera.main.ScreenPointToRay(worldPos);

                    canSlide = Physics.Raycast(ray, out RaycastHit hit);

                    worldPos = hit.point;

                    break;

                case TouchPhase.Moved:

                    if (!canSlide) break;

                    endTPos = touch.position;
                    Vector2 direction = endTPos - startTPos;

                    //checks the threshold
                    if (direction.sqrMagnitude > _slideThreshold * _slideThreshold && !_slided)
                    {
                        //once the threshold is exceeded, it fires the event containing the normalized direction of the slide movement
                        //OnDrag?.Invoke(worldPos, directionT.normalized);
                        _slided = true;

                        Drag currentDrag = new(worldPos, direction.normalized);
                        return currentDrag;
                    }

                    break;

                case TouchPhase.Ended:

                    _slided = false;
                    startTPos = Vector2.zero;
                    endTPos = Vector2.zero;
                    break;

                default:
                    break;
            }
        }

        return null;
    }
}
