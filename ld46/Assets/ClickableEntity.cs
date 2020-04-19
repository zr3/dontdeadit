using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ClickableEntity : MonoBehaviour
{
    public ClickableEntityEvent OnSelect;
    public ClickableEntityEvent OnDeselect;
    public ClickableEntityEvent OnActivate;

    private bool isSelected = false;

    private void OnMouseEnter()
    {
        Select();
    }
    private void OnMouseExit()
    {
        Deselect();
    }
    private void OnMouseDown()
    {
        Activate();
    }


    public void Select() {
        isSelected = true;
        OnSelect.Invoke(this);
    }
    public void Deselect()
    {
        isSelected = false;
        OnDeselect.Invoke(this);
    }
    public void Activate() {
        if (isSelected)
        {
            OnActivate.Invoke(this);
        }
    }

    [Serializable]
    public class ClickableEntityEvent : UnityEvent<ClickableEntity> { }
}
