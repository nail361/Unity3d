using UnityEngine;
using System.Collections.Generic;
using System.IO;

public enum LanguageIndex: int { Russian, English };

public class LanguageManager : MonoBehaviour
{
    private static Dictionary<string,string> textTable = new Dictionary<string,string>();

    public static Font curFont;

    public static LanguageIndex curLanguage = LanguageIndex.English;

    public static void AutoDetectLanguage()
    {
        switch (Application.systemLanguage) {
            case SystemLanguage.Russian: curLanguage = LanguageIndex.Russian; break;
            case SystemLanguage.English: curLanguage = LanguageIndex.English; break;
            default: curLanguage = LanguageIndex.English; break;
        }
        LoadLanguage((int)curLanguage);
    }
    
    public static void LoadLanguage(int language)
    {
        curLanguage = (LanguageIndex)language;

        string fullpath = "Languages/";

        switch (curLanguage)
        {
            case LanguageIndex.Russian: fullpath += "russian"; break;
            case LanguageIndex.English: fullpath += "english"; break;
        }
        
        TextAsset textAsset = (TextAsset) Resources.Load(fullpath, typeof(TextAsset));

        // clear the hashtable
        textTable.Clear();
        
        StringReader reader = new StringReader(textAsset.text);
        
        string key;
        string val;

        while (reader.ReadLine() != "#END#")
        {
            key = reader.ReadLine();
            val = reader.ReadLine();
            
            // TODO: add error handling here in case of duplicate keys
            textTable.Add(key, val);
        }
        
        reader.Close();

        Resources.UnloadAsset(textAsset);

        /*
        string fontpath = "Fonts/";
        switch (curLanguage)
        {
            case LanguageIndex.Russian:
            case LanguageIndex.English: fontpath += "BIPs"; break;
        }

        //if (curFont != null) Resources.UnloadAsset(curFont);

        curFont = (Font)Resources.Load(fontpath);
        */
    }
    
    public static string GetText(string key)
    {
        if (textTable.ContainsKey(key))
            return textTable[key];
        else {
            Debug.LogError( "No text" );
            return "ERROR";
        }
    }

    public static int GetLanguageIndex()
    {
        return (int)curLanguage;
    }

    public static void ChangeLanguage( int index )
    {
        curLanguage = (LanguageIndex)index;

        LoadLanguage((int)curLanguage);
    }

}