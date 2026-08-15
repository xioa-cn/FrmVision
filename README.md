# FrmVision

FrmVision 是一个基于 Windows Forms、Cognex VisionPro 和 HslCommunication 的工业视觉检测与自动化控制系统。项目将相机采集、视觉工具、PLC/光源通讯、配方管理和节点工作流整合在同一套桌面应用中。

## 已实现功能

### 视觉检测

- 多相机图像窗口，可配置窗口名称、产品 Key 和当前配方。
- Cognex `CogAcqFifo` 相机采集工具配置。
- Cognex `CogToolBlock` 视觉工具编辑、保存和精简保存。
- 支持单张图像、文件夹顺序调试、连续运行和当前图像复制。
- 支持双图像输入、双目录匹配和时间差匹配。
- 相机工具、视觉工具和手动触发菜单根据相机配置动态生成。

### 可视化工作流

- 节点式流程编辑、保存、加载和运行。
- 支持启动、结束、相机、视觉、PLC、光源、配方、等待和手动触发节点。
- 提供布尔、数值、字符串、比较、取反、替换、裁剪、字符合并和补偿等数据处理节点。
- 支持 PLC 写入、PLC 比较输出、图像保存和全局数据节点。
- 运行中的工作流支持暂停、重启和关闭时的限时安全停止。

### 设备通讯

- PLC、相机和光源设备统一配置与状态监控。
- PLC 协议基于 HslCommunication 动态发现，并按协议显示扩展参数。
- 支持网口和串口连接参数。
- 光源支持 TCP 和串口实现。
- 通讯配置支持连接检测、启用/停用和运行状态展示。
- 提供通讯秘钥配置页面，启动时自动应用 HslCommunication 授权，并显示当前授权状态。

### 配方与参数

- 用户可配置视觉文件根目录 `ConfigCommonDir`。
- 自动读取 `产品型录/产品/配方` 目录结构。
- 支持将选定配方应用到对应图像窗口和视觉工具。
- 支持图像窗口配置及相机、视觉工具目录的快速打开。

### 用户与权限

- 使用 SQLite 存储用户数据。
- 支持用户登录、注册、退出登录和用户管理。
- 支持记住密码和自动登录，敏感数据使用 Windows DPAPI 当前用户范围加密。
- 密码使用 PBKDF2-SHA256 加盐哈希保存，最低长度为 5 个字符。
- 系统中的第一个注册用户自动成为管理员，后续注册用户默认为员工。

权限划分如下：

| 功能 | 员工 | 工程师 | 管理员 |
| --- | :---: | :---: | :---: |
| 配方应用、日志查询、手动触发 | 是 | 是 | 是 |
| 暂停和重启工作流 | 是 | 是 | 是 |
| 图像窗口及参数路径配置 | 否 | 是 | 是 |
| 流程编辑、通讯配置 | 否 | 是 | 是 |
| 相机工具、视觉工具、通讯秘钥 | 否 | 是 | 是 |
| 日志存储设置 | 否 | 是 | 是 |
| 用户角色、状态、密码和删除管理 | 否 | 否 | 是 |

### 日志中心

- 实时日志按等级和关键词筛选。
- 历史日志按日期、等级和关键词查询。
- 日志目录和保留天数可由用户配置。
- 磁盘日志采用有界队列和批量异步写入，避免高频日志阻塞业务线程。
- 程序关闭时执行有时间上限的日志刷新，不因磁盘异常阻止退出。

### 启动体验

- 启动页显示配置加载、通讯授权和主工作区创建进度。
- 配置读取或通讯授权失败时显示对应状态。
- 主界面提供设备状态、当前用户、工作流状态和日志工作区。

## 运行环境

- Windows x64
- .NET Framework 4.8
- Visual Studio，安装“.NET 桌面开发”工作负载
- Cognex VisionPro 及其设计、运行时组件
- HslCommunication 授权码（使用相关通讯能力时需要）
- NuGet 包还原，包括 `sqlite-net-pcl` 和 `SQLitePCLRaw`

Cognex 引用默认来自：

```text
C:\Program Files\Cognex\VisionPro\ReferencedAssemblies
```

如果本机安装路径不同，需要同步调整各项目中的 Cognex `HintPath`。

## 构建与运行

1. 使用 Visual Studio 打开 `FrmVision.slnx`。
2. 还原解决方案中的 NuGet 包。
3. 选择 `Debug` 或 `Release`，目标平台使用 x64。
4. 将 `ApplicationStartup` 设置为启动项目并运行。

也可以在 Visual Studio Developer PowerShell 中构建：

```powershell
msbuild FrmVision.slnx /t:Rebuild /p:Configuration=Debug
```

Debug 输出程序位于：

```text
ApplicationStartup\bin\Debug\FrmVision.exe
```

## 首次使用

1. 启动程序并注册第一个用户，该用户将成为管理员。
2. 登录后进入“系统 > 参数路径设置”，配置包含 `产品型录` 的视觉文件根目录。
3. 在“系统 > 图像窗口配置”中设置相机窗口和产品 Key。
4. 在“通讯”中添加 PLC、相机或光源设备并测试连接。
5. 使用 HslCommunication 时，在“系统 > 通讯秘钥”中保存并验证授权码。
6. 在“流程编辑”中创建并保存检测流程。
7. 选择配方后，通过手动触发或设备信号运行工作流。

## 配置和数据位置

用户级数据默认保存在：

```text
%LOCALAPPDATA%\FrmVision
```

| 文件 | 用途 |
| --- | --- |
| `vision-config.json` | 视觉文件根目录配置 |
| `users.db3` | SQLite 用户数据库 |
| `login-preferences.json` | 登录名、记住密码和自动登录设置 |
| `hsl-authorization.json` | DPAPI 加密后的通讯授权码 |
| `log-settings.xml` | 日志目录与保留天数 |
| `Logs\frmvision-yyyy-MM-dd.log` | 默认日志文件 |

应用目录还包含以下运行配置：

| 相对路径 | 用途 |
| --- | --- |
| `parameter\camera-parameters.json` | 图像窗口配置 |
| `communcation\communication-config.json` | 设备通讯配置 |
| `Editor\editor_data.json` | 节点工作流数据 |

`communcation` 是当前代码使用的目录名称，部署时不要自行更名。

## 解决方案结构

```text
FrmVision
├─ ApplicationStartup       程序入口和启动页
├─ NodeEditor.Library       节点编辑器基础控件
└─ src
   ├─ FrmCommon             公共配置、MVVM、日志和扩展方法
   ├─ FrmMapper             配置及数据模型
   ├─ FrmServices           业务服务、通讯、用户和 ViewModel
   ├─ FrmViews              主窗体、页面和工作区控件
   └─ FrmVpComponents       VisionPro 工具窗体及视觉服务
```

## 发布注意事项

- 应用程序固定使用 x64，Cognex 和 SQLite 原生库必须与进程位数一致。
- 发布目录必须包含 `e_sqlite3.dll`，并与 `FrmVision.exe` 位于同一目录，或放在 `runtimes\win-x64\native` 下。
- 不要只复制主程序 EXE；同时复制全部托管依赖、Cognex 组件、原生 DLL 和运行配置目录。
- 登录密码和通讯授权使用 Windows 当前用户加密，更换 Windows 用户后需要重新保存。
- VisionPro 和 HslCommunication 的使用及部署需遵守各自授权许可。
