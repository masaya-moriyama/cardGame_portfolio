using System.Collections.Generic;
using UnityEngine;

public class GamePlayerManager : MonoBehaviour
{
    public List<int> deck;

    public int playerHp;
    public int manaCost;
    public int defaultManaCost;
    public int amountDeckCount;
    public int cemeteryCount;

    public List<int> cemeteryList;

    // オンライン対戦の際、カード識別に使用
    // FIXME:
    public int playHandCount;

    public void Init(List<int> cardDeck)
    {
        deck = cardDeck;
        playerHp = 20;
        defaultManaCost = manaCost = 0;
        amountDeckCount = deck.Count;
        cemeteryCount = 0;
    }

}
