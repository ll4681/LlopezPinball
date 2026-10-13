using UnityEngine;

public class CameraSway : MonoBehaviour
{

    public float swayMagnitude = 0.15f;
    public float swayFrequency = 0.8f;

    private Vector3 startPosition;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        float sway = Mathf.Sin(Time.time * swayFrequency);

        transform.localPosition = startPosition + new Vector3(sway * swayMagnitude, 0f, 0f);

    }
}
