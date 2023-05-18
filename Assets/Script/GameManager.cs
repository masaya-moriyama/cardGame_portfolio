using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// ゲームの全体管理
/// </summary>
public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] public GamePlayerManager player;
    [SerializeField] public GamePlayerManager enemy;
    [SerializeField] CPU enemyCpu;
    [SerializeField] UIManager uiManager;
    [SerializeField] PhotonManager photonManager;

    [SerializeField] GameObject TurnEndBottun;

    [SerializeField] CardController cardPrehub;

    public Transform playerHandTransform;
    public Transform enemyHandTransform;

    public Transform playerFieldTransForm;
    public Transform enemyFieldTransForm;

    public Transform playerHpTransform;
    public Transform enemyHpTransform;

    public Transform enemyManaCostTransForm;

    public Transform playerDeck;
    public Transform enemyDeck;

    public Transform cardViewArea;

    public bool isPlayerTurn;
    public bool canAddMana;

    bool isDoraggable;

    public int turnCount;

    bool isResult;

    int timeCount;

    int fieldUpperLimit = 5;
    int handUpperLimit = 6;

    // シングルトン化
    public static GameManager instance;

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
        StartGame();
    }

    void StartGame()
    {
        uiManager.HideResultPanel();
        player.Init(CreateDeck());
        enemy.Init(CreateDeck());

        // CPU戦の場合、初期デッキを相手デッキに設定する。
        if (!OnlineStatusManager.instance.IsOnlineBattle)
        {
            DefaultDeckEntity defaultDeckEntity = new DefaultDeckEntity();
            enemy.deck = defaultDeckEntity.deckList;
        }

        uiManager.ShowDeckAmountCount(player.amountDeckCount, enemy.amountDeckCount);

        timeCount = 999;
        turnCount = 1;
        uiManager.UpdateTime(timeCount);

        // ゲーム開始時にカードを3枚付与
        GiveCardToHandToCount(true, 3);

        if (!OnlineStatusManager.instance.IsOnlineBattle)
        {
            GiveCardToHandToCount(false, 3);
        }

        // CPU戦は先行、オンライン戦の場合はホストが先行
        // FIXME: 先行後攻ルール
        isPlayerTurn = true;
        if (OnlineStatusManager.instance.IsOnlineBattle)
        {
            isPlayerTurn = OnlineStatusManager.instance.IsOnlineHost;
        }

        uiManager.showUserHp(player.playerHp, enemy.playerHp);
        uiManager.ShowManaCost(player.manaCost, enemy.manaCost);
        uiManager.ShowDefaultManaCost(player.defaultManaCost, enemy.defaultManaCost);

        DOVirtual.DelayedCall(2F, () => TurnCalc());

        // ゲーム開始時のターンエンドボタンの初期化
        TurnEndBottun.GetComponent<Button>().interactable = isPlayerTurn;
    }

    List<int> CreateDeck()
    {
        DeckListEntity deck = new DeckListEntity();
        if (System.IO.File.Exists(Application.persistentDataPath + "/deck.json"))
        {
            deck = LoadDeck("deck");
        }
        else
        {
            DefaultDeckEntity defaultDeckEntity = new DefaultDeckEntity();
            deck.deckList1 = defaultDeckEntity.deckList;
        }
        List<int> deckData = deck.GetDeckList(OnlineStatusManager.instance.useDeckNo);
        return deckData.OrderBy(a => Guid.NewGuid()).ToList();
    }

    DeckListEntity LoadDeck(string deckName)
    {
        string datastr = "";
        StreamReader reader;
        reader = new StreamReader(Application.persistentDataPath + "/" + deckName + ".json");
        datastr = reader.ReadToEnd();
        reader.Close();

        return JsonUtility.FromJson<DeckListEntity>(datastr);
    }

    /// <summary>
    /// マナコストの支払いをを実施
    /// </summary>
    /// <param name="cost"></param>
    /// <param name="isPlayerCard"></param>
    public void ReduceManaCost(int cost, bool isPlayerCard)
    {
        if (isPlayerCard)
        {
            player.manaCost -= cost;
        }
        else
        {
            enemy.manaCost -= cost;
        }

        uiManager.ShowManaCost(player.manaCost, enemy.manaCost);
    }

    /// <summary>
    /// マナコストの追加処理
    /// カードを消費してマナコストを増やす
    /// </summary>
    /// <param name="card"></param>
    public void AddManaCost(CardController card)
    {
        player.defaultManaCost++;
        player.manaCost++;

        canAddMana = false;
        Destroy(card.gameObject);

        uiManager.ShowManaCost(player.manaCost, enemy.manaCost);
        uiManager.ShowDefaultManaCost(player.defaultManaCost, enemy.defaultManaCost);

        // SE再生
        SoundManager.instance.PlaySeToAddMana();

        // オンライン対戦時は、相手にカードIDを送信する。
        if (!OnlineStatusManager.instance.IsOnlineBattle)
        {
            return;
        }
        SendAddManaCostCard(card.model.cardPlayId);
    }

    public void GiveCardToHandToCount(bool isPlayer, int count)
    {
        Transform deckArea;
        Transform hand;

        if (isPlayer)
        {
            deckArea = playerDeck;
            hand = playerHandTransform;
        }
        else
        {
            deckArea = enemyDeck;
            hand = enemyHandTransform;
        }

        StartCoroutine(GiveCardToHandCoroutine(deckArea, hand, isPlayer, count));
    }

    /// <summary>
    /// GiveCardToHandToCount直だと、効果適用によるドローではコルーチンが使えないので
    /// 一回ラップする
    /// </summary>
    /// <param name="deckArea"></param>
    /// <param name="hand"></param>
    /// <param name="isPlayer"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public IEnumerator GiveCardToHandCoroutine(Transform deckArea, Transform hand, bool isPlayer, int count)
    {
        isDoraggable = false;
        for (int i = 0; i < count; i++)
        {
            GiveCardToHand(deckArea, hand, isPlayer);
            yield return new WaitForSeconds(0.5F);
        }
        isDoraggable = true;
    }

    /// <summary>
    /// 手札にカードを配る
    /// </summary>
    /// <param name="deckArea"></param>
    /// <param name="hand"></param>
    /// <param name="isPlayer"></param>
    public void GiveCardToHand(Transform deckArea, Transform hand, bool isPlayer)
    {
        int cardId = 0;
        List<int> deck;

        if (isPlayer)
        {
            deck = player.deck;
        }
        else
        {
            deck = enemy.deck;
        }

        if (deck.Count <= 0)
        {
            // デッキの残り枚数が0の場合
            LibraryOut(isPlayer);
            if (isPlayer)
            {
                SendLibraryOut();
            }
            return;
        }
        else
        {
            cardId = deck[0];
            deck.RemoveAt(0);
        }

        // オンライン対戦の場合、手札に加えたカードIDを相手に送信する。
        if (OnlineStatusManager.instance.IsOnlineBattle)
        {
            SendDrowCard(cardId);
        }

        DrowDeck(deckArea, hand, isPlayer, cardId);
    }

    /// <summary>
    /// デッキからカードを1枚ドローする。
    /// </summary>
    /// <param name="deck"></param>
    /// <param name="hand"></param>
    /// <param name="isPlayer"></param>
    /// <param name="cardId"></param>
    public void DrowDeck(Transform deck, Transform hand, bool isPlayer, int cardId)
    {
        if (isPlayer)
        {
            player.amountDeckCount--;
        }
        else
        {
            enemy.amountDeckCount--;
        }

        CardController drowCard = CrateCard(cardId, deck, isPlayer, true);
        uiManager.ShowDeckAmountCount(player.amountDeckCount, enemy.amountDeckCount);

        // 手札上限の場合、ドローしたカードは破棄する。
        if (isPlayer)
        {
            if (IsPlayerUpperlimitHandCard())
            {
                Destroy(drowCard.gameObject);
            }
            else
            {
                StartCoroutine(drowCard.move.MoveToHand(hand));
            }
        }
        else
        {
            if (IsEnemyUpperlimitHandCard())
            {
                Destroy(drowCard.gameObject);
            }
            else
            {
                StartCoroutine(drowCard.move.MoveToHand(hand));
            }
        }
    }

    /// <summary>
    /// カードの情報を生成し、初期化する。
    /// </summary>
    /// <param name="area"></param>
    public CardController CrateCard(int cardId, Transform area, bool isPlayer, bool isCard = true)
    {
        CardController entity = Instantiate(cardPrehub, area, false);

        // カードにプレイ番号を設定
        int cardPlayId = 0;
        if (isPlayer)
        {
            GameManager.instance.player.playHandCount++;
            cardPlayId = GameManager.instance.player.playHandCount;
        }
        else
        {
            GameManager.instance.enemy.playHandCount++;
            cardPlayId = GameManager.instance.enemy.playHandCount;
        }
        entity.Init(cardId, isPlayer, cardPlayId, isCard);
        return entity;
    }

    /// <summary>
    /// 破壊されたカード、および使用済みのカードを墓地情報に登録する。
    /// </summary>
    /// <param name="card"></param>
    /// <param name="isPlayer"></param>
    public void SetCemetery(CardController card)
    {
        if (card.IsSpell()) {
            // スペルなら墓地にカウントしない。
            return;
        }

        List<int> cemetery;
        if (card.model.isPlayerCard) {
            cemetery = player.cemeteryList;
        } else {
            cemetery = enemy.cemeteryList;
        }
        cemetery.Add(card.model.id);
        UpdateCemeteryCount();

        // 破壊時効果を所持の場合、効果を発動
        if (card.model.ability.isDestruction)
        {
            card.ExecEffect(card.model.isPlayerCard);
        }

        // デバッグ用 お互いの墓地のカードidをログに表示
        if (OnlineStatusManager.instance.IsDevelopMode)
        {
            Debug.Log("-- 自分の墓地 -- ");
            foreach (var item in player.cemeteryList)
            {
                Debug.Log(item);
            }
            Debug.Log("-- 相手の墓地 -- ");
            foreach (var item in enemy.cemeteryList)
            {
                Debug.Log(item);
            }
        }
    }

    /// <summary>
    /// 墓地の合計数を更新する。
    /// </summary>
    void UpdateCemeteryCount()
    {
        player.cemeteryCount = player.cemeteryList.Count;
        enemy.cemeteryCount = enemy.cemeteryList.Count;

        uiManager.UpdateCemeteryCountText(player.cemeteryCount, enemy.cemeteryCount);
    }

    public List<int> GetCemeteryListPlayer()
    {
        return player.cemeteryList;
    }

    public List<int> GetCemeteryListEnemy()
    {
        return enemy.cemeteryList;
    }

    public void RemoveRangeToCemetery(bool isPlayer, int index, int range)
    {
        List<int> cemetery = GameManager.instance.GetCemeteryListPlayer();
        if (!isPlayer)
        {
            cemetery = GameManager.instance.GetCemeteryListEnemy();
        }

        cemetery.RemoveRange(index, range);
        UpdateCemeteryCount();
    }

    /// <summary>
    /// /// 使用したカードを左に一定時間表示する。
    /// </summary>
    /// <param name="cardId"></param>
    public void ShowUseCard(int cardId)
    {
        CardController entity = Instantiate(cardPrehub, cardViewArea, false);
        entity.Init(cardId, true);
        entity.view.setActiveCardName(true);

        entity.gameObject.transform.localScale = new Vector3(2, 2, 2);

        // DOTweenを用いて画面外に移動させて削除
        DOVirtual.DelayedCall(1, () => entity.gameObject.transform.DOMoveX(-200, 0.5f), false);
        DOVirtual.DelayedCall(2, () => Destroy(entity.gameObject), false);
    }

    /// <summary>
    ///  カードを左に表示する。
    /// </summary>
    /// <param name="cardId"></param>
    public void ShowCardView(int cardId, bool isCard = true)
    {
        CardController entity = Instantiate(cardPrehub, cardViewArea, false);
        entity.Init(cardId, true, 0, isCard);
        entity.view.setActiveCardName(true);

        entity.gameObject.transform.localScale = new Vector3(2, 2, 2);
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

    /// <summary>
    /// ターン開始時のプレイヤーのターン処理
    /// </summary>
    void TurnCalc()
    {
        // 文字列型以外だとstopで完全に止まらない為、文字列型で設定
        StopCoroutine("CountDown");
        StartCoroutine("CountDown");
        uiManager.UpdateTurnCount(turnCount);

        if (turnCount != 1)
        {
            SoundManager.instance.PlaySeToTurnChange();
        }
        else
        {
            SoundManager.instance.PlaySeToBattleStart();
        }

        StartCoroutine(uiManager.ShowHaveTrunPlayer(isPlayerTurn));

        if (isPlayerTurn)
        {
            PlayerTurnStart();
        }
        else
        {
            // CPU戦の場合、CPUターンを実施
            if (!OnlineStatusManager.instance.IsOnlineBattle)
            {
                StartCoroutine(enemyCpu.EnemyTurn());
            }
            else
            {
                EnemyTurnStartOnlie();
            }
        }
    }

    IEnumerator CountDown()
    {
        timeCount = 999;
        uiManager.UpdateTime(timeCount);

        while (timeCount > 0)
        {
            yield return new WaitForSeconds(1);
            timeCount--;
            uiManager.timeCountText.text = timeCount.ToString();
        }

        ChangeTrun();
    }

    /// <summary>
    /// プレイヤーターン開始処理
    /// </summary>
    void PlayerTurnStart()
    {
        Debug.Log("プレイヤーターン開始");
        // ドロー処理
        if (turnCount != 2)
        {
            GiveCardToHandToCount(true, 1);
        } else {
            // 2ターン目(後攻1ターン目の場合は、追加で1ドロー)
            GiveCardToHandToCount(true, 2);
        }

        // マナ追加可能フラグを立てる
        canAddMana = true;

        // 自分フィールドカードに攻撃権付与
        CardController[] cardList = playerFieldTransForm.GetComponentsInChildren<CardController>();
        SettingCanAtttack(cardList, playerFieldTransForm, true);
    }

    /// <summary>
    /// オンライン時の相手ターン開始処理
    /// </summary>
    void EnemyTurnStartOnlie()
    {
        Debug.Log("相手ターン開始");

        // 相手フィールドカードに攻撃権付与
        CardController[] cardList = enemyFieldTransForm.GetComponentsInChildren<CardController>();
        SettingCanAtttack(cardList, enemyFieldTransForm, true);
    }

    public void SettingCanAtttack(CardController[] cardList, Transform field, bool canAttack)
    {
        foreach (CardController card in cardList)
        {
            card.InitCanAttackCount(canAttack);
        }
    }

    /// <summary>
    /// カードの戦闘処理
    /// HPの増減を実施する。
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="defender"></param>
    public void CardsBattle(CardController attacker, CardController defender)
    {
        attacker.Attack(defender);
        defender.Attack(attacker);

        ParticleManager.instance.StartHitParticle(defender.transform);
        SoundManager.instance.PlaySeToAttack();

        // 絆魂の処理
        if (attacker.model.ability.isLifeLink)
        {
            this.AddHp(attacker.model.isPlayerCard, attacker.model.atk);
        }

        attacker.CheckAlive();
        defender.CheckAlive();
    }

    /// <summary>
    /// プレイヤーへの直接攻撃の処理
    /// </summary>
    /// <param name="attaker"></param>
    /// <param name="isPlayerCard"></param>
    public void AttackToPlayer(CardController attacker, bool isPlayerCard)
    {
        if (isPlayerCard)
        {
            enemy.playerHp -= attacker.model.atk;
        }
        else
        {
            player.playerHp -= attacker.model.atk;
        }

        if (attacker.model.ability.isLifeLink)
        {
            this.AddHp(attacker.model.isPlayerCard, attacker.model.atk);
        }
        attacker.ReduceCanAtack();
        uiManager.showUserHp(player.playerHp, enemy.playerHp);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="attaker"></param>
    /// <param name="isPlayer"></param>
    public void AttackToPlayer(int attaker, bool isPlayer)
    {
        if (isPlayer)
        {
            enemy.playerHp -= attaker;
        }
        else
        {
            player.playerHp -= attaker;
        }

        uiManager.showUserHp(player.playerHp, enemy.playerHp);
    }

    public void checkPlayerHp()
    {
        if (player.playerHp > 0 && enemy.playerHp > 0)
        {
            return;
        }

        ShowResultPanel(player.playerHp);
    }

    public void AddHp(bool isPlayer, int num)
    {
        if (isPlayer)
        {
            this.player.playerHp += num;
        }
        else
        {
            this.enemy.playerHp += num;
        }
        uiManager.showUserHp(player.playerHp, enemy.playerHp);
    }

    void ShowResultPanel(int playerHp)
    {
        StopAllCoroutines();
        uiManager.ShowResultPanel(player.playerHp);
        SoundManager.instance.ChangeMusicToBattleEnd(playerHp > 0);
        isResult = true;
    }

    public void LibraryOut(bool isPlayer)
    {
        uiManager.ShowLibraryOutResult(isPlayer);
    }

    /// <summary>
    /// 対戦相手のフィールドカードを取得する
    /// </summary>
    /// <returns></returns>
    public CardController[] GetEnemyFieldCards()
    {
        return enemyFieldTransForm.GetComponentsInChildren<CardController>();
    }

    /// <summary>
    /// 自分の場のカードの合計を取得する
    /// </summary>
    /// <returns></returns>
    public int GetPlayerFieldCardCount()
    {
        return playerFieldTransForm.GetComponentsInChildren<CardController>().Length;
    }

    /// <summary>
    /// 自分の場のカードの合計を取得する
    /// </summary>
    /// <returns></returns>
    public int GetEnemyFieldCardCount()
    {
        return enemyFieldTransForm.GetComponentsInChildren<CardController>().Length;
    }


    /// <summary>
    /// 場の上限かどうか
    /// </summary>
    /// <returns></returns>
    public bool IsUpperlimitFieldCard()
    {
        return GetPlayerFieldCardCount() >= fieldUpperLimit;
    }

    /// <summary>
    /// 場の上限かどうか
    /// </summary>
    /// <returns></returns>
    public bool IsUpperlimitFieldCardEnemy()
    {
        return GetEnemyFieldCardCount() >= fieldUpperLimit;
    }

    /// <summary>
    /// 自分の手札のカードの合計を取得する
    /// </summary>
    /// <returns></returns>
    public int GetPlayerHandCardCount()
    {
        return playerHandTransform.GetComponentsInChildren<CardController>().Length;
    }

    /// <summary>
    /// 手札上限かどうか
    /// </summary>
    /// <returns></returns>
    public bool IsEnemyUpperlimitHandCard()
    {
        return GetEnemyHandCardCount() >= handUpperLimit;
    }

    /// <summary>
    /// 相手の手札のカードの合計を取得する
    /// </summary>
    /// <returns></returns>
    public int GetEnemyHandCardCount()
    {
        return enemyHandTransform.GetComponentsInChildren<CardController>().Length;
    }

    /// <summary>
    /// 相手が手札上限かどうか
    /// </summary>
    /// <returns></returns>
    public bool IsPlayerUpperlimitHandCard()
    {
        return GetPlayerHandCardCount() >= handUpperLimit;
    }

    /// <summary>
    /// ターンの切り替えを実施
    /// /// </summary>
    public void ChangeTrun()
    {
        if (!isDoraggable)
        {
            // ドラッグ操作が許可されていない場合は演出中なので、
            // ターンエンドも押下させない
            return;
        }

        isPlayerTurn = !isPlayerTurn;

        // フィールドの攻撃権を初期化する。
        CardController[] playerCardList = playerFieldTransForm.GetComponentsInChildren<CardController>();
        SettingCanAtttack(playerCardList, playerFieldTransForm, false);

        CardController[] enemyCardList = GetEnemyFieldCards();
        SettingCanAtttack(enemyCardList, enemyFieldTransForm, false);

        // ターン交代時、交代先のマナコスト初期化処理を行う。
        if (isPlayerTurn)
        {
            player.manaCost = player.defaultManaCost;
            uiManager.ShowManaCost(player.manaCost, enemy.manaCost);
        }
        else
        {
            enemy.manaCost = enemy.defaultManaCost;
            uiManager.ShowManaCost(player.manaCost, enemy.manaCost);
        }

        // ターンエンドボタンの活性・非活性を、ターンプレイヤーの状態に応じて切り替える。
        TurnEndBottun.GetComponent<Button>().interactable = isPlayerTurn;

        if (OnlineStatusManager.instance.IsOnlineBattle && !isPlayerTurn)
        {
            // オンライン対戦の場合かつ自分のターンだった場合、相手にターンチェンジの情報を送信
            SendChangeTurn();
        }

        turnCount++;
        TurnCalc();
    }

    public void SwitchScene()
    {
        if (OnlineStatusManager.instance.IsOnlineBattle)
        {
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LeaveRoom();
                PhotonNetwork.Disconnect();
            }
        }
        OnlineStatusManager.instance.IsOnlineBattle = false;
        SoundManager.instance.ChaneGameStatusToTilte();
        SceneManager.LoadScene("Title", LoadSceneMode.Single);
    }

    public override void OnPlayerLeftRoom(Player player)
    {
        if (!OnlineStatusManager.instance.IsOnlineBattle)
        {
            return;
        }
        if (!isResult)
        {
            UIManager.instance.ShowLeavePanel();
        }
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();
        }
        OnlineStatusManager.instance.IsOnlineBattle = false;
    }

    /// <summary>
    /// ドラッグ操作を許可しているか
    /// </summary>
    /// <returns></returns>
    public bool IsDoraggable()
    {
        return isDoraggable;
    }

    /// <summary>
    /// カードをドローした時、ドローしたカードIDを相手に送信する。
    /// </summary>
    public void SendDrowCard(int cardId)
    {
        photonManager.SendDrowCard(cardId);
    }

    /// <summary>
    /// カードをプレイした時、プレイしたカードIDを相手に送信する。
    /// </summary>
    /// <param name="number"></param>
    public void SendPlayCard(int cardPlayId)
    {
        photonManager.SendPlayCard(cardPlayId);
    }

    /// <summary>
    /// カードをプレイした時、プレイしたカードIDを相手に送信する。
    /// </summary>
    /// <param name="number"></param>
    public void SendPlaySpellCard(int cardPlayId, int targetCardPlayId, bool isTargetPlayer)
    {
        photonManager.SendPlaySpellCard(cardPlayId, targetCardPlayId, isTargetPlayer);
    }

    /// <summary>
    /// カードをマナコストにした時、マナコストにしたカードIDを相手に送信する。
    /// </summary>
    public void SendAddManaCostCard(int cardPlayId)
    {
        photonManager.SendAddManaCostCard(cardPlayId);
    }

    /// <summary>
    /// ターンエンドボタンを押下した時、ターンエンド処理を走らせ、相手にターンを渡す。
    /// </summary>
    public void SendChangeTurn()
    {
        photonManager.SendChangeTurn();
    }

    /// <summary>
    // 被攻撃処理（カード対カード）
    /// </summary>
    public void SendAtackCard(int attackCardFieldId, int defenceCardFieldId)
    {
        photonManager.SendAtackCard(attackCardFieldId, defenceCardFieldId);
    }

    /// <summary>
    // 非攻撃処理（カード対プレイヤー）
    /// </summary>
    public void SendAtackPlayer(int attackCardFieldId)
    {
        photonManager.SendAtackPlayer(attackCardFieldId);
    }

    /// <summary>
    /// ライブラリアウトしたことを送信する
    /// </summary>
    public void SendLibraryOut()
    {
        photonManager.SendLibraryOut();
    }
}
