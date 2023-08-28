using UnityEngine;

public class HeadHandler : MonoBehaviour
{
    public float maxRotationX = 90f;
    public float maxRotationY = 45f;
    public Transform headBone;
    public float rotationDuration = 1f;
    private Animator anim;
    private bool IsMouseKeyDown = false;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        // Check if the 'H' key is being pressed
        IsMouseKeyDown = Input.GetMouseButton(1);
    }

    private void LateUpdate()
    {
        // Check if the 'H' key is being pressed
        if (!IsMouseKeyDown)
        {
            return;
        }

        // Invert the direction of the mouse input by subtracting the mouse position from the screen width and height
        Vector3 mousePos = new Vector3(Screen.width - Input.mousePosition.x, Screen.height - Input.mousePosition.y, Input.mousePosition.z);

        // Convert the position of the mouse pointer on the screen to a position in the game world
        mousePos.z = Camera.main.transform.position.z;
        Vector3 targetPos = Camera.main.ScreenToWorldPoint(mousePos);

        // Calculate the rotation to look at the mouse pointer
        Quaternion targetRotation = Quaternion.LookRotation(targetPos - headBone.position);

        // Clamp the rotation angle in the X and Y axes
        float clampedRotationX = Mathf.Clamp(targetRotation.eulerAngles.x, -maxRotationX, maxRotationX);
        float clampedRotationY = Mathf.Clamp(targetRotation.eulerAngles.y, -maxRotationY, maxRotationY);
        Quaternion clampedRotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime / rotationDuration);

        // Apply the clamped rotation to the head bone
        if (anim == null)
        {
            headBone.rotation = clampedRotation;
        }
        else
        {
            anim.SetBoneLocalRotation(HumanBodyBones.Head, clampedRotation);
        }
    }
}
