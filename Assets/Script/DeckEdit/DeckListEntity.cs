using System.Collections.Generic;
using UnityEngine;

public class DeckListEntity
{
    [SerializeField] public List<int> deckList1;
    [SerializeField] public List<int> deckList2;
    [SerializeField] public List<int> deckList3;

    [SerializeField] public List<string> deckNames;

    public List<int> GetDeckList(int deckNo)
    {
        switch (deckNo)
        {
            case 1:
                return deckList1;
            case 2:
                return deckList2;
            case 3:
                return deckList3; 
            default:
                return deckList1;
        }
    }
}
