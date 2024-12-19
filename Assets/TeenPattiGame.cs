using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Realtime;
using UnityEngine;

[System.Serializable]
public class Card
{
    [SerializeField]
    public int Rank { get; } // 0 = Ace, 1 = 2, ..., 12 = King
    [SerializeField]
    public string Suit { get; } // "Hearts", "Spades", "Diamonds", "Clubs"

   
    public Card(int cardNumber)
    {
        Rank = cardNumber % 13;
        Suit = (cardNumber / 13) switch
        {
            0 => "Hearts",
            1 => "Spades",
            2 => "Diamonds",
            3 => "Clubs",
            _ => throw new ArgumentException("Invalid card number")
        };
    }

    public override string ToString()
    {
        string rankName = Rank switch
        {
            0 => "A",
            1 => "2",
            2 => "3",
            3 => "4",
            4 => "5",
            5 => "6",
            6 => "7",
            7 => "8",
            8 => "9",
            9 => "10",
            10 => "J",
            11 => "Q",
            12 => "K",
            _ => Rank.ToString()
        };
        return $"{rankName} of {Suit}";
    }
}

[System.Serializable]
public class PlayerTeenPatti
{
    [SerializeField]
    public string Name;
    [SerializeField]
    public Card[] Hand;

    public PlayerTeenPatti(string name, int[] hand)
    {
        Name = name;
        Hand = hand.Select(cardNumber => new Card(cardNumber)).ToArray();
    }
}

public class TeenPattiGame : MonoBehaviour
{
    public static TeenPattiGame Instance;

    private void Awake()
    {
        Instance = this;
    }
    [SerializeField] public List<PlayerTeenPatti> playersT ;


    void Start()
    {
        // Initialize players with card numbers
        /*playersT = new List<PlayerTeenPatti>
        {
            new PlayerTeenPatti("Player 1", new int[] { 0, 1, 2 }),  // Example: A, 2, 3 (Hearts)
            new PlayerTeenPatti("Player 2", new int[] { 13, 14, 15 }), // Example: A, 2, 3 (Spades)
            new PlayerTeenPatti("Player 3", new int[] { 10, 23, 36 }), // Example: A, 2, 3 (Diamonds)
            new PlayerTeenPatti("Player 4", new int[] { 39, 40, 41 }), // Example: A, 2, 3 (Clubs)
            new PlayerTeenPatti("Player 5", new int[] { 0, 13, 26 }),  // Example: A of Hearts, Spades, Diamonds
            new PlayerTeenPatti("Player 6", new int[] { 26, 27, 28 })  // Example: J of Hearts, J of Spades, J of Diamonds
        };*/
        
    }

    private int EvaluateHand(Card[] hand)
    {
        Array.Sort(hand, (x, y) => x.Rank.CompareTo(y.Rank));
        bool isSameSuit = hand.All(card => card.Suit == hand[0].Suit);
        bool isSequence = (hand[2].Rank == hand[1].Rank + 1 && hand[1].Rank == hand[0].Rank + 1) ||
                          (hand[2].Rank == 12 && hand[0].Rank == 0 && hand[1].Rank == 1); // Handle A, 2, 3
        bool isTrail = hand[0].Rank == hand[1].Rank && hand[1].Rank == hand[2].Rank;
        bool isPair = hand[0].Rank == hand[1].Rank || hand[1].Rank == hand[2].Rank;

        if (isTrail) return 6; // Trail/Trio
        if (isSameSuit && isSequence) return 5; // Pure Sequence
        if (isSequence) return 4; // Sequence
        if (isSameSuit) return 3; // Color/Flush
        if (isPair) return 2; // Pair
        return 1; // High Card
    }

    private int CompareHands(Card[] hand1, Card[] hand2)
    {
        int rank1 = EvaluateHand(hand1);
        int rank2 = EvaluateHand(hand2);

        if (rank1 != rank2) return rank2.CompareTo(rank1);

        // Compare individual card ranks if hands have the same type
        for (int i = 2; i >= 0; i--)
        {
            if (hand1[i].Rank != hand2[i].Rank)
                return hand2[i].Rank.CompareTo(hand1[i].Rank);
        }

        return 0; // Hands are identical
    }
    public void DisplayWinner()
    {
        // Determine the winner
        PlayerTeenPatti winner = DetermineWinner(playersT);
        Debug.Log($"Winner: {winner.Name}");
    }

    private PlayerTeenPatti DetermineWinner(List<PlayerTeenPatti> playersT)
    {
        playersT.Sort((p1, p2) => CompareHands(p1.Hand, p2.Hand));
        return playersT[0]; // Player with the best hand
    }
}
