using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// カードの各個別処理を記載するクラス
/// FIXME: 継承化をもう少し上手くして、スッキリさせたい。
/// </summary>
public class CardEffect
{
    /// <summary>
    /// 2枚ドロー
    /// </summary>
    public bool Effect_7(CardController use, CardController target, bool isPlayer)
    {
        if (OnlineStatusManager.instance.IsOnlineBattle && !isPlayer)
        {
            // オンライン対戦で相手が使用した場合、相手ドローカードはドロー時に同期を取るので処理をしない
            return true;
        }

        GameManager.instance.GiveCardToHandToCount(true, 2);
        return true;
    }

    /// <summary>
    /// 対象1枚を破壊
    /// </summary>
    /// <param name="target"></param>
    public bool Effect_8(CardController use, CardController target, bool isPlayer)
    {
        if (target == null || target.model.ability.isSkulk)
        {
            return false;
        }

        if (!target.model.ability.isIndestructible)
        {
            target.Destroy(false);
        }
        return true;
    }

    /// <summary>
    /// マナを2追加
    /// </summary>
    /// <param name="target"></param>
    public bool Effect_9(CardController use, CardController target, bool isPlayer)
    {
        GamePlayerManager targetPlayerManager = GameManager.instance.player;
        if (!isPlayer)
        {
            targetPlayerManager = GameManager.instance.enemy;
        }

        targetPlayerManager.defaultManaCost += 2;

        UIManager.instance.ShowManaCost(GameManager.instance.player.manaCost, GameManager.instance.enemy.manaCost);
        UIManager.instance.ShowDefaultManaCost(GameManager.instance.player.defaultManaCost, GameManager.instance.enemy.defaultManaCost);

        return true;
    }

    /// <summary>
    /// 対象のパワーの倍にする
    /// </summary>
    /// <param name="target"></param>
    public bool Effect_10(CardController use, CardController target, bool isPlayer)
    {
        if (target == null || target.model.ability.isSkulk)
        {
            return false;
        }
        target.model.atk *= 2;
        target.view.RefreshStatus(target.model);
        return true;
    }

    /// <summary>
    /// CIP:1枚ドロー
    /// </summary>
    public bool Effect_12(CardController use, CardController target, bool isPlayer)
    {
        if (OnlineStatusManager.instance.IsOnlineBattle && !isPlayer)
        {
            // オンライン対戦で相手が使用した場合、相手ドローカードはドロー時に同期を取るので処理をしない
            return true;
        }

        if (isPlayer)
        {
            GameManager.instance.GiveCardToHandToCount(true, 1);
        }
        else
        {
            GameManager.instance.GiveCardToHandToCount(false, 1);
        }
        return true;
    }

    /// <summary>
    /// プレイヤーを5回復
    /// </summary>
    public bool Effect_13(CardController use, CardController target, bool isPlayer)
    {
        if (isPlayer)
        {
            GameManager.instance.player.playerHp += 10;
        }
        else
        {
            GameManager.instance.enemy.playerHp += 10;
        }

        UIManager.instance.showUserHp(GameManager.instance.player.playerHp, GameManager.instance.enemy.playerHp);
        return true;
    }

    /// <summary>
    /// 全てのモンスターを破壊
    /// </summary>
    public bool Effect_16(CardController use, CardController target, bool isPlayer)
    {

        CardController[] eCardList = GameManager.instance.enemyFieldTransForm.GetComponentsInChildren<CardController>();
        CardController[] pCardList = GameManager.instance.playerFieldTransForm.GetComponentsInChildren<CardController>();

        foreach (var item in eCardList)
        {
            if ((!isPlayer && use.model.cardPlayId == item.model.cardPlayId) || item.model.ability.isIndestructible)
            {
                continue;
            }
            item.Destroy(false);
        }
        foreach (var item in pCardList)
        {
            if ((isPlayer && use.model.cardPlayId == item.model.cardPlayId) || item.model.ability.isIndestructible)
            {
                continue;
            }
            item.Destroy(false);
        }
        return true;
    }

    /// <summary>
    /// HPが多いモンスター1枚を破壊
    /// </summary>
    public bool Effect_17(CardController use, CardController target, bool isPlayer)
    {
        if (!isPlayer)
        {
            // 相手が使用した場合は、targetに指定されているカードを破壊
            if (target == null)
            {
                return false;
            }
            if (!target.model.ability.isIndestructible)
            {
                target.Destroy(false);
            }
        }
        else
        {
            // 自分が使用した場合、相手のフィールドの体力最大値のカードを破壊
            CardController[] cardList = GameManager.instance.enemyFieldTransForm.GetComponentsInChildren<CardController>();

            if (cardList.Length <= 0)
            {
                return false;
            }

            Array.Sort(cardList, (x, y) => y.model.hp.CompareTo(x.model.hp));
            use.randTempInt = cardList[0].model.cardPlayId;
            use.randTempTargetIsPlayer = cardList[0].model.isPlayerCard;
            if (!cardList[0].model.ability.isIndestructible)
            {
                cardList[0].Destroy(false);
            }
        }

        return true;
    }

    /// <summary>
    /// 対象に2ダメージ
    /// </summary>
    /// <param name="target"></param>
    public bool Effect_18(CardController use, CardController target, bool isPlayer)
    {
        if (target == null || target.model.ability.isSkulk)
        {
            return false;
        }
        target.model.Damege(2);
        target.view.RefreshStatus(target.model);
        target.CheckAlive();
        return true;
    }

    /// <summary>
    /// 1ドロー 1マナブースト
    /// </summary>
    public bool Effect_19(CardController use, CardController target, bool isPlayer)
    {
        GamePlayerManager targetPlayerManager = GameManager.instance.player;
        if (!isPlayer)
        {
            targetPlayerManager = GameManager.instance.enemy;
        }

        targetPlayerManager.defaultManaCost += 1;

        UIManager.instance.ShowManaCost(GameManager.instance.player.manaCost, GameManager.instance.enemy.manaCost);
        UIManager.instance.ShowDefaultManaCost(GameManager.instance.player.defaultManaCost, GameManager.instance.enemy.defaultManaCost);

        if (OnlineStatusManager.instance.IsOnlineBattle && !isPlayer)
        {
            // オンライン対戦で相手が使用した場合、相手ドローカードはドロー時に同期を取るので処理をしない
            return true;
        }
        GameManager.instance.GiveCardToHandToCount(true, 1);
        return true;
    }

    /// <summary>
    /// </summary>
    /// <param name="target"></param>
    public bool Effect_21(CardController use, CardController target, bool isPlayer)
    {
        if (target == null ||
            target.model.ability.isSkulk ||
            (target.model.id != 5 &&
            target.model.id != 29 &&
            target.model.id != 46))
        {
            return false;
        }
        target.model.atk += 5;
        target.view.RefreshStatus(target.model);
        return true;
    }

