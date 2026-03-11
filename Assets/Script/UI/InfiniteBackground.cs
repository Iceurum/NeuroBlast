using UnityEngine;

public class InfiniteBackground : MonoBehaviour
{
    [Header("Settings")]
    public float scrollSpeed = 2f;          // kecepatan scroll ke kiri
    public float bgWidth = 20f;             // lebar satu background (sesuaikan dengan sprite)

    private Transform[] backgrounds;        // array 2 background child
    private float resetPositionX;           // posisi X untuk reset

    // ===================== LIFECYCLE =====================

    void Start()
    {
        // Ambil semua child sebagai background panels
        backgrounds = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
            backgrounds[i] = transform.GetChild(i);

        // Posisi reset = lebar background
        resetPositionX = bgWidth;
    }

    void Update()
    {
        // Gerakkan semua background ke kiri
        foreach (Transform bg in backgrounds)
        {
            bg.Translate(Vector3.left * scrollSpeed * Time.deltaTime);

            // Kalau sudah keluar layar kiri, pindah ke kanan
            if (bg.position.x <= -resetPositionX)
            {
                RepositionBackground(bg);
            }
        }
    }

    // ===================== REPOSITION =====================

    void RepositionBackground(Transform bg)
    {
        // Cari background paling kanan
        float maxX = float.MinValue;
        foreach (Transform other in backgrounds)
        {
            if (other != bg && other.position.x > maxX)
                maxX = other.position.x;
        }

        // Pindah ke kanan background paling kanan
        bg.position = new Vector3(maxX + bgWidth, bg.position.y, bg.position.z);
    }

    // ===================== PUBLIC =====================

    // Pause scroll saat game pause
    public void SetScrolling(bool active)
    {
        scrollSpeed = active ? scrollSpeed : 0f;
    }
}
