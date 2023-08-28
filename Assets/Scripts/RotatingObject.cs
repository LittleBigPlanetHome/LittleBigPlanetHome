using UnityEngine;

public class RotatingObject : MonoBehaviour
{
    public float speed = 1f;
    public Axis rotationAxis;

    private void FixedUpdate() {
        if (rotationAxis == Axis.X)
            transform.Rotate(new Vector3(speed, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z));
        if (rotationAxis == Axis.Y)
            transform.Rotate(new Vector3(transform.rotation.eulerAngles.x, speed, transform.rotation.eulerAngles.z));
        if (rotationAxis == Axis.Z)
            transform.Rotate(new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, speed));
    }
}

public enum Axis {
    X,
    Y,
    Z
}