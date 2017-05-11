using AgileFramework.Text;
using AgileFramework.Web;
using Newtonsoft.Json;
using System.Xml;

namespace AgileFramework.Business
{
    /// <summary>
    /// 对PDA接口进行操作
    /// </summary>
    public static class AgilePDASoap
    {
        /// <summary>
        /// 通过GET方式访问接口
        /// </summary>
        /// <param name="address">接口地址</param>
        /// <param name="methodName">接口方法</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public static AgilePDAEntity Get(string address, AgilePDAMethod methodName, string parameters = "")
        {
            var url = address.TrimEnd('/') + "/" + methodName.ToString() + (string.IsNullOrWhiteSpace(parameters) ? "" : "?" + parameters);

            var html = AgileRequest.GetHtml(url);

            return ParseEntity(html);
        }

        /// <summary>
        /// 通过POST方式访问接口
        /// </summary>
        /// <param name="address">接口地址</param>
        /// <param name="methodName">接口方法</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public static AgilePDAEntity Post(string address, AgilePDAMethod methodName, string parameters = "")
        {
            var url = address.TrimEnd('/') + "/" + methodName.ToString();

            var html = AgileRequest.GetHtml(url, AgileEncoding.Default, 1000, AgileHttpActionType.Post, parameters);

            return ParseEntity(html);
        }

        /// <summary>
        /// 将XML解析为实体类的方法
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private static AgilePDAEntity ParseEntity(string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                return null;
            }
            var document = new XmlDocument();

            document.LoadXml(html);

            var jsonText = JsonConvert.SerializeXmlNode(document);

            var dynamicObject = JsonConvert.DeserializeObject<dynamic>(jsonText);

            if (dynamicObject.Feedback == null)
            {
                return null;
            }
            return new AgilePDAEntity()
            {
                Status = dynamicObject.Feedback.Status,
                Error = dynamicObject.Feedback.Error,
                Result = dynamicObject.Feedback.Result
            };
        }
    }

    /// <summary>
    /// PDA接口返回结果实体类
    /// </summary>
    public class AgilePDAEntity
    {
        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// 结果
        /// </summary>
        public string Result { get; set; }
    }

    /// <summary>
    /// PDA接口方法枚举
    /// </summary>
    public enum AgilePDAMethod
    {
        GetList,

        GetOrderStatus,

        GrabList,

        GrabToBind,

        Login,

        LoginAMS,

        UpdateDT
    }
}
