using System;
using System.Numerics;
using Unity.Netcode;
using UnityEngine;
using XRMultiplayer;

public class SpawnTennisRacket : NetworkBehaviour
{
    [SerializeField] private GameObject m_CurrentInteractable;
    [SerializeField] private Transform Spawn1;
    [SerializeField] private Transform Spawn2;
    [SerializeField] private NetworkManager TheMan;
    private GameObject temp1;
    private GameObject temp2;
    NetworkVariable<NetworkObjectReference> m_CurrentInteractableReference = new NetworkVariable<NetworkObjectReference>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public void PressedSummon()
    {
        SummonRpc();
    }
    [Rpc(SendTo.Server)]
    private void SummonRpc()
    {
        if (m_CurrentInteractable != null && Spawn1 != null && Spawn2 != null)
        {
            if (temp1 != null) { temp1.GetComponent<NetworkObject>().Despawn(); Destroy(temp1); }
            if (temp2 != null) { temp2.GetComponent<NetworkObject>().Despawn(); Destroy(temp2); }
            temp1 = Instantiate(m_CurrentInteractable, Spawn1.position, Spawn1.rotation);
            temp2 = Instantiate(m_CurrentInteractable, Spawn2.position, Spawn2.rotation);
            temp1.GetComponent<NetworkObject>().Spawn();
            temp2.GetComponent<NetworkObject>().Spawn();

        }
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        m_CurrentInteractableReference.OnValueChanged += OnCurrentInteractableReferenceChanged;
        NetworkManager.Singleton.OnSessionOwnerPromoted += SessionOwnerPromoted;

        UpdateCurrentInteractable();
    }
    void OnCurrentInteractableReferenceChanged(NetworkObjectReference previous, NetworkObjectReference current)
    {
        UpdateCurrentInteractable();
    }
    void UpdateCurrentInteractable()
    {
        if (!m_CurrentInteractableReference.Value.TryGet(out NetworkObject interactable) || !interactable.TryGetComponent(out m_CurrentInteractable))
        {
            m_CurrentInteractable = null;
        }
    }
    void SessionOwnerPromoted(ulong sessionOwnerId)
    {
        if (sessionOwnerId == NetworkManager.Singleton.LocalClientId)
        {
            UpdateCurrentInteractable();
        }
    }
}
