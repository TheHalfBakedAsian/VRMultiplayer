using UnityEngine;
using UnityEngine.Video;
public class AlanRemotefix : MonoBehaviour
{
    [SerializeField] private AlanNetFix videoPlayer;
    public void OnRemoteActived()
    {
        if(videoPlayer != null)
        {
            Debug.Log("attempt play/pause");
            videoPlayer.RequestPlayRpc();
        }
        else
        {
            Debug.LogWarning("ASSIGN the VIDEOPLAYER");
        }
    }
}
