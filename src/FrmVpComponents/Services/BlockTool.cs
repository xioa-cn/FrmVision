using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Cognex.VisionPro;
using Cognex.VisionPro.ToolBlock;
using FrmServices.LogServices;


namespace FrmVpComponents.Services
{
    internal sealed class ToolsModel
    {
        public Dictionary<string, Dictionary<string, CogToolBlock>> ToolBlocks { get; } =
            new Dictionary<string, Dictionary<string, CogToolBlock>>(StringComparer.OrdinalIgnoreCase);

        public Dictionary<string, Dictionary<string, CogAcqFifoTool>> Cameras { get; } =
            new Dictionary<string, Dictionary<string, CogAcqFifoTool>>(StringComparer.OrdinalIgnoreCase);
    }

    public sealed class RecipeToolSwitchResult
    {
        internal RecipeToolSwitchResult(string productionKey,
            int cameraToolCount, int visionToolCount)
        {
            ProductionKey = productionKey;
            CameraToolCount = cameraToolCount;
            VisionToolCount = visionToolCount;
        }

        public string ProductionKey { get; }
        public int CameraToolCount { get; }
        public int VisionToolCount { get; }
    }

    /// <summary>
    /// 管理 VisionPro 视觉工具和相机工具的加载、访问及释放。
    /// </summary>
    public sealed class BlockTool : IDisposable
    {
        private static readonly Lazy<BlockTool> LazyInstance =
            new Lazy<BlockTool>(() => new BlockTool(), LazyThreadSafetyMode.ExecutionAndPublication);

        private readonly ReaderWriterLockSlim _toolsLock =
            new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        private readonly ToolsModel _toolsModel = new ToolsModel();
        private bool _disposed;

        private BlockTool()
        {
        }

        public static BlockTool Instance => LazyInstance.Value;

        /// <summary>
        /// 获取指定产品下视觉工具集合的快照。集合本身可修改，但不会改变内部缓存。
        /// 需要与加载、卸载严格互斥执行时，请使用 UseCogToolBlock。
        /// </summary>
        public Dictionary<string, CogToolBlock> GetCogToolBlock(string productionName)
        {
            var productionKey = NormalizeKey(productionName, nameof(productionName));
            _toolsLock.EnterReadLock();
            try
            {
                ThrowIfDisposed();
                return _toolsModel.ToolBlocks.TryGetValue(productionKey, out var tools)
                    ? new Dictionary<string, CogToolBlock>(tools, StringComparer.OrdinalIgnoreCase)
                    : new Dictionary<string, CogToolBlock>(StringComparer.OrdinalIgnoreCase);
            }
            finally
            {
                _toolsLock.ExitReadLock();
            }
        }

        /// <summary>
        /// 获取指定产品下相机工具集合的快照。集合本身可修改，但不会改变内部缓存。
        /// 需要与加载、卸载严格互斥执行时，请使用 UseCogAcqFifo。
        /// </summary>
        public Dictionary<string, CogAcqFifoTool> GetCogAcqFifo(string productionName)
        {
            var productionKey = NormalizeKey(productionName, nameof(productionName));
            _toolsLock.EnterReadLock();
            try
            {
                ThrowIfDisposed();
                return _toolsModel.Cameras.TryGetValue(productionKey, out var tools)
                    ? new Dictionary<string, CogAcqFifoTool>(tools, StringComparer.OrdinalIgnoreCase)
                    : new Dictionary<string, CogAcqFifoTool>(StringComparer.OrdinalIgnoreCase);
            }
            finally
            {
                _toolsLock.ExitReadLock();
            }
        }

        /// <summary>
        /// 在防止工具被并发卸载的读锁范围内使用视觉工具。
        /// </summary>
        public TResult UseCogToolBlock<TResult>(
            string productionName, string toolName,
            Func<CogToolBlock, TResult> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            return UseTool(_toolsModel.ToolBlocks, productionName,
                toolName, action, "视觉工具");
        }

        /// <summary>
        /// 在防止工具被并发卸载的读锁范围内使用视觉工具。
        /// </summary>
        public void UseCogToolBlock(
            string productionName, string toolName,
            Action<CogToolBlock> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            UseCogToolBlock(productionName, toolName, tool =>
            {
                action(tool);
                return true;
            });
        }

