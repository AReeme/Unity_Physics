using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    [SerializeField] GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Trigger Entered");
            gameManager.SetWin();
        }
    }
}
