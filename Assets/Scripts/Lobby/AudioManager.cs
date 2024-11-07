using System.ComponentModel.Design.Serialization;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource audioSFX;
    [SerializeField] private AudioClip telePort;
    [SerializeField] private AudioClip rollCube;
    [SerializeField] private AudioClip clickButton;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    public void PlaySFX(SoundEffect effect)
    {
        switch (effect)
        {
            case SoundEffect.RollCube:
                audioSFX.PlayOneShot(rollCube);
                break;
            case SoundEffect.Teleport:
                audioSFX.PlayOneShot(telePort);
                break;
            case SoundEffect.ClickButton:
                audioSFX.PlayOneShot(clickButton);
                break;
            default:
                break;
        }
    }
   
}
 public enum SoundEffect
    {
        RollCube,
        ClickButton,
        Teleport
    }