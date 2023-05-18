using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 攻撃される側の処理
/// </summary>
public class AttackedCard : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (!GameManager.instance.isPlayerTurn)
        {
            return;
        }

        // フィールドのカードを取得
        // 攻撃するカードを選択
        CardController attacker = eventData.pointerDrag.GetComponent<CardController>();

        if (attacker.IsSpell())
        {
            return;
        }

        // 攻撃されるカードを選択
        CardController defender = GetComponent<CardController>();

        if (
            attacker == null ||
            defender == null ||
            attacker.model.ability.isNotAttackCard || // 「カードを攻撃できない」を所持
            !attacker.model.IsCanAttack() || // 攻撃権が無い
            !defender.model.isFieldCard || // 対象がフィールドのカードではない
            defender.model.ability.isSkulk || // 対象が「潜伏」を所持
            !attacker.model.isPlayerCard || // 攻撃カードが自分のカードでは無い
            attacker.model.isPlayerCard == defender.model.isPlayerCard // 攻撃と防御のカードが同一
        )
        {
            return;
        }

        CardController[] enemyFieldCards = GameManager.instance.GetEnemyFieldCards();

        // 敵フィールドにシールドが存在している場合、シールド以外なら実施しない。
        if (Array.Exists(enemyFieldCards, card => card.model.ability.isShield) &&
            !defender.model.ability.isShield
        )
        {
            // 攻撃カードが飛行では無いなら、攻撃できない。
            if (!attacker.model.ability.isPenetration)
            {
                return;
            }
            return;
        }

        // オンライン対戦時は、相手にカードIDを送信する。
        if (OnlineStatusManager.instance.IsOnlineBattle)
        {
            GameManager.instance.SendAtackCard(attacker.model.cardPlayId, defender.model.cardPlayId);
        }

        // 戦闘の実施
        GameManager.instance.CardsBattle(attacker, defender);
    }

}
