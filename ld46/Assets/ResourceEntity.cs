using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ResourceEntity : MonoBehaviour
{
    public enum ResourceType
    {
        Berries,
        Cookies,
        Fish
    }

    public ResourceType Resource;
    public OnResourceChangeEvent OnPick;
    public OnResourceChangeEvent OnFailedPick;
    public OnResourceChangeEvent OnRegrow;

    private string resourceName => Resource.ToString();

    [SerializeField]
    private int minAmount, maxAmount, minRegrowTime, maxRegrowTime;
    [SerializeField]
    private int currentAmount;

    private void Start()
    {
        if (currentAmount == 0)
        {
            StartCoroutine(WaitToRegrow());
        }
    }
    public void Configure(int minAmount, int maxAmount, int minRegrowTime, int maxRegrowTime) {
        this.minAmount = minAmount;
        this.maxAmount = maxAmount;
        this.minRegrowTime = minRegrowTime;
        this.maxRegrowTime = maxRegrowTime;
    }
    public void Regrow() {
        currentAmount = UnityEngine.Random.Range(minAmount, maxAmount + 1);
        OnRegrow.Invoke(currentAmount);
    }
    public void Pick()
    {
        if (currentAmount == 0)
        {
            OnFailedPick.Invoke(0);
            return;
        }

        if (!DataDump.Get<bool>(resourceName + "Discovered"))
        {
            DataDump.Set(resourceName + "Discovered", true);
        }
        DataDump.Set(resourceName, DataDump.Get<int>(resourceName) + currentAmount);
        OnPick.Invoke(currentAmount);
        currentAmount = 0;
        StartCoroutine(WaitToRegrow());
    }
    private void OnDestroy()
    {
        StopAllCoroutines();
    }
    private IEnumerator WaitToRegrow()
    {
        // todo: this (instead of handling on a server) allows for a cheat where the player reloads the webpage to regrow everything
        int waitSeconds = UnityEngine.Random.Range(minRegrowTime, maxRegrowTime);
        yield return new WaitForSeconds(waitSeconds);
        Regrow();
    }

    [Serializable]
    public class OnResourceChangeEvent : UnityEvent<int> { }
}
