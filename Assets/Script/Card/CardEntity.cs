using UnityEngine;

/// <summary>
/// カード情報の雛形
/// </summary>
[CreateAssetMenu(fileName = "CardEntity", menuName = "Create CardEntity")]
public class CardEntity : ScriptableObject
{
    public string cardName;
    public int hp;
    public int atk;
    public int cost;
    public Sprite icon;
    public string flavor;
    public TYPE cardType;

    public bool legendCard;

    // 攻撃可能回数
    public int canAttackCount = 1;

    // 付与アビリティ
    public bool initAttacakble;
    public bool shield;
    public bool cip;
    public bool destruction;
    public bool deathTouch;
    public bool indestructible;
    public bool penetration;
    public bool lifeLink;
    public bool skulk;
    public bool regenerate;
    public bool notAttackPlayer;
    public bool notAttackCard;
}

public enum TYPE
{
    MONSTER,
    SPELL,
}
