using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReimuDodgeBullet : MonoBehaviour {
    [Header("How fast the bullet goes")]
    [SerializeField]
    private float speed = 1f;

    [Header("Firing delay in seconds")]
    [SerializeField]
    private float delay = 1f;

    private Vector2 traj;

	// Use this for initialization
	void Start () {
        Invoke("setTraj", delay);
	}
	
	// Update is called once per frame
	void Update () {
		if (traj != null)
        {
            Vector2 newPos = (Vector2)transform.position + (traj * Time.deltaTime * speed);
            transform.position = newPos;
        }
	}

    void setTraj()
    {
        GameObject plr = GameObject.Find("Player");
        traj = (plr.transform.position - transform.position).normalized;
    }
}
