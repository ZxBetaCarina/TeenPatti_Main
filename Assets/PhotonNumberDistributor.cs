using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UIElements;
using System.Linq;

public class PhotonNumberDistributor : MonoBehaviourPunCallbacks
{
    public static PhotonNumberDistributor instance;
    private Dictionary<string, List<int>> playerNumbers = new Dictionary<string, List<int>>();
    public List<string> playerIDs = new List<string>();
    public List<string> playersName = new List<string>();
    public bool isCall = false;
    [SerializeField]
    private Dictionary<string, List<int>> recivePlayersCardNumbers = new Dictionary<string, List<int>>();
    void Awake()
    {
        instance = this;
    }


    public void GenerateAndDistributeNumbers()
    {
        isCall = true;

        if (!PhotonNetwork.IsMasterClient) return;

        List<int> numbers = new List<int>();
        while (numbers.Count < 18)
        {
            int randomNum = Random.Range(0, 51);
            if (!numbers.Contains(randomNum))
            {
                numbers.Add(randomNum);
            }
        }

        ShuffleList(numbers);

        foreach (var player in PhotonNetwork.PlayerList)
        {
            string userId = string.IsNullOrEmpty(player.UserId) ? System.Guid.NewGuid().ToString() : player.UserId;

            playerIDs.Add(userId);
            playersName.Add(player.NickName);

            playerNumbers[player.NickName] = new List<int>();
        }

        int index = 0;

        foreach (var playerName in playersName)
        {
            for (int i = 0; i < 3; i++)
            {
                playerNumbers[playerName].Add(numbers[index]);
                index++;
            }
        }

        foreach (var playerName in playersName)
        {
            photonView.RPC("ReceiveNumbers", RpcTarget.AllBuffered, playerName, playerNumbers[playerName].ToArray());
        }
        isCall = false;
    }

    [PunRPC]
    void ReceiveNumbers(string playerID, int[] numbers)
    {
        recivePlayersCardNumbers.Add(playerID, numbers.ToList());
        Debug.Log($"[RPC Received] LocalPlayer UserId: {PhotonNetwork.LocalPlayer.NickName}, Target PlayerId: {playerID}: {string.Join(", ", numbers)}");

        if (PhotonNetwork.LocalPlayer.UserId == playerID)
        {
            Debug.Log($"[RPC Success] Received numbers for {playerID}: {string.Join(", ", numbers)}");
        }
        PrintDictionaryValues();
    }
    private void PrintDictionaryValues()
    {
        foreach (KeyValuePair<string, List<int>> player in recivePlayersCardNumbers)
        {
            string cardNumbers = string.Join(", ", player.Value);
            Debug.Log($"Player: {player.Key}, Cards: {cardNumbers}");
            PlayerController playerController = GameObject.Find(player.Key).GetComponent<PlayerController>();
            playerController.cardsList = player.Value;
            UpdateTeenPatiGamePlayersData(player.Key, player.Value);
            playerController.UpdatePlayerCardsImage();
        }
        InGameHUD.Instance.startPoint = Random.Range(0,PhotonNetwork.PlayerList.Length-1);
    }

    private void UpdateTeenPatiGamePlayersData(string key, List<int> value)
    {
        // Example: A, 2, 3 (Hearts)
        TeenPattiGame.Instance.playersT.Add(new PlayerTeenPatti(key, value.ToArray()));
        Debug.Log("i ammm donkey");
       // TeenPattiGame.Instance.DisplayWinner();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (var entry in playerNumbers)
            {
                Debug.Log($"Sending numbers to late joiner: {newPlayer.NickName}");
                photonView.RPC("ReceiveNumbers", newPlayer, entry.Key, entry.Value.ToArray());
            }
        }

    }

    void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
}
