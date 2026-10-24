using System;
using System.IO;
using Unity.Netcode;
using UnityEngine;

public class Scribe : NetworkBehaviour
{
    private string Filename;
    private void Awake()
    {
        Filename = GetPath() + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") +".csv";
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void NoteDownRpc(QAresponses answers, RpcParams rpcParams = default)
    {
        ulong senderClientId = rpcParams.Receive.SenderClientId;
        string entry = senderClientId + "," + answers.ToString();
        WritetoFile(entry);
        Debug.Log("Wrote to file at "+ Application.dataPath + "/Data/");
    }

    private void WritetoFile(string entry)
    {
        File.AppendAllText(
        Filename,
        entry + Environment.NewLine);
        /*
        using (FileStream Responses = new FileStream(Filename, FileMode.Append, FileAccess.ReadWrite))
        {
            using (StreamWriter pencil = new StreamWriter(Responses))
            {
                pencil.WriteLine(entry);
                pencil.WriteLine("\n");
            }
        }
        */
    }

    private string GetPath()
    {
#if UNITY_EDITOR
        return Application.dataPath + "/Data/";
#elif UNITY_ANDROID
        return Application.persistentDataPath;
#else 
        return Application.dataPath + "/";
#endif
    }
}
