using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Holoi.Library.HoloKitApp
{
    [CreateAssetMenu(menuName = "ScriptableObjects/HoloKitAppGlobalSettings")]
    public class HoloKitAppGlobalSettings : ScriptableObject
    {
        public bool PromptGettingStartedInstruction;

        public bool Vibration;
        
        public bool HDR;
        
        public bool FourKResolution;
        
        public bool TechnicalInformationInStAR;
        
        private string SavedPath => Application.persistentDataPath + "/HoloKitAppGlobalSettings.save";

        public void Save()
        {
            //HoloKitAppGlobalSettings data = new HoloKitAppGlobalSettings(this);
            //BinaryFormatter formatter = new BinaryFormatter();

            //string path = SavedPath();
            //FileStream stream = new FileStream(path, FileMode.Create);
            //formatter.Serialize(stream, data);
            //stream.Close();
            
            //Debug.Log("[LocalPlayerSettings] Saved data to local storage");
        }

        //private HoloKitAppGlobalSettings LoadLocalPlayerSettings()
        //{
        //    //string path = SavedPath();
        //    //if (File.Exists(path))
        //    //{
        //    //    BinaryFormatter formatter = new BinaryFormatter();
        //    //    FileStream stream = new FileStream(path, FileMode.Open);

        //    //    HoloKitAppGlobalSettings data = formatter.Deserialize(stream) as HoloKitAppGlobalSettings;
        //    //    stream.Close();

        //    //    return data;
        //    //}
        //    //else
        //    //{
        //    //    Debug.Log("[SaveSystem] Save file not found at " + path);
        //    //    return null;
        //    //}
        //}
        
        // Local settings from the local storage.
        public void Load()
        {
            //HoloKitAppGlobalSettings data = SaveSystem.HoloKitAppGlobalSettings();
            //if (data != null)
            //{
            //    data.PromptGettingStartedInstruction = ;
            //    public bool Vibration;
            //    public bool HDR;
            //    public bool FourKResolution = 
            //    public bool TechnicalInformationInStAR;
            //    Debug.Log("[LocalPlayerSettings] Loaded data from local storage");
            //}
        }
    }
}