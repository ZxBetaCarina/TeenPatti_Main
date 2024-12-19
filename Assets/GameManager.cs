using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
   
    public List<GameObject> playerPositions;

    public List<GameObject> playerAAA;

    public int no_PlayersEnableDistribution=3;
    public bool enableDistribution_Bool=false;
    public bool isCardDistributed=false;
    public Button cardDistributeBtn;
    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        cardDistributeBtn.onClick.AddListener(CardDistribute);
        
    }

    // Update is called once per frame
    public void CardDistribute()
    {
        if (!isCardDistributed)
        {
            Debug.Log("CardDistribute");
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNumberDistributor.instance.GenerateAndDistributeNumbers();
            }
            else
            {
                Debug.LogWarning("This client is not the Master Client, skipping number generation.");
            }
        }
        
    }
}
