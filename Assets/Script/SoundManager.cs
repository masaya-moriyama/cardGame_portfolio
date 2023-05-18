using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip title;
    public AudioClip battle;
    public AudioClip card_16;
    public AudioClip card_23;
    public AudioClip card_49;
    public AudioClip card_50;
    public AudioClip card_51;
    public AudioClip card_57;
    public AudioClip card_59;

    public AudioClip bgmBattleWin;
    public AudioClip bgmBattleLose;

    public AudioClip seBattleStart;
    public AudioClip seTurnChange;
    public AudioClip seCardSummon;
    public AudioClip seCardAttack;
    public AudioClip seCardSplellUse;
    public AudioClip seAddMana;

    public AudioSource audioSourceBgm;
    public AudioSource audioSourceSe;

    GAME_STATUS gameStatus;

    enum GAME_STATUS
    {
        TITLE,
        BATTLE,
        RESULT
    }

    public static SoundManager instance { get; private set; }

    void Start()
    {
        if (instance == null)
        {
            instance = this;
            audioSourceBgm = GetComponent<AudioSource>();
            ChaneGameStatusToTilte();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (!audioSourceBgm.isPlaying)
        {
            if (gameStatus == GAME_STATUS.TITLE)
            {
                ChangeMusicToTitle();
            }
            else if (gameStatus == GAME_STATUS.BATTLE)
            {
                ChangeMusicToBattle();
            }
        }
    }

    public void ChaneGameStatusToTilte()
    {
        gameStatus = GAME_STATUS.TITLE;
        ChangeMusicToTitle();
    }

    public void ChaneGameStatusToBattle()
    {
        gameStatus = GAME_STATUS.BATTLE;
        ChangeMusicToBattle();
    }

    public void ChaneGameStatusToResult()
    {
        gameStatus = GAME_STATUS.RESULT;
    }

    public void ChangeMusicToTitle()
    {
        this.audioSourceBgm.clip = title;
        this.audioSourceBgm.loop = true;
        audioSourceBgm.Play();
    }

    public void ChangeMusicToBattle()
    {
        this.audioSourceBgm.clip = battle;
        this.audioSourceBgm.loop = true;
        audioSourceBgm.Play();
    }

    public void ChangeMusicToCard16()
    {
        this.audioSourceBgm.clip = card_16;
        this.audioSourceBgm.loop = false;
        audioSourceBgm.Play();
    }

    public void ChangeMusicToCard23()
    {
        this.audioSourceBgm.clip = card_23;
        this.audioSourceBgm.loop = false;
        audioSourceBgm.Play();
    }

    public void ChangeMusicToCard49()
    {
        this.audioSourceBgm.clip = card_49;
        this.audioSourceBgm.loop = false;
        audioSourceBgm.Play();
    }

    public void ChangeMusicToCard50()
    {
        this.audioSourceBgm.clip = card_50;
        this.audioSourceBgm.loop = false;
        audioSourceBgm.Play();
    }

    public void ChangeMusicToCard51()
    {
        this.audioSourceBgm.clip = card_51;
        this.audioSourceBgm.loop = false;
        audioSourceBgm.Play();
    }

    public void ChangeMusicToCard57()
    {
        this.audioSourceBgm.clip = card_57;
        this.audioSourceBgm.loop = false;
        audioSourceBgm.Play();
    }

    public void ChangeMusicToCard59()
    {
        this.audioSourceBgm.clip = card_59;
        this.audioSourceBgm.loop = false;
        audioSourceBgm.Play();
    }

    public void CheckChangeBGMToCardId(int cardId)
    {
        switch (cardId)
        {
            case 16:
                ChangeMusicToCard16();
                break;
            case 23:
                ChangeMusicToCard23();
                break;
            case 49:
                ChangeMusicToCard49();
                break;
            case 50:
                ChangeMusicToCard50();
                break;
            case 51:
                ChangeMusicToCard51();
                break;
            case 57:
                ChangeMusicToCard57();
                break;
            case 59:
                ChangeMusicToCard59();
                break;
            default:
                break;
        }
    }

    public void ChangeMusicToBattleWin()
    {
        this.audioSourceBgm.clip = bgmBattleWin;
        this.audioSourceBgm.loop = false;
        audioSourceBgm.Play();
    }

    public void ChangeMusicToBattleLose()
    {
        this.audioSourceBgm.clip = bgmBattleLose;
        this.audioSourceBgm.loop = false;
        audioSourceBgm.Play();
    }

    public void ChangeMusicToBattleEnd(bool isWin)
    {
        ChaneGameStatusToResult();
        if (isWin)
        {
            ChangeMusicToBattleWin();
        }
        else
        {
            ChangeMusicToBattleLose();
        }
    }

    public void PlaySeToBattleStart()
    {
        this.audioSourceSe.clip = seBattleStart;
        audioSourceSe.Play();
    }

    public void PlaySeToTurnChange()
    {
        this.audioSourceSe.clip = seTurnChange;
        audioSourceSe.Play();
    }

    public void PlaySeToTurnSummon()
    {
        this.audioSourceSe.clip = seCardSummon;
        audioSourceSe.Play();
    }

    public void PlaySeToAttack()
    {
        this.audioSourceSe.clip = seCardAttack;
        audioSourceSe.Play();
    }

    public void PlaySeToSplellUse()
    {
        this.audioSourceSe.clip = seCardSplellUse;
        audioSourceSe.Play();
    }

    public void PlaySeToAddMana()
    {
        this.audioSourceSe.clip = seAddMana;
        audioSourceSe.Play();
    }

    public void SetVolume(float value)
    {
        SoundManager.instance.audioSourceBgm.volume = value;
        SoundManager.instance.audioSourceSe.volume = value;
    }
}
