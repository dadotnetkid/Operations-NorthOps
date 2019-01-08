using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace NorthOps.Services.SmsService
{
    public class ResponseData : DynamicObject
    {
        private readonly XNode node;

        public ResponseData(DateTime acquisitionTime, XNode node)
        {
            this.node = node ?? throw new ArgumentNullException(nameof(node));
            AcquisitionTime = acquisitionTime;
        }

        /// <summary>
        /// 情報取得日時を取得します。
        /// </summary>
        public DateTime AcquisitionTime { get; }
        public bool IsResponse => node is XElement xe && xe.Name == "response";

        private static T OperatorClass<T>(ResponseData data, Func<string, T> parser) where T : class
        {
            if (data is null)
                return null;
            else
            {
                var str = (string)data;
                if (str == "")
                    return null;
                else
                    return parser(str);
            }
        }
        private static T? OperatorStruct<T>(ResponseData data, Func<string, T> parser) where T : struct
        {
            if (data is null)
                return null;
            else
            {
                var str = (string)data;
                if (str == "")
                    return null;
                else
                    return parser(str);
            }
        }

        public bool Contains(XName key) => node is XElement xe && xe.Element(key) != null;

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            if (node is XElement xe)
                return from item in xe.Elements() select item.Name.ToString();
            else
                return base.GetDynamicMemberNames();
        }

        public override string ToString() => node.ToString();

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (node is XElement xe)
            {
                var ch = xe.Element(binder.Name);
                result = ch != null ? new ResponseData(AcquisitionTime, ch) : null;
                return ch != null;
            }
            else
            {
                result = null;
                return false;
            }
        }

        protected dynamic GetValue(string key)
        {
            if (node is XElement xe)
                return new ResponseData(AcquisitionTime, xe.Element(key) ?? throw new KeyNotFoundException());
            else
                throw new InvalidOperationException();
        }

        public static explicit operator bool? (ResponseData data)
        {
            if (data is null)
                return null;
            else
            {
                var str = (string)data;
                if (str == "")
                    return null;
                if (str.Equals("true", StringComparison.OrdinalIgnoreCase))
                    return true;
                else if (str.Equals("false", StringComparison.OrdinalIgnoreCase))
                    return false;
                else if (long.TryParse(str, out var lval))
                    return lval != 0;
                else
                    throw new InvalidCastException();
            }
        }
        public static explicit operator int? (ResponseData data) => OperatorStruct(data, int.Parse);
        public static explicit operator long? (ResponseData data) => OperatorStruct(data, long.Parse);
        public static explicit operator string(ResponseData data)
        {
            if (data is null)
                return null;
            else if (data.node is XText xt)
                return xt.Value;
            else if (data.node is XElement xe)
                return xe.Value;
            else
                throw new InvalidCastException();
        }
        public static explicit operator DateTime? (ResponseData data) => OperatorStruct(data, DateTime.Parse);
        public static explicit operator IPAddress(ResponseData data) => OperatorClass(data, IPAddress.Parse);
        public static explicit operator XElement(ResponseData data)
        {
            if (data is null)
                return null;
            else if (data.node is XElement xe)
                return new XElement(xe);
            else
                throw new InvalidCastException();
        }
    }
}