    /// <summary>
    /// マナ全消費し、消費した数ダメージ
    /// </summary>
    /// <param name="target"></param>
    public bool Effect_22(CardController use, CardController target, bool isPlayer)
    {
        GamePlayerManager targetPlayerManager;
        if (!isPlayer)
        {
            targetPlayerManager = GameManager.instance.enemy;
        }
        else
        {
            targetPlayerManager = GameManager.instance.player;
        }

        if (targetPlayerManager.manaCost <= 0)
        {
            return false;
        }

        int mana = targetPlayerManager.manaCost;
        targetPlayerManager.manaCost = 0;

        UIManager.instance.ShowManaCost(GameManager.instance.player.manaCost, GameManager.instance.enemy.manaCost);
        UIManager.instance.ShowDefaultManaCost(GameManager.instance.player.defaultManaCost, GameManager.instance.enemy.defaultManaCost);

        CardController[] eCardList = GameManager.instance.enemyFieldTransForm.GetComponentsInChildren<CardController>();
        CardController[] pCardList = GameManager.instance.playerFieldTransForm.GetComponentsInChildren<CardController>();


        if (eCardList.Length <= 0 && pCardList.Length <= 0)
        {
            return false;
        }

        foreach (var item in eCardList)
        {
            item.model.Damege(mana);
            item.view.RefreshStatus(item.model);
            item.CheckAlive();
        }
        foreach (var item in pCardList)
        {
            item.model.Damege(mana);
            item.view.RefreshStatus(item.model);
            item.CheckAlive();
        }

        return true;
    }

    /// <summary>
    /// CIP:全HP+1
    /// </summary>
    /// <param name="target"></param>
    public bool Effect_24(CardController use, CardController target, bool isPlayer)
    {
        CardController[] cardList;
        if (isPlayer)
        {
            cardList = GameManager.instance.playerFieldTransForm.GetComponentsInChildren<CardController>();
        }
        else
        {
            cardList = GameManager.instance.enemyFieldTransForm.GetComponentsInChildren<CardController>();
        }

        foreach (var item in cardList)
        {
            item.model.hp++;
            item.view.RefreshStatus(item.model);
        }

        return true;
    }

    /// <summary>
    /// CIP:全1ダメージ
    /// </summary>
    /// <param name="target"></param>
    public bool Effect_25(CardController use, CardController target, bool isPlayer)
    {
        CardController[] cardList;
        if (isPlayer)
        {
            cardList = GameManager.instance.enemyFieldTransForm.GetComponentsInChildren<CardController>();
        }
        else
        {
            cardList = GameManager.instance.playerFieldTransForm.GetComponentsInChildren<CardController>();
        }

        foreach (var item in cardList)
        {
            item.model.Damege(1);
            item.view.RefreshStatus(item.model);
            item.CheckAlive();
        }

        return true;
    }

    /// <summary>
    /// 相手のHP分ダメージ（特殊勝利）
    /// </summary>
    /// <param name="target"></param>
    public bool Effect_26(CardController use, CardController target, bool isPlayer)
    {
        if (isPlayer)
        {
            GameManager.instance.enemy.playerHp -= GameManager.instance.enemy.playerHp;
        }
        else
        {
            GameManager.instance.player.playerHp -= GameManager.instance.player.playerHp;
        }
        GameManager.instance.checkPlayerHp();

        return true;
    }

    /// <summary>
    /// 敵味方問わず全HP+2
    /// </summary>
    /// <param name="target"></param>
    public bool Effect_27(CardController use, CardController target, bool isPlayer)
    {
        CardController[] pCardList = GameManager.instance.playerFieldTransForm.GetComponentsInChildren<CardController>();
        CardController[] eCardList = GameManager.instance.enemyFieldTransForm.GetComponentsInChildren<CardController>();

        if (pCardList.Length <= 0 && eCardList.Length <= 0)
        {
            return false;
        }

        foreach (var item in pCardList)
        {
            item.model.hp += 2;
            item.view.RefreshStatus(item.model);
        }
        foreach (var item in eCardList)
        {
            item.model.hp += 2;
            item.view.RefreshStatus(item.model);
        }

        return true;
    }

    /// <summary>
    /// CIP:手札のコスト-1
    /// </summary>
    /// <param name="target"></param>
    public bool Effect_28(CardController use, CardController target, bool isPlayer)
    {
        CardController[] cardList;
        if (isPlayer)
        {
            cardList = GameManager.instance.playerHandTransform.GetComponentsInChildren<CardController>();
        }
        else
        {
            cardList = GameManager.instance.enemyHandTransform.GetComponentsInChildren<CardController>();
        }

        foreach (var item in cardList)
        {
            if (item.model.cost <= 0)
            {
                continue;
            }
            item.model.cost -= 1;
            item.view.RefreshStatus(item.model);
        }

        return true;
    }

    /// <summary>
    /// 2ドロー 1マナブースト
    /// </summary>
    public bool Effect_30(CardController use, CardController target, bool isPlayer)
    {
        GamePlayerManager targetPlayerManager = GameManager.instance.player;
        if (!isPlayer)
        {
            targetPlayerManager = GameManager.instance.enemy;
        }

        targetPlayerManager.defaultManaCost += 1;

        UIManager.instance.ShowManaCost(GameManager.instance.player.manaCost, GameManager.instance.enemy.manaCost);
        UIManager.instance.ShowDefaultManaCost(GameManager.instance.player.defaultManaCost, GameManager.instance.enemy.defaultManaCost);

        if (OnlineStatusManager.instance.IsOnlineBattle && !isPlayer)
        {
            // オンライン対戦で相手が使用した場合、相手ドローカードはドロー時に同期を取るので処理をしない
            return true;
        }

        GameManager.instance.GiveCardToHandToCount(true, 2);
        return true;
    }

    /// <summary>
    /// トークンID1,ID2をプレイ
    /// </summary>
    public bool Effect_33(CardController use, CardController target, bool isPlayer)
    {
        if (isPlayer && GameManager.instance.IsUpperlimitFieldCard())
        {
            return false;
        }

        Transform field = GameManager.instance.playerFieldTransForm;
        if (!isPlayer)
        {
            field = GameManager.instance.enemyFieldTransForm;
        }

        CardController card_1 = GameManager.instance.CrateCard(1, field, isPlayer, false);
        card_1.model.isFieldCard = true;
        card_1.Show();

        CardController card_2 = GameManager.instance.CrateCard(2, field, isPlayer, false);
        card_2.model.isFieldCard = true;
        card_2.InitCanAttackCount(true);
        card_2.Show();

        return true;
    }

    /// <summary>
    /// CIP:ATKがプレイ時の手札の数になる
    /// </summary>
    /// <param name="target"></param>
    public bool Effect_34(CardController use, CardController target, bool isPlayer)
    {
        CardController[] cardList;
        if (isPlayer)
        {
            cardList = GameManager.instance.playerHandTransform.GetComponentsInChildren<CardController>();
        }
        else
        {
            cardList = GameManager.instance.enemyHandTransform.GetComponentsInChildren<CardController>();
        }

        use.model.atk = cardList.Length;
        use.view.RefreshStatus(use.model);
        return true;
    }

