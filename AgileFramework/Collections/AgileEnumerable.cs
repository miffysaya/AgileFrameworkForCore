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

            if (input.Count() > 0)
            {
                var maxCount = (input.Count() - 1) / count + 1;

                for (var i = 0; i < maxCount; i++)
                {
                    result.Add(input.Skip(i * count).Take(count));
                }
            }
            return result;
        }

        /// <summary>
        /// 获得层级树型结构，根节点为虚拟节点
        /// </summary>
        /// <typeparam name="T">树节点类泛型，需要继承树型基类SysTree</typeparam>
        /// <param name="input">输入节点集</param>
        /// <param name="rootID">根节点ID</param>
        /// <returns>层级树型结构</returns>
        public static List<T> GetTree<T>(List<T> input, string rootID = "0") where T : SysTree
        {
            var result = new List<T>();

            var clone = new Dictionary<string, List<T>>();

            input.ForEach(one =>
            {
                if (!clone.ContainsKey(one.ParentID))
                {
                    clone.Add(one.ParentID, new List<T>());
                }
                clone[one.ParentID].Add(one);
            });
            result.AddRange(GetTree(clone, rootID));

            return result;
        }

        /// <summary>
        /// 获得层级树型结构，包括真实根节点
        /// </summary>
        /// <typeparam name="T">树节点类泛型，需要继承树型基类SysTree</typeparam>
        /// <param name="input">输入节点集</param>
        /// <param name="root">根节点</param>
        /// <returns>层级树型结构</returns>
        public static T GetTree<T>(List<T> input, T root) where T : SysTree
        {
            var clone = new Dictionary<string, List<T>>();

            input.ForEach(one =>
            {
                if (!clone.ContainsKey(one.ParentID))
                {
                    clone.Add(one.ParentID, new List<T>());
                }
                clone[one.ParentID].Add(one);
            });
            root.Children.AddRange(GetTree(clone, root.ID));

            return root;
        }

        /// <summary>
        /// 生成层级树型结构的基础方法
        /// </summary>
        /// <typeparam name="T">树节点类泛型，需要继承树型基类SysTree</typeparam>
        /// <param name="input">输入节点集</param>
        /// <param name="rootID">根节点ID</param>
        /// <returns>层级树型结构</returns>
        private static List<T> GetTree<T>(Dictionary<string, List<T>> input, string rootID) where T : SysTree
        {
            var result = new List<T>();

            if (!input.ContainsKey(rootID))
            {
                return result;
            }

            result.AddRange(input[rootID]);

            foreach (var one in result)
            {
                one.Children.Clear();

                one.Children.AddRange(GetTree(input, one.ID));
            }
            return result; ;
        }
    }
    /// <summary>
    /// 树型基类
    /// </summary>
    public class SysTree
    {
        /// <summary>
        /// 节点ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 父节点ID
        /// </summary>
        public string ParentID { get; set; }

        /// <summary>
        /// 子节点集
        /// </summary>
        public List<SysTree> Children { get; set; }

        /// <summary>
        /// 重写GetHashCode方法，以ID为主键
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        /// <summary>
        /// 重写Equals方法，以ID为主键
        /// </summary>
        /// <param name="obj">比较对象</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var other = obj as SysTree;

            return ID == other.ID;
        }
    }
}
