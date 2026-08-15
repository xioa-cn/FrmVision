using System;
using System.IO;
using System.Runtime.Serialization;
using FrmCommon.ConfigUtils;

namespace FrmServices.ViewModel
{
    [DataContract]
    public sealed class EditorDataFile
    {
        [DataMember(Order = 1)]
        public int Version { get; set; }

        [DataMember(Order = 2)]
        public string CanvasData { get; set; }
    }

    public class EditorViewModel
    {
        private const int CurrentDataVersion = 1;
        private static readonly string EditorDataFilePathValue = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Editor",
            "editor_data.json");

        public string EditorDataFilePath => EditorDataFilePathValue;

        public void SaveEditorData(byte[] canvasData)
        {
            if (canvasData == null)
                throw new ArgumentNullException(nameof(canvasData));

            new EditorDataFile
            {
                Version = CurrentDataVersion,
                CanvasData = Convert.ToBase64String(canvasData)
            }.WriteJson(EditorDataFilePathValue);
        }

        public byte[] LoadEditorData()
        {
            EditorDataFile data = EditorDataFilePathValue.ReadJson<EditorDataFile>();
            if (data == null) return null;
            if (data.Version != CurrentDataVersion)
                throw new InvalidDataException(
                    "不支持的流程编辑器数据版本：" + data.Version + "。");
            if (string.IsNullOrWhiteSpace(data.CanvasData))
                throw new InvalidDataException("流程编辑器数据为空。");

            try
            {
                return Convert.FromBase64String(data.CanvasData);
            }
            catch (FormatException ex)
            {
                throw new InvalidDataException("流程编辑器画布数据格式无效。", ex);
            }
        }
    }
}
