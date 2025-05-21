using System.Globalization;
using UnityEngine;
using Unity.Netcode;

public class DeactiveIfNotOwner : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!IsOwner) 
            { gameObject.SetActive(false); }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
