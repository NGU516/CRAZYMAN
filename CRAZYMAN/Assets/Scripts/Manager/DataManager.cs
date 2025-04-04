using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Diagnostics.CodeAnalysis;

public class DataManager
{
    public void Init()
    {

    }

    // json ���� �Ͼ����
    public T ParseToList<T>([NotNull] string path)
    {
        using (var reader = new StringReader(Resources.Load<TextAsset>($"Data/{path}").text))
        {
            string json = reader.ReadToEnd();
            T gameData = JsonUtility.FromJson<T>(json);

            return gameData;
        }
    }
}
