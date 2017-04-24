using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DG.Tweening;
using UnityEngine;

public class StartSequence : MonoBehaviour
{
    public AudioSource bgm;
    public AudioSource explosion;

    public MeshRenderer fadeScreen;

    void Start () {
        explosion.Play();
        bgm.DOFade(1.0f, 10.0f);
        fadeScreen.material.DOColor(new Color(0, 0, 0, 0), 10.0f);
    }
}
