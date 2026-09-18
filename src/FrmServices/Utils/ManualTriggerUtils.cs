using System.Collections.Generic;

namespace FrmServices.Utils;

// 作者：xioa
// 作者邮箱：1327916255@qq.com
public class ManualTriggerUtils
{
    private static Dictionary<string,bool> _triggers = new Dictionary<string, bool>();

    public static bool GetTrigger(string triggerName)
    {
        if (_triggers.TryGetValue(key: triggerName, out var trigger))
        {
            return trigger;
        }

        return false;
    }

    public static void SetTrigger(string triggerName, bool value)
    {
        _triggers[triggerName] = value;
    }
}