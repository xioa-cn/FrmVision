using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using Cognex.VisionPro;
using Cognex.VisionPro.ImageFile;
using Cognex.VisionPro.ToolBlock;
using FrmVpComponents.Utils;

namespace FrmVpComponents
{
    public partial class frmToolBlock : Form
    {
        private string _fileName = string.Empty;

        private string _imagePath;

        private int _selectedImageIndex = 0;

        private List<FileInfo> ImageFilePathList = new List<FileInfo>();

        private List<FileInfo> ImageFilePathList2 = new List<FileInfo>();

        private bool 选择单张图像;

        private bool 取消连续运行;


        public frmToolBlock()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ToolBlock工具界面
        /// </summary>
        /// <param name="toolBlock">需要打开的ToolBlock</param>
        /// <param name="fileName">保存在本地时的文件名，不需要加后缀</param>
        public frmToolBlock(CogToolBlock toolBlock, string fileName) : this()
        {
            cogToolBlockEditV21.Subject = toolBlock;
            _fileName = fileName;
            labfileName.Text = fileName;
            toolTip1.SetToolTip(btnSave, "将ToolBlock以vpp的格式保存到Debug目录下的视觉工具文件夹");
            toolTip1.SetToolTip(btnSave, "将ToolBlock以vpp的格式保存到Debug目录下的视觉工具文件夹");
            toolTip1.SetToolTip(btnOpenImage, "选择一张图片，之后点击运行按钮，会运行该图片");
            toolTip1.SetToolTip(btnOpenImageFile, "选择一个文件夹，点击运行按钮，会顺序运行文件夹内的图片");
            toolTip1.SetToolTip(btnRunNextImage, "运行当前图片");
            toolTip1.SetToolTip(labfileName, "当前的ToolBlock的文件名");
            _imagePath = ReadWriteIni.ReadValue("系统参数", "图片路径");
            panel3.Visible = false;
            txt_图片1输入名称.Text = ReadWriteIni.ReadValue(base.Name, "图片1输入名称");
            txt_图片2输入名称.Text = ReadWriteIni.ReadValue(base.Name, "图片2输入名称");
            txt_文件夹1路径.Text = ReadWriteIni.ReadValue(base.Name, "文件夹路径");
            txt_文件夹2路径.Text = ReadWriteIni.ReadValue(base.Name, "文件夹路径2");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                CogSerializer.SaveObjectToFile(cogToolBlockEditV21.Subject, _fileName + ".vpp");
            }
            catch (Exception ex)
            {
                MessageBox.Show("保存失败：" + ex.Message);
            }
        }

