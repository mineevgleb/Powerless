using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProduceBtn : MonoBehaviour {

    public GameObject ActiveHint;
    public HeroController Player;
    public GameObject ActiveSkin;
    public GameObject NotActiveSkin;
    public EnergyText energyText;
    public ParticleSystem creationSmoke;
    public GameObject secondBot;
    public float ActivatableDistance = 0.1f;

    public bool IsActivated { get; private set; }

    private void Update()
    {
        bool isPlayerClose = IsPlayerClose();
        ActiveHint.SetActive(false);
        if (isPlayerClose && IsActivated)
        {
            ActiveHint.SetActive(true);
            if (Input.GetMouseButtonDown(0))
            {
                energyText.ShowFinalText();
                creationSmoke.Play();
                Player.FinalStep();
                Invoke("ShowSecondBot", 3.0f);
                IsActivated = false;
            }
        }
    }

    public void Activate()
    {
        IsActivated = true;
        ActiveSkin.SetActive(true);
        NotActiveSkin.SetActive(false);
    }

    public void Deactivate()
    {
        IsActivated = false;
        ActiveSkin.SetActive(false);
        NotActiveSkin.SetActive(true);
    }

    private bool IsPlayerClose()
    {
        return (Player.transform.position - transform.position).sqrMagnitude <
               ActivatableDistance * ActivatableDistance;
    }

    private void ShowSecondBot()
    {
        secondBot.SetActive(true);
    }
}
