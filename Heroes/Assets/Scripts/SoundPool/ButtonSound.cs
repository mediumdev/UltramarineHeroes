using UnityEngine;
using Utils;

public class ButtonSound : MonoBehaviour
{
    [SerializeField] private AudioClip _buttonSound;

    public void PlayButtonSound()
    {
        if (DynamicDataManager.IsSoundEnabled())
            GetComponent<AudioSource>().PlayOneShot(_buttonSound);
    }
}