        private void btnOpenImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "请选择要读取的图像文件";
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                ImageFilePathList.Clear();
                ImageFilePathList.Add(new FileInfo(openFileDialog.FileName));
                lab_当前图片路径.Text = openFileDialog.FileName;
                labCurrentImageIndex.Text = "/1";
                txtCurrentImageIndex.Text = "1";
                _selectedImageIndex = 0;
            }
        }

        private void btnOpenImage2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "请选择要读取的图像文件";
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                ImageFilePathList2.Clear();
                ImageFilePathList2.Add(new FileInfo(openFileDialog.FileName));
                选择单张图像 = true;
            }
        }

        private void btnOpenImageFile_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "请选择图片所在文件夹";
            folderBrowserDialog.SelectedPath = "D:";
            try
            {
                folderBrowserDialog.SelectedPath = ReadWriteIni.ReadValue(base.Name, "文件夹路径");
            }
            catch (Exception)
            {
            }

            if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            if (string.IsNullOrEmpty(folderBrowserDialog.SelectedPath))
            {
                MessageBox.Show(this, "文件夹路径不能为空", "提示");
                return;
            }

            ReadWriteIni.Write(base.Name, "文件夹路径", folderBrowserDialog.SelectedPath);
            DirectoryInfo directoryInfo = new DirectoryInfo(folderBrowserDialog.SelectedPath);
            FileInfo[] files = directoryInfo.GetFiles("*.*", SearchOption.TopDirectoryOnly);
            ImageFilePathList.Clear();
            FileInfo[] array = files;
            foreach (FileInfo fileInfo in array)
            {
                string text = fileInfo.Name.ToLower();
                if (text.EndsWith(".bmp") || text.EndsWith(".tif") || text.EndsWith(".jpg") || text.EndsWith(".png") ||
                    text.EndsWith(".cdb") || text.EndsWith(".idb"))
                {
                    ImageFilePathList.Add(new FileInfo(fileInfo.FullName));
                }
            }

            if (ImageFilePathList.Count > 0)
            {
                labCurrentImageIndex.Text = "/" + ImageFilePathList.Count;
                txtCurrentImageIndex.Text = "1";
            }
            else
            {
                labCurrentImageIndex.Text = "/0";
                txtCurrentImageIndex.Text = "0";
            }

            _selectedImageIndex = 0;
        }

        private void btnOpenImageFile1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "请选择图片所在文件夹";
            folderBrowserDialog.SelectedPath = "D:";
            try
            {
                folderBrowserDialog.SelectedPath = ReadWriteIni.ReadValue(base.Name, "文件夹路径");
            }
            catch (Exception)
            {
            }

            if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            if (string.IsNullOrEmpty(folderBrowserDialog.SelectedPath))
            {
                MessageBox.Show(this, "文件夹路径不能为空", "提示");
                return;
            }

            ReadWriteIni.Write(base.Name, "文件夹路径", folderBrowserDialog.SelectedPath);
            txt_文件夹1路径.Text = folderBrowserDialog.SelectedPath;
            DirectoryInfo directoryInfo = new DirectoryInfo(folderBrowserDialog.SelectedPath);
            FileInfo[] files = directoryInfo.GetFiles("*.*", SearchOption.TopDirectoryOnly);
            ImageFilePathList.Clear();
            FileInfo[] array = files;
            foreach (FileInfo fileInfo in array)
            {
                string text = fileInfo.Name.ToLower();
                if (text.EndsWith(".bmp") || text.EndsWith(".tif") || text.EndsWith(".jpg") || text.EndsWith(".png") ||
                    text.EndsWith(".cdb") || text.EndsWith(".idb"))
                {
                    ImageFilePathList.Add(new FileInfo(fileInfo.FullName));
                }
            }

            if (ImageFilePathList.Count > 0)
            {
                labCurrentImageIndex.Text = "/" + ImageFilePathList.Count;
                txtCurrentImageIndex.Text = "1";
            }
            else
            {
                labCurrentImageIndex.Text = "/0";
                txtCurrentImageIndex.Text = "0";
            }

            _selectedImageIndex = 0;
        }

        private void btnOpenImageFile2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "请选择图片所在文件夹";
            folderBrowserDialog.SelectedPath = "D:";
            try
            {
                folderBrowserDialog.SelectedPath = ReadWriteIni.ReadValue(base.Name, "文件夹路径");
            }
            catch (Exception)
            {
            }

            if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            if (string.IsNullOrEmpty(folderBrowserDialog.SelectedPath))
            {
                MessageBox.Show(this, "文件夹路径不能为空", "提示");
                return;
            }

            ReadWriteIni.Write(base.Name, "文件夹路径", folderBrowserDialog.SelectedPath);
            txt_文件夹2路径.Text = folderBrowserDialog.SelectedPath;
            DirectoryInfo directoryInfo = new DirectoryInfo(folderBrowserDialog.SelectedPath);
            FileInfo[] files = directoryInfo.GetFiles("*.*", SearchOption.TopDirectoryOnly);
            ImageFilePathList2.Clear();
            FileInfo[] array = files;
            foreach (FileInfo fileInfo in array)
            {
                string text = fileInfo.Name.ToLower();
                if (text.EndsWith(".bmp") || text.EndsWith(".tif") || text.EndsWith(".jpg") || text.EndsWith(".png") ||
                    text.EndsWith(".cdb") || text.EndsWith(".idb"))
                {
                    ImageFilePathList2.Add(new FileInfo(fileInfo.FullName));
                }
            }
        }

        private void btnRunNextImage_Click(object sender, EventArgs e)
        {
            if (ImageFilePathList.Count == 0)
            {
                MessageBox.Show("当前没有选中的图像");
                return;
            }

            string text = "";
            try
            {
                if (chb_启用双图像.Checked)
                {
                    if (选择单张图像)
                    {
                        text = ImageFilePathList2[0].FullName;
                    }
                    else if (rdB_文件名完全一致.Checked)
                    {
                        foreach (FileInfo item in ImageFilePathList2)
                        {
                            if (item.Name == ImageFilePathList[_selectedImageIndex].Name)
                            {
                                text = item.FullName;
                                break;
                            }
                        }

                        if (text == "")
                        {
                            MessageBox.Show("在文件夹2中，按照指定条件查找,未找到符合条件的文件");
                            _selectedImageIndex++;
                            labCurrentImageIndex.Text = "/" + ImageFilePathList.Count;
                            txtCurrentImageIndex.Text = _selectedImageIndex.ToString();
                            if (_selectedImageIndex >= ImageFilePathList.Count)
                            {
                                _selectedImageIndex = 0;
                            }

                            return;
                        }
                    }
                    else if (rdB_创建时间完全一致.Checked)
                    {
                        foreach (FileInfo item2 in ImageFilePathList2)
                        {
                            if (item2.CreationTime == ImageFilePathList[_selectedImageIndex].CreationTime)
                            {
                                text = item2.FullName;
                                break;
                            }
                        }

                        if (text == "")
                        {
                            MessageBox.Show("在文件夹2中，按照指定条件查找,未找到符合条件的文件");
                            _selectedImageIndex++;
                            labCurrentImageIndex.Text = "/" + ImageFilePathList.Count;
                            txtCurrentImageIndex.Text = _selectedImageIndex.ToString();
                            if (_selectedImageIndex >= ImageFilePathList.Count)
                            {
                                _selectedImageIndex = 0;
                            }

                            return;
                        }
                    }
                    else if (rdB_文件2秒可以比文件1晚1S.Checked)
                    {
                        int num = (int)nUD_允许时间差秒数.Value;
                        DateTime value = ImageFilePathList[_selectedImageIndex].CreationTime.AddSeconds(num);
                        List<FileInfo> list = new List<FileInfo>();
                        foreach (FileInfo item3 in ImageFilePathList2)
                        {
                            if (item3.CreationTime.CompareTo(value) <= 0)
                            {
                                list.Add(item3);
                            }
                        }

                        if (list.Count == 0)
                        {
                            MessageBox.Show("在文件夹2中，按照指定条件查找,未找到符合条件的文件");
                            _selectedImageIndex++;
                            labCurrentImageIndex.Text = "/" + ImageFilePathList.Count;
                            txtCurrentImageIndex.Text = _selectedImageIndex.ToString();
                            if (_selectedImageIndex >= ImageFilePathList.Count)
                            {
                                _selectedImageIndex = 0;
                            }

                            return;
                        }

                        FileInfo fileInfo = list[0];
                        foreach (FileInfo item4 in list)
                        {
                            if (fileInfo.CreationTime.CompareTo(item4) > 0)
                            {
                                fileInfo = item4;
                            }
                        }

                        text = fileInfo.FullName;
                    }

                    CogImageFile cogImageFile = new CogImageFile();
                    cogImageFile.Open(ImageFilePathList[_selectedImageIndex].FullName, CogImageFileModeConstants.Read);
                    lab_当前图片路径.Text = ImageFilePathList[_selectedImageIndex].FullName;
                    cogToolBlockEditV21.Subject.Inputs[txt_图片1输入名称.Text].Value = cogImageFile[0];
                    CogImageFile cogImageFile2 = new CogImageFile();
                    cogImageFile2.Open(text, CogImageFileModeConstants.Read);
                    cogToolBlockEditV21.Subject.Inputs[txt_图片2输入名称.Text].Value = cogImageFile2[0];
                    cogToolBlockEditV21.Subject.Run();
                    _selectedImageIndex++;
                    labCurrentImageIndex.Text = "/" + ImageFilePathList.Count;
                    txtCurrentImageIndex.Text = _selectedImageIndex.ToString();
                    if (_selectedImageIndex >= ImageFilePathList.Count)
                    {
                        _selectedImageIndex = 0;
                    }
                }
                else
                {
                    CogImageFile cogImageFile3 = new CogImageFile();
                    cogImageFile3.Open(ImageFilePathList[_selectedImageIndex].FullName, CogImageFileModeConstants.Read);
                    lab_当前图片路径.Text = ImageFilePathList[_selectedImageIndex].FullName;
                    cogToolBlockEditV21.Subject.Inputs["InputImage"].Value = cogImageFile3[0];
                    cogToolBlockEditV21.Subject.Run();
                    _selectedImageIndex++;
                    labCurrentImageIndex.Text = "/" + ImageFilePathList.Count;
                    txtCurrentImageIndex.Text = _selectedImageIndex.ToString();
                    if (_selectedImageIndex >= ImageFilePathList.Count)
                    {
                        _selectedImageIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtCurrentImageIndex_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\r')
            {
                return;
            }

            try
            {
                _selectedImageIndex = int.Parse(txtCurrentImageIndex.Text);
                labCurrentImageIndex.Text = "/" + ImageFilePathList.Count;
                txtCurrentImageIndex.Text = _selectedImageIndex.ToString();
                if (_selectedImageIndex >= ImageFilePathList.Count)
                {
                    _selectedImageIndex = 0;
                }
            }
            catch (Exception)
            {
                txtCurrentImageIndex.Text = "1";
                _selectedImageIndex = 1;
            }
        }

        private void btn_Save2_Click(object sender, EventArgs e)
        {
            try
            {
                CogSerializer.SaveObjectToFile(cogToolBlockEditV21.Subject, _fileName + ".vpp", typeof(BinaryFormatter),
                    CogSerializationOptionsConstants.Minimum);
            }
            catch (Exception ex)
            {
                MessageBox.Show("保存失败：" + ex.Message);
            }
        }

        private void btn_连续运行_Click(object sender, EventArgs e)
        {
            if (ImageFilePathList.Count == 0)
            {
                MessageBox.Show("当前没有选中的图像");
                return;
            }

            CogImageFile cogImageFile = new CogImageFile();
            while (_selectedImageIndex <= ImageFilePathList.Count)
            {
                if (取消连续运行)
                {
                    取消连续运行 = false;
                    break;
                }

                try
                {
                    cogImageFile.Open(ImageFilePathList[_selectedImageIndex].FullName, CogImageFileModeConstants.Read);
                    Application.DoEvents();
                    lab_当前图片路径.Text = ImageFilePathList[_selectedImageIndex].FullName;
                    Application.DoEvents();
                    cogToolBlockEditV21.Subject.Inputs["InputImage"].Value = cogImageFile[0];
                    Application.DoEvents();
                    cogToolBlockEditV21.Subject.Run();
                    Application.DoEvents();
                    try
                    {
                        if (cogToolBlockEditV21.Subject.Outputs[txt_Result.Text].Value.ToString() != txt_比较内容.Text)
                        {
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        break;
                    }

                    Application.DoEvents();
                    _selectedImageIndex++;
                    labCurrentImageIndex.Text = "/" + ImageFilePathList.Count;
                    Application.DoEvents();
                    txtCurrentImageIndex.Text = _selectedImageIndex.ToString();
                    Application.DoEvents();
                    if (_selectedImageIndex >= ImageFilePathList.Count)
                    {
                        _selectedImageIndex = 0;
                        break;
                    }
                }
                catch (Exception ex2)
                {
                    MessageBox.Show(ex2.Message);
                    break;
                }
            }
        }

        private void btn_取消连续运行_Click(object sender, EventArgs e)
        {
            取消连续运行 = true;
        }

        private void btn_拷贝当前图片_Click(object sender, EventArgs e)
        {
            try
            {
                if (ImageFilePathList.Count == 0)
                {
                    MessageBox.Show("当前没有选中的图像");
                    return;
                }

                if (!Directory.Exists("D:"))
                {
                    Directory.CreateDirectory("D:");
                }

                FileInfo fileInfo = new FileInfo(ImageFilePathList[_selectedImageIndex].FullName);
                File.Copy(fileInfo.FullName, "D:\\" + fileInfo.Name);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void label2_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(lab_当前图片路径.Text);
                Process.Start("explorer.exe", fileInfo.DirectoryName);
            }
            catch (Exception)
            {
            }
        }

        private void label3_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(labfileName.Text);
                Process.Start("explorer.exe", fileInfo.DirectoryName);
            }
            catch (Exception)
            {
            }
        }

        private void btn_屏蔽主程序传递图片_Click(object sender, EventArgs e)
        {
            CogToolBlock toolBlock = (CogToolBlock)CogSerializer.DeepCopyObject(cogToolBlockEditV21.Subject);
            frmToolBlock frmToolBlock2 = new frmToolBlock(toolBlock, labfileName.Text);
            MessageBox.Show("屏蔽后,工具的改动将不会同步到主程序中，故保存后需重启主程序才能使改动生效。");
            frmToolBlock2.Show();
        }

        private void cogToolBlockEditV21_Load(object sender, EventArgs e)
        {
        }

        private void btn_双图像_Click(object sender, EventArgs e)
        {
            if (!cogToolBlockEditV21.Visible)
            {
                panel3.Visible = false;
                cogToolBlockEditV21.Visible = true;
            }
            else
            {
                panel3.Visible = true;
                cogToolBlockEditV21.Visible = false;
            }
        }

        private void txt_图片1输入名称_TextChanged(object sender, EventArgs e)
        {
            ReadWriteIni.Write(base.Name, "图片1输入名称", txt_图片1输入名称.Text);
        }

        private void txt_图片2输入名称_TextChanged(object sender, EventArgs e)
        {
            ReadWriteIni.Write(base.Name, "图片2输入名称", txt_图片2输入名称.Text);
        }

        private void txt_文件夹1路径_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\r')
            {
                return;
            }

            try
            {
                ReadWriteIni.Write(base.Name, "文件夹路径", txt_文件夹1路径.Text);
                DirectoryInfo directoryInfo = new DirectoryInfo(txt_文件夹1路径.Text);
                FileInfo[] files = directoryInfo.GetFiles("*.*", SearchOption.TopDirectoryOnly);
                ImageFilePathList.Clear();
                FileInfo[] array = files;
                foreach (FileInfo fileInfo in array)
                {
                    string text = fileInfo.Name.ToLower();
                    if (text.EndsWith(".bmp") || text.EndsWith(".tif") || text.EndsWith(".jpg") ||
                        text.EndsWith(".png") || text.EndsWith(".cdb") || text.EndsWith(".idb"))
                    {
                        ImageFilePathList.Add(new FileInfo(fileInfo.FullName));
                    }
                }

                if (ImageFilePathList.Count > 0)
                {
                    labCurrentImageIndex.Text = "/" + ImageFilePathList.Count;
                    txtCurrentImageIndex.Text = "1";
                }
                else
                {
                    labCurrentImageIndex.Text = "/0";
                    txtCurrentImageIndex.Text = "0";
                }

                _selectedImageIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txt_文件夹2路径_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\r')
            {
                return;
            }

            try
            {
                ReadWriteIni.Write(base.Name, "文件夹路径2", txt_文件夹2路径.Text);
                DirectoryInfo directoryInfo = new DirectoryInfo(txt_文件夹2路径.Text);
                FileInfo[] files = directoryInfo.GetFiles("*.*", SearchOption.TopDirectoryOnly);
                ImageFilePathList2.Clear();
                FileInfo[] array = files;
                foreach (FileInfo fileInfo in array)
                {
                    string text = fileInfo.Name.ToLower();
                    if (text.EndsWith(".bmp") || text.EndsWith(".tif") || text.EndsWith(".jpg") ||
                        text.EndsWith(".png") || text.EndsWith(".cdb") || text.EndsWith(".idb"))
                    {
                        ImageFilePathList2.Add(new FileInfo(fileInfo.FullName));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
