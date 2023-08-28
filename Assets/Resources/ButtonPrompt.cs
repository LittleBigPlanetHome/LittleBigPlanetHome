using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class ButtonPrompt : MonoBehaviour
{
    public Sprite keyboardPrompt;
    public Sprite playstationPrompt;
    public Sprite nintendoPrompt;
    public Sprite xboxPrompt;
    public Sprite genericPrompt;

    private Image image;

    void Start()
    {
        image = GetComponent<Image>();
        string[] joystickNames = Input.GetJoystickNames();

        foreach (string joystickName in joystickNames)
        {
            UnityEngine.Debug.Log(joystickName);
        }
    }

    void Update()
    {
        string[] joystickNames = Input.GetJoystickNames();

        if (joystickNames.Length > 0)
        {
            string joystickName = joystickNames[0];

            if (joystickName.ToLower().Contains("wireless controller"))
            {
                image.sprite = playstationPrompt;
            }
            else if (joystickName.ToLower().Contains("wireless gamepad"))
            {
                image.sprite = nintendoPrompt;
            }
            // This might affect xbox and generic, but when connecting my joycon via bluetooth, unity reported that as the joystick name, correct this later if all bluetooth gamepad connectiosn use this.
            else if (joystickName.ToLower().Contains("xbox") || joystickName.ToLower().Contains("microsoft"))
            {
                image.sprite = xboxPrompt;
            }
            else
            {
                image.sprite = genericPrompt;
            }
        }
        else
        {
            image.sprite = keyboardPrompt;
        }
    }
}
