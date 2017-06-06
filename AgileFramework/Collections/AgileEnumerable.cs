using System.Collections.Generic;
using System.Linq;

namespace AgileFramework.Collections
{
    /// <summary>
    /// 可数序列帮助类
    /// </summary>
    public static class AgileEnumerable
    {
        /// <summary>
        /// 根据指定长度将序列分组
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="input">输入</param>
        /// <param name="count">每组数量</param>
        /// <returns>序列组</returns>
        public static List<IEnumerable<T>> Split<T>(IEnumerable<T> input, int count)
        {
            var result = new List<IEnumerable<T>>();

            for (var i = 0; i < input.Count() / count + 1; i++)
            {
                result.Add(input.Skip(i * count).Take(count));
            }
            return result;
        }
    }
}
