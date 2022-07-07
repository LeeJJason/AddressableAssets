using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableModule : MonoBehaviour
{
    private GameObject remoteStatic;
    private GameObject remoteNonStatic;
    private GameObject localStatic;
    private GameObject localNonStatic;

    private void OnGUI()
    {
        int fontSize = GUI.skin.button.fontSize;
        GUI.skin.button.fontSize = 50;
        GUILayout.BeginVertical();
        if(GUILayout.Button(remoteStatic == null ? "Create Remote Static" : "Release Remote Static")) 
        {
            if (remoteStatic != null)
            {
                Addressables.ReleaseInstance(remoteStatic);
                remoteStatic = null;
            }
            else
            {
                Addressables.InstantiateAsync("Assets/Res/remote_static111.prefab").Completed += (AsyncOperationHandle<GameObject> handle) =>
                {
                    remoteStatic = handle.Result;
                    handle.Result.transform.localPosition = new Vector3((Random.value * 2 - 1) * 5, (Random.value * 2 - 1) * 5, 0);
                };
            }
        }

        if (GUILayout.Button(remoteNonStatic == null ? "Create Remote Non Static" : "Release Remote Non Static"))
        {
            if (remoteNonStatic != null)
            {
                Addressables.ReleaseInstance(remoteNonStatic);
                remoteNonStatic = null;
            }
            else
            {
                Addressables.InstantiateAsync("Assets/Res/remote_non_static.prefab").Completed += (AsyncOperationHandle<GameObject> handle) =>
                {
                    remoteNonStatic = handle.Result;
                    handle.Result.transform.localPosition = new Vector3((Random.value * 2 - 1) * 5, (Random.value * 2 - 1) * 5, 0);
                };
            }
        }

        if (GUILayout.Button(localStatic == null ? "Create Local Static" : "Release Local Static"))
        {
            if (localStatic != null)
            {
                Addressables.ReleaseInstance(localStatic);
                localStatic = null;

            }
            else
            {
                Addressables.InstantiateAsync("Assets/Res/local_static.prefab").Completed += (AsyncOperationHandle<GameObject> handle) =>
                {
                    localStatic = handle.Result;
                    handle.Result.transform.localPosition = new Vector3((Random.value * 2 - 1) * 5, (Random.value * 2 - 1) * 5, 0);
                };
            }
        }

        if (GUILayout.Button(localNonStatic == null ? "Crate Local Non Static" : "Release Local Non Static"))
        {
            if (localNonStatic != null)
            {
                Addressables.ReleaseInstance(localNonStatic);
                localNonStatic = null;
            }
            else
            {
                Addressables.InstantiateAsync("Assets/Res/local_non_static.prefab").Completed += (AsyncOperationHandle<GameObject> handle) =>
                {
                    localNonStatic = handle.Result;
                    handle.Result.transform.localPosition = new Vector3((Random.value * 2 - 1) * 5, (Random.value * 2 - 1) * 5, 0);
                };
            }
        }


        if (GUILayout.Button("CheckForCatalogUpdates"))
        {
            var handle = Addressables.CheckForCatalogUpdates(false);
            handle.Completed += (AsyncOperationHandle<List<string>> h) =>
            {
                Debug.LogError(h.Status);
                if (h.Status == AsyncOperationStatus.Succeeded)
                {
                    for (int i = 0; i < h.Result.Count; ++i)
                    {
                        Debug.LogError(h.Result);
                    }
                }
            };
        }

        GUILayout.EndVertical();
        GUI.skin.button.fontSize = fontSize;
    }
}
