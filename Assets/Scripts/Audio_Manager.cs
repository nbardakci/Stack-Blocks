using UnityEngine;

public class Audio_Manager : MonoBehaviour
{
    [Header(" Settings ")]
    [SerializeField] private AudioClip gameOver, missedBlock, allBlockOnTarget, blockDestroy;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayAudio(string action)
    {
        AudioClip audioClip = null;

        switch (action)
        {
            case "GameOver":
                audioClip = gameOver;
                break;
            
            case "MissedBlock":
                audioClip = missedBlock;
                break;  
                
            case "AllBlockOnTarget":
                audioClip = allBlockOnTarget;
                break;   
                
            case "BlockDestroy":
                audioClip = blockDestroy;
                break;

            default:
                //
                break;
        }

        if (audioClip != null) 
            audioSource.PlayOneShot(audioClip);

    }

}