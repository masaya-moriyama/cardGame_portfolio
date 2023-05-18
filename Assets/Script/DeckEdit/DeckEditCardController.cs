using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class DeckEditCardController : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public CardView view;
    public CardModel model;

    public Transform defaultParent;

    public int siblingIndex;

    public bool isDeck;
    public bool isInDeck = false;

    /// <summary>
    ///  コントローラー起動時に子クラスを設定
    /// </summary>
    public void Awake()
    {
        view = GetComponent<CardView>();
    }

    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="cardId">カードID</param>
    public void Init(int cardId, bool isPlayer, int cardPlayId = 0)
    {
        model = new CardModel(cardId, isPlayer, cardPlayId);
        view.SetCard(model);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        siblingIndex = transform.GetSiblingIndex();
        CardController card = GetComponent<CardController>();

        defaultParent = transform.parent;
        transform.SetParent(defaultParent.root, false);
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(defaultParent, false);
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        if (isInDeck)
        {
            transform.SetAsLastSibling();
            isInDeck = false;

            DeckEditCardController card = DeckEditManager.instance.CrateCard(this.model.id, DeckEditManager.instance.cardSelectArea, false);
            card.transform.SetSiblingIndex(siblingIndex);
            DeckEditManager.instance.CheckDeckRegulation();
        }
        else
        {
            transform.SetSiblingIndex(siblingIndex);
        }
    }

    void Start()
    {
        defaultParent = transform.parent;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 対象のカード取得
        DeckEditCardController card = GetComponent<DeckEditCardController>();

        if (card == null || card.model == null)
        {
            // カードが取得できなかった場合
            return;
        }

        if (SceneManager.GetActiveScene().name != "Deck")
        {
            return;
        }

        DeckEditManager.instance.ShowCardView(card.model.id);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 対象のカード取得
        DeckEditCardController card = GetComponent<DeckEditCardController>();

        if (card == null || card.model == null)
        {
            // カードが取得できなかった場合
            return;
        }

        if (SceneManager.GetActiveScene().name != "Deck")
        {
            return;
        }

        DeckEditManager.instance.HideCardView();
    }
}
