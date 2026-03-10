using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverUI;

    private void Awake()
    {
        Time.timeScale = 1.0f;
    }

    public void TimeOver()
    {
        Time.timeScale = 0f;
        gameOverUI.SetActive(true);
    }
}
