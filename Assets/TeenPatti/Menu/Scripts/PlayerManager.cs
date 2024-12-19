using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PlayerManager : MonoBehaviour {
  PhotonView pv;

  private void Awake() {
    pv = GetComponent<PhotonView>();
        this.gameObject.name = RoomManager.Instance.playerName;
  }

  private void Start() {
    if (pv.IsMine) {
      CreateController();
    }
        if (GameManager.Instance.no_PlayersEnableDistribution == PhotonNetwork.PlayerList.Length)
        { 
            GameManager.Instance.enableDistribution_Bool=true;
        }

        if (GameManager.Instance.enableDistribution_Bool == true)
        {
            GameManager.Instance.cardDistributeBtn.gameObject.SetActive(true);
        }
  }

  private void CreateController() {
   
        //GameObject g= PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), Vector3.zero, Quaternion.identity);
        
        



    }
}
