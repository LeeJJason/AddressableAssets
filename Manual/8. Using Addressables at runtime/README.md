# Using Addressables at runtime

一旦您将可寻址资产组织成组并构建到 AssetBundles 中，您仍然必须在运行时加载、实例化并最终释放它们。

Addressables 使用引用计数系统来确保资产仅在需要时才保留在内存中。有关引用计数以及如何在任何给定时间最小化资产使用的内存量的更多信息，请参阅[Memory management](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/MemoryManagement.html)。

Addressables 提供了多种选项和 API 来加载和实例化可寻址资产。有关信息和示例，请参阅 [Loading Addressable assets](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/LoadingAddressableAssets.html)，包括：

- [Loading an single asset](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/LoadingAddressableAssets.html#loading-a-single-asset)
- [Loading multiple assets](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/LoadingAddressableAssets.html#loading-multiple-assets)
- [Loading an AssetReference](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/LoadingAddressableAssets.html#loading-an-assetreference)
- [Loading Scenes](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/LoadingAddressableAssets.html#loading-scenes)
- [Loading assets by location](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/LoadingAddressableAssets.html#loading-assets-by-location)
- [Instantiating objects from Addressables](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/LoadingAddressableAssets.html#instantiating-objects-from-addressables)
- [Releasing Addressable assets](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/LoadingAddressableAssets.html#releasing-addressable-assets)
- [Using Addressables in a Scene](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/LoadingAddressableAssets.html#using-addressables-in-a-scene)
- [Downloading dependencies in advance](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/DownloadDependenciesAsync.html)

Addressables 对大多数加载任务使用异步操作。有关如何处理代码中的 [Operations](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/AddressableAssetsAsyncOperationHandle.html)的信息，请参阅 [Operations](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/AddressableAssetsAsyncOperationHandle.html)，包括：

- [Releasing AsyncOperationHandle instances](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/AddressableAssetsAsyncOperationHandle.html#releasing-asyncoperationhandle-instances)
- [Coroutine- and IEnumerator-based operation handling](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/AddressableAssetsAsyncOperationHandle.html#coroutine-operation-handling)
- [Event-based operation handling](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/AddressableAssetsAsyncOperationHandle.html#event-based-operation-handling)
- [Task-based operation handling](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/AddressableAssetsAsyncOperationHandle.html#task-based-operation-handling)
- [Using operations synchronously](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/AddressableAssetsAsyncOperationHandle.html#using-operations-synchronously)
- [Custom operations](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/AddressableAssetsAsyncOperationHandle.html#custom-operations)
- [Using typed versus untyped operation handles](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/AddressableAssetsAsyncOperationHandle.html#using-typed-versus-typeless-operation-handles)
- [Reporting operation progress](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/AddressableAssetsAsyncOperationHandle.html#reporting-operation-progress)

有关其他运行时主题的信息，请参阅以下内容：

- [Customizing initialization](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/InitializeAsync.html)
- [Loading additional catalogs](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/LoadContentCatalogAsync.html#loading-additional-catalogs)
- [Updating catalogs](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/LoadContentCatalogAsync.html#updating-catalogs)
- [Modifying resource URLs at runtime](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/TransformInternalId.html)
- [Getting the address of an asset at runtime](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/GetRuntimeAddress.html)

