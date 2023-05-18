using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    [SerializeField] DeckEditCardController cardPrehub;
    [SerializeField] Transform titleArea;

    [SerializeField] GameObject viewRuleBottun;

    [SerializeField] GameObject viewOptionBottun;
    [SerializeField] GameObject tempImg3;

    [SerializeField] GameObject patchNotesPanel;
    [SerializeField] GameObject patchNotesArrow;
    [SerializeField] GameObject rulePanel;

    [SerializeField] GameObject titleLogo;

    int clickCount;

    void Start()
    {
        DeckEditCardController entity = Instantiate(cardPrehub, titleArea, false);
        Vector3 worldAngle = entity.transform.eulerAngles;
        worldAngle.y = 90;
        entity.transform.eulerAngles = worldAngle;
        entity.view.Hide();

        DOVirtual.DelayedCall(0.5f, () => LoopLogoCard(entity));

        patchNotesArrow.transform
                .DOMove(new Vector3(-50f, 0f, 0f), 1f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetRelative(true);
    }

    void LoopLogoCard(DeckEditCardController entity)
    {
        entity.transform.DORotate(new Vector3(0, 360, 0), 6f, RotateMode.WorldAxisAdd)
            .SetEase(Ease.Linear)
            .OnStepComplete(() =>
            {
                // 裏から表に変わる瞬間まで遅延させカード表示
                DOVirtual.DelayedCall(
                    3f, () =>
                    {
                        entity.Init(Random.Range(1, 68), true, 0);
                    }
                );
                DOVirtual.DelayedCall(
                    6f, () =>
                    {
                        entity.view.Hide();
                    }
                );
            })
            .OnStart(() =>
                {
                    DOVirtual.DelayedCall(
                        3f, () =>
                        {
                            entity.Init(Random.Range(1, 60), true, 0);
                        }
                    );
                    DOVirtual.DelayedCall(
                        6f, () =>
                        {
                            entity.view.Hide();
                        }
                    );
                })
            .SetLoops(-1, LoopType.Incremental);
    }

    public void SwitchSceneToTitle()
    {
        SoundManager.instance.ChaneGameStatusToTilte();
        SceneManager.LoadScene("Title", LoadSceneMode.Single);
    }

    public void SwitchSceneToGame()
    {
        OnlineStatusManager.instance.IsOnlineBattle = false;
        SoundManager.instance.ChaneGameStatusToBattle();
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

    public void SwitchSceneToDeckEdit()
    {
        OnlineStatusManager.instance.IsOnlineBattle = false;
        SceneManager.LoadScene("Deck", LoadSceneMode.Single);
    }

    public void SwitchSceneToOnline()
    {
        OnlineStatusManager.instance.IsOnlineBattle = true;
        SceneManager.LoadScene("Online", LoadSceneMode.Single);
    }

    public void SwitchSceneToConfig()
    {
        OnlineStatusManager.instance.IsOnlineBattle = true;
        SceneManager.LoadScene("Config", LoadSceneMode.Single);
    }

    public void ShowRule()
    {
        rulePanel.GetComponent<RectTransform>().DOAnchorPos(Vector2.zero, 0.5f);

    }

    public void HideRule()
    {
        rulePanel.GetComponent<RectTransform>().DOAnchorPos(new Vector2(1920, 0), 0.5f);

    }

    public void ShowPatchNotes()
    {
        patchNotesPanel.GetComponent<RectTransform>().DOAnchorPos(Vector2.zero, 0.5f);

    }

    public void HidePatchNotes()
    {
        patchNotesPanel.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-1920, 0), 0.5f);
    }
}
