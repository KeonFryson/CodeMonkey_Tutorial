using UnityEngine;

public class Stick_CircleAnimation : MonoBehaviour
{
    [SerializeField] private Cauldron cauldron;
    [SerializeField] private Transform centerPoint;
    [SerializeField] private float baseSpeed = 2f;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float speedIncrement = 2f;
    [SerializeField] private float radius = 3f;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private float angle = 0f;
    private float currentSpeed = 0f;
    private bool isAnimating = false;
    private void Start()
    {
        cauldron.OnCauldronCicked += StartCircleAnimation;
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    private void Update()
    {
        if (!isAnimating) return;

        angle += currentSpeed * Time.deltaTime;

        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;
        transform.position = centerPoint.position + new Vector3(x, 0, z);

        // Complete one full circle then stop
        if (angle >= Mathf.PI * 2f)
        {
            isAnimating = false;
            currentSpeed = 0f;
            angle = 0f;
            transform.position = originalPosition;
            transform.rotation = originalRotation;
        }
    }

    [ContextMenu("StartCircleAnimation")]
    public void StartCircleAnimation()
    {
        if (isAnimating)
        {
            // Speed up, capped at maxSpeed
            currentSpeed = Mathf.Min(currentSpeed + speedIncrement, maxSpeed);
        }
        else
        {
            isAnimating = true;
            angle = 0f;
            currentSpeed = baseSpeed;
            transform.rotation = Quaternion.identity;
        }
    }

    [ContextMenu("EndCircleAnimation")]
    public void EndCircleAnimation()
    {
        isAnimating = false;
        currentSpeed = 0f;
        angle = 0f;
        transform.position = originalPosition;
        transform.rotation = originalRotation;
    }

}