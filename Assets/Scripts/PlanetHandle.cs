using DG.Tweening;
using UnityEngine;

public class PlanetHandle : MonoBehaviour
{
    public PlanetFace FirstFace;
    public PlanetFace SecondFace;
    public GameObject Planet;
    public GameObject HandlesFolder;

    public AudioSource barrier;

    private float _currentZAngle;
    private bool _isInTransition;
    private GameObject _gate;
    private PlanetFace _activeFace = null;
    private bool isGateActive = false;

    private void Start()
    {
        _isInTransition = false;
        RotateMidway();
        _gate = transform.GetChild(0).gameObject;
        _gate.SetActive(false);
    }

    private void Update()
    {
        if (_isInTransition)
        {
            Vector3 eulerAngles = transform.eulerAngles;
            eulerAngles.z = _currentZAngle;
            transform.eulerAngles = eulerAngles;
        }
    }

    public bool SwitchToFirstFace()
    {
        return SwitchToFace(FirstFace);
    }

    public bool SwitchToSecondFace()
    {
        return SwitchToFace(SecondFace);
    }

    public bool IsGateActive()
    {
        return isGateActive;
    }

    public void ActivateGate()
    {
        isGateActive = true;
        if (_gate.activeInHierarchy) return;
        _gate.transform.localPosition = new Vector3(0, -0.5f, 0);
        _gate.transform.DOLocalMoveY(0, 0.3f);
        _gate.SetActive(true);
    }

    public void DeactivateGate()
    {
        isGateActive = false;
        if (!_gate.activeInHierarchy) return;
        _gate.transform.DOLocalMoveY(-0.5f, 0.3f).
            OnComplete(()=> _gate.SetActive(false));
    }

    public void FlipGate()
    {
        if (IsGateActive())
        {
            DeactivateGate();
        }
        else
        {
            ActivateGate();
        }
    }

    private bool SwitchToFace(PlanetFace face)
    {
        isGateActive = true;
        if (Planet.transform.parent != null)
        {
            return false;
        }   
        transform.rotation = Quaternion.LookRotation(
            transform.forward, face.transform.up);
        transform.parent = null;
        Planet.transform.parent = transform;
        _currentZAngle = transform.eulerAngles.z;
        if (_currentZAngle > 180) _currentZAngle = _currentZAngle - 360;
        DOTween.To(() => _currentZAngle, (val) => _currentZAngle = val, 0, 0.3f).
            OnComplete(OnTransitionComplete);
        _isInTransition = true;
        _activeFace = face;
        return true;
    }

    private void RotateMidway()
    {
        transform.rotation = Quaternion.LookRotation(
            transform.forward,
            (FirstFace.transform.up + SecondFace.transform.up).normalized);
    }

    private void OnTransitionComplete()
    {
        Planet.transform.parent = null;
        transform.parent = HandlesFolder.transform;
        _isInTransition = false;
        RotateMidway();
        ActivateGate();
        _activeFace.GetNextHandle(this).FlipGate();
        _activeFace.GetPreviousHandle(this).FlipGate();
        barrier.Play();
    }
}
