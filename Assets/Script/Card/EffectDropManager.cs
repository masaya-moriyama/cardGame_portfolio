using UnityEngine;
using UnityEngine.EventSystems;

public class EffectDropManager : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (!GameManager.instance.isPlayerTurn)
        {
            return;
        }

        // カードを取得
        CardController effectCard = eventData.pointerDrag.GetComponent<CardController>();
        if (!effectCard.IsSpell())
        {
            return;
        }

        CardController target = GetComponent<CardController>();

        if (
            effectCard == null ||
            (target != null && !target.model.isFieldCard) ||
            !effectCard.model.isPlayerCard)
        {
            return;
        }

        effectCard.UseSpell(target, true);
    }

}
