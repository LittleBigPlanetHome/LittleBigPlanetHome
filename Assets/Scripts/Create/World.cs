using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class World : MonoBehaviour
{
    public static World i { get; private set; }
    public List<CraftObject> Crafted = new();

    public Settings StageSettings = new();

    private void Awake()
    {
        if (i != null)
        {
            Destroy(gameObject);
        }
        else
        {
            i = this;
        }
    }

    public void Craft(CraftObject Object)
    {
        Crafted.Add(Object);
    }

    public void Save()
    {
        BinaryFormatter BFormatter = new BinaryFormatter();
        
    }

    public class Settings
    {
        public string WorldName = "My Level Badge";
    }
}
