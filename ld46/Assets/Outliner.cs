using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Outliner : MonoBehaviour
{
    public Sprite[] frames;
    public float rate = 0.5f;
    private SpriteRenderer renderer;
    private float timer;
    private int index = 0;

    private void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            timer = rate;
            renderer.sprite = frames[index];
            index = (index + 1) % frames.Length;
        }
    }
}
