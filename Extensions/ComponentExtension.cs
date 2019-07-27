using System;
using System.Collections.Concurrent;
using System.Linq;
using System.ComponentModel;
using System.Windows.Forms;
using CcNet.Utils.Helpers;

namespace CcNet.Utils.Extensions
{
    /// <summary>
    /// Component扩展类
    /// </summary>
    public static class ComponentExtension
    {
        /// <summary>
        /// 允许快捷键操作
        /// </summary>
        /// <param name="component"></param>
        /// <param name="selectAll">全选方法</param>
        /// <param name="copyData">复制方法</param>
        /// <param name="pastData">粘贴方法</param>
        public static void AllowShortcutsOperation(this Component component,
            Action selectAll = null, Func<(bool/*handled*/, object/*data*/)> copyData = null, Action<object> pastData = null)
        {
            if (null == component)
            {
                return;
            }

            var evtKeyUp = component.GetEventInfo("KeyUp")?.GetAddMethod();
            if (null == evtKeyUp)
            {
                return;
            }

            evtKeyUp.Invoke(component, new[] { new KeyEventHandler(OnKeyUp) });

            void OnKeyUp(object sender, KeyEventArgs e)
            {
                //Ctrl按键未按下
                if (!e.Control || e.Shift || e.Alt)
                {
                    return;
                }

                switch (e.KeyCode)
                {
                    case Keys.A: //全选
                        selectAll?.Invoke();
                        break;
                    case Keys.C: //复制
                        DoCopy();
                        break;
                    case Keys.V: //粘贴
                        pastData?.Invoke(Clipboard.GetData(DataFormats.Text));
                        break;
                    case Keys.Z: //撤销
                    default:
                        break;
                }

                #region 复制
                void DoCopy()
                {
                    if (null == copyData)
                    {
                        return;
                    }

                    var (handled, data) = copyData();
                    if (!handled)
                    {
                        return;
                    }

                    Clipboard.Clear();

                    if (data != null)
                    {
                        Clipboard.SetData(DataFormats.Text, data);
                    }
                }
                #endregion
            }
        }

        /// <summary>
        /// 判断是否可以绑定事件
        /// </summary>
        /// <param name="component"></param>
        /// <param name="eventName">事件名称</param>
        /// <param name="autoBind">是否自动绑定</param>
        /// <returns></returns>
        public static bool CanBindEvent(this Component component, string eventName, bool autoBind = true)
        {
            if (null == component || !eventName.IsValid())
            {
                return false;
            }

            BoundEvents.TryGetValue(component, out ConcurrentBag<string> events);
            if (events != null && events.Contains(eventName))
            {
                return false;
            }

            if (autoBind)
            {
                if (null == events)
                {
                    events = new ConcurrentBag<string>();
                }

                events.Add(eventName);

                BoundEvents.AddOrUpdate(component, events, (k, v) => { v = events; return v; });
            }

            return true;
        }

        /// <summary>
        /// 已绑定过的事件（防止重入）
        /// </summary>
        private static ConcurrentDictionary<Component, ConcurrentBag<string>> BoundEvents
            = new ConcurrentDictionary<Component, ConcurrentBag<string>>();
    }
}