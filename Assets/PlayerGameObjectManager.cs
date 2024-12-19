using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerGameObjectManager : MonoBehaviourPunCallbacks
{
    // Dictionary to map Photon players to their GameObjects
    public Dictionary<Player, GameObject> playerGameObjectMap = new Dictionary<Player, GameObject>();

    // List to store GameObjects
    public List<GameObject> playerGameObjectList = new List<GameObject>();

    // Initialize and populate for existing players in the room
    private void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            AddExistingPlayers(); // Add players already in the room
            PopulateGameObjectList(); // Populate the list with their GameObjects
        }
    }

    // Add all existing players in the room to the dictionary
    private void AddExistingPlayers()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (!playerGameObjectMap.ContainsKey(player))
            {
                GameObject playerObject = CreatePlayerGameObject(player);
                playerGameObjectMap[player] = playerObject;
                Debug.Log($"Added existing Player: {player.NickName}, UserId: {player.UserId}");
            }
        }
    }

    // Create a new GameObject for a player
    private GameObject CreatePlayerGameObject(Player player)
    {
        GameObject playerObject = new GameObject($"{player.NickName}_{player.UserId}");
        // You can add components, assign properties, or parent the object as needed
        return playerObject;
    }

    // Populate the GameObject list from the dictionary
    public void PopulateGameObjectList()
    {
        playerGameObjectList.Clear();

        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (playerGameObjectMap.TryGetValue(player, out GameObject playerObject))
            {
                playerGameObjectList.Add(playerObject);
                Debug.Log($"Added {playerObject.name} to the GameObject list for Player: {player.NickName}");
            }
            else
            {
                Debug.LogWarning($"No GameObject found for Player: {player.NickName}, UserId: {player.UserId}");
            }
        }

        Debug.Log("Final GameObject List:");
        foreach (var obj in playerGameObjectList)
        {
            Debug.Log($"GameObject Name: {obj.name}");
        }
    }

    // Handle new player joining the room
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Player entered: {newPlayer.NickName}");
        if (!playerGameObjectMap.ContainsKey(newPlayer))
        {
            GameObject newPlayerObject = CreatePlayerGameObject(newPlayer);
            playerGameObjectMap[newPlayer] = newPlayerObject;
            Debug.Log($"Created and added GameObject for Player: {newPlayer.NickName}");
        }

        PopulateGameObjectList(); // Update the list with the new player's GameObject
    }

    // Handle player leaving the room
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (playerGameObjectMap.TryGetValue(otherPlayer, out GameObject playerObject))
        {
            playerGameObjectMap.Remove(otherPlayer);
            Destroy(playerObject);
            Debug.Log($"Removed GameObject for Player: {otherPlayer.NickName}");
        }

        PopulateGameObjectList(); // Update the list after removing the player's GameObject
    }
}
