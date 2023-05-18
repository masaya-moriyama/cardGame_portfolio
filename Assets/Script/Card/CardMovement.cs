using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CardMovement : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Transform defaultParent;

    public bool isDoraggable;
    public int siblingIndex;

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 対象のカード取得
        CardController card = GetComponent<CardController>();

        if (card != null && OnlineStatusManager.instance.IsDevelopMode)
        {
            Debug.Log("cardId : [" + card.model.id + "] -- cardPlayId : [" + card.model.cardPlayId + "]");
            Debug.Log("parent : " + card.gameObject.transform.parent);
        }

        if (card == null || !card.model.isPlayerCard)
        {
            // カードが取得できなかった場合・ドラッグ中の場合
            return;
        }

        GameManager.instance.ShowCardView(card.model.id, !card.model.isToken);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GameManager.instance.HideCardView();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        siblingIndex = transform.GetSiblingIndex();

        CardController card = GetComponent<CardController>();

        if (!card.model.isPlayerCard)
        {
            // プレイヤーのカードではない場合
            isDoraggable = false;
        }
        else if (!GameManager.instance.IsDoraggable())
        {
            // ドラッグ操作が不許可の場合
            isDoraggable = false;
        }
        else if (!card.model.isFieldCard)
        {
            // 手札のカードの場合
            isDoraggable = true;
        }
        else if (card.model.isFieldCard && card.model.IsCanAttack())
        {
            // フィールドのカードかつ、攻撃が可能の時
            isDoraggable = true;
        }
        else
        {
            isDoraggable = false;
        }

        if (!isDoraggable)
        {
            return;
        }

        defaultParent = transform.parent;
        transform.SetParent(defaultParent.parent, false);
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDoraggable)
        {
            return;
        }

        // transform.position = eventData.position;
        //マウスの座標を取得してスクリーン座標を更新
        Vector3 thisPosition = Input.mousePosition;
        //スクリーン座標→ワールド座標
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(thisPosition);
        worldPosition.z = 0f;

        this.transform.position = worldPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDoraggable)
        {
            return;
        }

        transform.SetParent(defaultParent, false);
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        transform.SetSiblingIndex(siblingIndex);
    }

    public IEnumerator MoveToField(Transform field)
    {
        transform.SetParent(defaultParent.parent);
        transform.DOMove(field.position, 0.25F);
        yield return new WaitForSeconds(0.25F);

        defaultParent = field;
        transform.SetParent(defaultParent);
    }

    public IEnumerator MoveToManaCost(Transform field)
    {
        transform.SetParent(defaultParent.parent);
        transform.DOMove(field.position, 0.25F);
        yield return new WaitForSeconds(0.25F);

        Destroy(gameObject);
    }

    public IEnumerator MoveToTarget(Transform target)
    {
        Vector3 currentPosition = transform.position;
        siblingIndex = transform.GetSiblingIndex();

        transform.SetParent(defaultParent.parent);
        transform.DOMove(target.position, 0.25F).SetLink(this.gameObject);
        yield return new WaitForSeconds(0.25F);

        transform.DOMove(currentPosition, 0.25F).SetLink(this.gameObject);
        yield return new WaitForSeconds(0.25F);
        transform.SetParent(defaultParent);
        transform.SetSiblingIndex(siblingIndex);
    }

    public IEnumerator MoveToUseSpell(Transform target)
    {
        transform.SetParent(defaultParent.parent);
        transform.DOMove(target.position, 0.25F).SetLink(this.gameObject);
        yield return new WaitForSeconds(0.25F);
    }

    public IEnumerator MoveToHand(Transform target)
    {
        transform.DOMove(target.position, 0.25F).SetLink(this.gameObject);
        yield return new WaitForSeconds(0.25F);
        transform.SetParent(target);
    }

    void Start()
    {
        defaultParent = transform.parent;
    }
}
