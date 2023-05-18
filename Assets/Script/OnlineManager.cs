using System.IO;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OnlineManager : MonoBehaviourPunCallbacks
{
    // ボタンを押したらマッチング開始
    // ランダムマッチを実施
    // なければ生成
    // 対戦が成立すればシーン遷移

    bool inRoom;
    bool isMatching;

    [SerializeField] GameObject matchingBottun;
    [SerializeField] GameObject matchingText;

    [SerializeField] GameObject backBottun;

    [SerializeField] GameObject deckSelect1;
    [SerializeField] GameObject deckSelect2;
    [SerializeField] GameObject deckSelect3;

    private void Start()
    {
        OnlineStatusManager.instance.useDeckNo = 1;
        deckSelect1.GetComponent<Button>().interactable = false;
    }

    public void OnStartButton()
    {
        // デッキチェック
        DeckEditManager deckEditManager = new DeckEditManager();
        DeckListEntity entity = deckEditManager.LoadDeck("deck");
        if (!deckEditManager.ExecDeckCheck(entity.GetDeckList(OnlineStatusManager.instance.useDeckNo))) {
            matchingText.GetComponent<Text>().text = "デッキにエラーがあります。";
            matchingText.SetActive(true);
            return;
        }

        deckSelect1.GetComponent<Button>().interactable = false;
        deckSelect2.GetComponent<Button>().interactable = false;
        deckSelect3.GetComponent<Button>().interactable = false;

        // PhotonServerSettingsの設定内容を使ってマスターサーバーへ接続する
        PhotonNetwork.ConnectUsingSettings();
        matchingBottun.GetComponent<Button>().interactable = false;
    }

    public void OnDeckSelect1()
    {
        OnlineStatusManager.instance.useDeckNo = 1;
        deckSelect1.GetComponent<Button>().interactable = false;
        deckSelect2.GetComponent<Button>().interactable = true;
        deckSelect3.GetComponent<Button>().interactable = true;
    }

    public void OnDeckSelect2()
    {
        OnlineStatusManager.instance.useDeckNo = 2;
        deckSelect1.GetComponent<Button>().interactable = true;
        deckSelect2.GetComponent<Button>().interactable = false;
        deckSelect3.GetComponent<Button>().interactable = true;
    }

    public void OnDeckSelect3()
    {
        OnlineStatusManager.instance.useDeckNo = 3;
        deckSelect1.GetComponent<Button>().interactable = true;
        deckSelect2.GetComponent<Button>().interactable = true;
        deckSelect3.GetComponent<Button>().interactable = false;
    }

    // マスターサーバーへの接続が成功した時に呼ばれるコールバック
    public override void OnConnectedToMaster()
    {
        // "Room"という名前のルームに参加する（ルームが存在しなければ作成して参加する）
        PhotonNetwork.JoinRandomRoom();
        matchingText.SetActive(true);
    }

    // ゲームサーバーへの接続が成功した時に呼ばれるコールバック
    public override void OnJoinedRoom()
    {
        inRoom = true;
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        // ルームの参加人数を2人に設定する
        var roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;

        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    private void Update()
    {
        if (isMatching)
        {
            return;
        }

        // 最大人数の場合、シーン移動する。
        if (inRoom &&
            PhotonNetwork.CurrentRoom.MaxPlayers == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            OnlineStatusManager.instance.IsOnlineHost = PhotonNetwork.LocalPlayer.IsMasterClient;
            Debug.Log(PhotonNetwork.LocalPlayer.UserId + " :Master?: " + PhotonNetwork.LocalPlayer.IsMasterClient);
            isMatching = true;
            matchingText.SetActive(false);
            SoundManager.instance.ChaneGameStatusToBattle();
            SceneManager.LoadScene("Game");
        }
    }

    public void SwitchSceneToTitle()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();
        }
        isMatching = false;
        SceneManager.LoadScene("Title", LoadSceneMode.Single);
    }
}
