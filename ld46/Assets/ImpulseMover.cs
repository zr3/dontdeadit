using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpulseMover : MonoBehaviour
{
    public void MoveRight(float amount)
    {
        transform.Translate(new Vector3(amount, 0));
    }
    public void MoveUp(float amount)
    {
        transform.Translate(new Vector3(0, amount));
    }

    public void GoHome()
    {
        transform.position = Vector3.zero;
    }

    public static void GoHomeHack()
    {
        GameObject.Find("nav")?.GetComponent<ImpulseMover>().GoHome();
    }
}
