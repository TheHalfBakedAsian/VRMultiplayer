using Unity.Netcode;
using UnityEngine;

public class SpawnTennisRacket : NetworkBehaviour
{
    [SerializeField] private GameObject m_CurrentInteractable;
    [SerializeField] private Transform Spawn1;
    [SerializeField] private Transform Spawn2;

    private GameObject temp1;
    private GameObject temp2;

    public void pressedbutton()
    {
        if (!IsSpawned)
        {
            Debug.LogWarning("Spawner is not network spawned.");
            return;
        }

        PressedSummonRpc();
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void PressedSummonRpc()
    {
        if (m_CurrentInteractable == null)
        {
            Debug.LogWarning("m_CurrentInteractable is NULL");
            return;
        }

        if (Spawn1 == null || Spawn2 == null)
        {
            Debug.LogWarning("Spawn points not assigned");
            return;
        }

        DespawnExisting();

        temp1 = Instantiate(
            m_CurrentInteractable,
            Spawn1.position,
            Spawn1.rotation);

        temp2 = Instantiate(
            m_CurrentInteractable,
            Spawn2.position,
            Spawn2.rotation);

        NetworkObject netObj1 = temp1.GetComponent<NetworkObject>();
        NetworkObject netObj2 = temp2.GetComponent<NetworkObject>();

        if (netObj1 != null)
            netObj1.Spawn();

        if (netObj2 != null)
            netObj2.Spawn();

        Debug.Log("Spawned Objects");
    }

    private void DespawnExisting()
    {
        if (temp1 != null)
        {
            NetworkObject netObj = temp1.GetComponent<NetworkObject>();

            if (netObj != null && netObj.IsSpawned)
                netObj.Despawn(true);

            temp1 = null;
        }

        if (temp2 != null)
        {
            NetworkObject netObj = temp2.GetComponent<NetworkObject>();

            if (netObj != null && netObj.IsSpawned)
                netObj.Despawn(true);

            temp2 = null;
        }
    }
}