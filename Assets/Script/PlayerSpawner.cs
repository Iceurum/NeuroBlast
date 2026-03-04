using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    void Start()
    {
        Instantiate(playerPrefab, transform.position, Quaternion.identity);
    }
}
