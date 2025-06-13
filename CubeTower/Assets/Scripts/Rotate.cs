using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;

public class Rotate : MainMono
{
    public const float speed = 10f;
    private Transform _rotate;
    private void Start()
    {
        _rotate = GetComponent<Transform>();

    }
    private void Update()
    {
        _rotate.Rotate(0, speed*Time.deltaTime, 0);
    }
}
