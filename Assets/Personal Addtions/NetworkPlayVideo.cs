using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Video;




[RequireComponent(typeof(VideoPlayer))]
public class NetworkPlayVideo : NetworkBehaviour
{
    [Tooltip("Whether video should play on load")]
    [SerializeField] private bool playAtStart = false;

    [Tooltip("Material used for playing the video (Uses URP/Unlit by default)")]
    [SerializeField] private Material videoMaterial = null;

    [Tooltip("List of video clips to pull from")]
    [SerializeField] private List<VideoClip> videoClips = new List<VideoClip>();

    private VideoPlayer videoPlayer = null;
    private MeshRenderer meshRenderer = null;

    private readonly string shaderUsed = "Universal Render Pipeline/Unlit";

    private Material offMaterial = null;
    private int index = 0;
    private NetworkVariable<bool> Paused = new NetworkVariable<bool>(true,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);

    private void Awake()
    {
        Paused.OnValueChanged += VideoFeedClientRpc;
        meshRenderer = GetComponent<MeshRenderer>();
        videoPlayer = GetComponent<VideoPlayer>();

        if (videoClips.Count > 0)
            videoPlayer.clip = videoClips[0];

        offMaterial = meshRenderer.material;
    }
    public void flipPaused()
    {
        Paused.Value = !Paused.Value;
    }
    private void OnPausedChanged(bool previousValue, bool newValue)
    {
        if (!Paused.Value) {
            ApplyVideoMaterial(videoPlayer);
            videoPlayer.Play(); 
        }
        else {
            meshRenderer.material = offMaterial;
            videoPlayer.Stop();
        }
    }
    private void ApplyVideoMaterial(VideoPlayer source)
    {
        meshRenderer.material = videoMaterial;
        videoMaterial.color = Color.white; // allows video to be displayed - black material hides video
    }

    [ClientRpc]
    private void VideoFeedClientRpc(bool previousValue, bool newValue)
    {
        if (!Paused.Value)
        {
            ApplyVideoMaterial(videoPlayer);
            videoPlayer.Play();
        }
        else
        {
            meshRenderer.material = offMaterial;
            videoPlayer.Stop();
        }
    }
    private void OnValidate()
    {
        if (!videoMaterial)
        {
            videoMaterial = new Material(Shader.Find(shaderUsed));
        }

        if (TryGetComponent(out VideoPlayer videoPlayer))
            videoPlayer.targetMaterialProperty = "_BaseMap";
    }
}