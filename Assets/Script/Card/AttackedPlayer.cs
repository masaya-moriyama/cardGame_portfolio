using System;
using UnityEngine;
using UnityEngine.EventSystems;


public class AttackedPlayer : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (!GameManager.instance.isPlayerTurn)
        {
            return;
        }

        // 攻撃するカードを選択
        CardController attacker = eventData.pointerDrag.GetComponent<CardController>();

        // 攻撃する・されるカードのデータが取得できなかった場合
        // または攻撃可能状態ではない場合、戦闘は実施しない。
        if (
            attacker == null ||
            attacker.model.ability.isNotAttackPlayer ||
            attacker.IsSpell() ||
            !attacker.model.IsCanAttack())
        {
            return;
        }

        CardController[] enemyFieldCards = GameManager.instance.GetEnemyFieldCards();

        // 敵フィールドにシールドが存在している場合
        if (Array.Exists(enemyFieldCards, card => card.model.ability.isShield))
        {
            // 攻撃カードが飛行では無いなら、攻撃できない。
            if (!attacker.model.ability.isPenetration)
            {
                return;
            }
        }

        // オンライン対戦時は、相手にカードIDを送信する。
        if (OnlineStatusManager.instance.IsOnlineBattle)
        {
            GameManager.instance.SendAtackPlayer(attacker.model.cardPlayId);
        }

        // 戦闘の実施
        // プレイヤーへの攻撃
        ParticleManager.instance.StartHitParticle(this.transform);
        GameManager.instance.AttackToPlayer(attacker, true);
        SoundManager.instance.PlaySeToAttack();
        GameManager.instance.checkPlayerHp();
    }
}
