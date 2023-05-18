using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility
{
    public bool isInitAttacakble = false;
    public bool isShield = false;
    public bool isCip = false;
    public bool isDestruction = false;
    public bool isDeathTouch = false;
    public bool isIndestructible = false;
    public bool isLifeLink = false;
    public bool isPenetration = false;
    public bool isSkulk = false;
    public bool isRegenerate = false;
    public bool isNotAttackPlayer = false;
    public bool isNotAttackCard = false;

    public CardAbility(CardEntity entity)
    {
        this.isInitAttacakble = entity.initAttacakble;
        this.isShield = entity.shield;
        this.isCip = entity.cip;
        this.isDestruction = entity.destruction;
        this.isDeathTouch = entity.deathTouch;
        this.isIndestructible = entity.indestructible;
        this.isPenetration = entity.penetration;
        this.isLifeLink = entity.lifeLink;
        this.isSkulk = entity.skulk;
        this.isRegenerate = entity.regenerate;
        this.isNotAttackPlayer = entity.notAttackPlayer;
        this.isNotAttackCard = entity.notAttackCard;
    }

    public void setIsInitAttacakble(bool flg)
    {
        this.isInitAttacakble = flg;
    }

    public void setIsShield(bool flg)
    {
        this.isShield = flg;
    }

    public void setIsDeathTouch(bool flg)
    {
        this.isDeathTouch = flg;
    }

    public void setIsIndestructible(bool flg)
    {
        this.isIndestructible = flg;
    }

    public void setIsPenetration(bool flg)
    {
        this.isPenetration = flg;
    }

    public void setIsLifeLink(bool flg)
    {
        this.isLifeLink = flg;
    }

    public void setIsSkulk(bool flg)
    {
        this.isSkulk = flg;
    }

    public void setIsRegenerate(bool flg)
    {
        this.isRegenerate = flg;
    }

    public void setAllAbilityFlg(bool flg)
    {
        this.setIsInitAttacakble(flg);
        this.setIsShield(flg);
        this.setIsDeathTouch(flg);
        this.setIsIndestructible(flg);
        this.setIsPenetration(flg);
        this.setIsLifeLink(flg);
        this.setIsSkulk(flg);
        this.setIsRegenerate(flg);
    }
}
