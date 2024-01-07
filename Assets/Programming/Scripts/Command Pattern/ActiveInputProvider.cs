using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ActiveInputProvider : CommandProvider
{
    [Header("Active input")]
    [SerializeField, Tooltip("Lo slide necessario per essere riconosciuto come tale")] float _slideThreshold = 80;

    Touch touch;

    Vector2 startTPos = Vector2.zero;
    Vector2 endTPos = Vector2.zero;

    Vector3 worldPos = Vector3.zero;

    bool _slided = false;
    bool canSlide = false;

    public override ICommand GetCommand()
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
                    
                    //Spariamo un raggio che si suppone colpirà il piano invisibile posizionato in scena.
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
