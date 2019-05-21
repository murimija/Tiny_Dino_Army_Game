using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    private void Awake()
    {
        if (instance != null)
            return;

        instance = this;
    }

    [SerializeField] private float panSpeed;

    [SerializeField] private float minZ;
    [SerializeField] private float maxZ;
    [SerializeField] private float minX;
    [SerializeField] private float maxX;

    [SerializeField] private Vector3 target;
    [SerializeField] private float smoothSpeed = 0.03f;
    [SerializeField] private float moveWithButtonSpeed = 0.03f;

    private void Start()
    {
        target = new Vector3(0, 0.55f, 0);
    }

    public void CameraFollow(Vector3 newPosition)
    {
        target = newPosition;
    }

    void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, target) > 0.1)
        {
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, target, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }


    private void Update()
    {
        if (Input.GetKey("w"))
        {
            //transform.Translate(Vector3.forward * panSpeed * Time.deltaTime, Space.World);
            target += Vector3.forward * moveWithButtonSpeed;
        }

        if (Input.GetKey("s"))
        {
            //           transform.Translate(Vector3.back * panSpeed * Time.deltaTime, Space.World);
            target += Vector3.back * moveWithButtonSpeed;
        }

        if (Input.GetKey("d"))
        {
            //    transform.Translate(Vector3.right * panSpeed * Time.deltaTime, Space.World);
            target += Vector3.right * moveWithButtonSpeed;
        }

        if (Input.GetKey("a"))
        {
            //     transform.Translate(Vector3.left * panSpeed * Time.deltaTime, Space.World);
            target += Vector3.left * moveWithButtonSpeed;
        }

        var pos = transform.position;

        pos.z = Mathf.Clamp(pos.z, minZ, maxZ);
        pos.x = Mathf.Clamp(pos.x, minX, maxX);

        transform.position = pos;
    }
}