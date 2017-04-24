using UnityEngine;

public class EnergyBtn : MonoBehaviour
{
    public GameObject ActiveHint;
    public GameObject NotActiveHint;
    public HeroController Player;
    public GameObject ActiveSkin;
    public GameObject NotActiveSkin;
    public GameObject Lightning;
    public EnergyText energyText;

    public AudioSource click;
    public AudioSource lightningAudio;
    public float ActivatableDistance = 0.1f;

    public bool IsActivated { get; private set; }

    private void Start()
    {
        energyText.Buttons.Add(this);
    }

    private void Update ()
    {
        bool isPlayerClose = IsPlayerClose();
        ActiveHint.SetActive(false);
        NotActiveHint.SetActive(false);
        if (isPlayerClose && !IsActivated)
        {
            NotActiveHint.SetActive(true);  
            if (Input.GetMouseButtonDown(0))
            {
                click.Play();
                Activate();
            }
        }
        else
        {
            if (isPlayerClose)
            {
                ActiveHint.SetActive(true);
                if (Input.GetMouseButtonDown(0))
                {
                    click.Play();
                    Deactivate();
                }
            }  
        }
    }

    public void Activate()
    {
        IsActivated = true;
        ActiveSkin.SetActive(true);
        NotActiveSkin.SetActive(false);
        energyText.UpdateText();
    }

    public void Deactivate()
    {
        SilentDeactivate();
        bool isFirst = true;
        foreach (PlanetHandle planetHandle in Player.CurrentFace.Handles)
        {
            if (planetHandle.IsGateActive() && isFirst)
            {
                isFirst = false;
                planetHandle.barrier.Play();
            }
            planetHandle.DeactivateGate();   
        }
        foreach (EnergyBtn energyBtn in energyText.Buttons)
        {
            if (Random.value <= 0.7f)
            {
                energyBtn.SilentDeactivate();
            }
        }
        Lightning.SetActive(true);
        lightningAudio.Play();
        Invoke("HideLightning", 1.0f);
    }

    public void SilentDeactivate()
    {
        IsActivated = false;
        ActiveSkin.SetActive(false);
        NotActiveSkin.SetActive(true);
        energyText.UpdateText();
    }

    private void HideLightning()
    {
        Lightning.SetActive(false);
    }

    private bool IsPlayerClose()
    {
        return (Player.transform.position - transform.position).sqrMagnitude <
               ActivatableDistance * ActivatableDistance;
    }
}
