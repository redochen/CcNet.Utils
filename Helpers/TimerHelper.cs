using System;
using System.Windows.Forms;

namespace CcNet.Utils.Helpers
{
    /// <summary>
    /// 计时器帮助类
    /// </summary>
    public class TimerHelper
    {
        private Timer _Timer = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="interval">间隔时间，单位：毫秒</param>
        /// <param name="onTick">触发事件回调方法</param>
        /// <param name="enabled">是否立即启用</param>
        /// <param name="tag">自定义数据</param>
        public TimerHelper(int interval, Action onTick, bool enabled, object tag = null)
        {
            _Timer = new Timer
            {
                Interval = enabled ? 1 : interval,
                Tag = tag,
                Enabled = enabled
            };

            _Timer.Tick += (sender, e) =>
            {
                _Timer.Interval = interval;
                onTick();
            };
        }

        /// <summary>
        /// 获取自定义数据
        /// </summary>
        public object Tag => _Timer?.Tag;

        /// <summary>
        /// 获取计时器的当前状态
        /// </summary>
        public bool Enabled => _Timer?.Enabled ?? false;

        /// <summary>
        /// 启动计时器
        /// </summary>
        public void Start() => _Timer?.Start();

        /// <summary>
        /// 停止计时器
        /// </summary>
        public void Stop() => _Timer?.Stop();
    }
}