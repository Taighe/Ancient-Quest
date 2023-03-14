using AQEngine.Data;
using AQEngine.Events;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class SaveManager : MonoBehaviour
    {
        public int NumberOfSaveSlots = 3;
        public SaveData[] SaveDataSlots => _saveDataSlots;
        public static SaveManager Instance
        {
            get
            {
                if (_manager == null)
                {
                    _manager = new GameObject("SaveManager").AddComponent<SaveManager>().Init();
                }

                return _manager;
            }
        }

        private SaveData[] _saveDataSlots;
        private static SaveManager _manager;

        public SaveManager Init()
        {
            _saveDataSlots = new SaveData[NumberOfSaveSlots];
            for(int i = 0; i < NumberOfSaveSlots; i++)
            {
                SaveData data = null;
                if(Load(i, out data))
                {
                    _saveDataSlots[i] = data.Init();
                }
            }

            return this;
        }

        public static bool Editor_SaveTest(int slot, SaveData testSaveData)
        {
            if(Save(slot, testSaveData))
            {
                Debug.Log($"Save Created for Slot {slot} at {Application.persistentDataPath}");
                return true;
            }

            return false;
        }

        public static bool Save(int slot = 0, SaveData data = null)
        {
            if (data == null)
            {
                data = new SaveData();
                GameEvents.Instance.Game_OnSave(new SaveEventArgs(ref data));
                slot = data.SaveSlot;
            }
            else
            {
                data.SaveSlot = slot;
            }

            string path = Application.persistentDataPath + $"/save{slot}.dat";
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                using (FileStream file = File.Create(path))
                {
                    bf.Serialize(file, data);
                    file.Close();
                }

                return data != null;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }

        public bool Load(int slot, out SaveData data)
        {
            string path = Application.persistentDataPath + $"/save{slot}.dat";
            data = null;
            try
            {
                if (File.Exists(path))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    using (FileStream file = File.OpenRead(path))
                    {
                        data = bf.Deserialize(file) as SaveData;
                        _saveDataSlots[slot] = data;
                        file.Close();
                    }
                }

                return data != null;
            }
            catch(Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }
    }
}
