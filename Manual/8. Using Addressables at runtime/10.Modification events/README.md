# Modification events

修改事件用于信号到Addressables系统的部件时某些数据被操纵，诸如`AddressableAssetGroup`或`AddressableAssetEntry`获取添加或移除。

修改事件作为`SetDirty`Addressables 内部调用的一部分触发。 `SetDirty`用于指示资产何时需要由 重新序列化`AssetDatabase`。作为 this 的一部分，`SetDirty`可以触发两个修改事件回调：

- `public static event Action<AddressableAssetSettings, ModificationEvent, object> OnModificationGlobal`
- `public Action<AddressableAssetSettings, ModificationEvent, object> OnModification { get; set; }`

可以分别[`AddressableAssetSettings`](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Settings.AddressableAssetSettings.html)通过静态或实例访问器找到它们。

#### Code Samples

```csharp
AddressableAssetSettings.OnModificationGlobal += (settings, modificationEvent, data) =>
        {
            if(modificationEvent == AddressableAssetSettings.ModificationEvent.EntryAdded)
            {
                //Do work
            }
        };

        AddressableAssetSettingsDefaultObject.Settings.OnModification += (settings, modificationEvent, data) =>
        {
            if (modificationEvent == AddressableAssetSettings.ModificationEvent.EntryAdded)
            {
                //Do work
            }
        };
```

修改事件传入`object`与事件关联的数据的泛型。下面是修改事件和随它们传递的数据类型的列表。

#### The Data Passed with Each ModificationEvent:

- GroupAdded 随此事件传递的数据是[`AddressableAssetGroup`](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Settings.AddressableAssetGroup.html)已添加的组或组列表。
- GroupRemoved 随此事件传递的数据是[`AddressableAssetGroup`](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Settings.AddressableAssetGroup.html)已删除的组或组列表。
- GroupRenamed 与此事件一起传递的数据是[`AddressableAssetGroup`](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Settings.AddressableAssetGroup.html)已重命名的组或组列表。
- GroupSchemaAdded 随此事件传递的数据是[`AddressableAssetGroup`](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Settings.AddressableAssetGroup.html)添加了架构的 或组列表。
- GroupSchemaRemoved 随此事件传递的数据是[`AddressableAssetGroup`](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Settings.AddressableAssetGroup.html)已从中删除架构的组或组列表。
- GroupSchemaModified 与此事件一起传递的数据是[`AddressableAssetGroupSchema`](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Settings.AddressableAssetGroupSchema.html)被修改的数据。
- GroupTemplateAdded 与此事件一起传递的数据是`ScriptableObject`，通常是实现 的数据[`IGroupTemplate`](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Settings.IGroupTemplate.html)，即添加的组模板对象。
- GroupTemplateRemoved 随此事件传递的数据是`ScriptableObject`，通常是实现的数据，即已[`IGroupTemplate`](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Settings.IGroupTemplate.html)删除的组模板对象。
- GroupTemplateSchemaAdded 与此事件一起传递的数据是[`AddressableAssetGroupTemplate`](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Settings.AddressableAssetGroupTemplate.html)添加了架构的数据。
- GroupTemplateSchemaRemoved 随此事件传递的数据是[`AddressableAssetGroupTemplate`](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Settings.AddressableAssetGroupTemplate.html)已删除架构的数据。
- EntryCreated 与此事件一起传递的数据[`AddressableAssetEntry`](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Settings.AddressableAssetEntry.html)是创建的数据。
- EntryAdded 随此事件传递的数据[`AddressableAssetEntry`](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Settings.AddressableAssetEntry.html)是添加的条目或条目列表。
- EntryMoved 随此事件传递的数据是[`AddressableAssetEntry`](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Settings.AddressableAssetEntry.html)从一组移动到另一组的条目或条目列表。
- EntryRemoved 随此事件传递的数据是[`AddressableAssetEntry`](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Settings.AddressableAssetEntry.html)已删除的条目或条目列表。
- LabelAdded 与此事件一起传递的数据`string`是添加的标签。
- LabelRemoved 此事件传递的数据是`string`被移除的标签。
- ProfileAdded 与此事件一起传递的数据[`BuildProfile`](xref:UnityEditor.AddressableAssets.Settings.BuildProfile)是添加的。
- ProfileRemoved 随此事件传递的数据是`string`已删除的配置文件 ID。
- ProfileModified 与此事件一起传递的数据是[`BuildProfile`](xref:UnityEditor.AddressableAssets.Settings.BuildProfile)被修改的，或者`null`是一批`BuildProfiles`被修改的数据。
- ActiveProfileSet 如果`string`配置文件 ID 设置为活动配置文件，则随此事件传递的数据。
- EntryModified 随此事件传递的数据是[`AddressableAssetEntry`](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Settings.AddressableAssetEntry.html)已修改的条目或条目列表。
- BuildSettingsChanged 这个事件传递的数据[`AddressableAssetBuildSettings`](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Settings.AddressableAssetBuildSettings.html)就是被修改的对象。
- ActiveBuildScriptChanged 与此事件一起传递的数据[`IDataBuilder`](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Build.IDataBuilder.html)是设置为活动构建器的构建脚本。
- DataBuilderAdded 与此事件一起传递的数据是`ScriptableObject`，通常是实现 的数据[`IDataBuilder`](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Build.IDataBuilder.html)，该数据已添加到 DataBuilder 列表中。
- DataBuilderRemoved 与此事件一起传递的数据是从 DataBuilder 列表中删除的`ScriptableObject`，通常是实现[`IDataBuilder`](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Build.IDataBuilder.html)的数据。
- InitializationObjectAdded 与此事件一起传递的数据是被添加到 InitializationObjects 列表中的数据`ScriptableObject`，通常是实现的数据[`IObjectInitializationDataProvider`](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.Util.IObjectInitializationDataProvider.html)。
- InitializationObjectRemoved 随此事件传递的数据是从 InitializationObjects 列表中删除的`ScriptableObject`，通常是实现的数据[`IObjectInitializationDataProvider`](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.Util.IObjectInitializationDataProvider.html)。
- ActivePlayModeScriptChanged 与此事件一起传递的数据[`IDataBuilder`](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Build.IDataBuilder.html)是设置为新的活动播放模式数据构建器的数据。
- BatchModification 与此事件一起传递的数据是`null`。该事件主要用于指示多个修改事件同时发生并且[`AddressableAssetSettings`](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Settings.AddressableAssetSettings.html)需要将对象标记为脏的。
- HostingServicesManagerModified 传递的数据要么是[`HostingServicesManager`](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.HostingServices.HostingServicesManager.html)，要么[`HttpHostingService`](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.HostingServices.HttpHostingService.html)是被修改的。
- GroupMoved 与此事件一起传递的数据是 的完整列表[`AddressableAssetGroups`](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Settings.AddressableAssetGroup.html)。
- CertificateHandlerChanged 与此事件一起传递的数据是`System.Type`要使用的证书处理程序的新数据。