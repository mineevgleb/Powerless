using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStarter : MonoBehaviour
{
    public Button startBtn;

    void Start ()
    {
        startBtn.onClick.AddListener(StartGame);
    }
    
    private void StartGame () {
        SceneManager.LoadScene("Gameplay");
    }
}
