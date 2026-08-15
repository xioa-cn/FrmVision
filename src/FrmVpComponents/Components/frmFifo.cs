using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using Cognex.VisionPro;

namespace FrmVpComponents
{
    public partial class frmFifo : Form
    {
        private string _fileName = string.Empty;

        public frmFifo()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 相机工具界面
        /// </summary>
        /// <param name="acqFifoTool">需要打开的AcqFifoTool</param>
        /// <param name="fileName">保存在本地时的文件名，不需要加后缀</param>
        public frmFifo(CogAcqFifoTool acqFifoTool, string fileName) : this()
        {
            cogAcqFifoEditV21.Subject = acqFifoTool;
            _fileName = fileName;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                CogSerializer.SaveObjectToFile(cogAcqFifoEditV21.Subject,
                    _fileName + ".vpp");
            }
            catch (Exception ex)
            {
                MessageBox.Show("保存失败：" + ex.Message);
            }
        }

        private void btn_保存不带图像的工具_Click(object sender, EventArgs e)
        {
            try
            {
                CogSerializer.SaveObjectToFile(cogAcqFifoEditV21.Subject,
                    _fileName + ".vpp", typeof(BinaryFormatter),
                    CogSerializationOptionsConstants.Minimum);
            }
            catch (Exception ex)
            {
                MessageBox.Show("保存失败：" + ex.Message);
            }
        }
    }
}
