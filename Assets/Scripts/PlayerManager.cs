using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using PopitStuff.Enums;
using System.Collections;

public class PlayerManager : MonoBehaviour
{
    public bool debug;
    public PlayerControls controls;

    public bool sidewaysMode = true;
    public bool lockLayerCount = true;
    public int maxLayerCount = 4;
    public int minimumLayerCount = -4;
    public int layer = 0;
    public bool reverseMovement = false;
    public bool canControl = true;
    public float speed = 5f;
    public float zIntervals = 0.5f;
    public float yOffsetZCollisionCheck = 0.5f;
    Vector2 input = new Vector2();
    Rigidbody rb;
    bool swappingLayers = false;
    public Animator anim;

    private Vector3 anim_PodState = new Vector3(-0.0270000212f, 6.19601917f, 1.69299996f);

    private void Awake() {
        controls = new PlayerControls();
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    } 
    
    private void Start() {
        controls.Gameplay.Move.performed += ctx => GetMoveValues(ctx.ReadValue<Vector2>());
        controls.Gameplay.Move.canceled += ctx => { input = Vector2.zero; };
        print("Controls (Player) set up!");
    }

    private void GetMoveValues(Vector2 dir) {
        if (!reverseMovement) {
            input.x = -(dir.x * Time.fixedDeltaTime * speed);
            input.y = -(dir.y * Time.fixedDeltaTime * speed);
        }
        else {
            input.x = dir.x * Time.fixedDeltaTime * speed;
            input.y = dir.y * Time.fixedDeltaTime * speed;
        }

        if (dir.y >= 0.8 && !swappingLayers && canControl && layer < maxLayerCount) {
            if (!CheckZCollision(yOffsetZCollisionCheck, 1)) return;

            swappingLayers = true;
            layer++;
            transform.DOMoveZ(reverseMovement ? transform.position.z - zIntervals : transform.position.z + zIntervals, 0.15f).OnComplete(() => {
                swappingLayers = false;
            });

            // Play LayerBack animation
            StartCoroutine("SwitchLayersAnimation", 0);
        }

        if (dir.y <= -0.8 && !swappingLayers && canControl && layer > minimumLayerCount) {
            if (!CheckZCollision(yOffsetZCollisionCheck, -1)) return;

            swappingLayers = true;
            layer--;
            transform.DOMoveZ(reverseMovement ? transform.position.z + zIntervals : transform.position.z - zIntervals, 0.15f).OnComplete(() => {
                swappingLayers = false;
            });

            // Play LayerForward animation
            StartCoroutine("SwitchLayersAnimation", 1);
        }
    }

    private void FixedUpdate() {
        if (canControl)
        {
            rb.MovePosition(transform.position - new Vector3(input.x, 0, 0));
        }
    }

    private bool CheckZCollision(float yOff = 0.5f, int dir = 1) {
        Vector3 rayStart = new Vector3(transform.position.x, transform.position.y + yOff, transform.position.z);
        Vector3 rayEnd = new Vector3(transform.position.x, transform.position.y + yOff, transform.position.z + zIntervals);
        Debug.DrawLine(rayStart, rayEnd, Color.red);
        
        RaycastHit hit;
        if (Physics.Raycast(rayStart, new Vector3(0, 0, dir), out hit, zIntervals)) {
            print($"Hit {hit.collider.name}. Cancel the Z layer swap.");
            return false;
        }
        else {
            print("Z collision checking didn't hit anything! Free to go.");
            return true;
        }
    }