    /// <summary>
    /// CIP:マナを+2し、2ドロー
    /// </summary>
    /// <param name="target"></param>
    public bool Effect_35(CardController use, CardController target, bool isPlayer)
    {
        GamePlayerManager targetPlayerManager = GameManager.instance.player;
        if (!isPlayer)
        {
            targetPlayerManager = GameManager.instance.enemy;
        }

        targetPlayerManager.defaultManaCost += 2;

        UIManager.instance.ShowManaCost(GameManager.instance.player.manaCost, GameManager.instance.enemy.manaCost);
        UIManager.instance.ShowDefaultManaCost(GameManager.instance.player.defaultManaCost, GameManager.instance.enemy.defaultManaCost);

        if (OnlineStatusManager.instance.IsOnlineBattle && !isPlayer)
        {
            // オンライン対戦で相手が使用した場合、相手ドローカードはドロー時に同期を取るので処理をしない
            return true;
        }

        GameManager.instance.GiveCardToHandToCount(true, 2);
        return true;
    }

    /// <summary>
    /// 相手の場の数だけドロー
    /// </summary>
    public bool Effect_38(CardController use, CardController target, bool isPlayer)
    {
        if (OnlineStatusManager.instance.IsOnlineBattle && !isPlayer)
        {
            // オンライン対戦で相手が使用した場合、相手ドローカードはドロー時に同期を取るので処理をしない
            return true;
        }
        CardController[] cardList = GameManager.instance.enemyFieldTransForm.GetComponentsInChildren<CardController>();

        // ドロー処理
        int drowCount = cardList.Length;
        GameManager.instance.GiveCardToHandToCount(true, drowCount);

        // 相手のドロー処理は、ドロー時の同期の兼ね合いで実施しない
        return true;
    }

    /// <summary>
    /// 全てのカードを破壊する
    /// </summary>
    public bool Effect_39(CardController use, CardController target, bool isPlayer)
    {
        CardController[] eCardList = GameManager.instance.enemyFieldTransForm.GetComponentsInChildren<CardController>();
        CardController[] pCardList = GameManager.instance.playerFieldTransForm.GetComponentsInChildren<CardController>();

        if (pCardList.Length + eCardList.Length <= 0)
        {
            return false;
        }

        foreach (var item in eCardList)
        {
            if (item.model.ability.isIndestructible)
            {
                continue;
            }
            item.Destroy(false);
        }
        foreach (var item in pCardList)
        {
            if (item.model.ability.isIndestructible)
            {
                continue;
            }
            item.Destroy(false);
        }
        return true;
    }

    /// <summary>
    /// 3枚ドロー
    /// </summary>
    public bool Effect_40(CardController use, CardController target, bool isPlayer)
    {
        if (OnlineStatusManager.instance.IsOnlineBattle && !isPlayer)
        {
            // オンライン対戦で相手が使用した場合、相手ドローカードはドロー時に同期を取るので処理をしない
            return true;
        }
        GameManager.instance.GiveCardToHandToCount(true, 3);
        return true;
    }

    /// <summary>
    /// 能力剥奪
    /// </summary>
    /// <param name="target"></param>
    public bool Effect_41(CardController use, CardController target, bool isPlayer)
    {
        CardController[] pCardList = GameManager.instance.playerFieldTransForm.GetComponentsInChildren<CardController>();
        CardController[] eCardList = GameManager.instance.enemyFieldTransForm.GetComponentsInChildren<CardController>();

        if (pCardList.Length <= 0 && eCardList.Length <= 0)
        {
            return false;
        }

        foreach (var item in pCardList)
        {
            item.model.ability.setAllAbilityFlg(false);
            item.view.SetCardAbility(item.model);
        }
        foreach (var item in eCardList)
        {
            item.model.ability.setAllAbilityFlg(false);
            item.view.SetCardAbility(item.model);
        }

        return true;
    }

    /// <summary>
    /// 対象のHPを倍にする
    /// </summary>
    /// <param name="target"></param>
    public bool Effect_42(CardController use, CardController target, bool isPlayer)
    {
        if (target == null || target.model.ability.isSkulk)
        {
            return false;
        }
        target.model.hp *= 2;
        target.view.RefreshStatus(target.model);
        return true;
    }

    /// <summary>
    /// CIP:ATKに20からHP分引いた数を加算する
    /// </summary>
    /// <param name="target"></param>
    public bool Effect_43(CardController use, CardController target, bool isPlayer)
    {
        int playerHp;
        if (isPlayer)
        {
            playerHp = GameManager.instance.player.playerHp;
        }
        else
        {
            playerHp = GameManager.instance.enemy.playerHp;
        }

        if (playerHp >= 20)
        {
            return true;
        }

        int addInt = 20 - playerHp;

        use.model.atk += addInt;
        use.view.RefreshStatus(use.model);
        return true;
    }

    /// <summary>
    /// cip:マナを1追加
    /// </summary>
    /// <param name="target"></param>
    public bool Effect_44(CardController use, CardController target, bool isPlayer)
    {
        GamePlayerManager targetPlayerManager = GameManager.instance.player;
        if (!isPlayer)
        {
            targetPlayerManager = GameManager.instance.enemy;
        }

        targetPlayerManager.defaultManaCost += 1;

        UIManager.instance.ShowManaCost(GameManager.instance.player.manaCost, GameManager.instance.enemy.manaCost);
        UIManager.instance.ShowDefaultManaCost(GameManager.instance.player.defaultManaCost, GameManager.instance.enemy.defaultManaCost);

        return true;
    }

    /// <summary>
    /// 破壊時:コストが最大のランダムに破壊
    /// </summary>
    public bool Effect_45(CardController use, CardController target, bool isPlayer)
    {
        List<CardController> cardList = new List<CardController>();

        if (isPlayer)
        {
            GameManager.instance.enemyFieldTransForm.GetComponentsInChildren<CardController>(cardList);
        }
        else
        {
            GameManager.instance.playerFieldTransForm.GetComponentsInChildren<CardController>(cardList);
        }

        if (cardList.Count <= 0)
        {
            return false;
        }

        cardList.Sort((x, y) =>
            {
                int result = x.model.isAlive ? 0 : 1; // 第一条件 破壊が確定していない
                int result2 = result != 0 ? result : y.model.cost.CompareTo(x.model.cost); // 第二条件 コストが最大
                return result2 != 0 ? result : x.model.cardPlayId.CompareTo(y.model.cardPlayId); // 第三条件 最初にプレイした
            });

        foreach (var item in cardList)
        {
            Debug.Log("Effect_45 target:" + item.model.id + " , name : " + item.model.cardName);
            if (!item.model.isAlive)
            {
                // 生存してないなら対象を次へ
                Debug.Log("破壊が確定しているのでSkip");
                continue;
            }

            if (item.model.ability.isIndestructible)
            {
                // 破壊不能が対象となった場合、破壊せず終了
                break;
            }

            item.Destroy(false);
            break;
        }
        return true;
    }

