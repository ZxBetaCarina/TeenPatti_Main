using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;

public class SeatAssignment : MonoBehaviourPunCallbacks
{
    public List<int> availableSeats = new List<int>(); // List of available seats
    private Dictionary<string, int> playerSeats = new Dictionary<string, int>(); // Tracks player-to-seat mapping

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            InitializeSeats();
        }

        if (photonView.IsMine)
        {
            AssignLocalSeat();
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(AssignSeatAfterDelay(newPlayer));
        }
    }

    private IEnumerator AssignSeatAfterDelay(Player player)
    {
        yield return new WaitForSeconds(0.1f);  // Small delay to ensure UserId is properly set
        AssignSeat(player);
    }
    // Initialize the seating positions (Master Client)
    void InitializeSeats()
    {
        availableSeats.Clear();
        for (int i = 0; i < 9; i++) // Nine seats (0 to 8)
        {
            availableSeats.Add(i);
        }
    }

    void AssignLocalSeat()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Master Client handles its seat differently
            Debug.Log("Master Client cannot preassign itself Seat 1. Assignments proceed normally.");
            return;
        }

        // Local player always gets Seat 1 if available
        if (availableSeats.Contains(1))
        {
            availableSeats.Remove(1);
            playerSeats[PhotonNetwork.LocalPlayer.UserId] = 1;

            // Notify all players about the assignment
            photonView.RPC("ReceiveSeatAssignment", RpcTarget.All, PhotonNetwork.LocalPlayer.UserId, 1);
            Debug.Log("Local player assigned to Seat 1.");
        }
        else
        {
            Debug.LogWarning("Seat 1 is not available.");
        }
    }

   

    void AssignSeat(Player player)
    {
        if (player == null || string.IsNullOrEmpty(player.UserId))
        {
            Debug.LogError("Player has no valid UserId.");
            return;
        }

        if (playerSeats.ContainsKey(player.UserId))
        {
            Debug.Log($"Player {player.UserId} already has a seat.");
            return;
        }

        if (availableSeats.Count > 0)
        {
            // Assign a random seat from available seats
            int randomIndex = Random.Range(0, availableSeats.Count);
            int seatNumber = availableSeats[randomIndex];
            availableSeats.RemoveAt(randomIndex);

            // Store the seat assignment
            playerSeats[player.UserId] = seatNumber;

            // Notify all players about the assignment
            photonView.RPC("ReceiveSeatAssignment", RpcTarget.All, player.UserId, seatNumber);

            Debug.Log($"Assigned seat {seatNumber} to player {player.UserId}");
        }
        else
        {
            Debug.LogWarning($"No seats available for player {player.UserId}");
        }
    
        if (playerSeats.ContainsKey(player.UserId))
        {
            Debug.Log($"Player {player.UserId} already has a seat.");
            return;
        }

        if (availableSeats.Count > 0)
        {
            // Assign a random seat from available seats
            int randomIndex = Random.Range(0, availableSeats.Count);
            int seatNumber = availableSeats[randomIndex];
            availableSeats.RemoveAt(randomIndex);

            // Store the seat assignment
            playerSeats[player.UserId] = seatNumber;

            // Notify all players about the assignment
            photonView.RPC("ReceiveSeatAssignment", RpcTarget.All, player.UserId, seatNumber);

            Debug.Log($"Assigned seat {seatNumber} to player {player.UserId}");
        }
        else
        {
            Debug.LogWarning($"No seats available for player {player.UserId}");
        }
    }

    [PunRPC]
    void ReceiveSeatAssignment(string userId, int seatNumber)
    {
        if (PhotonNetwork.LocalPlayer.UserId == userId)
        {
            Debug.Log($"You have been assigned seat {seatNumber}");
            // Perform seat-related logic here, e.g., move the player to the seat position
        }
        else
        {
            Debug.Log($"Player {userId} was assigned seat {seatNumber}");
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            HandlePlayerLeaving(otherPlayer);
        }
    }

    void HandlePlayerLeaving(Player player)
    {
        if (playerSeats.TryGetValue(player.UserId, out int seatNumber))
        {
            // Free up the seat
            availableSeats.Add(seatNumber);
            playerSeats.Remove(player.UserId);
            Debug.Log($"Seat {seatNumber} is now available again.");
        }
    }
}