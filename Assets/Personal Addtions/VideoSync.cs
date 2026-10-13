using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Unity.Netcode;
using System.Collections;

/// <summary>
/// Networked VR Video Player handling clip indices, material swapping, 
/// and playback sync across all clients using Netcode for GameObjects.
/// </summary>
[RequireComponent(typeof(VideoPlayer))]
[RequireComponent(typeof(NetworkObject))]
public class VideoSync : NetworkBehaviour
{
    public enum PlaybackState { Stopped, Playing, Paused }

    [Tooltip("Whether video should play on load")]
    public bool playAtStart = false;

    [Tooltip("Material used for playing the video (Uses URP/Unlit by default)")]
    public Material videoMaterial = null;

    [Tooltip("List of video clips to pull from")]
    public List<VideoClip> videoClips = new List<VideoClip>();

    [Header("Network Drift Settings")]
    [SerializeField] private float driftThreshold = 0.5f; // Threshold in seconds to force snap sync
    [SerializeField] private float syncInterval = 5f;     // How often the host broadcasts current time

    private VideoPlayer videoPlayer = null;
    private MeshRenderer meshRenderer = null;
    private Material offMaterial = null;
    private readonly string shaderUsed = "Universal Render Pipeline/Unlit";

    // NetworkVariables automatically sync late-joiners to the current state
    private NetworkVariable<int> currentClipIndex = new NetworkVariable<int>(0);
    private NetworkVariable<PlaybackState> currentPlaybackState = new NetworkVariable<PlaybackState>(PlaybackState.Stopped);

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        videoPlayer = GetComponent<VideoPlayer>();
        offMaterial = meshRenderer.material;
    }

    private void OnEnable()
    {
        videoPlayer.prepareCompleted += ApplyVideoMaterial;
    }

    private void OnDisable()
    {
        videoPlayer.prepareCompleted -= ApplyVideoMaterial;
    }

    public override void OnNetworkSpawn()
    {
        // Hook up network data change callbacks for clients
        currentClipIndex.OnValueChanged += OnClipIndexChanged;
        currentPlaybackState.OnValueChanged += OnPlaybackStateChanged;

        if (IsServer)
        {
            // Host initializes state based on inspector config
            if (playAtStart && videoClips.Count > 0)
            {
                currentClipIndex.Value = 0;
                currentPlaybackState.Value = PlaybackState.Playing;
                ExecuteLocalPlayState(0, PlaybackState.Playing);
            }
            else
            {
                currentPlaybackState.Value = PlaybackState.Stopped;
                ExecuteLocalPlayState(0, PlaybackState.Stopped);
            }

            StartCoroutine(HostSyncLoop());
        }
        else
        {
            // Client late-join updates immediately to match current network state
            ExecuteLocalPlayState(currentClipIndex.Value, currentPlaybackState.Value);
        }
    }

    public override void OnNetworkDespawn()
    {
        currentClipIndex.OnValueChanged -= OnClipIndexChanged;
        currentPlaybackState.OnValueChanged -= OnPlaybackStateChanged;
    }

    // --- PUBLIC METHODS (Call these safely from your VR UI / Buttons) ---

    public void NextClip()
    {
        if (videoClips.Count == 0) return;
        RequestChangeIndexServerRpc((currentClipIndex.Value + 1) % videoClips.Count);
    }

    public void PreviousClip()
    {
        if (videoClips.Count == 0) return;

        // Fixed negative modulo behavior so index 0 wraps to the last clip smoothly
        int prevIndex = currentClipIndex.Value - 1;
        if (prevIndex < 0) prevIndex = videoClips.Count - 1;

        RequestChangeIndexServerRpc(prevIndex);
    }

    public void RandomClip()
    {
        if (videoClips.Count > 0)
        {
            int randIndex = Random.Range(0, videoClips.Count);
            RequestChangeIndexServerRpc(randIndex);
        }
    }

    public void PlayAtIndex(int value)
    {
        if (videoClips.Count > 0)
        {
            int clampedIndex = Mathf.Clamp(value, 0, videoClips.Count - 1);
            RequestChangeIndexServerRpc(clampedIndex);
        }
    }

    public void Play() => RequestStateServerRpc(PlaybackState.Playing);
    public void Stop() => RequestStateServerRpc(PlaybackState.Stopped);

    public void TogglePlayStop()
    {
        PlaybackState nextState = (currentPlaybackState.Value == PlaybackState.Playing) ? PlaybackState.Stopped : PlaybackState.Playing;
        RequestStateServerRpc(nextState);
    }

    public void TogglePlayPause()
    {
        PlaybackState nextState = (currentPlaybackState.Value == PlaybackState.Playing) ? PlaybackState.Paused : PlaybackState.Playing;
        RequestStateServerRpc(nextState);
    }

    public void SetPlay(bool value)
    {
        RequestStateServerRpc(value ? PlaybackState.Playing : PlaybackState.Stopped);
    }

    // --- NETCODE SERVER RPCs (Validates and updates states on host) ---

    [ServerRpc(RequireOwnership = false)]
    private void RequestChangeIndexServerRpc(int newIndex)
    {
        currentClipIndex.Value = newIndex;
        currentPlaybackState.Value = PlaybackState.Playing;

        // Notify all clients instantly via RPC to swap clips
        SyncPlayStateClientRpc(newIndex, PlaybackState.Playing);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestStateServerRpc(PlaybackState newState)
    {
        currentPlaybackState.Value = newState;
        SyncPlayStateClientRpc(currentClipIndex.Value, newState);
    }

    // --- NETCODE CLIENT RPCs (Executes instructions locally on devices) ---

    [ClientRpc]
    private void SyncPlayStateClientRpc(int clipIndex, PlaybackState state)
    {
        ExecuteLocalPlayState(clipIndex, state);
    }

    [ClientRpc]
    private void SyncTimeClientRpc(double hostTime)
    {
        if (IsServer) return; // Host skips sync with self

        // If a client drifts behind or ahead of host time by more than threshold, force-seek
        if (videoPlayer.isPlaying && System.Math.Abs(videoPlayer.time - hostTime) > driftThreshold)
        {
            videoPlayer.time = hostTime;
        }
    }

    // --- DATA MONITOR CALLBACKS ---

    private void OnClipIndexChanged(int oldIdx, int newIdx)
    {
        if (!IsServer) ExecuteLocalPlayState(newIdx, currentPlaybackState.Value);
    }

    private void OnPlaybackStateChanged(PlaybackState oldState, PlaybackState newState)
    {
        if (!IsServer) ExecuteLocalPlayState(currentClipIndex.Value, newState);
    }

    // --- LOCAL VIDEO ENGINE PLAYER UTILITIES ---

    private void ExecuteLocalPlayState(int clipIndex, PlaybackState state)
    {
        if (videoClips.Count == 0) return;

        // Verify bounds safety before matching index
        int safeIndex = Mathf.Clamp(clipIndex, 0, videoClips.Count - 1);

        // Assign clip if it has changed
        if (videoPlayer.clip != videoClips[safeIndex])
        {
            videoPlayer.clip = videoClips[safeIndex];
        }

        switch (state)
        {
            case PlaybackState.Playing:
                ApplyVideoMaterial(videoPlayer);
                videoPlayer.Play();
                break;

            case PlaybackState.Paused:
                meshRenderer.material = videoMaterial;
                videoPlayer.Pause();
                break;

            case PlaybackState.Stopped:
                meshRenderer.material = offMaterial;
                videoPlayer.Stop();
                break;
        }
    }

    private void ApplyVideoMaterial(VideoPlayer source)
    {
        meshRenderer.material = videoMaterial;
        if (videoMaterial != null)
        {
            videoMaterial.color = Color.white;
        }
    }

    private IEnumerator HostSyncLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(syncInterval);
            if (IsServer && videoPlayer.isPlaying)
            {
                SyncTimeClientRpc(videoPlayer.time);
            }
        }
    }

    private void OnValidate()
    {
        if (!videoMaterial)
        {
            videoMaterial = new Material(Shader.Find(shaderUsed));
        }

        if (TryGetComponent(out VideoPlayer targetPlayer))
        {
            targetPlayer.targetMaterialProperty = "_BaseMap";
        }
    }
}
