using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnTrigger : MonoBehaviour
{
    private GameManager gameManager;
    private PlayerLife pl;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        pl = FindObjectOfType<PlayerLife>();
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.position = new Vector3(-44.73f, -20.19f, 0f); // Reset player position to (0, 0, 0)
            gameManager.IncrementDeathCount();
            pl.deathSound.Play();
        }
    }
}
