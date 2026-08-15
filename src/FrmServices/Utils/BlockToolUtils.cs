using FrmVpComponents;
using FrmVpComponents.Services;

namespace FrmServices.Utils;

public static class BlockToolUtils
{
    /// <summary>
    /// 打开视觉工具窗口
    /// </summary>
    /// <param name="cogToolBlockDir">视觉工具vpp路径</param>
    /// <param name="productionName">产品名称</param>
    /// <param name="cogToolBlockName">视觉工具名</param>
    public static void OpenCogToolBlock(
        string cogToolBlockDir, string productionName, string cogToolBlockName)
    {
        var cogToolBlockDic = BlockTool.Instance.GetCogToolBlock(productionName);
        var cogToolBlock = cogToolBlockDic[cogToolBlockName];
        frmToolBlock frmToolBlock = new frmToolBlock(cogToolBlock, cogToolBlockDir);
        frmToolBlock.Text = cogToolBlockDir;
        frmToolBlock.Show();
    }

    /// <summary>
    /// 打开相机工具窗口
    /// </summary>
    /// <param name="cogAcqFifoDir">相机工具vpp路径</param>
    /// <param name="productionName">产品名称</param>
    /// <param name="cogAcqFifoName">相机工具名</param>
    public static void OpenCogAcqFifo(
        string cogAcqFifoDir, string productionName, string cogAcqFifoName)
    {
        var cogAcqDir = BlockTool.Instance.GetCogAcqFifo(productionName);
        var cogAcq = cogAcqDir[cogAcqFifoName];
        frmFifo frmFifo = new frmFifo(cogAcq, cogAcqFifoDir);
        frmFifo.Text = cogAcqFifoDir;
        frmFifo.Show();
    }
}