    /// <summary>
    /// </summary>
    /// <param name="target"></param>
    public bool Effect_46(CardController use, CardController target, bool isPlayer)
    {
        double turn = GameManager.instance.turnCount / 2;
        use.model.atk += (int)Math.Ceiling(turn);
        use.view.RefreshStatus(use.model);
        return true;
    }

    /// <summary>
    /// </summary>
    /// <param name="target"></param>
    public bool Effect_47(CardController use, CardController target, bool isPlayer)
    {
        Transform field = GameManager.instance.playerFieldTransForm;
        if (!isPlayer)
        {
            field = GameManager.instance.enemyFieldTransForm;
        }
        CardController[] cardList = field.GetComponentsInChildren<CardController>();

        if (cardList.Length >= 5)
        {
            return false;
        }

        for (int i = cardList.Length; 5 > i; i++)
        {
            CardController card = GameManager.instance.CrateCard(5, field, isPlayer, true);
            card.model.isFieldCard = true;
            card.Show();
        }

        return true;
    }

    /// <summary>
    /// </summary>
    /// <param name="target"></param>
    public bool Effect_48(CardController use, CardController target, bool isPlayer)
    {
        CardController[] cardList;
        if (isPlayer)
        {
            cardList = GameManager.instance.playerFieldTransForm.GetComponentsInChildren<CardController>();
        }
        else
        {
            cardList = GameManager.instance.enemyFieldTransForm.GetComponentsInChildren<CardController>();
        }

        foreach (var item in cardList)
        {
            if (item.model.id != 5 &&
            item.model.id != 29 &&
            item.model.id != 46)
            {
                continue;
            }
            item.InitCanAttackCount(true);
        }

        return true;
    }

    /// <summary>
    /// 相手の場を全て破壊（破壊無効を無視）
    /// </summary>
    public bool Effect_51(CardController use, CardController target, bool isPlayer)
    {
        CardController[] cardList;
        if (isPlayer)
        {
            cardList = GameManager.instance.enemyFieldTransForm.GetComponentsInChildren<CardController>();
        }
        else
        {
            cardList = GameManager.instance.playerFieldTransForm.GetComponentsInChildren<CardController>();
        }

        if (cardList.Length <= 0)
        {
            return false;
        }

        foreach (var item in cardList)
        {
            // 破壊不能・消滅を無視する。
            item.Destroy(true);
        }
        return true;
    }

    /// <summary>
    /// 相手の場のHPを1に
    /// </summary>
    public bool Effect_52(CardController use, CardController target, bool isPlayer)
    {
        CardController[] cardList;
        if (isPlayer)
        {
            cardList = GameManager.instance.enemyFieldTransForm.GetComponentsInChildren<CardController>();
        }
        else
        {
            cardList = GameManager.instance.playerFieldTransForm.GetComponentsInChildren<CardController>();
        }

        if (cardList.Length <= 0)
        {
            return false;
        }

        foreach (var item in cardList)
        {
            item.model.hp = 1;
            item.view.RefreshStatus(item.model);
        }
        return true;
    }

    /// <summary>
    /// プレイ時：自傷5
    /// </summary>
    public bool Effect_55(CardController use, CardController target, bool isPlayer)
    {
        if (isPlayer)
        {
            GameManager.instance.AttackToPlayer(5, false);
            GameManager.instance.checkPlayerHp();
        }
        else
        {
            GameManager.instance.AttackToPlayer(5, true);
            GameManager.instance.checkPlayerHp();
        }

        return true;
    }

    /// <summary>
    /// 「接死」「潜伏」「再生」を付与
    /// </summary>
    /// <param name="target"></param>
    public bool Effect_58(CardController use, CardController target, bool isPlayer)
    {
        if (target == null || target.model.ability.isSkulk)
        {
            return false;
        }

        target.model.ability.isDeathTouch = true;
        target.model.ability.isSkulk = true;
        target.model.ability.isRegenerate = true;

        target.view.RefreshAbility(target.model);
        return true;
    }

    /// <summary>
    /// CIP:HPを1にし、その差分HP・ATKに加算
    /// </summary>
    /// <param name="target"></param>
    public bool Effect_59(CardController use, CardController target, bool isPlayer)
    {
        int addInt = 0;
        if (isPlayer)
        {
            addInt = GameManager.instance.player.playerHp - 1;
            GameManager.instance.AttackToPlayer(addInt, false);
            GameManager.instance.checkPlayerHp();
        }
        else
        {
            addInt = GameManager.instance.enemy.playerHp - 1;
            GameManager.instance.AttackToPlayer(addInt, true);
            GameManager.instance.checkPlayerHp();
        }

        use.model.atk += addInt;
        use.model.hp += addInt;
        use.view.RefreshStatus(use.model);
        return true;
    }

    /// <summary>
    /// 対象以外を破壊し、対象に合算する
    /// </summary>
    /// <param name="target"></param>
    public bool Effect_60(CardController use, CardController target, bool isPlayer)
    {
        if (target == null ||
            target.model.ability.isSkulk ||
            target.model.id != 59 ||
            isPlayer != target.model.isPlayerCard)
        {
            return false;
        }

        int addIntHp = 0;
        int addIntAtk = 0;
        CardController[] cardList;

        if (isPlayer)
        {
            cardList = GameManager.instance.playerFieldTransForm.GetComponentsInChildren<CardController>();

        }
        else
        {
            cardList = GameManager.instance.enemyFieldTransForm.GetComponentsInChildren<CardController>();
        }

        if (cardList.Length <= 0)
        {
            return false;
        }

        foreach (var item in cardList)
        {
            if (item.model.cardPlayId == target.model.cardPlayId ||
                !item.model.IsCanAttack() ||
                item.model.ability.isIndestructible)
            {
                continue;
            }
            addIntHp += item.model.hp;
            addIntAtk += item.model.atk;
            item.Destroy(false);
        }

        target.model.hp += addIntHp;
        target.model.atk += addIntAtk;
        target.view.RefreshStatus(target.model);

        return true;
    }

    /// <summary>
    /// 対象を破壊し、破壊したHP分回復
    /// </summary>
    /// <param name="target"></param>
    public bool Effect_61(CardController use, CardController target, bool isPlayer)
    {
        if (target == null ||
            target.model.ability.isSkulk ||
            target.model.id != 59 ||
            isPlayer != target.model.isPlayerCard)
        {
            return false;
        }

        int addInt = target.model.hp;
        target.Destroy(false);

        if (isPlayer)
        {
            GameManager.instance.player.playerHp += addInt;
        }
        else
        {
            GameManager.instance.enemy.playerHp += addInt;
        }
        UIManager.instance.showUserHp(GameManager.instance.player.playerHp, GameManager.instance.enemy.playerHp);

        return true;
    }

