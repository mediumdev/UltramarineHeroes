using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioController : MonoBehaviour
{
    [Header("Tags")] [SerializeField] private string createdTag;
    
    private void Awake()
    {
        GameObject obj = GameObject.FindWithTag(createdTag);
        if (obj != null)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.tag = createdTag;
            DontDestroyOnLoad(gameObject);
        }
        
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            GetComponent<AudioSource>().mute = true;
        }
        else
        {
            GetComponent<AudioSource>().mute = false;
        }
    }
}
