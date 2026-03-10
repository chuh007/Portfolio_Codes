using TMPro;
using UnityEngine;

public class EnemyCounter : MonoSingleton<EnemyCounter>
{
    public TextMeshProUGUI text;
    public bool isClear = false;

    public int enemyCount = 0;

    public void EnemyCountValueChange(int i)
    {
        enemyCount += i;
        text.text = enemyCount.ToString();
        if (enemyCount <= 0)
        {
            isClear = true;
        }
    }
}