    /// <summary>
    /// </summary>
    /// <param name="target"></param>
    public bool Effect_62(CardController use, CardController target, bool isPlayer)
    {
        Transform hand = GameManager.instance.playerHandTransform;
        if (!isPlayer)
        {
            hand = GameManager.instance.enemyHandTransform;
        }

        GameManager.instance.CrateCard(59, hand, isPlayer, true);
        return true;
    }

    /// <summary>
    /// 全員をモスに変える。
    /// </summary>
    /// <param name="target"></param>
    public bool Effect_65(CardController use, CardController target, bool isPlayer)
    {
        Transform field = GameManager.instance.playerFieldTransForm;
        if (!isPlayer)
        {
            field = GameManager.instance.enemyFieldTransForm;
        }
        CardController[] cardList = field.GetComponentsInChildren<CardController>();

        int createInt = cardList.Length;

        if (cardList.Length <= 0)
        {
            return false;
        }

        foreach (var item in cardList)
        {
            item.Destroy(true);
        }

        for (int i = 0; i < createInt; i++)
        {
            CardController card = GameManager.instance.CrateCard(11, field, isPlayer, true);
            card.model.isFieldCard = true;
            card.Show();
        }

        return true;
    }

    /// <summary>
    /// 自分の場の「モス」と名のついたカードを全て破壊し、その合計の倍プレイヤーのHPを回復
    /// </summary>
    /// <param name="target"></param>
    public bool Effect_66(CardController use, CardController target, bool isPlayer)
    {
        CardController[] cardList;
        if (isPlayer)
        {
            cardList = GameManager.instance.playerFieldTransForm.GetComponentsInChildren<CardController>();
        }
        else
        {
            cardList = GameManager.instance.enemyFieldTransForm.GetComponentsInChildren<CardController>();
        }

        if (cardList.Length <= 0)
        {
            return false;
        }

        int addCount = 0;

        foreach (var item in cardList)
        {
            if (!item.model.cardName.Contains("モス"))
            {
                continue;
            }
            if (item.model.ability.isIndestructible)
            {
                continue;
            }
            addCount++;
            item.Destroy(false);
        }

        if (isPlayer)
        {
            GameManager.instance.player.playerHp += addCount * 10;
        }
        else
        {
            GameManager.instance.enemy.playerHp += addCount * 10;
        }
        UIManager.instance.showUserHp(GameManager.instance.player.playerHp, GameManager.instance.enemy.playerHp);

        return true;
    }

    /// <summary>
    /// プレイ時：自傷50
    /// </summary>
    public bool Effect_67(CardController use, CardController target, bool isPlayer)
    {
        GameManager.instance.AttackToPlayer(50, !isPlayer);
        GameManager.instance.checkPlayerHp();

        return true;
    }

    /// <summary>
    /// コスト8以上の全てのカードを破壊する
    /// </summary>
    public bool Effect_68(CardController use, CardController target, bool isPlayer)
    {
        CardController[] eCardList = GameManager.instance.enemyFieldTransForm.GetComponentsInChildren<CardController>();
        CardController[] pCardList = GameManager.instance.playerFieldTransForm.GetComponentsInChildren<CardController>();

        if (pCardList.Length + eCardList.Length <= 0)
        {
            return false;
        }

        foreach (var item in eCardList)
        {
            if (item.model.ability.isIndestructible || item.model.cost > 8)
            {
                continue;
            }
            item.Destroy(false);
        }
        foreach (var item in pCardList)
        {
            if (item.model.ability.isIndestructible || item.model.cost > 8)
            {
                continue;
            }
            item.Destroy(false);
        }
        return true;
    }

    /// <summary>
    /// Hp1の全てのカードを破壊する
    /// </summary>
    public bool Effect_69(CardController use, CardController target, bool isPlayer)
    {
        CardController[] eCardList = GameManager.instance.enemyFieldTransForm.GetComponentsInChildren<CardController>();
        CardController[] pCardList = GameManager.instance.playerFieldTransForm.GetComponentsInChildren<CardController>();

        if (pCardList.Length + eCardList.Length <= 0)
        {
            return false;
        }

        foreach (var item in eCardList)
        {
            if (item.model.ability.isIndestructible || item.model.hp != 1)
            {
                continue;
            }
            item.Destroy(false);
        }
        foreach (var item in pCardList)
        {
            if (item.model.ability.isIndestructible || item.model.hp != 1)
            {
                continue;
            }
            item.Destroy(false);
        }
        return true;
    }

    /// <summary>
    /// エンハンス6 再生
    /// </summary>
    public bool Effect_70(CardController use, CardController target, bool isPlayer)
    {
        int mana = GameManager.instance.player.manaCost;
        if (!isPlayer)
        {
            mana = GameManager.instance.enemy.manaCost;
        }

        if (mana < 2)
        {
            // できないなら何もしない
            return true;
        }

        if (isPlayer)
        {
            GameManager.instance.player.manaCost -= 2;
        }
        else
        {
            GameManager.instance.enemy.manaCost -= 2; ;
        }
        UIManager.instance.ShowManaCost(GameManager.instance.player.manaCost, GameManager.instance.enemy.manaCost);

        // エンハンス可能なら、再生を付与
        use.model.ability.isRegenerate = true;
        use.view.RefreshAbility(use.model);

        return true;
    }

    /// <summary>
    /// 対象1枚を消滅させ、持ち主のマナを+1
    /// </summary>
    /// <param name="target"></param>
    public bool Effect_71(CardController use, CardController target, bool isPlayer)
    {
        if (target == null || target.model.ability.isSkulk)
        {
            return false;
        }

        target.Destroy(true);

        if (target.model.isPlayerCard)
        {
            GameManager.instance.player.defaultManaCost += 1;
        }
        else
        {
            GameManager.instance.enemy.defaultManaCost += 1;
        }
        UIManager.instance.ShowManaCost(GameManager.instance.player.manaCost, GameManager.instance.enemy.manaCost);
        UIManager.instance.ShowDefaultManaCost(GameManager.instance.player.defaultManaCost, GameManager.instance.enemy.defaultManaCost);
        return true;
    }

    /// <summary>
    /// 自分の場のカードを全て破壊し、その枚数*10プレイヤーのHPを回復
    /// </summary>
    /// <param name="target"></param>
    public bool Effect_72(CardController use, CardController target, bool isPlayer)
    {
        CardController[] cardList;
        if (isPlayer)
        {
            cardList = GameManager.instance.playerFieldTransForm.GetComponentsInChildren<CardController>();
        }
        else
        {
            cardList = GameManager.instance.enemyFieldTransForm.GetComponentsInChildren<CardController>();
        }

        if (cardList.Length <= 0)
        {
            return false;
        }

        int addCount = 0;

        foreach (var item in cardList)
        {
            if (item.model.ability.isIndestructible)
            {
                continue;
            }
            addCount++;
            item.Destroy(false);
        }

        if (isPlayer)
        {
            GameManager.instance.player.playerHp += addCount * 10;
        }
        else
        {
            GameManager.instance.enemy.playerHp += addCount * 10;
        }
        UIManager.instance.showUserHp(GameManager.instance.player.playerHp, GameManager.instance.enemy.playerHp);

        return true;
    }

