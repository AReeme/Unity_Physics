using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public PlayerLife life;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerLife playerLife = collision.gameObject.GetComponent<PlayerLife>();
            if (playerLife != null)
            {
                playerLife.Die();
            }
        }
        Destroy(this.gameObject);
    }
}