        /// <summary>
        /// 在防止工具被并发卸载的读锁范围内使用相机工具。
        /// </summary>
        public TResult UseCogAcqFifo<TResult>(
            string productionName, string toolName,
            Func<CogAcqFifoTool, TResult> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            return UseTool(_toolsModel.Cameras, productionName,
                toolName, action, "相机工具");
        }

        /// <summary>
        /// 在防止工具被并发卸载的读锁范围内使用相机工具。
        /// </summary>
        public void UseCogAcqFifo(
            string productionName, string toolName,
            Action<CogAcqFifoTool> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            UseCogAcqFifo(productionName, toolName, tool =>
            {
                action(tool);
                return true;
            });
        }

        /// <summary>
        /// 原子加载指定目录下的视觉工具。加载失败时保留原有工具集合。
        /// </summary>
        public void LoadCogToolBlock(string toolFileDir, string productionName)
        {
            var productionKey = NormalizeKey(productionName, nameof(productionName));
            Dictionary<string, CogToolBlock> loadedTools = null;
            try
            {
                loadedTools = LoadTools<CogToolBlock>(toolFileDir, "视觉工具");
                ReplaceToolSet(_toolsModel.ToolBlocks, productionKey, loadedTools, "视觉工具");
                loadedTools = null;
            }
            catch (Exception ex)
            {
                if (loadedTools != null) DisposeTools(loadedTools.Values, "视觉工具");
                AppLog.Error("视觉工具加载失败" + Environment.NewLine + ex, nameof(BlockTool));
                throw;
            }
        }

        /// <summary>
        /// 卸载指定产品下的视觉工具。
        /// </summary>
        public void UnloadCogToolBlock(string productionName)
        {
            var productionKey = NormalizeKey(productionName, nameof(productionName));
            UnloadToolSet(_toolsModel.ToolBlocks, productionKey, "视觉工具");
        }

        /// <summary>
        /// 清空并释放全部视觉工具。
        /// </summary>
        public void ClearCogToolBlock()
        {
            ClearToolSets(_toolsModel.ToolBlocks, "视觉工具");
        }

        /// <summary>
        /// 原子加载指定目录下的相机工具。加载失败时保留原有工具集合。
        /// </summary>
        public void LoadCogAcqFifo(string toolFileDir, string productionName)
        {
            var productionKey = NormalizeKey(productionName, nameof(productionName));
            Dictionary<string, CogAcqFifoTool> loadedTools = null;
            try
            {
                loadedTools = LoadTools<CogAcqFifoTool>(toolFileDir, "相机工具");
                ReplaceToolSet(_toolsModel.Cameras, productionKey, loadedTools, "相机工具");
                loadedTools = null;
            }
            catch (Exception ex)
            {
                if (loadedTools != null) DisposeTools(loadedTools.Values, "相机工具");
                AppLog.Error("相机工具加载失败" + Environment.NewLine + ex, nameof(BlockTool));
                throw;
            }
        }

        /// <summary>
        /// 卸载指定产品下的相机工具。
        /// </summary>
        public void UnloadCogAcqFifo(string productionName)
        {
            var productionKey = NormalizeKey(productionName, nameof(productionName));
            UnloadToolSet(_toolsModel.Cameras, productionKey, "相机工具");
        }

        /// <summary>
        /// 清空并释放全部相机工具。
        /// </summary>
        public void ClearCogAcqFifo()
        {
            ClearToolSets(_toolsModel.Cameras, "相机工具");
        }

