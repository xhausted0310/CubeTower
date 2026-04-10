using UnityEngine;
using UnityEngine.SceneManagement;

namespace CubeTower.UI
{
    public class CanvasButton : MonoBehaviour
    {
        public void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void Insta()
        {
            Application.OpenURL("https://www.instagram.com/fi33y_/");
        }
    }
}
