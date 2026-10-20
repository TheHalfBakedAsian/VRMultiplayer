using Unity.Netcode;
using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class AlanNetFix : NetworkBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private VideoClip clip;
    public override void OnNetworkSpawn()
    {
        if(videoPlayer == null) { videoPlayer = GetComponent<VideoPlayer>(); }
        if (clip == null && videoPlayer != null)
        {
            videoPlayer.clip = clip;
        }
        if (videoPlayer != null)
        {
            videoPlayer.playOnAwake = false;
        }

        base.OnNetworkSpawn();
    }
    public void RequestPlay()
    {
        if (!IsSpawned)
        {
            Debug.LogWarning("Tv Obj not spawned");
        }
        PlayVideoRpc();
    }
    [Rpc(SendTo.ClientsAndHost)]
    private void PlayVideoRpc()
    {
        if (!videoPlayer.isPlaying)
        {

            if (videoPlayer == null)
            {
               Debug.LogWarning("Videoplayer not assigned");
            }
            if (clip != null)
            {
                videoPlayer.clip = clip;
            }
            videoPlayer.Play();
            }
        else if (videoPlayer.isPaused)
        {
            videoPlayer.Play();
        }
        else
        {
            videoPlayer.Pause();
        }
        
    }
}
