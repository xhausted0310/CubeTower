using UnityEngine;
using UnityEngine.SceneManagement;
public class CanvasButton : MonoBehaviour
{
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void Insta()
    {
        Application.OpenURL("https://www.instagram.com/realxhausted/");
    }
}
