using UnityEngine;

public class OnlineStatusManager : MonoBehaviour
{
    // オンライン戦かどうか
    public bool IsOnlineBattle { get; set; }
    // オンラインのホストかどうか
    public bool IsOnlineHost { get; set; }
    // 開発者モードかどうか
    public bool IsDevelopMode = false;
    // 使用デッキNo
    public int useDeckNo = 1;

    // シングルトン化
    public static OnlineStatusManager instance { get; private set; }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
