# Getting addresses at runtime

默认情况下，Addressables 使用您分配给资产的地址作为其[IResourceLocation](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation.html)实例的[PrimaryKey](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation.PrimaryKey.html#UnityEngine_ResourceManagement_ResourceLocations_IResourceLocation_PrimaryKey)值。（如果您禁用资产所属的 Addressables 组的 **[Include Addresses in Catalog](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/GroupSettings.html#advanced-options)**中的**[地址](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/GroupSettings.html#advanced-options)**选项，PrimaryKey 可以是 GUID、标签或空字符串。）如果您想获取您加载的资产的地址AssetReference 或标签，您可以先加载资产的位置，如[Loading Assets by Location](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/LoadingAddressableAssets.html#loading-assets-by-location)所述。然后，您可以使用 IResourceLocation 实例访问 PrimaryKey 值并加载资产。

以下示例获取分配给名为的[AssetReference](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.AddressableAssets.AssetReference.html)对象的资产地址`MyRef1`：

标签通常指多个资产。以下示例说明了如何加载多个 Prefab 资产并使用它们的主键值将它们添加到字典中：

