using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerLife : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private ControllerCharacter2D cc;
    private GameManager gameManager;
    public AudioSource deathSound;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        cc = GetComponent<ControllerCharacter2D>();
        cc.respawnPoint = transform.position;
        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Trap"))
        {
            Die();
        }
    }

    public void Die()
    {
        rb.bodyType = RigidbodyType2D.Static;
        anim.SetTrigger("death");
        gameManager.IncrementDeathCount();
        deathSound.Play();
        Invoke("Respawn", 1f);
    }

    public void Respawn()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        transform.position = cc.respawnPoint;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }
}
