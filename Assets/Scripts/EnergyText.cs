using System.Collections.Generic;
using System.Text;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EnergyText : MonoBehaviour
{
    public List<EnergyBtn> Buttons;
    public Text energyIndication;
    public Text energyText;
    public Text confirmationText;
    public GameObject glowbot;
    public ProduceBtn produceBtn;

    public AudioSource coin;
    public AudioSource alarm;
    public AudioSource bgm;
    public AudioSource explosion;

    public GameObject fadeScreen;

    public GameObject logo;

    public List<Text> finalTexts;
    private float[] finalTextDelays = {10.0f, 3.0f, 2.0f,
        2.0f, 5.0f, 5.0f, 1.0f, 1.0f, 1.0f}; 
    private int finalTextIdx = 0;

    private void Awake ()
    {
        Buttons = new List<EnergyBtn>();
    }

    public void UpdateText()
    {
        var indicationText = new StringBuilder();
        int activeCount = 0;
        for (int i = 0; i < Buttons.Count; ++i)
        {
            if (Buttons[i].IsActivated)
            {
                indicationText.Append("1");
                ++activeCount;
            }
            else
            {
                indicationText.Append("0");
            }
            if (i == 5) indicationText.Append("\n");
        }
        energyIndication.text = indicationText.ToString();
        confirmationText.gameObject.SetActive(activeCount == 12);
        energyText.gameObject.SetActive(activeCount != 12);
        energyIndication.gameObject.SetActive(activeCount != 12);
        if (activeCount == 12)
        {
            produceBtn.Activate();
        }
        else
        {
            produceBtn.Deactivate();
        }
    }

    public void ShowFinalText()
    {
        if (finalTextIdx >= finalTexts.Count)
        {
            fadeScreen.SetActive(true);
            fadeScreen.GetComponent<MeshRenderer>().sharedMaterial.color 
                = Color.black;
            logo.SetActive(true);
            alarm.Stop();
            explosion.Play();
            bgm.DOFade(1.0f, 6.0f);
            Invoke("FinishGame", 10.0f);
            return;
        }
        if (finalTextIdx < 2) coin.Play();
        if (finalTextIdx == 2)
        {
            alarm.Play();
            bgm.DOFade(0, 6.0f);
        }
        confirmationText.gameObject.SetActive(false);
        if (finalTextIdx != 0) finalTexts[finalTextIdx - 1].gameObject.SetActive(false);
        if (finalTextIdx == 2) glowbot.SetActive(false);
        finalTexts[finalTextIdx].gameObject.SetActive(true);
        Invoke("ShowFinalText", finalTextDelays[finalTextIdx]);
        ++finalTextIdx;
    }

    public void FinishGame()
    {
        Application.Quit();
    }
}
