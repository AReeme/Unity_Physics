using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerLife : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private ControllerCharacter2D cc;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        cc = GetComponent<ControllerCharacter2D>();
        cc.respawnPoint = transform.position;
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
        Invoke("RestartLevel", 1f);
    }

    private void RestartLevel()
    {
        PlayerPrefs.SetFloat("SavedTime", FindObjectOfType<GameManager>().timer);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
