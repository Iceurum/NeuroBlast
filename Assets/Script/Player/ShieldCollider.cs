using UnityEngine;

public class ShieldCollider : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EnemyBullet"))
            Destroy(other.gameObject);
    }
}
