using UnityEngine;

public class ButtonSFX : MonoBehaviour
{
    public void PlaySFX()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonSFX();
    }
}
