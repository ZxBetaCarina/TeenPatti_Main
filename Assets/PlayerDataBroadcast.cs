using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerDataBroadcast : MonoBehaviourPunCallbacks
{
    // This function is called when a new player joins the room
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // You can call an RPC to broadcast the new player's data
        BroadcastPlayerDataToAll(newPlayer);
    }

    // This is an RPC method to send player data to all clients
    [PunRPC]
    public void BroadcastPlayerDataToAll(Player newPlayer)
    {
        string playerName = newPlayer.NickName;  // or any other player data you want
        int playerID = newPlayer.ActorNumber;

        // Send data to all players (you can extend this to send other player data)
        Debug.Log("New player joined: " + playerName + " (ID: " + playerID + ")");

        // Now broadcast to all clients (e.g., display it in the UI)
        // You can trigger UI updates or other actions based on this data
    }
}