using UnityEngine;
using Unity.Netcode;
public class RemotetoggleTV : NetworkBehaviour
{
    public PlayVideo screen;
    private NetworkVariable<bool> Paused = new NetworkVariable<bool>(true,
     NetworkVariableReadPermission.Everyone,
     NetworkVariableWritePermission.Owner);
    [Rpc(SendTo.Server)]
    public void xyzRpc(bool previousValue, bool newValue)
    {
        screen.TogglePlayPause();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Paused.OnValueChanged += xyzRpc;
    }
    public void flipPaused()
    {
        Paused.Value = !Paused.Value;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
