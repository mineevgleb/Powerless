using DG.Tweening;
using UnityEngine;

public class HeroController : MonoBehaviour
{
    public PlanetFace CurrentFace;
    public GameObject Hint;
    public GameObject FinalLookPoint;

    private bool _isActive = true;

    private void Update ()
    {
        if (!_isActive) return;
        if (Input.GetMouseButtonDown(1))
            Hint.SetActive(false);
        Vector3 moveDir = GetMovementDirection();
        UpdatePosition(moveDir);
        Vector3 cross = Vector3.Cross(transform.forward, moveDir);
        var rot = transform.eulerAngles;
        rot.y += Time.deltaTime * 100 * cross.y;
        transform.eulerAngles = rot;
        UpdatePlanetFace();
    }

    public void FinalStep()
    {
        _isActive = false;
        transform.DORotate(FinalLookPoint.transform.eulerAngles, 1.0f);
    }

    private void UpdatePosition(Vector3 dir)
    {
        Vector3 pos = transform.position;
        pos += dir * Time.deltaTime * 0.3f;
        if (!CurrentFace.IsPositionOnFace(pos) &&
            CurrentFace.GetClosestHandle(pos).IsGateActive())
            return;
        transform.position = pos;
    }

    private void UpdatePlanetFace()
    {
        if (!CurrentFace.IsPositionOnFace(transform.position))
        {
            PlanetHandle handle =
                CurrentFace.GetClosestHandle(transform.position);
            if (handle.FirstFace != CurrentFace && handle.SwitchToFirstFace())
            {
                CurrentFace = handle.FirstFace;
            }
            if (handle.SecondFace != CurrentFace && handle.SwitchToSecondFace())
            {
                CurrentFace = handle.SecondFace;
            }
        }
    }

    private Vector3 GetMovementDirection()
    {
        if(!Input.GetMouseButton(1)) return Vector3.zero;
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (mouseRay.direction.y > 0) return Vector3.zero;
        Vector3 rayOrigin = mouseRay.origin;
        Vector3 rayDirection = mouseRay.direction;
        float heroPosY = transform.position.y;
        float deltaY = heroPosY - mouseRay.origin.y;
        rayDirection /= mouseRay.direction.y;
        rayDirection *= deltaY;
        Vector3 target = rayOrigin + rayDirection;
        Vector3 result = target - transform.position;
        result.y = 0;
        return result.normalized;
    }
}
