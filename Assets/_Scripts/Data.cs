using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class Data
{

    public static void SaveUnlocks(Unlocks unlocks)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/unlocks.dat";
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, unlocks);

        stream.Close();
    }

    public static Unlocks LoadUnlocks()
    {
        Debug.Log(Application.persistentDataPath);
        string path = Application.persistentDataPath + "/unlocks.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            Unlocks unlocks = formatter.Deserialize(stream) as Unlocks;
            stream.Close();
            return unlocks;
        }
        else
        {
            return new Unlocks();
        }
    }
}
