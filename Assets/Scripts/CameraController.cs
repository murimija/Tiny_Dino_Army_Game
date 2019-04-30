using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float panSpeed;

    [SerializeField] private float minZ;
    [SerializeField] private float maxZ;
    [SerializeField] private float minX;
    [SerializeField] private float maxX;

    private void Update()
    {
        if (Input.GetKey("w"))
        {
            transform.Translate(Vector3.forward * panSpeed * Time.deltaTime, Space.World);
        }

        if (Input.GetKey("s"))
        {
            transform.Translate(Vector3.back * panSpeed * Time.deltaTime, Space.World);
        }

        if (Input.GetKey("d"))
        {
            transform.Translate(Vector3.right * panSpeed * Time.deltaTime, Space.World);
        }

        if (Input.GetKey("a"))
        {
            transform.Translate(Vector3.left * panSpeed * Time.deltaTime, Space.World);
        }

        var pos = transform.position;

        pos.z = Mathf.Clamp(pos.z, minZ, maxZ);
        pos.x = Mathf.Clamp(pos.x, minX, maxX);

        transform.position = pos;
    }
}