using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class Mag : MonoBehaviour
{
    private List<GameObject> bullets;
    public GameObject Itself;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (GameObject child in Itself.transform)
        {
            if(child.tag == "Bullets")
            {
                bullets.Add(child);
            }
        }
    }
    public List<GameObject> getBullets()
    {
        return bullets;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
