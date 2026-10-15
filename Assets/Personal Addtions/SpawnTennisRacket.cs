using System;
using System.Numerics;
using Unity.Netcode;
using UnityEngine;

public class SpawnTennisRacket : NetworkBehaviour
{
    [SerializeField] private GameObject Tennis_Racket;
    [SerializeField] private Transform Spawn1;
    [SerializeField] private Transform Spawn2;
    private GameObject temp1;
    private GameObject temp2;

    public void PressedSummon()
    {
        SummonsRpc();
    }
    [Rpc(SendTo.Server)]
    private void SummonsRpc()
    {
        if(Tennis_Racket != null && Spawn1 != null && Spawn2 != null)
        {
            if(temp1 != null) { temp1.GetComponent<NetworkObject>().Despawn(); Destroy(temp1); }
            if(temp2 != null) { temp2.GetComponent<NetworkObject>().Despawn(); Destroy(temp2); }
            temp1 = Instantiate(Tennis_Racket, Spawn1.position,Spawn1.rotation);
            temp2 = Instantiate(Tennis_Racket, Spawn2.position, Spawn2.rotation);
            temp1.GetComponent<NetworkObject>().Spawn();
            temp2.GetComponent<NetworkObject>().Spawn();
        }
    }
}
