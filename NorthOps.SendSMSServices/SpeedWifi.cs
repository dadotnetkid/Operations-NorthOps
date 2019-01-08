using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NorthOps.Services.SmsService
{
    public class SpeedWiFi : IDisposable
    {
        private const int TokenLength = 32;

        private static readonly HttpClientHandler handler = new HttpClientHandler()
        {
            UseCookies = true,
            UseProxy = false
        };

        private readonly HttpClient client;
        private Task<RSA> encPublickey;
        private readonly Task init;
        private string msisdn;

        public SpeedWiFi(string address) : this(new Uri(address)) { }
        public SpeedWiFi(Uri address) : this()
        {
            Address = address ?? throw new ArgumentNullException(nameof(address));
            client = new HttpClient(handler, false)
            {
                BaseAddress = address,
                Timeout = TimeSpan.FromSeconds(5000)
            };

            init = RenewSessionAsync();
        }
        private SpeedWiFi()
        {
            encPublickey = new Task<RSA>(() =>
            {
                var res = GetXmlAsync("/api/webserver/publickey", default).Result;
                var prm = new RSAParameters()
                {
                    Exponent = FromHex(res.Value.Element("encpubkeye")?.Value ?? throw new FormatException()),
                    Modulus = FromHex(res.Value.Element("encpubkeyn")?.Value ?? throw new FormatException())
                };

                var rsa = RSA.Create();
                rsa.ImportParameters(prm);
                return rsa;
            });

            Information = new HybridAsyncData<Device.InformationData>(GetInformationAsync);
            Statistics3Days = new HybridAsyncData<Monitoring.Statistics3DaysData>(GetStatistics3DaysAsync);
            Status = new HybridAsyncData<Monitoring.StatusData>(GetStatusAsync);
        }

        public Uri Address { get; }
        public HybridAsyncData<Device.InformationData> Information { get; }
        /// <summary>
        /// MSISDN（電話番号）を取得します。
        /// </summary>
        public string Msisdn
        {
            get => msisdn;
            private set
            {
                if (msisdn == value)
                    return;
                var oldValue = msisdn;
                msisdn = value;
                try
                {
                    MsisdnChanged?.Invoke(this, new MsisdnChangedEventArgs(oldValue, value));
                }
                catch { }
            }
        }
        public Cookie Session => handler.CookieContainer.GetCookies(Address)["SessionID"];
        public HybridAsyncData<Monitoring.Statistics3DaysData> Statistics3Days { get; }
        public HybridAsyncData<Monitoring.StatusData> Status { get; }

        public event Action<SpeedWiFi, MsisdnChangedEventArgs> MsisdnChanged;

        private static byte[] FromHex(string hex)
        {
            var buf = new List<byte>();

            if (hex.Length % 2 != 0)
                throw new FormatException();
            for (int i = 0; i < hex.Length; i += 2)
            {
                buf.Add(byte.Parse(hex.Substring(i, 2), NumberStyles.HexNumber));
            }
            return buf.ToArray();
        }

        private static bool IsResponse(XElement xe) => xe.Name == "response";

        private static string ToHex(IReadOnlyList<byte> dat)
        {
            var sb = new StringBuilder();
            foreach (var item in dat)
            {
                var s = item.ToString("x");
                if (s.Length == 1)
                    s = "0" + s;
                sb.Append(s);
            }
            return sb.ToString();
        }

        public virtual void Dispose()
        {
            client.Dispose();
        }

        public async Task<dynamic> GetDynamicAsync(string requestUri, CancellationToken cancellationToken = default)
        {
            var res = await GetXmlAsync(requestUri, cancellationToken);
            return new ResponseData(res.Time, res.Value);
        }

        public async Task<RSAParameters> GetPublicKeyAsync() => (await encPublickey).ExportParameters(false);

        /// <summary>
        /// 予め機器と接続を確立します。
        /// </summary>
        /// <returns></returns>
        public async Task InitAsync()
        {
            if (init.Status == TaskStatus.Created)
            {
                lock (init)
                {
                    if (init.Status == TaskStatus.Created)
                        init.Start();
                }
            }

            await init;
        }

        /// <summary>
        /// 機器にログインしているかどうかを取得します。
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> IsLoginedAsync(CancellationToken cancellationToken = default)
        {
            var res = await GetDynamicAsync("/api/user/state-login", cancellationToken);
            return res.IsResponse && (int)res.State != -1;
        }

        public async Task<bool> LoginAsync(string username, string password, CancellationToken cancellationToken = default)
        {
            //var encKey = await GetPublicKey();
            var utf8 = Encoding.UTF8;

            string token, passkey;
            using (var sha = SHA256.Create())
            {
                var passhash = ToHex(sha.ComputeHash(utf8.GetBytes(password)));
                var passhash64 = Convert.ToBase64String(utf8.GetBytes(passhash));
                token = await RefreshTokenAsync(cancellationToken);
                passkey = ToHex(sha.ComputeHash(utf8.GetBytes(username + passhash64 + token)));
                passkey = Convert.ToBase64String(utf8.GetBytes(passkey));
            }

            var xml = $"<?xml version=\"1.0\" encoding=\"UTF-8\"?><request><Username>{username}</Username><Password>{passkey}</Password><password_type>4</password_type></request>";
            //var xml64 = Convert.ToBase64String(utf8.GetBytes(xml));
            //var len245 = xml64.Length / 245D;
            //var rsatotal = new StringBuilder();
            //for (int i = 0; i < len245; i++)
            //{
            //    string substr;
            //    if (i == (int)len245)
            //        substr = xml64.Substring(i * 245);
            //    else
            //        substr = xml64.Substring(i * 245, 245);

            //    var enc = ToHex(encKey.Encrypt(utf8.GetBytes(substr), RSAEncryptionPadding.Pkcs1)); // 問題あり
            //    rsatotal.Append(enc);
            //}

            var content = new StringContent(xml, Encoding.UTF8, "text/html");
            //content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded; charset=UTF-8;enc");
            using (var res = await PostAsync("api/user/login", content, token, cancellationToken))
            {
                var xe = await ReadXmlAsync(res);
                return xe.Name == "response" && xe.Value == "OK";
            }
        }

        public async Task<bool> LogoutAsync(CancellationToken cancellationToken = default)
        {
            var content = new StringContent("<?xml version=\"1.0\" encoding=\"UTF-8\"?><request><Logout>1</Logout></request>", Encoding.UTF8, "text/html");
            using (var res = await PostAsync("/api/user/logout", content, await RefreshTokenAsync(cancellationToken), cancellationToken))
            {
                var xe = await ReadXmlAsync(res);
                return xe.Name == "response" && xe.Value == "OK";
            }
        }

        public async Task<bool> RebootAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var content = new StringContent("<?xml version=\"1.0\" encoding=\"UTF-8\"?><request><Control>1</Control></request>", Encoding.UTF8, "text/html");
                using (var res = await PostAsync("/api/device/control", content, await RefreshTokenAsync(cancellationToken), cancellationToken))
                {
                    if (IsResponse(await ReadXmlAsync(res)))
                    {
                        Msisdn = null;
                        return true;
                    }
                    return false;
                }
            }
            catch (AggregateException)
            {
                return false;
            }
        }

        public void SetSessionId(string id)
        {
            handler.CookieContainer.SetCookies(new Uri(client.BaseAddress, "/api/user/authentication_login"), $"SessionID={id}; path=/; HttpOnly;");
        }

        public async Task SetSleepTimeAsync(int time, CancellationToken cancellationToken = default)
        {
            var content = new StringContent($"<?xml version:\"1.0\" encoding=\"UTF-8\"?><request><sleeptime>{time}</sleeptime></request>");
            using (var res = await PostAsync("/api/device/sleep-time", content, await RefreshTokenAsync(cancellationToken), cancellationToken))
            {
                if (!IsResponse(await ReadXmlAsync(res)))
                    throw new ApplicationException("設定変更に失敗しました");
            }
        }

        private async Task<Device.InformationData> GetInformationAsync()
        {
            var res = await GetXmlAsync("/api/device/information", default);
            return new Device.InformationData(res.Time, res.Value);
        }

        private async Task<RSA> GetPublicKey()
        {
            if (encPublickey.IsCompleted)
                return encPublickey.Result;
            lock (encPublickey)
            {
                if (encPublickey.IsCompleted)
                    return encPublickey.Result;
                encPublickey.Start();
            }
            return await encPublickey;
        }

        private async Task<Monitoring.Statistics3DaysData> GetStatistics3DaysAsync()
        {
            var res = await GetXmlAsync("/api/monitoring/statistics_3days", default);
            return new Monitoring.Statistics3DaysData(res.Time, res.Value);
        }

        private async Task<Monitoring.StatusData> GetStatusAsync()
        {
            var res = await GetXmlAsync("/api/monitoring/status", default);
            var dat = new Monitoring.StatusData(res.Time, client.BaseAddress, res.Value);
            if (dat.msisdn != Msisdn)
                Msisdn = dat.msisdn;
            return dat;
        }

        private async Task<(DateTime Time, XElement Value)> GetXmlAsync(string requestUri, CancellationToken cancellationToken)
        {
            int renewCnt = 0;
            Retry:
            var res = await SendAsync(HttpMethod.Get, requestUri, null, null, cancellationToken);
            try
            {
                var xe = await ReadXmlAsync(res.Response);
                if (xe.Name == "error")
                {
                    var code = xe.Element("code")?.Value;
                    switch (code)
                    {
                        case "100003":
                            throw new UnauthorizedAccessException();
                        case "125002":
                            if (++renewCnt <= 1) // セッションに関するエラー
                            {
                                await RenewSessionAsync();
                                goto Retry;
                            }
                            break;
                    }

                    var message = xe.Element("message")?.Value;
                    if (string.IsNullOrEmpty(message))
                        message = "情報の取得に失敗しました。";

                    throw new ApplicationException($"{message}\nエラーコード：{code}");
                }
                return (res.Time, xe);
            }
            finally
            {
                res.Response.Dispose();
            }
        }

        private async Task RenewSessionAsync()
        {
            try
            {
                (await client.GetAsync("/", HttpCompletionOption.ResponseHeadersRead)).Dispose();
            }
            catch
            {
                Msisdn = null;
                throw;
            }
        }

        private async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content, string token, CancellationToken cancellationToken)
        {
            return (await SendAsync(HttpMethod.Post, requestUri, content, token, cancellationToken)).Response;
        }

        private async Task<XElement> ReadXmlAsync(HttpResponseMessage responseMessage)
        {
            using (var stream = await responseMessage.Content.ReadAsStreamAsync())
            {
                return XElement.Load(stream);
            }
        }

        private async Task<string> RefreshTokenAsync(CancellationToken cancellationToken = default)
        {
            var res = await GetXmlAsync("/api/webserver/token", cancellationToken);
            return res.Value.Element("token")?.Value?.Substring(32) ?? throw new FormatException();
        }

        private async Task<(DateTime Time, HttpResponseMessage Response)> SendAsync(HttpMethod method, string requestUri, HttpContent content, string token, CancellationToken cancellationToken)
        {
            await InitAsync();

            int retryCnt = 0;
            Retry:
            try
            {
                bool reconnected = false;
                Reconnect:
                using (var req = new HttpRequestMessage(method, requestUri))
                {
                    req.Content = content;
                    if (token != null)
                        req.Headers.Add("__RequestVerificationToken", token);

                    var tsk = client.SendAsync(req, cancellationToken);
                    var acq = tsk.ContinueWith(t => DateTime.Now);
                    var delay = Task.Delay(10000); // 不具合対策　タイムアウト用
                    if (await Task.WhenAny(tsk, delay) == delay)
                    {
                        if (reconnected)
                            throw new InvalidProgramException(nameof(HttpClient) + "の不具合により、通信できません");
                        reconnected = true;
                        var _ = tsk.ContinueWith(t =>
                        {
                            if (t.Status == TaskStatus.RanToCompletion)
                                t.Result.Dispose();
                        });
                        goto Reconnect;
                    }

                    return (await acq, await tsk);
                }
            }
            catch (TaskCanceledException)
            {
                Debug.WriteLine($"通信タイムアウト発生：{retryCnt + 1}回目");
                if (++retryCnt > 3)
                    throw;
                goto Retry;
            }
        }
    }
}