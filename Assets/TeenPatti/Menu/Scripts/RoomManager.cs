using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.InputSystem.Controls;
using System;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;
    public List<GameObject> playersList;
    public List<GameObject> playerGameObjects = new List<GameObject>();
    public string playerName;
    private const byte MoneyUpdateEventCode = 1;
    private const byte IntStartEventCode = 1;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    void Start()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEventReceived;
        PhotonNetwork.NetworkingClient.EventReceived += OnEventIntStartReceived;
    }

    void OnDestroy()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEventReceived;
        PhotonNetwork.NetworkingClient.EventReceived -= OnEventIntStartReceived;
    }

    public void SendEventToAll(int money, string playerName, string eventName)
    {
        Debug.Log("--- OnBetMoneyOnTable ----");
        object[] data = new object[] { money, playerName, eventName };

        // Raise an event
        PhotonNetwork.RaiseEvent(
            MoneyUpdateEventCode,            // Event code
            data,                            // Data to send
            new RaiseEventOptions
            {
                Receivers = ReceiverGroup.All // Send to all players in the room
            },
            SendOptions.SendReliable          // Ensure reliable delivery
        );
    }
    // Method to send an integer value
    public void SendIntStartToRoom(int value)
    {
        object[] content = new object[] { value }; // Pack the integer into an object array.
        RaiseEventOptions options = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All // Send to all players in the room.
        };
        SendOptions sendOptions = new SendOptions
        {
            Reliability = true // Ensure the event is reliably sent.
        };

        PhotonNetwork.RaiseEvent(IntStartEventCode, content, options, sendOptions);
    }


    private void OnEventReceived(EventData photonEvent)
    {
        if (photonEvent.Code == MoneyUpdateEventCode) // Check for the correct event code
        {
            object[] data = (object[])photonEvent.CustomData; // Extract data

            int money = (int)data[0];
            string playerName = (string)data[1];
            string eventName = (string)data[2];

            // Handle the data (e.g., display it or update UI)
            Debug.Log($"Player--- {playerName}---- sent *** ${money} -- event *** ${eventName}");
            switch (eventName)
            {
                case "bet":
                    Debug.Log($"event Name--- bet");
                    InGameHUD.Instance.betAmt = money;
                    break;
                case "show":
                    Debug.Log($"event Name--- show");
                    break;

            }
            InGameHUD.Instance.UpdateTotalMnyTxt(money);
            InGameHUD.Instance.UpdatePlayerMnyTxt(money, playerName);
        }
    }


    // Method to handle received events
    private void OnEventIntStartReceived(EventData photonEvent)
    {
        if (photonEvent.Code == IntStartEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            int receivedValue = (int)data[0]; // Extract the integer value from the event data.

            Debug.Log("Received integer value: " + receivedValue);
            // Handle the received value as needed.
        }
    }


    void Update()
    {

    }


    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
    }


    public void DebugGameObjectList()
    {
        Debug.Log("Current List of Player GameObjects:");

        foreach (GameObject obj in playerGameObjects)
        {
            Debug.Log($"GameObject Name: {obj.name}, Position: {obj.transform.position}");
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {

        // You can get the player's position in the player list (index)
        int playerIndex = PhotonNetwork.PlayerList.Length;

        Debug.Log("Player entered111 Name : " + newPlayer.NickName + "   total no of player ---  " + PhotonNetwork.PlayerList.Length);
        InGameHUD.Instance.totalPotMoneyTxt.text = (InGameHUD.Instance.betAmt * PhotonNetwork.PlayerList.Length).ToString();

        DebugGameObjectList();

    }



    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Master Client generates and distributes numbers
            // PhotonNumberDistributor.instance.GenerateAndDistributeNumbers();
        }
        if (scene.buildIndex == 1)
        {
            // This is the game scene
            GameObject g = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), Vector3.zero, Quaternion.identity);
            int playerIndex = PhotonNetwork.PlayerList.Length;  // The last player is at index (length - 1)


            //g.name = PhotonNetwork.PlayerList[playerIndex - 1].NickName.ToString();
            g.transform.position = GameManager.Instance.playerPositions[playerIndex - 1].gameObject.transform.position;
            playerGameObjects.Add(g);

            Debug.Log("Player entered00000: " + g.name + " totalPotMoneyTxt: " + InGameHUD.Instance.totalPotMoneyTxt.text);

        }
    }
}
