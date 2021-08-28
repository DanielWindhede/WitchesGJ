using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hover : MonoBehaviour
{
    public float speed = 0.5f;
    public float amount = 0.3f;
    public float offset = 0f;

    private float initialYPosition;
    private Vector3 position;

    void Start()
    {
        position = transform.position;
        initialYPosition = position.y;
    }

    void Update()
    {
        position.y = initialYPosition + Mathf.Sin(Time.time * speed + offset) * amount;
        transform.position = position;
    }
}
