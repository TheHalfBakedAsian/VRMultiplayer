using UnityEngine;
using UnityEngine.Video;
public class AlanRemotefix : MonoBehaviour
{
    [SerializeField] private AlanNetFix videoPlayer;
    public void OnRemoteActived()
    {
        if(videoPlayer != null)
        {
            videoPlayer.RequestPlay();
        }
        else
        {
            Debug.LogWarning("ASSIGN the VIDEOPLAYER");
        }
    }
}
