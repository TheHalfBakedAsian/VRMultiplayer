using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using XRMultiplayer;
[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(ClientNetworkTransform))]
[RequireComponent(typeof(NetworkPhysicsInteractable))]
[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(NetworkRigidbody))]
public class NetworkGrab : MonoBehaviour
{
    void Start()
    {
        this.GetComponent<NetworkObject>().DontDestroyWithOwner = true;
        this.GetComponent<NetworkPhysicsInteractable>().spawnLocked = false;
    }

    void Update()
    {
        
    }
    private void OnValidate()
    {
        this.GetComponent<NetworkObject>().DontDestroyWithOwner = true;
        this.GetComponent<NetworkPhysicsInteractable>().spawnLocked = false;
    }
}
