using UnityEngine;

public class EnemyBoundary : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        EnemyBase enemy = other.GetComponent<EnemyBase>();

        if (enemy != null)
        {
            // Tambah breach meter sesuai nilai enemy
            if (GameManager.Instance != null)
                GameManager.Instance.AddBreachMeter(enemy.breachValue);

            // Panggil OnEscaped via GameManager counter
            GameManager.Instance?.AddEnemyEscaped();

            // Destroy enemy tanpa trigger Die() supaya tidak tambah score
            enemy.ForceDestroy();
        }
    }
}