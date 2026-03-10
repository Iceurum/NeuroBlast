using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject creditPanel;

    public void StartGame()
    {
        GameManager.Instance.StartGame();
    }

    public void OpenCredit()
    {
        mainPanel.SetActive(false);
        creditPanel.SetActive(true);
    }

    public void CloseCredit()
    {
        creditPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Closed");
    }
}