    private void Update() {
        if (debug) print(controls.Gameplay.Move.ReadValue<Vector2>());
        if (!SettingsManager.i || !Popit.i) return;

        var flag = FindObjectOfType<PodManagerImproved>() == null || !FindObjectOfType<PodManagerImproved>().ControlEnabled;

        if (canControl && !SettingsManager.i.pauseMenu.interactable && controls.Gameplay.Back.triggered && flag) {
            canControl = false;
            SettingsManager.i.ShowPauseMenu();
        } 
        else if (!canControl && SettingsManager.i.pauseMenu.interactable && controls.Gameplay.Back.triggered && !SettingsManager.i.settingsMenu.interactable) {
            canControl = true;
            SettingsManager.i.HidePauseMenu();
        }
        else if (!canControl && SettingsManager.i.pauseMenu.interactable && controls.Gameplay.Back.triggered && SettingsManager.i.settingsMenu.interactable)
        {
            canControl = false;
            SettingsManager.i.HideSettings();
        }

        if (Popit.i.currentState == PopitState.Closed && controls.Gameplay.Popit.triggered && canControl) {
            Popit.i.OpenPopit();
        } else if (Popit.i.currentState == PopitState.ScreenRightSide && controls.Gameplay.Popit.triggered) {
            canControl = true;
            Popit.i.ChangePopitState(PopitState.Default);
        }
        else if (Popit.i.currentState == PopitState.Default && controls.Gameplay.Popit.triggered)
        {
            canControl = true;
            Popit.i.ClosePopit();
        }

        if (canControl)
        {
            if (input.x > 0)
            {
                // Player is moving to the left
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    anim.SetFloat("Speed", 1f);
                    speed = 3f;
                }
                else
                {
                    anim.SetFloat("Speed", 2f);
                    speed = 5f;
                }
                transform.DORotateQuaternion(Quaternion.Euler(0, 260, 0), 0.5f);
            }
            else if (input.x < 0)
            {
                // Player is moving to the right
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    anim.SetFloat("Speed", 1f);
                    speed = 3f;
                }
                else
                {
                    anim.SetFloat("Speed", 2f);
                    speed = 5f;
                }
                if (transform.rotation.eulerAngles.y > 90 && transform.rotation.eulerAngles.y < 270)
                {
                    float angle = transform.rotation.eulerAngles.y;
                    if (angle > 180) angle -= 360;
                    transform.DORotateQuaternion(Quaternion.Euler(0, 100, 0), 0.5f);
                }
                else
                {
                    transform.DORotateQuaternion(Quaternion.Euler(0, 180, 0), 0.5f);
                }
            }
            else if (input.x == 0)
            {
                // Player is idle / TODO: kinda buggy when moving left & idle
                anim.SetFloat("Speed", 0f);
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    speed = 3f;
                }
                else
                {
                    speed = 5f;
                }
                transform.DORotateQuaternion(Quaternion.Euler(0, 180, 0), 0.5f);
            }
        }            
    }

    public void TriggerAnimation(string animName, bool bReset)
    {
        if (!bReset)
        {
          anim.SetTrigger(animName);
        }
        else
        {
            anim.ResetTrigger(animName);
        }
    }

    public void EnterPodStateAnim()
    {
        transform.DOMove(anim_PodState, 1f);
        transform.DORotateQuaternion(Quaternion.Euler(0, 180, 0), 0.1f);
        anim.SetInteger("PodState", 0);
        StartCoroutine("EnterPodStateAnimation");
    }

    public void ExitPodStateAnim()
    {
        anim.SetInteger("PodState", 2);
        StartCoroutine("ExitPodStateAnimation");
    }

    public void CostumeAnimationIdx(int Idx, float seconds)
    {
        Debug.Log("Costume animation called");
        StartCoroutine(CostumeAnimIdx(Idx, seconds));
    }

    private IEnumerator CostumeAnimIdx(int Idx, float seconds)
    {
        anim.SetInteger("CostumeIdx", Idx);
        yield return new WaitForSeconds(seconds);
        anim.SetInteger("CostumeIdx", -99);
    }

    private IEnumerator EnterPodStateAnimation()
    {
        yield return new WaitForSeconds(0.65f);
        anim.SetInteger("PodState", 1);
    }

    private IEnumerator ExitPodStateAnimation()
    {
        yield return new WaitForSeconds(0.65f);
        anim.SetInteger("PodState", -1);
    }

    private IEnumerator SwitchLayersAnimation(int layerIdx)
    {
        //back = 0
        //front = 1

        anim.SetLayerWeight(anim.GetLayerIndex("Mask Layer"), 1f);
        if (layerIdx == 0)
        {
            anim.SetTrigger("LayerBack");
        }
        else if (layerIdx == 1)
        {
            anim.SetTrigger("LayerFront");
        }

        yield return new WaitForSeconds(0.35f);
        anim.SetLayerWeight(anim.GetLayerIndex("Mask Layer"), 0f);
    }

    private void OnEnable() {
        controls.Enable();
    }

    private void OnDisable() {
        controls.Disable();
    }
}
