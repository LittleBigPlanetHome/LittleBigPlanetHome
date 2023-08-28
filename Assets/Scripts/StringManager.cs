using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class StringManager : MonoBehaviour
{
    public static StringManager instance { get; private set; }

    public GameString[] strings;

    private void Awake() {
        if (instance != null) {
            Destroy(transform);
        }
        else {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public string GetString(string key) {
        GameString theString = strings.Where(gs => gs.name.ToLower() == key.ToLower()).FirstOrDefault();
        return theString != null ? theString.value : "[UNKOWN STRING: " + key + "]";
    }

    public int GetGameStringIndex(string key) {
        GameString theString = strings.Where(gs => gs.name.ToLower() == key.ToLower()).FirstOrDefault();
        return Array.IndexOf(strings, theString);
    }

    public int GetGameStringIndex(GameString key) {
        return Array.IndexOf(strings, key);
    }
}

[System.Serializable]
public class GameString {
    public string name;
    public string value;
}
