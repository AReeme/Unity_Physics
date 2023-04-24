using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class turretScript : MonoBehaviour
{
    public float Range;
    public Transform Target;
    Vector2 direction;
    public GameObject alarmLight;
    public GameObject gun;
    public GameObject bullet;
    public float fireRate;
    float nextTimeToFire = 0;
    public Transform shootPoint;
    public float force;
    public PlayerLife life;
    private TurretState currentState = TurretState.Idle;
    private bool detected = false;

    private enum TurretState
    {
        Idle,
        Detected,
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject playerObject = GameObject.Find("Player");
        life = playerObject.GetComponent<PlayerLife>();
        TransitionToIdleState();
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case TurretState.Idle:
                UpdateIdleState();
                break;
            case TurretState.Detected:
                UpdateDetectedState();
                break;
        }
    }

    private void UpdateIdleState()
    {
        detected = false;
        alarmLight.GetComponent<SpriteRenderer>().color = Color.green;
        Vector2 targetPos = Target.position;
        direction = targetPos - (Vector2)transform.position;
        RaycastHit2D rayInfo = Physics2D.Raycast(transform.position, direction, Range);
        if (rayInfo && rayInfo.collider.gameObject.tag == "Player")
        {
            TransitionToDetectedState();
        }
    }

    private void UpdateDetectedState()
    {
        if (detected == false)
        {
            detected = true;
            alarmLight.GetComponent<SpriteRenderer>().color = Color.red;
        }

        Vector2 targetPos = Target.position;
        direction = targetPos - (Vector2)transform.position;
        gun.transform.up = direction;

        if (Time.time > nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1 / fireRate;
            Shoot();
        }

        RaycastHit2D rayInfo = Physics2D.Raycast(transform.position, direction, Range);
        if (!(rayInfo && rayInfo.collider.gameObject.tag == "Player"))
        {
            TransitionToIdleState();
        }
    }

    private void TransitionToIdleState()
    {
        currentState = TurretState.Idle;
    }

    private void TransitionToDetectedState()
    {
        currentState = TurretState.Detected;
    }

    void Shoot()
    {
        GameObject bulletIns = Instantiate(bullet, shootPoint.position, Quaternion.identity);
        BulletScript bulletScript = bulletIns.GetComponent<BulletScript>();
        bulletScript.life = life;
        bulletIns.GetComponent<Rigidbody2D>().AddForce(direction * force);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, Range);
    }
}
