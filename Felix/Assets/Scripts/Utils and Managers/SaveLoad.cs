using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveLoad : MonoBehaviour {

	public static void SaveData( object obj, string fileName ){
		BinaryFormatter bf = new BinaryFormatter();
		MemoryStream ms = new MemoryStream();

		bf.Serialize( ms, obj );
		byte[] bytes = ms.GetBuffer();
        
		StreamWriter file = new StreamWriter( Application.persistentDataPath + "/" + fileName );
		file.Write( bytes.Length + ":" + Convert.ToBase64String( bytes, 0, bytes.Length, Base64FormattingOptions.None ) );
		file.Flush();
		file.Close();
	}

    public static bool HasData(string fileName)
    {
        if ( File.Exists(Application.persistentDataPath + "/" + fileName) )
        {
            return true;
        }

        return false;
    }

	public static object LoadData( string fileName ){

		if ( !(File.Exists( Application.persistentDataPath + "/" + fileName )) ){
			return null;
		}

		//Debug.Log( Application.persistentDataPath );

		StreamReader file = new StreamReader( Application.persistentDataPath + "/" + fileName );

		string base64 = file.ReadToEnd();

		file.Close();
		file.Dispose();

		int p = base64.IndexOf(":");
		int length = Convert.ToInt32(base64.Substring(0, p));
		byte[] data = Convert.FromBase64String(base64.Substring(p + 1));

		MemoryStream ms = new MemoryStream(data, 0, length);
		BinaryFormatter bf = new BinaryFormatter();

		object obj = bf.Deserialize(ms);
		ms.Close();
		ms.Dispose();

		return obj;
	}

}