    /// <summary>
    /// 破壊時:マナ++
    /// </summary>
    public bool Effect_73(CardController use, CardController target, bool isPlayer)
    {
        GamePlayerManager targetPlayerManager = GameManager.instance.player;
        if (!isPlayer)
        {
            targetPlayerManager = GameManager.instance.enemy;
        }

        targetPlayerManager.defaultManaCost++;

        UIManager.instance.ShowManaCost(GameManager.instance.player.manaCost, GameManager.instance.enemy.manaCost);
        UIManager.instance.ShowDefaultManaCost(GameManager.instance.player.defaultManaCost, GameManager.instance.enemy.defaultManaCost);

        return true;
    }

    /// <summary>
    /// 対象のパワーの1/4にする
    /// </summary>
    /// <param name="target"></param>
    public bool Effect_74(CardController use, CardController target, bool isPlayer)
    {
        if (target == null || target.model.ability.isSkulk || target.model.atk == 0)
        {
            return false;
        }

        double targetAtk = target.model.atk / 4;
        target.model.atk = (int)Math.Ceiling(targetAtk);
        target.model.atk = target.model.atk != 0 ? target.model.atk : 1; // 切り捨て0になってしまっているなら、1に丸める。
        target.view.RefreshStatus(target.model);
        return true;
    }

    /// <summary>
    /// </summary>
    /// <param name="target"></param>
    public bool Effect_75(CardController use, CardController target, bool isPlayer)
    {
        Transform field = GameManager.instance.playerFieldTransForm;
        if (!isPlayer)
        {
            field = GameManager.instance.enemyFieldTransForm;
        }
        CardController[] cardList = field.GetComponentsInChildren<CardController>();

        if (cardList.Length <= 0)
        {
            return false;
        }

        int addIntHp = 0;
        int addIntAtk = 0;

        foreach (var item in cardList)
        {
            if (item.model.ability.isIndestructible ||
                (item.model.id != 5 &&
                item.model.id != 29 &&
                item.model.id != 46
                ))
            {
                continue;
            }
            addIntHp += item.model.hp;
            addIntAtk += item.model.atk;
            item.Destroy(true);
        }

        CardController card = GameManager.instance.CrateCard(5, field, isPlayer, true);
        card.model.hp += addIntHp - 1;
        card.model.atk += addIntAtk - 1;
        card.model.isFieldCard = true;
        card.Show();
        card.view.RefreshStatus(card.model);

        return true;
    }

    /// <summary>
    /// CIP:全1ダメージ+貫通付与
    /// </summary>
    /// <param name="target"></param>
    public bool Effect_76(CardController use, CardController target, bool isPlayer)
    {
        CardController[] pCardList;
        CardController[] eCardList;

        if (isPlayer)
        {
            pCardList = GameManager.instance.playerFieldTransForm.GetComponentsInChildren<CardController>();
            eCardList = GameManager.instance.enemyFieldTransForm.GetComponentsInChildren<CardController>();
        }
        else
        {
            pCardList = GameManager.instance.enemyFieldTransForm.GetComponentsInChildren<CardController>();
            eCardList = GameManager.instance.playerFieldTransForm.GetComponentsInChildren<CardController>();
        }

        foreach (var item in eCardList)
        {
            item.model.Damege(1);
            item.view.RefreshStatus(item.model);
            item.CheckAlive();
        }

        foreach (var item in pCardList)
        {
            if (use.model.cardPlayId == item.model.cardPlayId)
            {
                continue;
            }
            item.model.ability.isPenetration = true;
            item.view.RefreshAbility(item.model);
        }

        return true;
    }

    /// <summary>
    /// トークンID3を3枚プレイ
    /// </summary>
    public bool Effect_77(CardController use, CardController target, bool isPlayer)
    {
        if (isPlayer)
        {
            if (GameManager.instance.GetPlayerFieldCardCount() > 1)
            {
                return false;
            }
        }
        else
        {
            if (GameManager.instance.GetEnemyFieldCardCount() > 1)
            {
                return false;
            }
        }

        Transform field = GameManager.instance.playerFieldTransForm;
        if (!isPlayer)
        {
            field = GameManager.instance.enemyFieldTransForm;
        }

        for (int i = 0; i < 3; i++)
        {
            CardController card = GameManager.instance.CrateCard(3, field, isPlayer, false);
            card.model.isFieldCard = true;
            card.Show();
        }
        return true;
    }

    /// <summary>
    /// 墓地の枚数の半分ダメージ
    /// </summary>
    public bool Effect_78(CardController use, CardController target, bool isPlayer)
    {
        // 墓地を参照
        List<int> cemetery;
        CardController[] eCardList;

        if (isPlayer)
        {
            cemetery = GameManager.instance.GetCemeteryListPlayer();
            eCardList = GameManager.instance.enemyFieldTransForm.GetComponentsInChildren<CardController>();
        }
        else
        {
            cemetery = GameManager.instance.GetCemeteryListEnemy();
            eCardList = GameManager.instance.playerFieldTransForm.GetComponentsInChildren<CardController>();
        }

        if (cemetery.Count == 0 || eCardList.Length <= 0)
        {
            return false;
        }

        double cemeteryDouble = cemetery.Count / 2;
        int damege = (int)Math.Ceiling(cemeteryDouble);

        foreach (var item in eCardList)
        {
            item.model.Damege(damege);
            item.view.RefreshStatus(item.model);
            item.CheckAlive();
        }

        return true;
    }

    /// <summary>
    /// 墓地の枚数分、HP回復
    /// </summary>
    public bool Effect_79(CardController use, CardController target, bool isPlayer)
    {
        // 墓地を参照
        List<int> cemetery;

        if (isPlayer)
        {
            cemetery = GameManager.instance.GetCemeteryListPlayer();
            GameManager.instance.player.playerHp += cemetery.Count;
        }
        else
        {
            cemetery = GameManager.instance.GetCemeteryListEnemy();
            GameManager.instance.enemy.playerHp += cemetery.Count;
        }

        UIManager.instance.showUserHp(GameManager.instance.player.playerHp, GameManager.instance.enemy.playerHp);
        return true;
    }

    /// <summary>
    /// お互いの墓地の枚数の合計が40枚以上の場合、特殊勝利
    /// </summary>
    public bool Effect_80(CardController use, CardController target, bool isPlayer)
    {
        // 墓地を参照
        int cemeteryCount = GameManager.instance.GetCemeteryListPlayer().Count + GameManager.instance.GetCemeteryListEnemy().Count;

        if (cemeteryCount < 40)
        {
            return false;
        }

        if (isPlayer)
        {
            GameManager.instance.enemy.playerHp -= GameManager.instance.enemy.playerHp;
        }
        else
        {
            GameManager.instance.player.playerHp -= GameManager.instance.player.playerHp;
        }
        GameManager.instance.checkPlayerHp();

        return true;
    }

