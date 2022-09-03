using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.InputSystem;

public class GlobalInputs : MonoBehaviour
{
    public InputMain input;
    public float vertical, horizontal, drift, reset, addCheckpoint, removeCheckpoint, changeCamera;

    public void SaveBindingOverrides()
    {
        var asset = input.asset;
        var overrides = new Dictionary<Guid, string>();
        foreach (InputActionMap map in asset.actionMaps)
            foreach (var binding in map.bindings)
            {
                if (!string.IsNullOrEmpty(binding.overridePath))
                    overrides[binding.id] = binding.overridePath;
            }
        GlobalInputsSave.Save(overrides);
    }

    public void LoadBindingOverrides()
    {
        var asset = input.asset;
        var overrides = GlobalInputsSave.Load();
        foreach (var map in asset.actionMaps)
        {
            var bindings = map.bindings;
            for (int i = 0; i < bindings.Count; ++i)
            {
                if (overrides.TryGetValue(bindings[i].id, out var _overridePath))
                    map.ApplyBindingOverride(i, new InputBinding { overridePath = _overridePath });
            }
        }
    }

    void Awake()
    {
        input = new InputMain();
        LoadBindingOverrides();
        input.Enable();
    }
}

public static class GlobalInputsSave
{
    static string path = Application.persistentDataPath + "/input_overrides.bin";

    public static void Save(Dictionary<Guid, string> overrides)
    {
        BinaryFormatter formatter = new();
        FileStream stream = new(path, FileMode.Create);

        formatter.Serialize(stream, overrides);
        stream.Close();
    }

    public static Dictionary<Guid, string> Load()
    {
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new();
            FileStream stream = new(path, FileMode.Open);

            Dictionary<Guid, string> output;

            output = formatter.Deserialize(stream) as Dictionary<Guid, string>;
            return output;
        }
        else return new Dictionary<Guid, string>();
    }
}