        /// <summary>
        /// 原子切换指定产品的相机工具和视觉工具。两组工具全部加载成功后才替换缓存。
        /// </summary>
        public RecipeToolSwitchResult SwitchRecipeTools(
            string productionName,
            string cameraToolDirectory,
            string visionToolDirectory)
        {
            string productionKey = NormalizeKey(productionName, nameof(productionName));
            Dictionary<string, CogAcqFifoTool> loadedCameras = null;
            Dictionary<string, CogToolBlock> loadedVisionTools = null;
            Dictionary<string, CogAcqFifoTool> oldCameras = null;
            Dictionary<string, CogToolBlock> oldVisionTools = null;

            try
            {
                loadedCameras = LoadTools<CogAcqFifoTool>(
                    cameraToolDirectory, "相机工具");
                if (loadedCameras.Count == 0)
                    throw new InvalidDataException("配方目录中没有相机工具 VPP 文件。");

                loadedVisionTools = LoadTools<CogToolBlock>(
                    visionToolDirectory, "视觉工具");
                if (loadedVisionTools.Count == 0)
                    throw new InvalidDataException("配方目录中没有视觉工具 VPP 文件。");

                int cameraToolCount = loadedCameras.Count;
                int visionToolCount = loadedVisionTools.Count;
                _toolsLock.EnterWriteLock();
                try
                {
                    ThrowIfDisposed();
                    _toolsModel.Cameras.TryGetValue(productionKey, out oldCameras);
                    _toolsModel.ToolBlocks.TryGetValue(productionKey, out oldVisionTools);
                    _toolsModel.Cameras[productionKey] = loadedCameras;
                    _toolsModel.ToolBlocks[productionKey] = loadedVisionTools;
                    loadedCameras = null;
                    loadedVisionTools = null;
                }
                finally
                {
                    _toolsLock.ExitWriteLock();
                }

                if (oldCameras != null)
                    DisposeTools(oldCameras.Values, "相机工具");
                if (oldVisionTools != null)
                    DisposeTools(oldVisionTools.Values, "视觉工具");

                return new RecipeToolSwitchResult(productionKey,
                    cameraToolCount, visionToolCount);
            }
            catch (Exception ex)
            {
                if (loadedCameras != null)
                    DisposeTools(loadedCameras.Values, "相机工具");
                if (loadedVisionTools != null)
                    DisposeTools(loadedVisionTools.Values, "视觉工具");
                AppLog.Error("配方工具切换失败" + Environment.NewLine + ex,
                    nameof(BlockTool));
                throw;
            }
        }

        public void Dispose()
        {
            List<CogToolBlock> toolBlocks;
            List<CogAcqFifoTool> cameras;

            _toolsLock.EnterWriteLock();
            try
            {
                if (_disposed) return;
                _disposed = true;
                toolBlocks = _toolsModel.ToolBlocks.Values.SelectMany(tools => tools.Values).ToList();
                cameras = _toolsModel.Cameras.Values.SelectMany(tools => tools.Values).ToList();
                _toolsModel.ToolBlocks.Clear();
                _toolsModel.Cameras.Clear();
            }
            finally
            {
                _toolsLock.ExitWriteLock();
            }

            DisposeTools(toolBlocks, "视觉工具");
            DisposeTools(cameras, "相机工具");
        }

        private TResult UseTool<TTool, TResult>(
            Dictionary<string, Dictionary<string, TTool>> toolSets,
            string productionName,
            string toolName,
            Func<TTool, TResult> action,
            string toolType)
            where TTool : class, IDisposable
        {
            var productionKey = NormalizeKey(productionName, nameof(productionName));
            var name = NormalizeKey(toolName, nameof(toolName));

            _toolsLock.EnterReadLock();
            try
            {
                ThrowIfDisposed();
                if (!toolSets.TryGetValue(productionKey, out var tools) ||
                    !tools.TryGetValue(name, out var tool))
                    throw new KeyNotFoundException(
                        $"未找到产品“{productionKey}”下的{toolType}“{name}”。");
                return action(tool);
            }
            finally
            {
                _toolsLock.ExitReadLock();
            }
        }

        private void ReplaceToolSet<TTool>(
            Dictionary<string, Dictionary<string, TTool>> toolSets,
            string productionKey,
            Dictionary<string, TTool> newTools,
            string toolType)
            where TTool : class, IDisposable
        {
            Dictionary<string, TTool> oldTools = null;
            _toolsLock.EnterWriteLock();
            try
            {
                ThrowIfDisposed();
                toolSets.TryGetValue(productionKey, out oldTools);
                toolSets[productionKey] = newTools;
            }
            finally
            {
                _toolsLock.ExitWriteLock();
            }

            if (oldTools != null) DisposeTools(oldTools.Values, toolType);
        }

