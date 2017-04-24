using UnityEngine;

public class SineWavePosition : MonoBehaviour
{
    public float TimeScaler = 1.0f;
    public float PosScaler = 1.0f;
    private float _value = 0;

    private void Start()
    {
        _value = Random.value * Mathf.PI;
        Vector3 pos = transform.position;
        float currentDelta = Mathf.Sin(_value * TimeScaler) * PosScaler;
        pos.y = pos.y + currentDelta;
        transform.position = pos;
    }

    private void Update ()
    {
        float previousDelta = Mathf.Sin(_value * TimeScaler) * PosScaler;
        _value += Time.deltaTime;
        float currentDelta = Mathf.Sin(_value * TimeScaler) * PosScaler;
        Vector3 pos = transform.position;
        pos.y = pos.y + currentDelta - previousDelta;
        transform.position = pos;
    }
}
