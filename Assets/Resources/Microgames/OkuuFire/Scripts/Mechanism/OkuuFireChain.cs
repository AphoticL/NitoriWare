﻿using UnityEngine;

public class OkuuFireChain : MonoBehaviour, IOkuuFireMechanism
{
    // How far the chain can move.
    public float reach;

    // Size of the repeated texture.
    public float tileSize;
    
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.localPosition;
    }
    
    public void Move(float completion)
    {
        float move = Mathf.Abs(this.reach * completion);

        float newPosition = Mathf.Repeat(move, this.tileSize);
        this.transform.localPosition = this.startPosition + Vector3.up * newPosition;
    }
}
