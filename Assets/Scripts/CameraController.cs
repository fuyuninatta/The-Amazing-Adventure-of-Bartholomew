using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;
    public Transform target;

    private float shakeDuration = 0.4f;
    private float shakeMagnitude = 0.4f;
    private float currentShakeTime = 0f;

    private void Awake()
    {
        Instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 basePosition = target.position;
        Quaternion baseRotation = target.rotation;

        if (currentShakeTime > 0)
        {
            currentShakeTime -= Time.deltaTime;

            //decay 
            float percent = currentShakeTime / shakeDuration;
            float currentMag = Mathf.Lerp(0f, shakeMagnitude, percent);

            //random offset
            float x = Random.Range(-1f, 1f) * currentMag;
            float y = Random.Range(-1f, 1f) * currentMag;
            Vector3 shakeOffset = new Vector3(x, y, 0);

            //random rotation offset
            float zRot = Random.Range(-0.5f, 0.5f) * currentMag;
            Quaternion shakeRot = Quaternion.Euler(0, 0, zRot);

            //transform + offset
            transform.position = basePosition + shakeOffset;
            transform.rotation = baseRotation * shakeRot;
        }
        else
        {
            //without shake
            transform.position = basePosition;
            transform.rotation = baseRotation;
        }
    }

    public void Shake()
    {
        currentShakeTime = shakeDuration;
    }
}
