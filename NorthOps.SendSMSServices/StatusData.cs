using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NorthOps.Services.SmsService.Monitoring
{
    [Serializable]
    public class StatusData : ResponseData
    {
        private Uri url;

        public StatusData(DateTime acquisitionTime, Uri url, XElement response) : base(acquisitionTime, response)
        {
            this.url = url;
        }

        public int ConnectionStatus => (int)GetValue(nameof(ConnectionStatus));
        public int CurrentWifiUser => (int)GetValue(nameof(CurrentWifiUser));
        public IPAddress PrimaryDns => (IPAddress)GetValue(nameof(PrimaryDns));
        public IPAddress PrimaryIPv6Dns => (IPAddress)GetValue(nameof(PrimaryIPv6Dns));
        public IPAddress SecondaryDns => (IPAddress)GetValue(nameof(SecondaryDns));
        public IPAddress SecondaryIPv6Dns => (IPAddress)GetValue(nameof(SecondaryIPv6Dns));
        public int SignalIcon => (int)GetValue(nameof(SignalIcon));
        public int TotalWifiUser => (int)GetValue(nameof(TotalWifiUser));
#pragma warning disable IDE1006 // 命名スタイル
        public string msisdn => (string)GetValue(nameof(msisdn));
#pragma warning restore IDE1006 // 命名スタイル

        public Uri GetSignalIcon() => new Uri(url, $"/images/level_{SignalIcon}.png");
    }
    [Serializable]
    public class Statistics3DaysData : ResponseData
    {
        public Statistics3DaysData(DateTime acquisitionTime, XElement response) : base(acquisitionTime, response) { }

        public bool IsYestodayFluxOverLimit => (bool)GetValue(nameof(IsYestodayFluxOverLimit));
        public DateTime LastClearTime3days => (DateTime)GetValue(nameof(LastClearTime3days));
        public long ToTodayDownload => (long)GetValue(nameof(ToTodayDownload));
        public long ToTodayDuration => (long)GetValue(nameof(ToTodayDuration));
        public long ToTodayTotal => ToTodayDownload + ToTodayUpload;
        public long ToTodayUpload => (long)GetValue(nameof(ToTodayUpload));
        public long ToYestodayDownload => (long)GetValue(nameof(ToYestodayDownload));
        public long ToYestodayDuration => (long)GetValue(nameof(ToYestodayDuration));
        public long ToYestodayTotal => ToYestodayDownload + ToYestodayUpload;
        public long ToYestodayUpload => (long)GetValue(nameof(ToYestodayUpload));
    }
}
