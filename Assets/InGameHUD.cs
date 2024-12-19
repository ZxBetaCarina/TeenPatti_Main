using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameHUD : MonoBehaviour
{
    public static InGameHUD Instance;

    [SerializeField] public TMP_Text totalPotMoneyTxt;    
    [SerializeField] public TMP_InputField inputBetMoney;
    [SerializeField] public TMP_Text betAmtTxt;
    public Button betBtn;
    public Button showBtn;
    public Button foldBtn;
    public int betAmt, totalBet;

    [SerializeField] public Sprite cardBG;
    [SerializeField] public List<Sprite> newSpriteList;

    public int startPoint=0;

    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        betAmt = 10;
        totalPotMoneyTxt.text = PhotonNetwork.PlayerList.Length * betAmt + "";
        betBtn.onClick.AddListener(OnBetCall);
        showBtn.onClick.AddListener(OnShowCall);
        foldBtn.onClick.AddListener(OnFoldCall);
    }
    private void Update()
    {
        betAmtTxt.text=betAmt.ToString();
    }
    public void OnBetCall()
    {
        int getInputBet = int.Parse(inputBetMoney.text);
        int detuctPotMny= betAmt;

        if (getInputBet >= betAmt)
        {
            betAmt= getInputBet;
            detuctPotMny = betAmt;
        }
         

        //string eventName = "bet";
        RoomManager.Instance.SendEventToAll(detuctPotMny, RoomManager.Instance.playerName, "bet");
    }

    public void OnShowCall()
    {
        int detuctPotMny = betAmt*2;
        RoomManager.Instance.SendEventToAll(detuctPotMny, RoomManager.Instance.playerName,"show");
    }
    public void OnFoldCall()
    {

    }
    public void UpdateTotalMnyTxt(int _val)
    {
        totalPotMoneyTxt.text = (int.Parse(totalPotMoneyTxt.text) + _val).ToString();
        Debug.Log("Total Money text call -- " + totalPotMoneyTxt.text);
    }
    public void UpdatePlayerMnyTxt(int _val, string _strPlayer)
    {
        PlayerController playerController = GameObject.Find(_strPlayer).GetComponent<PlayerController>();
        playerController.moneyText.text = (int.Parse(playerController.moneyText.text) - _val).ToString();
        Debug.Log("Player Money text call -- " + playerController.moneyText.text);
    }
}
