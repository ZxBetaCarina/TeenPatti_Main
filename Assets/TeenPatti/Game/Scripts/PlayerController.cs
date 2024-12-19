using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

//[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviourPunCallbacks
{
  //[SerializeField] private float playerSpeed = 2.0f;
  //[SerializeField] private float jumpHeight = 1.0f;
 // [SerializeField] private float gravityValue = -9.81f;

  //private CharacterController controller;
  private PhotonView pv;
    // private Vector3 playerVelocity;
    // private bool groundedPlayer;
    //private Transform cameraTransform;
    public string name;
    public List<int> cardsList;
    public int money;

    [SerializeField]
    public TMP_Text nameText;
    public TMP_Text moneyText;
    public SpriteRenderer sprite1; 
    public SpriteRenderer sprite2; 
    public SpriteRenderer sprite3;
    [SerializeField] public TMP_Text blindShowTxt;
    public Toggle showCardToggle;
    public bool isCardShow=false;

    private void Start() {

        name=photonView.Owner.NickName;
        this.gameObject.name= photonView.Owner.NickName;
        // controller = GetComponent<CharacterController>();
        pv = GetComponent<PhotonView>();
        nameText.text = name;

        money = 1000;
        moneyText.text=money.ToString();

        if (pv.IsMine)
        {

            showCardToggle.gameObject.SetActive(true);
        }
        else
        {
            showCardToggle.gameObject.SetActive(false);
        }

  }
    void UpdateMoney(int val)
    {
        moneyText.text = val.ToString();
    }

    public void UpdatePlayerCardsImage() {
        Debug.Log("UpdatePlayerCardsImage---");
        if (isCardShow)
        {
            sprite1.sprite = InGameHUD.Instance.newSpriteList[cardsList[0]];
            sprite2.sprite = InGameHUD.Instance.newSpriteList[cardsList[1]];
            sprite3.sprite = InGameHUD.Instance.newSpriteList[cardsList[2]];
        }
        else
        {
            if (pv.IsMine && isCardShow)
            {
                sprite1.sprite = InGameHUD.Instance.newSpriteList[cardsList[0]];
                sprite2.sprite = InGameHUD.Instance.newSpriteList[cardsList[1]];
                sprite3.sprite = InGameHUD.Instance.newSpriteList[cardsList[2]];
            }
            else {
                sprite1.sprite = InGameHUD.Instance.cardBG;
                sprite2.sprite = InGameHUD.Instance.cardBG;
                sprite3.sprite = InGameHUD.Instance.cardBG;
            }
        }
        
    }
    public void OnShowCardToggle()
    {
        isCardShow=showCardToggle.isOn;
        UpdatePlayerCardsImage();
    }
    void Update() {
    if (!pv.IsMine) {
      // This isn't our player, so ignore controls and let Photon update their movement
      return;
    }
  
  }
}
