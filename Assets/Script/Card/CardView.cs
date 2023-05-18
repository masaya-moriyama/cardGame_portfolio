using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// カード情報の表示操作
/// </summary>
public class CardView : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI cardName;
    [SerializeField] TextMeshProUGUI hpText;
    [SerializeField] TextMeshProUGUI atkText;
    [SerializeField] TextMeshProUGUI costText;

    [SerializeField] Image hpArea;
    [SerializeField] Image atkArea;
    [SerializeField] Image costArea;

    [SerializeField] GameObject legendIcon;

    [SerializeField] TextMeshProUGUI flavor;
    [SerializeField] Image iconImage;
    [SerializeField] GameObject selectablePanel;
    [SerializeField] GameObject maskPanel;

    [SerializeField] GameObject shieldPanel;
    [SerializeField] GameObject indestructible;
    [SerializeField] GameObject deathTouch;
    [SerializeField] GameObject initAttacakble;
    [SerializeField] GameObject penetration;
    [SerializeField] GameObject lifeLink;
    [SerializeField] GameObject skulk;
    [SerializeField] GameObject regenerate;

    public void SetCard(CardModel model)
    {
        costText.text = model.cost.ToString();
        flavor.text = model.flavor;
        iconImage.sprite = model.icon;

        // 相手カードの場合はマスクする。
        if (OnlineStatusManager.instance != null && !OnlineStatusManager.instance.IsDevelopMode)
        {
            maskPanel.SetActive(!model.isPlayerCard);
            legendIcon.gameObject.SetActive(model.isPlayerCard);
            hpArea.gameObject.SetActive(model.isPlayerCard);
            atkArea.gameObject.SetActive(model.isPlayerCard);
            costArea.gameObject.SetActive(model.isPlayerCard);
            cardName.gameObject.SetActive(model.isPlayerCard);
            flavor.gameObject.SetActive(model.isPlayerCard);
        }
        else
        {
            maskPanel.SetActive(false);
        }

        cardName.text = model.cardName.ToString();
        legendIcon.gameObject.SetActive(model.legendCard);

        if (model.cardType == TYPE.MONSTER)
        {
            hpText.text = model.hp.ToString();
            atkText.text = model.atk.ToString();
        }
        else if (model.cardType == TYPE.SPELL)
        {
            hpArea.gameObject.SetActive(false);
            atkArea.gameObject.SetActive(false);
        }

        SetCardAbility(model);
    }

    public void SetCardAbility(CardModel model)
    {
        shieldPanel.SetActive(false);
        indestructible.SetActive(false);
        deathTouch.SetActive(false);
        initAttacakble.SetActive(false);
        penetration.SetActive(false);
        lifeLink.SetActive(false);
        skulk.SetActive(false);
        regenerate.SetActive(false);

        this.RefreshAbility(model);
    }

    public void Show()
    {
        maskPanel.SetActive(false);
        hpArea.gameObject.SetActive(true);
        atkArea.gameObject.SetActive(true);
        costArea.gameObject.SetActive(true);
        flavor.gameObject.SetActive(true);
        cardName.gameObject.SetActive(false);
    }

    public void Hide()
    {
        maskPanel.SetActive(true);
        hpArea.gameObject.SetActive(false);
        atkArea.gameObject.SetActive(false);
        costArea.gameObject.SetActive(false);
        flavor.gameObject.SetActive(false);
        cardName.gameObject.SetActive(false);
    }

    public void RefreshStatus(CardModel model)
    {
        hpText.text = model.hp.ToString();
        atkText.text = model.atk.ToString();
        costText.text = model.cost.ToString();
    }

    public void RefreshAbility(CardModel model)
    {
        // 守護
        if (model.ability.isShield)
        {
            shieldPanel.SetActive(true);
        }
        // 破壊不能
        if (model.ability.isIndestructible)
        {
            indestructible.SetActive(true);
        }
        // 接死
        if (model.ability.isDeathTouch)
        {
            deathTouch.SetActive(true);
        }
        // 速攻
        if (model.ability.isInitAttacakble)
        {
            initAttacakble.SetActive(true);
        }
        // 貫通
        if (model.ability.isPenetration)
        {
            penetration.SetActive(true);
        }
        // 絆魂
        if (model.ability.isLifeLink)
        {
            lifeLink.SetActive(true);
        }
        // 潜伏
        if (model.ability.isSkulk)
        {
            skulk.SetActive(true);
        }
        // 再生
        if (model.ability.isRegenerate)
        {
            regenerate.SetActive(true);
        }
    }

    public void setActiveSelectablePanel(bool flg)
    {
        selectablePanel.SetActive(flg);
    }

    public void setActiveCardName(bool flg)
    {
        cardName.gameObject.SetActive(flg);
    }
}
