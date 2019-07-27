namespace CcNet.Utils.Controls
{
    /// <summary>
    /// Control Tag上下文类
    /// </summary>
    public class TagContext
    {
        /// <summary>
        /// 自定义数据
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public TagContext() { }

        /// <summary>
        /// 拷贝构造函数
        /// </summary>
        /// <param name="ctx"></param>
        public TagContext(TagContext ctx)
        {
            if (ctx != null)
            {
                Data = ctx.Data;
            }
        }
    }
}