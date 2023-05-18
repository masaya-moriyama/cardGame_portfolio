using System;
using System.Collections;
using UnityEngine;

public class CPU : MonoBehaviour
{
    GameManager gameManager;
    UIManager uiManager;

    void Start()
    {
        gameManager = GameManager.instance;
        uiManager = UIManager.instance;
    }

    /// <summary>
    /// エネミーターン処理
    /// </summary>
    public IEnumerator EnemyTurn()
    {
        Debug.Log("相手ターン開始");
        if (gameManager.turnCount != 2)
        {
            gameManager.GiveCardToHandToCount(false, 1);
        }
        else
        {
            gameManager.GiveCardToHandToCount(false, 2);
        }

        // 攻撃権付与
        CardController[] cardList = gameManager.enemyFieldTransForm.GetComponentsInChildren<CardController>();
        gameManager.SettingCanAtttack(cardList, gameManager.enemyFieldTransForm, true);

        // 2秒待機
        yield return new WaitForSeconds(2);

        /* 手札処理 */
        // 手札のカードリストを取得
        CardController[] handCardList = gameManager.enemyHandTransform.GetComponentsInChildren<CardController>();

        // プレイできないカードがあるなら、その中からランダムでコストにする。
        if (Array.Exists(handCardList, card => card.model.cost > gameManager.enemy.manaCost))
        {
            // コスト支払い不可能のカードリストを取得
            CardController[] canNotPlayHandCardList = Array.FindAll(handCardList, card => card.model.cost > gameManager.enemy.manaCost);

            int randomInt = UnityEngine.Random.Range(0, canNotPlayHandCardList.Length);
            CardController costCard = canNotPlayHandCardList[randomInt];

            gameManager.enemy.defaultManaCost++;
            gameManager.enemy.manaCost = gameManager.enemy.defaultManaCost;
            uiManager.ShowManaCost(gameManager.player.manaCost, gameManager.enemy.manaCost);
            uiManager.ShowDefaultManaCost(gameManager.player.defaultManaCost, gameManager.enemy.defaultManaCost);

            StartCoroutine(costCard.move.MoveToManaCost(gameManager.enemyManaCostTransForm));
            // SE再生
            SoundManager.instance.PlaySeToAddMana();
            yield return new WaitForSeconds(1);
        }

        // マナコスト増加後の手札を再取得
        handCardList = gameManager.enemyHandTransform.GetComponentsInChildren<CardController>();

        // コスト以下のカードを出し続ける
        while (Array.Exists(handCardList, card => (card.model.cost <= gameManager.enemy.manaCost) && (card.model.IsMonster())))
        {
            // コスト支払い可能のカードリストを取得
            CardController[] selectableHandCardList = Array.FindAll(handCardList, card => (card.model.cost <= gameManager.enemy.manaCost) && (card.model.IsMonster()));

            // 場に出すカードを選択
            CardController enemyCard = selectableHandCardList[0];
            // カードを移動
            StartCoroutine(enemyCard.move.MoveToField(gameManager.enemyFieldTransForm));

            enemyCard.OnField(false);
            enemyCard.Show();

            // 更新
            handCardList = gameManager.enemyHandTransform.GetComponentsInChildren<CardController>();
            yield return new WaitForSeconds(1);
        }

        /* 攻撃処理 */
        // フィールドのカードを取得
        CardController[] enemyFieldCardList = gameManager.enemyFieldTransForm.GetComponentsInChildren<CardController>();

        while (Array.Exists(enemyFieldCardList, card => card.model.IsCanAttack() && card.model.atk > 0))
        {
            // 攻撃可能カードを取得
            CardController[] enemyFieldCanAttackCardList = Array.FindAll(enemyFieldCardList, card => card.model.IsCanAttack() && card.model.atk > 0);

            // 攻撃するカードを選択
            CardController attacker = enemyFieldCanAttackCardList[0];
            // 攻撃されるカードを選択
            CardController[] playerFieldCardList = gameManager.playerFieldTransForm.GetComponentsInChildren<CardController>();

            if (playerFieldCardList.Length > 0)
            {
                // カードが存在しているなら、攻撃対象はカードへ
                CardController defender = playerFieldCardList[0];
                StartCoroutine(attacker.move.MoveToTarget(defender.transform));

                yield return new WaitForSeconds(0.51F);

                // 戦闘の実施
                gameManager.CardsBattle(attacker, defender);
            }
            else
            {
                StartCoroutine(attacker.move.MoveToTarget(uiManager.PlayerHpArea.transform));

                // カードが存在していないなら、攻撃対象はプレイヤーへ
                yield return new WaitForSeconds(0.25F);
                gameManager.AttackToPlayer(attacker, false);
                SoundManager.instance.PlaySeToAttack();
                yield return new WaitForSeconds(0.25F);

                gameManager.checkPlayerHp();
            }

            enemyFieldCardList = gameManager.enemyFieldTransForm.GetComponentsInChildren<CardController>();
            yield return new WaitForSeconds(1);
        }

        gameManager.ChangeTrun();
    }
}
