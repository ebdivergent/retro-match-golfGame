using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] _sound;
    bool isPlay;

    void Start()
    {
        isPlay = true;
        ActiveMusic();
        ActiveSound();
        BackMusic();

    }


    public void EffectHit()
    {
        AppCore.AudioController.Instance.Play(_sound[2]);
    }
    public void EffectScore()
    {
        AppCore.AudioController.Instance.Play(_sound[3]);
    }
    public void EffectMissHealth()
    {
        AppCore.AudioController.Instance.Play(_sound[4]);
    }
    public void EffectLose()
    {
        AppCore.AudioController.Instance.Play(_sound[5]);
    }

    //---UI settings---//
    public void EffectClick()
    {
        AppCore.AudioController.Instance.Play(_sound[0]);
    }

    public void BackMusic()
    {
        if(isPlay)
        {
            AppCore.DataManager.Instance.DataContainer.VolumeMusic = 0.2f;
            AppCore.AudioController.Instance.PlayMusic(_sound[1]);
        }
         
    }
    public void DisActiveSound()
    {
        AppCore.DataManager.Instance.DataContainer.VolumeSound = 0f;
    }
    public void ActiveSound()
    {
        AppCore.DataManager.Instance.DataContainer.VolumeSound = 1f;
    }
    public void DisActiveMusic()
    {
        AppCore.AudioController.Instance.PauseMusic();
        isPlay = false;
    }
    public void ActiveMusic()
    {
        isPlay = true;
        AppCore.DataManager.Instance.DataContainer.VolumeMusic = 0.2f;
    }
}
