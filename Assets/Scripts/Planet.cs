using UnityEngine;

public class Planet : MonoBehaviour
{
    void Start() {
        print("thanks chatgpt");
        foreach(Transform child in transform) {
            RaycastHit hit;
            //GameObject.CreatePrimitive(PrimitiveType.Sphere).transform.position = transform.position - child.position;
            Debug.DrawRay(child.position, transform.position - child.position);
            if (Physics.Raycast(child.position, transform.position - child.position, out hit)) {
                // If the ray hits the parent object, snap the child object to the point where it hits the parent's model surface
                print(hit.collider.name);
                child.position = hit.point;
                child.rotation = Quaternion.LookRotation(hit.normal);
            }
        }
    }

    public void RefreshPlanetEntries(CommunityLevel[] levels) {

    }

    void Update()
    {
        // a
    }
}
