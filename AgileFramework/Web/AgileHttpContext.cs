using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace AgileFramework.Web
{
    /*
     * core中请使用AppContext.StartUpTime和Directory获取路径和时间
     * 
    /// <summary>
    /// Http上下文
    /// </summary>
    public static class AgileHttpContext
    {
        /// <summary>
        /// 启动时间
        /// </summary>
        public readonly static DateTime StartupTime = DateTime.Now;

        /// <summary>
        /// Bin目录（有斜线结尾）
        /// </summary>
        public static readonly string BinDirectory = Path.GetDirectoryName(HttpRuntime.BinDirectory).TrimEnd('\\') + "\\";

        /// <summary>
        /// 项目所在目录（有斜线结尾）
        /// </summary>
        public static readonly string Directory = Path.GetDirectoryName(HttpRuntime.AppDomainAppPath).TrimEnd('\\') + "\\";
    }
    *
    *
    */



    /*
     * 废弃，直接使用mvc/web中的ContextAccessor
     * 
    /// <summary>
    /// Http上下文，不建议使用
    /// </summary>
    internal static class HttpContext
    {
        private static IHttpContextAccessor _accessor;

        public static Microsoft.AspNetCore.Http.HttpContext Current = _accessor.HttpContext;

        internal static void Configure(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }
    }

    /// <summary>
    /// Http拓展
    /// </summary>
    public static class AgileHttpContextExtensions
    {
        /// <summary>
        /// 未注入HttpContextAccessor时需在ConfigureServices中调用services.AddHttpContextAccessor();
        /// </summary>
        /// <param name="services"></param>
        public static void AddHttpContextAccessor(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        /// <summary>
        /// Configure中需调用app.UseStaticHttpContext();
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseStaticHttpContext(this IApplicationBuilder app)
        {
            var httpContextAccessor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();
            HttpContext.Configure(httpContextAccessor);
            return app;
        }
    }

    *
    */
}
