using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeckEditManager : MonoBehaviour
{
    [SerializeField] DeckEditCardController cardPrehub;

    public Transform deckViewArea;
    public Transform cardSelectArea;
    public Transform cardViewArea;

    public GameObject upperLimitText;
    public GameObject saveButton;

    public GameObject deck1Button;
    public GameObject deck2Button;
    public GameObject deck3Button;

    DeckListEntity deckListEntity;

    int selectDeckNum;

    // シングルトン化
    public static DeckEditManager instance;

    int maxCardId = 100;

    bool isCostSortDeck;
    bool isCostSortSelect;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    void Init()
    {
        // デッキを取得する。
        deckListEntity = new DeckListEntity();
        Debug.Log("targetFile :" + Application.persistentDataPath + "/deck.json");
        if (System.IO.File.Exists(Application.persistentDataPath + "/deck.json"))
        {
            deckListEntity = LoadDeck("deck");
        }
        else
        {
            DefaultDeckEntity defaultDeckEntity = new DefaultDeckEntity();
            deckListEntity.deckList1 = defaultDeckEntity.deckList;
            InitSave(deckListEntity.deckList1);
        }
        selectDeckNum = 1;

        CreateDeckCards(deckListEntity.deckList1);
        CreateSelectAreaCard();

        CheckDeckRegulation();
    }

    /// <summary>
    /// デッキを表示する。
    /// </summary>
    /// <param name="deck"></param>
    void CreateDeckCards(List<int> deck)
    {
        foreach (var item in deck)
        {
            CrateCard(item, deckViewArea, true);
        }
    }

    /// <summary>
    /// カード一覧を表示する
    /// </summary>
    void CreateSelectAreaCard()
    {
        for (int i = 1; i <= maxCardId; i++)
        {
            CrateCard(i, cardSelectArea, false);
        }
    }

    /// <summary>
    /// カードの情報を生成し、初期化する。
    /// </summary>
    /// <param name="area"></param>
    public DeckEditCardController CrateCard(int cardId, Transform area, bool isDeck)
    {
        DeckEditCardController entity = Instantiate(cardPrehub, area, false);
        entity.Init(cardId, true, 0);
        entity.isDeck = isDeck;
        return entity;
    }

    public void OnSaveButton()
    {
        SaveDeckData();
        SceneManager.LoadScene("Title", LoadSceneMode.Single);
    }

    public void OnDeck1()
    {
        SaveDeckData();
        ClearDeckArea();
        CheckDeckRegulation();
        CreateDeckCards(deckListEntity.deckList1);
        selectDeckNum = 1;
    }

    public void OnDeck2()
    {
        SaveDeckData();
        ClearDeckArea();
        CheckDeckRegulation();
        CreateDeckCards(deckListEntity.deckList2);
        selectDeckNum = 2;
    }

    public void OnDeck3()
    {
        SaveDeckData();
        ClearDeckArea();
        CheckDeckRegulation();
        CreateDeckCards(deckListEntity.deckList3);
        selectDeckNum = 3;
    }

    void ClearDeckArea()
    {
        DeckEditCardController[] cardList = deckViewArea.GetComponentsInChildren<DeckEditCardController>();
        foreach (var item in cardList)
        {
            Destroy(item.gameObject);
        }
    }

    void ClearCardSelectArea()
    {
        DeckEditCardController[] cardList = cardSelectArea.GetComponentsInChildren<DeckEditCardController>();
        foreach (var item in cardList)
        {
            Destroy(item.gameObject);
        }
    }

    public void OnBack()
    {
        SceneManager.LoadScene("Title", LoadSceneMode.Single);
    }

    void SaveDeckData()
    {
        if (!isDeckCheckOk())
        {
            return;
        }
        DeckEditCardController[] cardList = deckViewArea.GetComponentsInChildren<DeckEditCardController>();
        List<int> saveDeckIdList = new List<int>();
        foreach (var item in cardList)
        {
            saveDeckIdList.Add(item.model.id);
        }
        ExecSave(saveDeckIdList, selectDeckNum);
    }

    public void OnClear()
    {
        ClearDeckArea();
        CheckDeckRegulation();
    }

    public void InitSave(List<int> deck)
    {
        DeckListEntity entity = new DeckListEntity();
        entity.deckList1 = deck;
        entity.deckList2 = deck;
        entity.deckList3 = deck;

        string jsonstr = JsonUtility.ToJson(entity);
        StreamWriter writer;
        writer = new StreamWriter(Application.persistentDataPath + "/deck.json", false);
        writer.Write(jsonstr);
        writer.Flush();
        writer.Close();
    }

    public void ExecSave(List<int> deck, int deckIndex = 1)
    {
        DeckListEntity entity = LoadDeck();

        switch (deckIndex)
        {
            case 1:
                entity.deckList1 = deck;
                deckListEntity.deckList1 = deck;
                break;
            case 2:
                entity.deckList2 = deck;
                deckListEntity.deckList2 = deck;
                break;
            case 3:
                entity.deckList3 = deck;
                deckListEntity.deckList3 = deck;
                break;
            default:
                break;
        }

        string jsonstr = JsonUtility.ToJson(entity);

        StreamWriter writer;
        writer = new StreamWriter(Application.persistentDataPath + "/deck.json", false);
        writer.Write(jsonstr);
        writer.Flush();
        writer.Close();
    }

    public DeckListEntity LoadDeck(string deckName = "deck")
    {
        string datastr = "";
        StreamReader reader;
        reader = new StreamReader(Application.persistentDataPath + "/" + deckName + ".json");
        datastr = reader.ReadToEnd();
        reader.Close();

        return JsonUtility.FromJson<DeckListEntity>(datastr);
    }

    public void CheckDeckRegulation()
    {
        bool result = isDeckCheckOk();

        if (result)
        {
            upperLimitText.SetActive(false);
            saveButton.GetComponent<Button>().interactable = true;
            deck1Button.GetComponent<Button>().interactable = true;
            deck2Button.GetComponent<Button>().interactable = true;
            deck3Button.GetComponent<Button>().interactable = true;
        }
        else
        {
            upperLimitText.SetActive(true);
            saveButton.GetComponent<Button>().interactable = false;
            deck1Button.GetComponent<Button>().interactable = false;
            deck2Button.GetComponent<Button>().interactable = false;
            deck3Button.GetComponent<Button>().interactable = false;
        }
    }

    private bool isDeckCheckOk()
    {
        DeckEditCardController[] cardList = deckViewArea.GetComponentsInChildren<DeckEditCardController>();

        List<int> ids = new List<int>();

        foreach (var item in cardList)
        {
            ids.Add(item.model.id);
        }

        return ExecDeckCheck(ids);
    }

    public bool ExecDeckCheck(List<int> ids)
    {
        List<int> duplicationLimit = ids.GroupBy(x => x)
                        .Where(g => g.Count() > 4)
                        .Select(x => x.Key)
                        .ToList();

        List<int> limit1 = ids.GroupBy(x => x)
            .Where(x => x.Count() > 1)
            .Select(x => x.Key)
            .ToList();

        string entityPath = "";
        CardEntity entity;

        // レジェンドカードチェック
        // FIXME: 雑すぎる、どうにかしたい。
        foreach (var item in limit1)
        {
            entityPath = "CardEntityList/Card_" + item;
            entity = Resources.Load<CardEntity>(entityPath);

            if (entity.legendCard)
            {
                // レジェンドカードが2枚以上入っている場合
                return false;
            }
        }

        int legendCount = 0;
        foreach (var item in ids)
        {
            entityPath = "CardEntityList/Card_" + item;
            entity = Resources.Load<CardEntity>(entityPath);

            if (entity.legendCard)
            {
                legendCount++;
            }
        }

        return ids.Count == 40 && duplicationLimit.Count == 0 && legendCount <= 4;
    }

    /// <summary>
    ///  カードを左に表示する。
    /// </summary>
    /// <param name="cardId"></param>
    public void ShowCardView(int cardId)
    {
        DeckEditCardController entity = Instantiate(cardPrehub, cardViewArea, false);
        entity.Init(cardId, true);
        entity.view.setActiveCardName(true);

        entity.gameObject.transform.localScale = new Vector3(3, 3, 3);
    }

    /// <summary>
    /// 表示しているカード詳細を非表示にする。
    /// </summary>
    public void HideCardView()
    {
        CardController[] cards = cardViewArea.GetComponentsInChildren<CardController>();

        foreach (var item in cards)
        {
            Destroy(item.gameObject);
        }
    }

    public void OnSortCardSelectArea()
    {
        List<DeckEditCardController> cardList = new List<DeckEditCardController>();
        cardSelectArea.GetComponentsInChildren<DeckEditCardController>(cardList);
        ClearCardSelectArea();
        if (!isCostSortSelect)
        {
            cardList.Sort((x, y) =>
            {
                int result = x.model.cost.CompareTo(y.model.cost);
                return result != 0 ? result : x.model.id.CompareTo(y.model.id);
            });
        }
        else
        {
            cardList.Sort((x, y) => x.model.id.CompareTo(y.model.id));
        }
        foreach (var item in cardList)
        {
            CrateCard(item.model.id, cardSelectArea, false);
        }
        isCostSortSelect = !isCostSortSelect;
    }

    public void OnSortDeck()
    {
        List<DeckEditCardController> cardList = new List<DeckEditCardController>();
        deckViewArea.GetComponentsInChildren<DeckEditCardController>(cardList);

        ClearDeckArea();
        if (!isCostSortDeck)
        {
            cardList.Sort((x, y) =>
            {
                int result = x.model.cost.CompareTo(y.model.cost);
                return result != 0 ? result : x.model.id.CompareTo(y.model.id);
            });
        }
        else
        {
            cardList.Sort((x, y) => x.model.id.CompareTo(y.model.id));
        }

        foreach (var item in cardList)
        {
            CrateCard(item.model.id, deckViewArea, true);
        }
        isCostSortDeck = !isCostSortDeck;
    }

}
