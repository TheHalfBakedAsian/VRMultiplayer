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
        if (clip != null && videoPlayer != null)
        {
            videoPlayer.clip = clip;
            videoPlayer.waitForFirstFrame = true;
            videoPlayer.sendFrameReadyEvents = true;
            videoPlayer.frameReady += (vp, frame) =>
            {
                if (frame < 10)
                    Debug.Log($"Frame {frame}");
            };
        }
        if (videoPlayer != null)
        {
            videoPlayer.playOnAwake = false;
        }

        base.OnNetworkSpawn();
    }
    [Rpc(SendTo.Server)]
    public void RequestPlayRpc()
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
        if (videoPlayer == null)
            {
               Debug.LogWarning("Videoplayer not assigned");
               return;
            }
        else if (!videoPlayer.isPlaying)
        {

            if (clip != null)
            {
                videoPlayer.clip = clip;
            }
            videoPlayer.Play();
            }
        
        else
        {
            videoPlayer.Pause();
        }
        
    }
}