        private void UnloadToolSet<TTool>(
            Dictionary<string, Dictionary<string, TTool>> toolSets,
            string productionKey,
            string toolType)
            where TTool : class, IDisposable
        {
            Dictionary<string, TTool> removedTools = null;
            _toolsLock.EnterWriteLock();
            try
            {
                ThrowIfDisposed();
                if (toolSets.TryGetValue(productionKey, out removedTools))
                    toolSets.Remove(productionKey);
            }
            finally
            {
                _toolsLock.ExitWriteLock();
            }

            if (removedTools != null) DisposeTools(removedTools.Values, toolType);
        }

        private void ClearToolSets<TTool>(
            Dictionary<string, Dictionary<string, TTool>> toolSets,
            string toolType)
            where TTool : class, IDisposable
        {
            List<TTool> removedTools;
            _toolsLock.EnterWriteLock();
            try
            {
                ThrowIfDisposed();
                removedTools = toolSets.Values.SelectMany(tools => tools.Values).ToList();
                toolSets.Clear();
            }
            finally
            {
                _toolsLock.ExitWriteLock();
            }

            DisposeTools(removedTools, toolType);
        }

        private static Dictionary<string, TTool> LoadTools<TTool>(string toolFileDir, string toolType)
            where TTool : class, IDisposable
        {
            if (string.IsNullOrWhiteSpace(toolFileDir))
                throw new ArgumentException("工具目录不能为空。", nameof(toolFileDir));

            var fullPath = Path.GetFullPath(Environment.ExpandEnvironmentVariables(toolFileDir.Trim()));
            if (!Directory.Exists(fullPath))
                throw new DirectoryNotFoundException($"工具目录不存在：{fullPath}");

            var loadedTools = new Dictionary<string, TTool>(StringComparer.OrdinalIgnoreCase);
            try
            {
                var files = Directory.EnumerateFiles(fullPath, "*.vpp", SearchOption.TopDirectoryOnly)
                    .OrderBy(Path.GetFileName, StringComparer.OrdinalIgnoreCase);

                foreach (var filePath in files)
                {
                    TTool tool = null;
                    object serializedObject = null;
                    try
                    {
                        serializedObject = CogSerializer.LoadObjectFromFile(filePath);
                        tool = serializedObject as TTool;
                        if (tool == null)
                        {
                            var actualType = serializedObject?.GetType().FullName ?? "null";
                            throw new InvalidDataException(
                                $"文件内容不是有效的{toolType}，实际类型：{actualType}。");
                        }

                        var toolName = Path.GetFileNameWithoutExtension(filePath);
                        if (string.IsNullOrWhiteSpace(toolName))
                            throw new InvalidDataException("无法从文件名确定工具名称。");
                        if (loadedTools.ContainsKey(toolName))
                            throw new InvalidDataException($"存在重复的{toolType}名称：{toolName}。");

                        SetToolName(tool, toolName);
                        loadedTools.Add(toolName, tool);
                        tool = null;
                        serializedObject = null;
                    }
                    catch (Exception ex)
                    {
                        SafeDispose(tool ?? serializedObject as IDisposable, toolType);
                        var rootCause = ex.GetBaseException();
                        throw new InvalidDataException(
                            $"加载文件失败：{filePath}。根因：{rootCause.GetType().FullName}: {rootCause.Message}",
                            ex);
                    }
                }

                return loadedTools;
            }
            catch
            {
                DisposeTools(loadedTools.Values, toolType);
                throw;
            }
        }

        private static void SetToolName<TTool>(TTool tool, string name) where TTool : class
        {
            if (tool is CogToolBlock toolBlock)
            {
                toolBlock.Name = name;
                return;
            }

            if (tool is CogAcqFifoTool camera)
            {
                camera.Name = name;
                return;
            }

            throw new NotSupportedException($"不支持的工具类型：{typeof(TTool).FullName}");
        }

        private static void DisposeTools<TTool>(IEnumerable<TTool> tools, string toolType)
            where TTool : class, IDisposable
        {
            foreach (var tool in tools)
                SafeDispose(tool, toolType);
        }

        private static void SafeDispose(IDisposable tool, string toolType)
        {
            if (tool == null) return;
            try
            {
                tool.Dispose();
            }
            catch (Exception ex)
            {
                AppLog.Error($"{toolType}释放失败" + Environment.NewLine + ex, nameof(BlockTool));
            }
        }

        private static string NormalizeKey(string value, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("值不能为空。", parameterName);
            return value.Trim();
        }

        private void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(BlockTool));
        }
    }
}
