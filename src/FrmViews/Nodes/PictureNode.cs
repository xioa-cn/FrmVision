using System;
using Cognex.VisionPro;
using FrmServices.Services.EditorServices;
using FrmServices.Utils;
using ST.Library.UI.NodeEditor;

namespace FrmViews.Nodes
{
    public sealed class PictureNodeExecutionResult
    {
        public bool IsSuccess { get; set; }
        public string CameraKey { get; set; }
        public int RecordIndex { get; set; }
        public ICogRecords Records { get; set; }
        public ICogRecord Record { get; set; }
        public string Message { get; set; }
    }

    [STNode("视觉图像", "xioa", null, null,
        "接收视觉子记录集合，并按索引显示到指定相机页面。")]
    public class PictureNode : WorkflowNode, IEditorExecutableNode
    {
        public PictureNode()
        {
            SetNodeTypeTitle("图片显示");
            TitleColor = System.Drawing.Color.FromArgb(220, 44, 108, 176);
            LetGetOptions = true;
            InputRecord = InputOptions.Add("视觉图片", typeof(ICogRecords), true);
            InputRecord.DataTransfer += InputRecordOnDataTransfer;
        }

        public STNodeOption InputRecord { get; }

        [STNodeProperty("相机 Key", "需要显示视觉图片的页面相机名称。")]
        public string CameraKey { get; set; } = string.Empty;

        [STNodeProperty("记录索引", "需要显示的视觉子记录索引，从 0 开始。")]
        public int RecordIndex { get; set; }

        public PictureNodeExecutionResult LastExecutionResult { get; private set; }

        public PictureNodeExecutionResult Execute()
        {
            return Execute(InputRecord.Data as ICogRecords);
        }

        public PictureNodeExecutionResult Execute(ICogRecords records)
        {
            ICogRecord selectedRecord = null;
            try
            {
                if (records == null)
                    throw new InvalidOperationException(
                        "图片显示节点的输入记录集合不能为空。");
                if (records.Count == 0)
                    throw new InvalidOperationException(
                        "图片显示节点的输入记录集合为空。");
                if (RecordIndex < 0 || RecordIndex >= records.Count)
                    throw new InvalidOperationException("记录索引 " + RecordIndex +
                        " 超出范围，当前记录数量为 " + records.Count + "。");
                if (string.IsNullOrWhiteSpace(CameraKey))
                    throw new InvalidOperationException("图片显示节点的相机 Key 不能为空。");

                selectedRecord = records[RecordIndex];
                if (selectedRecord == null)
                    throw new InvalidOperationException("索引 " + RecordIndex +
                        " 对应的视觉记录为空。");

                var camera = PictureUtils.GetAllCamera(CameraKey);
                if (camera == null)
                    throw new InvalidOperationException(
                        "未找到相机页面：" + CameraKey + "。");

                PictureUtils.SetCamera(camera, selectedRecord);
                LastExecutionResult = new PictureNodeExecutionResult
                {
                    IsSuccess = true,
                    CameraKey = CameraKey,
                    RecordIndex = RecordIndex,
                    Records = records,
                    Record = selectedRecord,
                    Message = "已显示索引 " + RecordIndex + " 的视觉图片。"
                };
            }
            catch (Exception ex)
            {
                LastExecutionResult = new PictureNodeExecutionResult
                {
                    IsSuccess = false,
                    CameraKey = CameraKey,
                    RecordIndex = RecordIndex,
                    Records = records,
                    Record = selectedRecord,
                    Message = ex.GetBaseException().Message
                };
            }

            return LastExecutionResult;
        }

        public EditorNodeExecutionResult Execute(EditorExecutionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            context.CancellationToken.ThrowIfCancellationRequested();
            PictureNodeExecutionResult result = Execute();
            return result.IsSuccess
                ? EditorNodeExecutionResult.Success(result.Message)
                : EditorNodeExecutionResult.Failure(result.Message);
        }

        private void InputRecordOnDataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status != ConnectionStatus.Connected || e.TargetOption == null)
            {
                InputRecord.Data = null;
                return;
            }

            InputRecord.Data = e.TargetOption.Data;
        }
    }
}
