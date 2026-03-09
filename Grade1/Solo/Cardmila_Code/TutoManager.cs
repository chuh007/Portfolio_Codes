using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutoManager : MonoBehaviour
{
    // 6: 경험치 바 : 최하단에 위치.\n적을 잡으면 경험치를 획득하며, 경험치 바가 가득 차면 레벨 업.\n레벨 업 시 카드 3장중 1장을 덱에 추가 할 수 있다.
    [SerializeField] TextMeshProUGUI textMeshProUGUI;
    [SerializeField] string[] texts;
    [SerializeField] Button gotoGame;
    int i = 0;
    private void Start()
    {
        gotoGame.gameObject.SetActive(false);
        textMeshProUGUI.text = texts[0].Replace("\\n","\n");
    }
    public void NextTxt()
    {
        if(i >= texts.Length-1)
        {
            gotoGame.gameObject.SetActive(true);
            return;
        }
        i++;
        textMeshProUGUI.text = texts[i].Replace("\\n", "\n");
    }
    public void BackTxt()
    {
        if (i <= 0) return;
        i--;
        textMeshProUGUI.text = texts[i].Replace("\\n", "\n");
    }
    public void GotoGame()
    {
        SceneManager.LoadScene(1);
    }
}
