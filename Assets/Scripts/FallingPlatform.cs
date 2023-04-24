using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    Rigidbody2D rb;
    Vector3 initialPosition;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        initialPosition = transform.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name.Equals("Player"))
        {
            StartCoroutine(DropPlatformAndResetAfterDelay(0.5f, 2f));
        }
    }

    IEnumerator DropPlatformAndResetAfterDelay(float dropDelay, float resetDelay)
    {
        yield return new WaitForSeconds(dropDelay);
        rb.isKinematic = false;
        yield return new WaitForSeconds(resetDelay);
        ResetPlatform();
    }

    void ResetPlatform()
    {
        Debug.Log("Platform reset");
        transform.position = initialPosition;
        rb.isKinematic = true;
    }
}

