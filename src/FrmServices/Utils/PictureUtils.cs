using System;
using System.Collections.ObjectModel;
using System.Linq;
using Cognex.VisionPro;
using FrmServices.ViewModel;

namespace FrmServices.Utils;

public class PictureUtils
{
    private static ObservableCollection<CameraPanelViewModel> _allCamera;

    public static void SetAllCamera(ObservableCollection<CameraPanelViewModel> camera)
    {
        _allCamera = camera;
    }

    public static CameraPanelViewModel GetAllCamera(string camerakey)
    {
        if (_allCamera == null || string.IsNullOrWhiteSpace(camerakey)) return null;
        string key = camerakey.Trim();
        return _allCamera.FirstOrDefault(camera =>
            string.Equals(camera.Name, key, StringComparison.OrdinalIgnoreCase));
    }

    public static string GetCurrentParameterName(string productOrCameraKey)
    {
        if (_allCamera == null || string.IsNullOrWhiteSpace(productOrCameraKey))
            return string.Empty;

        string key = productOrCameraKey.Trim();
        CameraPanelViewModel camera = _allCamera.FirstOrDefault(item =>
            string.Equals(item.ProductName, key,
                StringComparison.OrdinalIgnoreCase));
        if (camera == null)
        {
            camera = _allCamera.FirstOrDefault(item =>
                string.Equals(item.Name, key,
                    StringComparison.OrdinalIgnoreCase));
        }

        return camera == null ? string.Empty : camera.ParameterName ?? string.Empty;
    }

    public static void SetCamera(CameraPanelViewModel camera, ICogRecord cogRecord)
    {
        if (camera == null) throw new ArgumentNullException(nameof(camera));
        if (cogRecord == null) throw new ArgumentNullException(nameof(cogRecord));
        var image = FindImage(cogRecord);
        if (image == null)
            throw new InvalidOperationException("视觉记录及其子记录中都没有视觉图片。");

        var displayRecord = CogSerializer.DeepCopyObject(cogRecord) as ICogRecord;
        if (displayRecord == null)
            throw new InvalidOperationException("无法创建用于界面显示的视觉记录快照。");

        var display = camera.CogRecordDisplay;
        if (display == null)
            throw new InvalidOperationException("相机页面尚未创建图片显示控件。");
        if (display.IsDisposed || display.Disposing)
            throw new ObjectDisposedException("CogRecordDisplay");
        if (!display.IsHandleCreated)
            throw new InvalidOperationException("图片显示控件尚未加载完成。");

        if (display.InvokeRequired)
        {
            display.Invoke(new Action(() => DisplayRecord(display, displayRecord)));
            return;
        }

        DisplayRecord(display, displayRecord);
    }

    private static void DisplayRecord(CogRecordDisplay display, ICogRecord cogRecord)
    {
        display.Record = null;
        display.Image = null;
        display.Record = cogRecord;
        if (display.Image == null)
            display.Image = FindImage(cogRecord);
        display.Fit(true);
        display.Invalidate(true);
        display.Update();
    }

    private static ICogImage FindImage(ICogRecord record)
    {
        if (record == null) return null;
        var image = record.Content as ICogImage;
        if (image != null) return image;

        var subRecords = record.SubRecords;
        if (subRecords == null) return null;
        for (int index = 0; index < subRecords.Count; index++)
        {
            image = FindImage(subRecords[index]);
            if (image != null) return image;
        }

        return null;
    }
}
