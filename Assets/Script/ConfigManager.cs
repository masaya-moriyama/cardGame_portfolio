using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConfigManager : MonoBehaviour
{
    [SerializeField] GameObject DevModeButton;
    [SerializeField] GameObject DevModePanel;
    [SerializeField] TMP_InputField DevModeTextArea;
    [SerializeField] GameObject DevModeSubmit;

    public Slider slider;

    void Start()
    {
        if (OnlineStatusManager.instance.IsDevelopMode)
        {
            DevModeButton.GetComponent<Button>().interactable = false;
        }

        slider.value = SoundManager.instance.audioSourceBgm.volume;

        slider.onValueChanged.AddListener(
            value => SoundManager.instance.SetVolume(value)
        );
    }

    public void SwitchFullHD()
    {
        Screen.SetResolution(1920, 1080, false);
    }

    public void SwitchHD()
    {
        Screen.SetResolution(1280, 720, false);
    }

    public void SwitchWQHD()
    {
        Screen.SetResolution(2560, 1440, false);
    }

    public void SwitchFullScreenMode()
    {
        Screen.fullScreen = true;
    }

    public void SwitchWindowMode()
    {
        Screen.fullScreen = false;
    }

    public void ShowDevMode()
    {
        DevModePanel.GetComponent<RectTransform>().DOAnchorPos(Vector2.zero, 0.5f);

    }

    public void HideDevMode()
    {
        DevModePanel.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-1920, 0), 0.5f);
    }

    public void OnDevMode()
    {
        DevModeButton.GetComponent<Button>().interactable = false;
        OnlineStatusManager.instance.IsDevelopMode = true;

        DevModePanel.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-1920, 0), 0.5f);
    }

    public void InputText()
    {
        if (DevModeTextArea.text == "yonagi")
        {
            DevModeSubmit.GetComponent<Button>().interactable = true;
        }
        else
        {
            DevModeSubmit.GetComponent<Button>().interactable = false;
        }
    }

    public void SwitchSceneToTitle()
    {
        SceneManager.LoadScene("Title", LoadSceneMode.Single);
    }
}
