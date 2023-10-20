using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.EventSystems;

public class RightPad : MonoBehaviour, IPointerClickHandler
{
    private static RightPad _instance;
    public static RightPad Instance => _instance;
    public static Action OnClick;

    void Awake()
    {
        _instance = this;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick?.Invoke();
    }
}
