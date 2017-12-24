using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReimuDodgePlayer : MonoBehaviour {

    [SerializeField]
    private AudioClip deathSound;

    private bool alive = true;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (alive)
        {
            kill();
        }
    }

    void kill()
    {
        alive = false;
        MicrogameController.instance.setVictory(victory: false, final: true);
        SpriteRenderer sprRdr = GetComponentInChildren<SpriteRenderer>();
        sprRdr.gameObject.SetActive(false);

        FollowCursor flwCsr = GetComponent<FollowCursor>();
        flwCsr.enabled = false;

        // Play the death sound effect
        // At a custom volume
        // And panned to the player's X Posision
        MicrogameController.instance.playSFX(deathSound, volume: 0.5f);

        ParticleSystem particleSystem = GetComponentInChildren<ParticleSystem>();
        particleSystem.Play();
    }
}
