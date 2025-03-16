using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MainGameManagerScript : MonoBehaviour
{
   public void OnServerButtonClick()
   {
      NetworkManager.Singleton.StartServer();
   }
   public void OnHostButtonClick()
   {
      NetworkManager.Singleton.StartHost();
   }
   public void OnClientButtonClick()
   {
      NetworkManager.Singleton.StartClient();
   }
}
