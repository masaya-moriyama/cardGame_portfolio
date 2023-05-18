using UnityEngine;
using UnityEngine.EventSystems;

public class DropPlace : MonoBehaviour, IDropHandler
{
    public enum TYPE
    {
        HAND,
        FIELD,
        COST
    }

    public TYPE type;

    public void OnDrop(PointerEventData eventData)
    {
        // 手札内でのドラッグドロップ、もしくは相手ターンの場合、何もしない。
        if (type == TYPE.HAND || !GameManager.instance.isPlayerTurn)
        {
            return;
        }

        CardController card = eventData.pointerDrag.GetComponent<CardController>();

        // 対象のカードが不在・対象が相手のカード・対象のドラッグが非許可、対象がフィールドのカード
        if (card == null || !card.model.isPlayerCard || !card.move.isDoraggable || card.model.isFieldCard)
        {
            return;
        }

        if (type == TYPE.FIELD && 
            card.model.IsMonster() &&
            card.model.cost <= GameManager.instance.player.manaCost &&
            !GameManager.instance.IsUpperlimitFieldCard())
        {
            // 親をドロップ先に設定
            card.move.defaultParent = this.transform;
            // ドロップ時の処理
            card.OnField(true);
        }
        else if (type == TYPE.COST && GameManager.instance.canAddMana)
        {
            // マナコストゾーンにドロップかつ、このターン、マナコストを増やしていない場合
            GameManager.instance.AddManaCost(card);
        }
    }
}
