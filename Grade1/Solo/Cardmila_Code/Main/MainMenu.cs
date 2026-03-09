using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Image setting;
    [SerializeField] private Image whatStart;
    [SerializeField] private Image Fade;
    public void StartBtnCl()
    {
        whatStart.gameObject.SetActive(true);
    }
    public void RelStart()
    {
        StartCoroutine(StartFade());
    }
    private IEnumerator StartFade()
    {
        Fade.DOFade(1, 1);
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(1);
    }
    public void SettingBtnCl()
    {
        setting.gameObject.SetActive(true);
    }
    public void ExitBtnCl()
    {
        Application.Quit();
    }
    public void TutoStart()
    {
        StartCoroutine(TutoFade());
    }
    private IEnumerator TutoFade()
    {
        Fade.DOFade(1, 1);
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(2);
    }
    public void Back()
    {
        whatStart.gameObject.SetActive(false);
    }
}
