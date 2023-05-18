using System;
using System.Reflection;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// カード情報の操作指示
/// </summary>
public class CardController : MonoBehaviour
{
    public CardView view;
    public CardModel model;
    public CardMovement move;
    public MethodInfo effectMethod;

    // 効果適用のランダムに使用したcardIdを一時的に保持する変数
    public int randTempInt;
    public bool randTempTargetIsPlayer;

    /// <summary>
    ///  コントローラー起動時に子クラスを設定
    /// </summary>
    public void Awake()
    {
        view = GetComponent<CardView>();
        move = GetComponent<CardMovement>();
    }

    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="cardId">カードID</param>
    public void Init(int cardId, bool isPlayer, int cardPlayId = 0, bool isCard = true)
    {
        model = new CardModel(cardId, isPlayer, cardPlayId, isCard);
        view.SetCard(model);

        CardEffect effect = new CardEffect();
        // // カードIDに応じて動的に呼び出すメソッドを変更する。
        effectMethod = effect.GetType().GetMethod("Effect_" + this.model.id);
    }

    /// <summary>
    /// 攻撃処理
    /// </summary>
    /// <param name="enemyCard">攻撃対象のカード</param>
    public void Attack(CardController enemyCard)
    {
        model.Attack(enemyCard);
        ReduceCanAtack();
    }

    /// <summary>
    /// 攻撃可能回数を減らす。
    /// </summary>
    public void ReduceCanAtack()
    {
        this.model.canAttackCount--;

        if (this.model.canAttackCount > 0)
        {
            return;
        }

        setCanAttack(false);
    }

    /// <summary>
    /// 攻撃権を初期化する。
    /// </summary>
    /// <param name="canAttack"></param>
    public void InitCanAttackCount(bool canAttack)
    {
        if (canAttack)
        {
            this.model.InitCanAttackCount();
            this.setCanAttack(true);
        }
        else
        {
            this.setCanAttackCount(0);
        }
    }

    public void setCanAttackCount(int count)
    {
        this.model.canAttackCount = count;
        view.setActiveSelectablePanel(count > 0);
    }

    public void setCanAttack(bool canAttack)
    {
        // model.canAttack = canAttack;
        view.setActiveSelectablePanel(canAttack);
    }

    /// <summary>
    /// カードが破壊されてないならば、ステータスの再描画を、
    /// 破壊されているならば、削除を実施する。
    /// </summary>
    public void CheckAlive()
    {
        if (model.isAlive)
        {
            view.RefreshStatus(model);
        }
        else
        {
            if (this.model.ability.isRegenerate)
            {
                // 「再生」を所持している場合、再生を無効にしスタッツを元に戻す。
                this.ExecRegenerate();
            }
            else
            {
                this.Destroy(false);
            }
        }
    }

    public void ExecRegenerate()
    {
        // 再生の演出
        ParticleManager.instance.StartRegenerateParticle(this.transform);

        // 再生を設定せず、カードを初期化する。
        this.model.Init();
        this.model.ability.isRegenerate = false;
        this.view.RefreshStatus(this.model);
        this.view.SetCardAbility(this.model);

        // 破壊時効果を適用する。
        if (this.model.ability.isDestruction)
        {
            this.ExecEffect(this.model.isPlayerCard);
        }
    }

    /// <summary>
    /// カードをフィールドにドロップした際の処理
    /// </summary>
    /// <param name="isPlayer"></param>
    public void OnField(bool isPlayer)
    {
        // マナコストの支払いを実施
        GameManager.instance.ReduceManaCost(this.model.cost, isPlayer);
        this.model.isFieldCard = true;
        this.view.setActiveCardName(false);

        // TODO: 汎用化
        // "速攻"を所持の場合、攻撃権を付与
        this.InitCanAttackCount(this.model.ability.isInitAttacakble);

        // CIP所持の場合、効果を発動
        if (this.model.ability.isCip)
        {
            ExecEffect(isPlayer);
        }

        SoundManager.instance.PlaySeToTurnSummon();
        SoundManager.instance.CheckChangeBGMToCardId(this.model.id);

        // オンライン対戦時は、相手にカードIDを送信する。
        if (!isPlayer || !OnlineStatusManager.instance.IsOnlineBattle)
        {
            return;
        }
        GameManager.instance.SendPlayCard(this.model.cardPlayId);
    }

    public void ExecEffect(bool isPlayer)
    {
        effectMethod.Invoke(new CardEffect(), new object[] { this, null, isPlayer });
    }

    public void UseSpell(CardController target, bool isPlayer)
    {
        int manaCost = GameManager.instance.player.manaCost;
        if (!isPlayer)
        {
            manaCost = GameManager.instance.enemy.manaCost;
        }

        if (this.model.cost > manaCost)
        {
            // コストが支払えない場合は処理しない
            return;
        }

        bool isTargetPlayer = true;
        int targetCardPlayId = 0;
        if (target != null)
        {
            isTargetPlayer = target.model.isPlayerCard;
            targetCardPlayId = target.model.cardPlayId;
        }

        // 効果実行
        bool result = (bool)effectMethod.Invoke(new CardEffect(), new object[] { this, target, isPlayer });

        // 使用に失敗した場合は何もしない
        if (!result)
        {
            return;
        }

        if (this.randTempInt != 0)
        {
            // ランダムな効果処理をするスペルを用いた場合、対象となったカードIDを取得する。
            isTargetPlayer = this.randTempTargetIsPlayer;
            targetCardPlayId = this.randTempInt;
        }

        // マナコストの支払いを実施
        GameManager.instance.ReduceManaCost(this.model.cost, isPlayer);

        // 使用したスペルカードを表示
        GameManager.instance.ShowUseCard(this.model.id);

        // 自分のカードなら即座に破棄、相手のカードなら演出待ちのためディレイして破棄
        if (isPlayer)
        {
            this.Destroy(false);
        }
        else
        {
            DOVirtual.DelayedCall(0.2f, () => this.Destroy(false), false);
        }

        // 使用SE再生
        SoundManager.instance.PlaySeToSplellUse();

        // BGM再生判定
        SoundManager.instance.CheckChangeBGMToCardId(this.model.id);

        // オンライン対戦時は、相手にカードIDを送信する。
        if (!isPlayer || !OnlineStatusManager.instance.IsOnlineBattle)
        {
            return;
        }

        GameManager.instance.SendPlaySpellCard(this.model.cardPlayId, targetCardPlayId, isTargetPlayer);
    }

    private CardController getSelectableEnemyFieldCard(int cardPlayId)
    {
        CardController[] handCardList = GameManager.instance.enemyFieldTransForm.GetComponentsInChildren<CardController>();
        return Array.Find(handCardList, card => card.model.cardPlayId == cardPlayId);
    }

    public void Show()
    {
        this.view.Show();
    }

    public void Destroy(bool annihilation)
    {
        if (this.model.ability.isRegenerate && !annihilation)
        {
            // 「再生」を所持している場合、再生の処理を実施する。
            this.ExecRegenerate();
        }
        else if (annihilation)
        {
            // 消滅扱いなら、そのままオブジェクトを破壊
            MonoBehaviour.Destroy(this.gameObject);
        }
        else
        {
            // 消滅扱いでないなら、墓地にカードIDを設定
            GameManager.instance.SetCemetery(this);
            MonoBehaviour.Destroy(this.gameObject);
        }
    }

    public bool IsSpell()
    {
        return this.model.cardType == TYPE.SPELL;
    }
}
