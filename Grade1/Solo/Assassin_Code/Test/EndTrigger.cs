using UnityEngine;
using UnityEngine.SceneManagement;

public class EndTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!EnemyCounter.Instance.isClear) return;
        SceneManager.LoadScene("EndScene");
    }
}
