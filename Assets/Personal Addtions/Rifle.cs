using NUnit.Framework;
using Oculus.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Rifle : MonoBehaviour
{
    private bool triggered;
    private byte triggerDebounce;
    private int BulletCount;
    public GameObject Room;
    public Mag Mag;
    public GameObject InnerBarrel;
    public GameObject BarrelExit;
    public float Force;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        triggered = false;
        triggerDebounce = 0;
    }
   public void ActivateTrigger()
    {
        triggered = true;
    }
    public void DeactivateTrigger()
    {
        triggered = false;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if(triggerDebounce >= 50)
        {
            triggerDebounce = 0;
        }
        if(triggered && triggerDebounce++ % 10 == 0 )
        {
            Vector3 barrelDirection = (BarrelExit.transform.position - InnerBarrel.transform.position).normalized;
            RaycastHit hit;
            if (Physics.Raycast(InnerBarrel.transform.position,barrelDirection ,out hit, 30.0f))
            {
                Console.WriteLine("it hit something");


                /*
                GameObject ShotFired = Mag.getBullets().ElementAt(BulletCount - 1) as GameObject;
                ShotFired.transform.SetParent(Room.transform);
                ShotFired.transform.position = BarrelExit.transform.position;
                ShotFired.GetComponent<Rigidbody>().AddForce(barrelDirection * Force, ForceMode.Impulse);
                */


                hit.rigidbody.AddForce(barrelDirection * Force, ForceMode.Impulse);
                


            }
        }
    }
    public void CheckMag()
    {
        BulletCount = Mag.getBullets().Count;
    }
}
