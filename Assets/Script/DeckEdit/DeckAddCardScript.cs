using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeckAddCardScript : MonoBehaviour, IDropHandler
{
    public enum TYPE
    {
        DECK,
        SELECT,
    }

    public TYPE type;

    public void OnDrop(PointerEventData eventData)
    {
        DeckEditCardController card = eventData.pointerDrag.GetComponent<DeckEditCardController>();
        if (card == null)
        {
            return;
        }

        if (type == TYPE.DECK)
        {
            if (card.isDeck)
            {
                return;
            }

            CardController[] cardList = DeckEditManager.instance.deckViewArea.GetComponentsInChildren<CardController>();

            if (cardList.Length >= 40)
            {
                return;
            }

            // デッキ追加
            card.defaultParent = this.transform;
            card.isDeck = true;
            card.isInDeck = true;
        }
        else
        {
            if (!card.isDeck)
            {
                return;
            }
            // デッキ削除
            Destroy(card.gameObject);
            DeckEditManager.instance.CheckDeckRegulation();
        }
    }
}
