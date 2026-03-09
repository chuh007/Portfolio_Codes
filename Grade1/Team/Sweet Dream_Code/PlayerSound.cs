using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    public void PlaySound()
    {
        SoundManager.Instance.PlaySfx(SFXEnum.Move);
    }
}
