using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject resultPanel;
    [SerializeField] GameObject leavePanel;

    [SerializeField] Text resultText;

    public Text playerHpText;
    public Text playerManaCostText;
    public Text playerDefaultManaCostText;
    public Text playerAmountDeckCountText;
    public Text playerCemeteryCountText;

    public Transform PlayerHpArea;

    public Text enemyHpText;
    public Text enemyManaCostText;
    public Text enemyDefaultManaCostText;
    public Text enemyAmountDeckCountText;
    public Text enemyCemeteryCountText;

    public Text timeCountText;
    public Text turnCountText;

    public Text haveTurnPlayer;

    public GameObject turnChangeViewPanel;

    // シングルトン
    public static UIManager instance { get; private set; }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void HideResultPanel()
    {
        resultPanel.SetActive(false);
    }

    /// <summary>
    /// マナコストの再描画を行う。
    /// </summary>
    public void ShowManaCost(int playerManaCost, int enemyManaCost)
    {
        playerManaCostText.text = playerManaCost.ToString();
        enemyManaCostText.text = enemyManaCost.ToString();
    }

    /// <summary>
    /// デフォルトマナコストの再描画を行う。
    /// </summary>
    public void ShowDefaultManaCost(int playerManaCost, int enemyManaCost)
    {
        playerDefaultManaCostText.text = playerManaCost.ToString();
        enemyDefaultManaCostText.text = enemyManaCost.ToString();
    }

    public void ShowDeckAmountCount(int playerAmountDeckCount, int enemyAmountDeckCount)
    {
        playerAmountDeckCountText.text = playerAmountDeckCount.ToString();
        enemyAmountDeckCountText.text = enemyAmountDeckCount.ToString();
    }

    /// <summary>
    /// 対戦者のHP再描画
    /// </summary>
    public void showUserHp(int playerHp, int enemyHp)
    {
        playerHpText.text = playerHp.ToString();
        enemyHpText.text = enemyHp.ToString();
    }

    public void UpdateTime(int timeCount)
    {
        timeCountText.text = timeCount.ToString();
    }

    public void UpdateTurnCount(int count) {
        turnCountText.text = "Turn " + count.ToString();
    }

    public void ShowResultPanel(int playerHp)
    {
        if (playerHp <= 0)
        {
            resultText.text = "You Lose";
        }
        else
        {
            resultText.text = "You Win";
        }
        resultPanel.SetActive(true);
    }

    public void ShowLibraryOutResult(bool isPlayer)
    {
        if (isPlayer)
        {
            resultText.text = "You Lose\nLibray Out";
        }
        else
        {
            resultText.text = "You Win\nLibray Out";
        }
        resultPanel.SetActive(true);
    }

    public void ShowLeavePanel()
    {
        leavePanel.SetActive(true);
    }

    public IEnumerator ShowHaveTrunPlayer(bool isPlayer)
    {
        Image image = turnChangeViewPanel.gameObject.GetComponent<Image>();
        if (isPlayer)
        {
            image.color = new Color(0, 0, 0.3f, 0.5f);
        }
        else
        {
            image.color = new Color(0.3f, 0, 0, 0.5f);
        }

        turnChangeViewPanel.gameObject.SetActive(true);
        RectTransform rectTransform = turnChangeViewPanel.gameObject.GetComponent<RectTransform>();
        rectTransform.DOSizeDelta(new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y + 200), 0.5f);
        DOVirtual.DelayedCall(1f, () =>
            {
                rectTransform.DOSizeDelta(new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y - 200), 0.5f);
            }
        );

        if (isPlayer)
        {
            haveTurnPlayer.text = "Your Trun";
            haveTurnPlayer.color = new Color(0, 0, 1, 1);
        }
        else
        {
            haveTurnPlayer.text = "Enemy Trun";
            haveTurnPlayer.color = new Color(1, 0, 0, 1);
        }

        yield return new WaitForSeconds(2);
        turnChangeViewPanel.gameObject.SetActive(false);
    }

    public void UpdateCemeteryCountText(int playerCemeteryCount, int enemyCemeteryCount)
    {
        playerCemeteryCountText.text = playerCemeteryCount.ToString();
        enemyCemeteryCountText.text = enemyCemeteryCount.ToString();
    }
}