    /// <summary>
    /// 自分の墓地の枚数が7枚の場合、相手の場と相手に7ダメージ
    /// </summary>
    public bool Effect_81(CardController use, CardController target, bool isPlayer)
    {
        List<int> cemetery;
        CardController[] cardList;

        if (isPlayer)
        {
            cemetery = GameManager.instance.GetCemeteryListPlayer();
            cardList = GameManager.instance.enemyFieldTransForm.GetComponentsInChildren<CardController>();
        }
        else
        {
            cemetery = GameManager.instance.GetCemeteryListEnemy();
            cardList = GameManager.instance.playerFieldTransForm.GetComponentsInChildren<CardController>();
        }

        if (cemetery.Count != 7)
        {
            return false;
        }

        foreach (var item in cardList)
        {
            item.model.Damege(7);
            item.view.RefreshStatus(item.model);
            item.CheckAlive();
        }

        if (isPlayer)
        {
            GameManager.instance.AttackToPlayer(7, true);
        }
        else
        {
            GameManager.instance.AttackToPlayer(7, false);
        }
        GameManager.instance.checkPlayerHp();

        UIManager.instance.showUserHp(GameManager.instance.player.playerHp, GameManager.instance.enemy.playerHp);
        return true;
    }


    /// <summary>
    /// 相手の場のカードを1枚破壊
    /// ネクロマンス3 破壊ではなく消滅
    /// </summary>
    public bool Effect_82(CardController use, CardController target, bool isPlayer)
    {
        List<int> cemetery = GameManager.instance.GetCemeteryListPlayer();
        if (!isPlayer)
        {
            cemetery = GameManager.instance.GetCemeteryListEnemy();
        }

        if (target == null || target.model.ability.isSkulk)
        {
            return false;
        }

        if (cemetery.Count >= 3)
        {
            GameManager.instance.RemoveRangeToCemetery(isPlayer, 0, 3);
            target.Destroy(true);
        }
        else
        {
            if (!target.model.ability.isIndestructible)
            {
                target.Destroy(false);
            }
        }
        return true;
    }

    /// <summary>
    /// 破壊時、ネクロマンス５を条件に再生を自身に付与
    /// </summary>
    public bool Effect_84(CardController use, CardController target, bool isPlayer)
    {
        List<int> cemetery = GameManager.instance.GetCemeteryListPlayer();
        if (!isPlayer)
        {
            cemetery = GameManager.instance.GetCemeteryListEnemy();
        }

        if (cemetery.Count >= 5)
        {
            GameManager.instance.RemoveRangeToCemetery(isPlayer, 0, 5);
            use.model.ability.isRegenerate = true;
            use.view.RefreshAbility(use.model);
        }
        return true;
    }

    /// <summary>
    /// 対象に「守護」「接死」「破壊不能」「貫通」「絆魂」「潜伏」「再生」のうち、どれか一つをランダムに付与する。
    /// 実際にはランダムではなく、ターン数と使用時のマナを参照し、付与効果を確定させる。
    /// </summary>
    public bool Effect_85(CardController use, CardController target, bool isPlayer)
    {
        if (target == null || target.model.ability.isSkulk)
        {
            return false;
        }

        int targetInt = GameManager.instance.turnCount + GameManager.instance.player.manaCost + GameManager.instance.enemy.manaCost;
        if (targetInt >= 7)
        {
            targetInt = targetInt % 7;
        }

        switch (targetInt)
        {
            case 0:
                target.model.ability.isShield = true;
                break;
            case 1:
                target.model.ability.isDeathTouch = true;
                break;
            case 2:
                target.model.ability.isIndestructible = true;
                break;
            case 3:
                target.model.ability.isLifeLink = true;
                break;
            case 4:
                target.model.ability.isPenetration = true;
                break;
            case 5:
                target.model.ability.isSkulk = true;
                break;
            case 6:
                target.model.ability.isRegenerate = true;
                break;
            default:
                break;
        }
        target.view.RefreshAbility(target.model);
        return true;
    }


    /// <summary>
    /// 自分の場のコストの合計が、相手の場のコストの合計より低い場合、接死を持つcardId4をプレイする。
    /// </summary>
    /// <param name="target"></param>
    public bool Effect_86(CardController use, CardController target, bool isPlayer)
    {
        CardController[] pCardList = GameManager.instance.playerFieldTransForm.GetComponentsInChildren<CardController>();
        CardController[] eCardList = GameManager.instance.enemyFieldTransForm.GetComponentsInChildren<CardController>();

        if (pCardList.Length + eCardList.Length <= 0)
        {
            return false;
        }

        int playerTotalCost = 0;
        foreach (var item in pCardList)
        {
            playerTotalCost += item.model.cost;
        }

        int enemyTotalCost = 0;
        foreach (var item in eCardList)
        {
            enemyTotalCost += item.model.cost;
        }

        if (isPlayer && playerTotalCost >= enemyTotalCost)
        {
            return false;
        }

        Transform field = GameManager.instance.playerFieldTransForm;
        if (!isPlayer)
        {
            field = GameManager.instance.enemyFieldTransForm;
        }

        CardController card = GameManager.instance.CrateCard(4, field, isPlayer, true);
        card.model.isFieldCard = true;
        card.model.ability.isDeathTouch = true;

        card.Show();
        card.view.RefreshAbility(card.model);

        return true;
    }

    /// <summary>
    /// </summary>
    /// <param name="target"></param>
    public bool Effect_87(CardController use, CardController target, bool isPlayer)
    {
        if (OnlineStatusManager.instance.IsOnlineBattle && !isPlayer)
        {
            // オンライン対戦で相手が使用した場合、相手ドローカードはドロー時に同期を取るので処理をしない
            return true;
        }

        Transform field = GameManager.instance.playerFieldTransForm;
        if (!isPlayer)
        {
            field = GameManager.instance.enemyFieldTransForm;
        }
        CardController[] cardList = field.GetComponentsInChildren<CardController>();

        if (cardList.Length <= 0)
        {
            return false;
        }

        int drowCount = 0;
        foreach (var item in cardList)
        {
            if (item.model.id != 5 &&
                item.model.id != 29 &&
                item.model.id != 46
                )
            {
                continue;
            }
            drowCount++;
        }

        if (drowCount <= 0)
        {
            return false;
        }

        GameManager.instance.GiveCardToHandToCount(true, drowCount);

        return true;
    }

