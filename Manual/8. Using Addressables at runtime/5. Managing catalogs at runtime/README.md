# Managing catalogs at runtime

默认情况下，Addressables 系统在运行时自动管理目录。如果您使用远程目录构建应用程序，Addressables 系统会自动检查您是否上传了新目录，如果是，则下载新版本并将其加载到内存中。

您可以在运行时加载其他目录。例如，您可以加载由单独的兼容项目生成的目录，以加载该项目构建的可寻址资产。（请参阅 [Loading Content from Multiple Projects](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/MultiProject.html)）

如果要更改 Addressables 系统的默认目录更新行为，可以关闭自动检查并手动检查更新。请参阅 [Updating catalogs](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/LoadContentCatalogAsync.html#updating-catalogs)。

## Loading additional catalogs

使用[Addressables.LoadContentCatalogAsync](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.AddressableAssets.Addressables.LoadContentCatalogAsync.html)从您的托管服务或本地文件系统加载其他内容目录。加载目录的操作完成后，您可以使用新目录中的键调用任何可寻址加载函数。

如果您在与目录相同的 URL 处提供目录哈希文件，Addressables 会缓存辅助目录。当客户端应用程序将来加载目录时，它只会在哈希更改时下载目录的新版本。

一旦加载目录，就无法卸载它。但是，您可以更新加载的目录。在更新目录之前，您必须释放加载目录的操作的操作句柄。有关更多信息，请参阅[Updating catalogs](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/LoadContentCatalogAsync.html#updating-catalogs)。

一般情况下，加载目录后没有理由保留操作句柄。您可以`autoReleaseHandle`在加载目录时通过将参数设置为 true 来自动释放它，如下例所示：

**NOTE**

您可以使用Addressables 设置的 [Catalog Download Timeout](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/AddressableAssetSettings.html#downloads)属性来指定下载目录的超时时间。

## Updating catalogs

如果目录哈希文件可用，Addressables 在加载目录时检查哈希，以确定提供的 URL 上的版本是否比目录的缓存版本更新。如果需要，您可以关闭默认目录检查，并在想要更新目录时调用[Addressables.UpdateCatalogs](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.AddressableAssets.Addressables.UpdateCatalogs.html)函数。如果您使用[LoadContentCatalogAsync](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.AddressableAssets.Addressables.LoadContentCatalogAsync.html)手动加载目录，则必须先释放操作句柄，然后才能更新目录。

当您调用 UpdateCatalog 函数时，所有其他 Addressable 请求都会被阻止，直到操作完成。您可以在操作完成后立即释放 UpdateCatalogs 返回的操作句柄（或将`autoRelease`参数设置为 true）。

如果您在未提供目录列表的情况下调用 UpdateCatalog，Addressables 会检查所有当前加载的目录是否有更新。

您也可以直接调用[Addressables.CheckForCatalogUpdates](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.AddressableAssets.Addressables.CheckForCatalogUpdates.html)来获取有更新的目录列表，然后执行更新：

**IMPORTANT**

*如果在已经从相关 AssetBundle 加载内容后更新目录，则可能会遇到加载的 AssetBundle 与更新版本之间的冲突。您可以在可寻址设置中启用[Unique Bundle Ids](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/ContentUpdateWorkflow.html#unique-bundle-ids-setting)选项，以消除运行时捆绑 ID 冲突的可能性。但是，启用此选项也意味着在执行内容更新时通常必须重建更多 AssetBundle。有关更多信息，请参阅 [Content update builds](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/ContentUpdateWorkflow.html)。另一种选择是首先卸载任何必须更新的内容和 AssetBundle，这可能是一个缓慢的操作。*

