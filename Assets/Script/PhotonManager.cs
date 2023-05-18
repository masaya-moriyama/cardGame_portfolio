using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    /// <summary>
    /// カードをドローした時、ドローしたカードIDを相手に送信する。
    /// </summary>
    public void SendDrowCard(int cardId)
    {
        photonView.RPC(nameof(RPCOnRecievedDrowCard), RpcTarget.Others, cardId);
    }

    [PunRPC]
    void RPCOnRecievedDrowCard(int cardId)
    {
        GameManager.instance.DrowDeck(GameManager.instance.enemyDeck, GameManager.instance.enemyHandTransform, false, cardId);
        Debug.Log("RPCOnRecievedDrowCard : " + cardId);
    }

    /// <summary>
    /// カードをプレイした時、プレイしたカードIDを相手に送信する。
    /// </summary>
    /// <param name="number"></param>
    public void SendPlayCard(int cardPlayId)
    {
        photonView.RPC(nameof(RPCOnRecievedPlayCard), RpcTarget.Others, cardPlayId);
    }

    [PunRPC]
    void RPCOnRecievedPlayCard(int cardPlayId)
    {
        // 手札のカードをフィールドにプレイ
        CardController selectableHandCard = getSelectableEnemyHandCard(cardPlayId);

        StartCoroutine(selectableHandCard.move.MoveToField(GameManager.instance.enemyFieldTransForm));

        // モンスターカードのプレイ処理
        selectableHandCard.OnField(false);
        selectableHandCard.Show();

        Debug.Log("RPCOnRecievedCard : " + cardPlayId);
    }

    /// <summary>
    /// スペルカードをプレイした時、プレイしたカードIDを相手に送信する。
    /// </summary>
    /// <param name="number"></param>
    public void SendPlaySpellCard(int cardPlayId, int targetCardPlayId, bool isTargetPlayer)
    {
        photonView.RPC(nameof(RPCOnRecievedPlaySpellCard), RpcTarget.Others, cardPlayId, targetCardPlayId, isTargetPlayer);
    }

    [PunRPC]
    void RPCOnRecievedPlaySpellCard(int cardPlayId, int targetCardPlayId, bool isTargetPlayer)
    {
        // 手札のカードを取得
        CardController selectableHandCard = getSelectableEnemyHandCard(cardPlayId);

        // 対象となるプレイヤーのフィールドのカードを取得する。
        CardController targetCard = null;

        if (!isTargetPlayer)
        {
            targetCard = getSelectablePlayerFieldCard(targetCardPlayId);
        }
        else
        {
            targetCard = getSelectableEnemyFieldCard(targetCardPlayId);
        }

        // 対象が存在するならば、移動先は対象にする。
        Transform transform = GameManager.instance.enemyFieldTransForm;
        if (targetCard != null)
        {
            transform = targetCard.gameObject.transform;
        }

        // 使用を表示
        StartCoroutine(selectableHandCard.move.MoveToUseSpell(transform));

        // 使用SE再生
        SoundManager.instance.PlaySeToSplellUse();

        // BGM再生判定
        SoundManager.instance.CheckChangeBGMToCardId(selectableHandCard.model.id);

        // スペルカードのプレイ処理
        selectableHandCard.UseSpell(targetCard, false);
        Debug.Log("RPCOnRecievedPlaySpellCard : " + cardPlayId + ", " + targetCardPlayId);
    }

    private CardController getSelectableEnemyHandCard(int cardPlayId)
    {
        CardController[] handCardList = GameManager.instance.enemyHandTransform.GetComponentsInChildren<CardController>();
        return Array.Find(handCardList, card => card.model.cardPlayId == cardPlayId);
    }

    private CardController getSelectablePlayerFieldCard(int cardPlayId)
    {
        CardController[] handCardList = GameManager.instance.playerFieldTransForm.GetComponentsInChildren<CardController>();
        return Array.Find(handCardList, card => card.model.cardPlayId == cardPlayId);
    }

    private CardController getSelectableEnemyFieldCard(int cardPlayId)
    {
        CardController[] handCardList = GameManager.instance.enemyFieldTransForm.GetComponentsInChildren<CardController>();
        return Array.Find(handCardList, card => card.model.cardPlayId == cardPlayId);
    }

    /// <summary>
    /// カードをマナコストにした時、マナコストにしたカードIDを相手に送信する。
    /// </summary>
    public void SendAddManaCostCard(int cardPlayId)
    {
        Debug.Log("SendAddManaCostCard" + cardPlayId);
        photonView.RPC(nameof(RPCOnRecievedAddManaCostCard), RpcTarget.Others, cardPlayId);
    }

    [PunRPC]
    void RPCOnRecievedAddManaCostCard(int cardPlayId)
    {
        GameManager.instance.enemy.defaultManaCost++;
        GameManager.instance.enemy.manaCost++;

        CardController selectableHandCard = getSelectableEnemyHandCard(cardPlayId);

        UIManager.instance.ShowManaCost(GameManager.instance.player.manaCost, GameManager.instance.enemy.manaCost);
        UIManager.instance.ShowDefaultManaCost(GameManager.instance.player.defaultManaCost, GameManager.instance.enemy.defaultManaCost);
        StartCoroutine(selectableHandCard.move.MoveToManaCost(GameManager.instance.enemyManaCostTransForm));
        
        // SE再生
        SoundManager.instance.PlaySeToAddMana();

        Debug.Log("RPCOnRecievedAddManaCostCard : " + cardPlayId);
    }

    /// <summary>
    /// ターンエンドボタンを押下した時、ターンエンド処理を走らせ、相手にターンを渡す。
    /// </summary>
    public void SendChangeTurn()
    {
        photonView.RPC(nameof(RPCOnRecievedChangeTurn), RpcTarget.Others);
    }

    [PunRPC]
    void RPCOnRecievedChangeTurn()
    {
        GameManager.instance.ChangeTrun();
    }

    /// <summary>
    // 被攻撃処理（カード対カード）
    /// </summary>
    public void SendAtackCard(int attackCardFieldId, int defenceCardFieldId)
    {
        photonView.RPC(nameof(RPCOnRecievedAtackCard), RpcTarget.Others, attackCardFieldId, defenceCardFieldId);
    }

    [PunRPC]
    IEnumerator RPCOnRecievedAtackCard(int attackCardPlayId, int defenceCardPlayId)
    {
        // 対象のカードを取得
        CardController[] fieldCardList = GameManager.instance.playerFieldTransForm.GetComponentsInChildren<CardController>();
        CardController[] enemyFieldCardList = GameManager.instance.enemyFieldTransForm.GetComponentsInChildren<CardController>();

        // 被攻撃処理のため、攻撃側は相手で固定
        CardController attacker = Array.Find(enemyFieldCardList, card => card.model.cardPlayId == attackCardPlayId);
        CardController defender = Array.Find(fieldCardList, card => card.model.cardPlayId == defenceCardPlayId);

        // 戦闘の実施
        StartCoroutine(attacker.move.MoveToTarget(defender.transform));
        yield return new WaitForSeconds(0.51F);

        GameManager.instance.CardsBattle(attacker, defender);

        Debug.Log("RPCOnRecievedAtackCard : " + attackCardPlayId + " to " + defenceCardPlayId);
    }

    /// <summary>
    // 非攻撃処理（カード対プレイヤー）
    /// </summary>
    public void SendAtackPlayer(int attackCardFieldId)
    {
        photonView.RPC(nameof(RPCOnRecievedAtackPlayer), RpcTarget.Others, attackCardFieldId);
    }

    [PunRPC]
    IEnumerator RPCOnRecievedAtackPlayer(int attackCardPlayId)
    {
        // 対象のカードを取得
        CardController[] enemyFieldCardList = GameManager.instance.enemyFieldTransForm.GetComponentsInChildren<CardController>();

        CardController attacker = Array.Find(enemyFieldCardList, card => card.model.cardPlayId == attackCardPlayId);

        // 戦闘の実施
        StartCoroutine(attacker.move.MoveToTarget(UIManager.instance.PlayerHpArea.transform));

        yield return new WaitForSeconds(0.25F);
        SoundManager.instance.PlaySeToAttack();
        GameManager.instance.AttackToPlayer(attacker, false);
        yield return new WaitForSeconds(0.25F);

        GameManager.instance.checkPlayerHp();

        Debug.Log("RPCOnRecievedAtackPlayer : " + attackCardPlayId + " to Player");
    }

    /// <summary>
    /// 自分がライブラリアウトしたことを送信する
    /// </summary>
    public void SendLibraryOut()
    {
        photonView.RPC(nameof(RPCOnRecievedLibraryOut), RpcTarget.Others);
    }

    [PunRPC]
    void RPCOnRecievedLibraryOut()
    {
        GameManager.instance.LibraryOut(false);
    }
}
