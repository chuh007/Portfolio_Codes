using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PruseM : MonoBehaviour
{
    [SerializeField] Image pruse;
    private void Start()
    {
        pruse.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pruse.gameObject.SetActive(true);
            Time.timeScale = 0;
        }
    }
    public void PruseExit()
    {
        Application.Quit();
    }
    public void PruseCancle()
    {
        pruse.gameObject.SetActive(false);
        Time.timeScale = 1;
    }
}
