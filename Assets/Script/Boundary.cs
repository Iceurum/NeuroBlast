using UnityEngine;

public class EnemyBoundary : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
{
    EnemyBase enemy = other.GetComponent<EnemyBase>();

    if (enemy != null)
    {
      //  enemy.Die(false);
    }
}
}