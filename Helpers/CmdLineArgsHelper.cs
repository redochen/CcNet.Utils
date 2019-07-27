using System.Collections.Generic;
using System.Linq;
using System.Text;
using CcNet.Utils.Extensions;

namespace CcNet.Utils.Helpers
{
    /// <summary>
    /// 命令行参数类
    /// </summary>
    public class CmdLineArg
    {
        private readonly int Index = 0;
        private string ArgumentText = string.Empty;
        private List<CmdLineArg> Arguments = null;

        /// <summary>
        /// 后一个参数
        /// </summary>
        public CmdLineArg Next
            => (Index < Arguments.Count - 1) ? Arguments[Index + 1] : null;

        /// <summary>
        /// 前一个参数
        /// </summary>
        public CmdLineArg Prev
            => (Index > 0) ? Arguments[Index - 1] : null;

        /// <summary>
        /// 内部构造函数
        /// </summary>
        /// <param name="args"></param>
        /// <param name="index"></param>
        /// <param name="argument"></param>
        internal CmdLineArg(List<CmdLineArg> args, int index, string argument)
        {
            Arguments = args;
            Index = index;
            ArgumentText = argument;
        }

        /// <summary>
        /// 获取后一个参数
        /// </summary>
        /// <returns></returns>
        public CmdLineArg Take() => Next;

        /// <summary>
        /// 获取后续N个参数
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public IEnumerable<CmdLineArg> Take(int count)
        {
            var list = new List<CmdLineArg>();

            var parent = this;
            for (int i = 0; i < count; i++)
            {
                var next = parent.Next;
                if (next == null)
                {
                    break;
                }

                list.Add(next);

                parent = next;
            }

            return list;
        }

        /// <summary>
        /// 转换成参数字符串（有空格时自动添加双引号）
        /// </summary>
        public string ToParamString()
        {
            if (!ArgumentText.Contains(Chars.空格))
            {
                return ArgumentText;
            }

            var sbParam = new StringBuilder();

            if (!ArgumentText.StartsWith(Chars.双引号.ToString()))
            {
                sbParam.Append(Chars.双引号);
            }

            sbParam.Append(ArgumentText);

            if (!ArgumentText.EndsWith(Chars.双引号.ToString()))
            {
                sbParam.Append(Chars.双引号);
            }

            return sbParam.ToString();
        }

        /// <summary>
        /// 转换自参数字符串（有双引号时自动移除）
        /// </summary>
        /// <returns></returns>
        public string FromParamString()
            => ArgumentText.Trim(Chars.双引号);

        /// <summary>
        /// 强制类型转换
        /// </summary>
        /// <param name="argument"></param>
        public static implicit operator string(CmdLineArg argument)
        {
            return argument.ArgumentText;
        }

        public override string ToString()
        {
            return ArgumentText;
        }
    }

    /// <summary>
    /// 命令行参数解析类
    /// </summary>
    public class CmdLineArgParser
    {
        private List<CmdLineArg> Arguments = null;

        /// <summary>
        /// 静态解析方法
        /// </summary>
        /// <param name="args">参数数组</param>
        /// <returns></returns>
        public static CmdLineArgParser Parse(string[] args)
        {
            return new CmdLineArgParser(args);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="args"></param>
        public CmdLineArgParser(string[] args)
        {
            Arguments = new List<CmdLineArg>();

            for (int i = 0; i < args.Length; i++)
            {
                Arguments.Add(new CmdLineArg(Arguments, i, args[i]));
            }
        }

        /// <summary>
        /// 获取参数值
        /// </summary>
        /// <param name="argumentName">参数名</param>
        /// <returns></returns>
        public CmdLineArg GetValue(string argumentName)
            => GetKey(argumentName)?.Next;

        /// <summary>
        /// 获取参数键
        /// </summary>
        /// <param name="argumentName">参数名</param>
        /// <returns></returns>
        public CmdLineArg GetKey(string argumentName)
            => Arguments.FirstOrDefault(p => p == argumentName);

        /// <summary>
        /// 是否存在参数
        /// </summary>
        /// <param name="argumentName">参数名</param>
        /// <returns></returns>
        public bool Exists(string argumentName)
            => Arguments.Exists(p => p == argumentName);

        /// <summary>
        /// 删除参数
        /// </summary>
        /// <param name="argumentName">参数名</param>
        /// <param name="removeValue">是否同时删除参数值</param>
        /// <returns></returns>
        public bool Remove(string argumentName, bool removeValue = false)
        {
            if (!Exists(argumentName))
            {
                return false;
            }

            var key = GetKey(argumentName);
            var value = removeValue ? GetValue(argumentName) : null;

            if (key != null)
            {
                Arguments.Remove(key);
            }

            if (value != null)
            {
                Arguments.Remove(value);
            }

            return true;
        }

        public override string ToString()
        {
            if (Arguments.IsEmpty())
            {
                return string.Empty;
            }

            return string.Join(Chars.空格.ToString(),
                Arguments.Select(x => x.ToParamString()));
        }
    }
}