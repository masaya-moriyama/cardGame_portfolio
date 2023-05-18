using UnityEngine;

/// <summary>
/// カード情報のシステム操作
/// </summary>
public class CardModel
{
    public int id;
    public string cardName;
    public int hp;
    public int atk;
    public int cost;
    public Sprite icon;
    public string flavor;
    public TYPE cardType;

    public bool legendCard;

    public bool isAlive;
    public bool isFieldCard;
    public bool isPlayerCard;

    public int canAttackCount;
    public int defaultCanAttackCount;

    public int cardPlayId;

    public bool isToken;

    // アビリティ
    public CardAbility ability;

    public CardModel(int cardId, bool isPlayer, int cardPlayId, bool isCard = true)
    {
        string entityPath = "";
        if (isCard)
        {
            entityPath = "CardEntityList/Card_" + cardId;
        }
        else
        {
            entityPath = "CardEntityList/Token_" + cardId;
        }

        CardEntity entity = Resources.Load<CardEntity>(entityPath);

        this.id = cardId;
        this.cardName = entity.cardName;
        this.hp = entity.hp;
        this.atk = entity.atk;
        this.cost = entity.cost;
        this.icon = entity.icon;
        this.flavor = entity.flavor;
        this.cardType = entity.cardType;

        this.legendCard = entity.legendCard;

        this.cardPlayId = cardPlayId;

        this.isAlive = true;
        this.isPlayerCard = isPlayer;

        this.ability = new CardAbility(entity);

        this.defaultCanAttackCount = entity.canAttackCount;
        
        this.isToken = !isCard;
    }

    public void Init()
    {
        int cardId = this.id;
        string entityPath = "";
        if (!isToken)
        {
            entityPath = "CardEntityList/Card_" + cardId;
        }
        else
        {
            entityPath = "CardEntityList/Token_" + cardId;
        }

        CardEntity entity = Resources.Load<CardEntity>(entityPath);

        this.hp = entity.hp;
        this.atk = entity.atk;
        this.cost = entity.cost;

        this.isAlive = true;

        this.ability = new CardAbility(entity);

        this.defaultCanAttackCount = entity.canAttackCount;
    }

    public void Damege(int damege)
    {
        hp -= damege;

        if (hp <= 0)
        {
            hp = 0;
            isAlive = false;
        }
    }

    public void Attack(CardController target)
    {
        if (this.ability.isDeathTouch && !target.model.ability.isIndestructible)
        {
            target.model.Damege(target.model.hp);
        }
        else
        {
            target.model.Damege(atk);
        }
    }

    public void InitCanAttackCount()
    {
        canAttackCount = defaultCanAttackCount;
    }

    public bool IsCanAttack()
    {
        if (this.ability.isNotAttackCard && this.ability.isNotAttackPlayer)
        {
            // カードもプレイヤーも殴れない場合は攻撃不可とする
            return false;
        }

        return canAttackCount > 0;
    }

    public bool IsMonster()
    {
        return this.cardType == TYPE.MONSTER;
    }

    public bool IsSpell()
    {
        return this.cardType == TYPE.SPELL;
    }

}
