using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine.UI;

public class AssetChecker : MonoBehaviour
{
    public Text context;
    // Start is called before the first frame update
    public void  AssetStart()
    {
        StartCoroutine("CheckAssetList");
    }
    int index;
    IEnumerator CheckAssetList()
    {
        var targetDir = Application.streamingAssetsPath+"/";
        var directoryInfo = new DirectoryInfo(targetDir);
        var files = directoryInfo.GetFiles("*.rar", SearchOption.AllDirectories);

         index = 0;

        StringBuilder text = new StringBuilder();
        for (int i = 0; i < files.Length; i++)
        {
            var temp = files[i];
            var assetPath = temp.FullName.Replace("\\", "/").Replace(targetDir,"");
            text.AppendLine(assetPath);
            index++;
            yield return null;
            context.text = assetPath + " : " + index.ToString();
        }


        var targetMenuePath = targetDir + "Debug.txt";
#if UNITY_EDITOR
        if(File.Exists(targetMenuePath))
        {
            File.Delete(targetMenuePath);
        }
        File.WriteAllText(targetMenuePath, text.ToString());
#endif

#if !UNITY_EDITOR
        index = 0;
        using (StreamReader sr = new StreamReader(File.OpenRead(targetMenuePath)))
        {
            string data = string.Empty;
            while( (data = sr.ReadLine()) !=null)
            {
                index++;
                StartCoroutine("CopyFile",data);
            }
        }
#endif
    }

    IEnumerator CopyFile(string fileName)
    {
        context.text = "Copy - " + fileName + " : " + index.ToString();

        var source = Application.streamingAssetsPath + "/" + fileName;
        var target = Application.persistentDataPath + "/" + fileName;
        Debug.Log(target);
        if (File.Exists(target))
        {
            yield break;
        }

        WWW www = new WWW(Application.streamingAssetsPath + "/" + fileName);

        yield return www;

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError(www.error);
        }
        else
        {
            try
            {
                if (File.Exists(target))
                { 
                    File.Delete(target);
                }
                File.WriteAllBytes(target, www.bytes);
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }
    }
}