    /// <summary>
    /// 破壊時：特殊敗北
    /// </summary>
    /// <param name="target"></param>
    public bool Effect_89(CardController use, CardController target, bool isPlayer)
    {
        if (isPlayer)
        {
            GameManager.instance.AttackToPlayer(GameManager.instance.player.playerHp, true);
        }
        else
        {
            GameManager.instance.AttackToPlayer(GameManager.instance.enemy.playerHp, false);
        }
        GameManager.instance.checkPlayerHp();

        return true;
    }

    /// <summary>
    /// 対象1枚を破壊しATK分お互いにダメージ
    /// </summary>
    /// <param name="target"></param>
    public bool Effect_90(CardController use, CardController target, bool isPlayer)
    {
        if (target == null || target.model.ability.isSkulk)
        {
            return false;
        }

        int atk = 0;
        if (!target.model.ability.isIndestructible)
        {
            atk = target.model.atk;
            target.Destroy(false);
        }

        GameManager.instance.AttackToPlayer(atk, false);
        GameManager.instance.checkPlayerHp();

        if (GameManager.instance.player.playerHp <= 0)
        {
            // 自分が死亡する場合、相手側へのダメージは実施せず自分のみ敗北度する。
            return true;
        }

        GameManager.instance.AttackToPlayer(atk, true);
        GameManager.instance.checkPlayerHp();

        return true;
    }

    /// <summary>
    /// シン・ゴニア
    /// 被破壊時:ATKを+7する。
    /// </summary>
    public bool Effect_91(CardController use, CardController target, bool isPlayer)
    {
        use.model.atk += 5;
        use.view.RefreshStatus(use.model);

        return true;
    }

    /// <summary>
    /// 自分の場のと名のついたカードに破壊不能を付与
    /// </summary>
    /// <param name="target"></param>
    public bool Effect_94(CardController use, CardController target, bool isPlayer)
    {
        CardController[] cardList;
        if (isPlayer)
        {
            cardList = GameManager.instance.playerFieldTransForm.GetComponentsInChildren<CardController>();
        }
        else
        {
            cardList = GameManager.instance.enemyFieldTransForm.GetComponentsInChildren<CardController>();
        }

        if (cardList.Length <= 0)
        {
            return true;
        }

        foreach (var item in cardList)
        {
            if (!item.model.cardName.Contains("ゴニア"))
            {
                continue;
            }
            item.model.ability.isIndestructible = true;
            item.view.RefreshAbility(item.model);
        }

        return true;
    }

    /// <summary>
    /// 5体で特殊勝利
    /// </summary>
    /// <param name="target"></param>
    public bool Effect_95(CardController use, CardController target, bool isPlayer)
    {
        Transform field = GameManager.instance.playerFieldTransForm;
        if (!isPlayer)
        {
            field = GameManager.instance.enemyFieldTransForm;
        }
        CardController[] cardList = field.GetComponentsInChildren<CardController>();

        if (cardList.Length <= 0)
        {
            return false;
        }

        // FIXME: 突貫で実装 あとで直す
        bool flg1 = false;
        bool flg2 = false;
        bool flg3 = false;
        bool flg4 = false;
        bool flg5 = false;

        foreach (var item in cardList)
        {
            switch (item.model.id)
            {
                case 20:
                    flg1 = true;
                    continue;
                case 37:
                    flg2 = true;
                    continue;
                case 91:
                    flg3 = true;
                    continue;
                case 92:
                    flg4 = true;
                    continue;
                case 94:
                    flg5 = true;
                    continue;
                default:
                    continue;
            }
        }

        if (!flg1 || !flg2 || !flg3 || !flg4 || !flg5)
        {
            return false;
        }

        // FIXME: あとで直すここまで いつか直せ

        if (isPlayer)
        {
            GameManager.instance.enemy.playerHp -= GameManager.instance.enemy.playerHp;
        }
        else
        {
            GameManager.instance.player.playerHp -= GameManager.instance.player.playerHp;
        }
        GameManager.instance.checkPlayerHp();

        return true;
    }

    /// <summary>
    /// が場に存在する時、対象のモンスターを消滅させる。
    /// </summary>
    /// <param name="target"></param>
    public bool Effect_96(CardController use, CardController target, bool isPlayer)
    {
        if (target == null || target.model.ability.isSkulk)
        {
            return false;
        }

        Transform field = GameManager.instance.playerFieldTransForm;
        if (!isPlayer)
        {
            field = GameManager.instance.enemyFieldTransForm;
        }
        CardController[] cardList = field.GetComponentsInChildren<CardController>();

        bool canEffect = false;

        foreach (var item in cardList)
        {
            if (item.model.id == 93)
            {
                canEffect = true;
                break;
            }
        }

        if (!canEffect)
        {
            return false;
        }

        target.Destroy(true);

        return true;
    }

    /// <summary>
    /// 対象に破壊不能と再生を付与する。
    /// </summary>
    /// <param name="target"></param>
    public bool Effect_97(CardController use, CardController target, bool isPlayer)
    {
        if (target == null || target.model.ability.isSkulk)
        {
            return false;
        }

        target.model.ability.isIndestructible = true;
        target.model.ability.isRegenerate = true;

        target.view.RefreshAbility(target.model);
        return true;
    }

    /// <summary>
    /// 使用時、自身の最大マナを0にする。マナ+20。この効果で得たマナは即時使用でき、このターンのみ使用可能
    /// </summary>
    /// <param name="target"></param>
    public bool Effect_98(CardController use, CardController target, bool isPlayer)
    {
        GamePlayerManager targetPlayerManager = GameManager.instance.player;
        if (!isPlayer)
        {
            targetPlayerManager = GameManager.instance.enemy;
        }

        targetPlayerManager.defaultManaCost = 0;
        targetPlayerManager.manaCost += 20;

        UIManager.instance.ShowManaCost(GameManager.instance.player.manaCost, GameManager.instance.enemy.manaCost);
        UIManager.instance.ShowDefaultManaCost(GameManager.instance.player.defaultManaCost, GameManager.instance.enemy.defaultManaCost);

        return true;
    }


    /// <summary>
    /// 破壊不能 被破壊時:自身の手札に
    /// </summary>
    /// <param name="target"></param>
    public bool Effect_99(CardController use, CardController target, bool isPlayer)
    {
        Transform hand = GameManager.instance.playerHandTransform;
        if (!isPlayer)
        {
            hand = GameManager.instance.enemyHandTransform;
        }

        GameManager.instance.CrateCard(99, hand, isPlayer, true);
        return true;
    }

    /// <summary>
    /// 対象の攻撃可能回数は100回となり、相手プレイヤーを攻撃できない
    /// </summary>
    /// <param name="target"></param>
    public bool Effect_100(CardController use, CardController target, bool isPlayer)
    {
        if (target == null || target.model.ability.isSkulk)
        {
            return false;
        }

        target.model.ability.isNotAttackPlayer = true;
        target.model.defaultCanAttackCount = 100;
        target.InitCanAttackCount(true);

        target.view.RefreshAbility(target.model);
        return true;
    }
}
