using System;
using System.IO;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;

namespace FrmCommon.ConfigUtils
{
    public static class HslAuthExtensios
    {
        private static readonly object SyncRoot = new object();
        private static readonly byte[] AdditionalEntropy = Encoding.UTF8.GetBytes(
            "FrmVision.HslAuthorization.v1");

        public static string AuthorizationFilePath { get; } = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "FrmVision", "hsl-authorization.json");

        public static bool IsAuthorized { get; private set; }
        public static string LastError { get; private set; } = string.Empty;

        public static bool HasSavedAuthorizationCode
        {
            get
            {
                lock (SyncRoot)
                {
                    try
                    {
                        HslAuthorizationData data =
                            AuthorizationFilePath.ReadJson<HslAuthorizationData>();
                        return !string.IsNullOrWhiteSpace(data?.ProtectedCode);
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
        }

        public static bool SetAuthTool()
        {
            lock (SyncRoot)
            {
                try
                {
                    HslAuthorizationData data =
                        AuthorizationFilePath.ReadJson<HslAuthorizationData>();
                    if (string.IsNullOrWhiteSpace(data?.ProtectedCode))
                    {
                        IsAuthorized = false;
                        LastError = "尚未配置通讯秘钥。";
                        return false;
                    }

                    string authorizationCode = Unprotect(data.ProtectedCode);
                    IsAuthorized =
                        HslCommunication.Authorization.SetAuthorizationCode(
                            authorizationCode);
                    LastError = IsAuthorized ? string.Empty : "通讯秘钥无效。";
                    return IsAuthorized;
                }
                catch (Exception ex)
                {
                    IsAuthorized = false;
                    LastError = "通讯秘钥解密或应用失败：" +
                                ex.GetBaseException().Message;
                    return false;
                }
            }
        }

        public static bool SaveAuthorizationCode(string authorizationCode)
        {
            string normalizedCode = (authorizationCode ?? string.Empty).Trim();
            if (normalizedCode.Length == 0)
                throw new ArgumentException("通讯秘钥不能为空。",
                    nameof(authorizationCode));

            lock (SyncRoot)
            {
                IsAuthorized =
                    HslCommunication.Authorization.SetAuthorizationCode(
                        normalizedCode);
                if (!IsAuthorized)
                {
                    LastError = "通讯秘钥无效。";
                    return false;
                }

                var data = new HslAuthorizationData
                {
                    ProtectedCode = Protect(normalizedCode)
                };
                data.WriteJson(AuthorizationFilePath);
                return SetAuthTool();
            }
        }

        private static string Protect(string value)
        {
            byte[] plainBytes = Encoding.UTF8.GetBytes(value);
            byte[] protectedBytes = ProtectedData.Protect(plainBytes,
                AdditionalEntropy, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(protectedBytes);
        }

        private static string Unprotect(string value)
        {
            byte[] protectedBytes = Convert.FromBase64String(value);
            byte[] plainBytes = ProtectedData.Unprotect(protectedBytes,
                AdditionalEntropy, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(plainBytes);
        }

        [DataContract]
        private sealed class HslAuthorizationData
        {
            [DataMember(Order = 1)]
            public string ProtectedCode { get; set; }
        }
    }
}
