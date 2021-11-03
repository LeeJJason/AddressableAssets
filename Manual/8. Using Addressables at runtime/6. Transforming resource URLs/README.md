# Transforming resource URLs

Addressables 提供以下方法来修改它用于在运行时加载资产的 URL：

- Profile 变量中的静态属性
- 实现 ID 转换功能

## Static Profile variables

您可以在定义[RemoteLoadPath Profile variable](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/AddressableAssetsProfiles.html)时使用静态属性来指定应用程序从中加载远程内容（包括目录、目录哈希文件和 AssetBundles）的全部或部分 URL。有关在Profile 变量中指定属性名称的信息，请参阅[Profile variable syntax](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/AddressableAssetsProfiles.html#profile-variable-syntax)。必须在 Addressables 初始化之前设置静态属性的值。初始化后更改值无效。

## ID transform function

您可以为[Addressables.ResourceManager](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.AddressableAssets.Addressables.ResourceManager.html#UnityEngine_AddressableAssets_Addressables_ResourceManager)对象的[InternalIdTransformFunc](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.ResourceManager.InternalIdTransformFunc.html#UnityEngine_ResourceManagement_ResourceManager_InternalIdTransformFunc)属性分配一个函数，以单独更改 Addressables 从中加载资产的 URL。您必须在相关操作开始前分配函数，否则使用默认 URL。

使用 TransformInternalId 提供了相当大的灵活性，尤其是在远程托管方面。给定单个 IResourceLocation，您可以将 ID 转换为指向在运行时指定的服务器。如果您的服务器 IP 地址发生变化，或者您使用不同的 URL 来提供应用程序资产的不同变体，这将特别有用。

ResourceManager 在查找资产时调用您的 TransformInternalId 函数，将资产的[IResourceLocation](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation.html)实例传递给您的函数。您可以更改此 IResourceLocation的[InternalId](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation.InternalId.html#UnityEngine_ResourceManagement_ResourceLocations_IResourceLocation_InternalId)属性并将修改后的对象返回给 ResourceManager。

以下示例说明了如何将查询字符串附加到所有 URL：