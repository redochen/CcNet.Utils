using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using CcNet.Utils.Controls;

namespace CcNet.Utils.Extensions
{
    /// <summary>
    /// Control扩展类
    /// </summary>
    public static class ControlExtension
    {
        /// <summary>
        /// 改变控件大小
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="width">宽度改变量</param>
        /// <param name="height">高度改变量</param>
        public static void ResizeCtrl(this Control ctrl, int width, int height)
        {
            if (null == ctrl)
            {
                return;
            }

            if (width == 0 && height == 0)
            {
                return;
            }

            ctrl.Width = ctrl.Width + width;
            ctrl.Height = ctrl.Height + height;
        }

        /// <summary>
        /// 移动控件
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="x">水平方向位移量</param>
        /// <param name="y">垂直方向位移量</param>
        public static void MoveCtrl(this Control ctrl, int x, int y)
        {
            if (null == ctrl)
            {
                return;
            }

            if (x == 0 && y == 0)
            {
                return;
            }

            var point = new Point
            {
                X = ctrl.Location.X + x,
                Y = ctrl.Location.Y + y
            };

            ctrl.Location = point;
        }

        /// <summary>
        /// 获取所有控件
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="includeSelf">是否包含自身</param>
        /// <param name="includeSub">是否包含子控件的子控件</param>
        /// <param name="includeDisposed">是否包含已释放的控件</param>
        /// <param name="includeNoneTopLevelForm">是否包含非顶级窗体</param>
        /// <param name="predicate">过滤方法</param>
        public static List<Control> GetControls(this Control ctrl, bool includeSelf = true,
            bool includeSub = false, bool includeDisposed = true, bool includeNoneTopLevelForm = true,
            Func<Control /*ctrl*/, bool /*isTarget*/> predicate = null)
        {
            List<Control> controls = null;
            ctrl.GetAllControls(includeSelf, true, includeSub, includeDisposed,
                includeNoneTopLevelForm, ref controls, predicate);
            return controls;
        }


        /// <summary>
        /// 获取所有控件（包括控件自身）
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="includeSelf">是否包含自身</param>
        /// <param name="includeChildren">是否包含子控件</param>
        /// <param name="includeDescends">是否包含子控件的子控件</param>
        /// <param name="includeDisposed">是否包含已释放的控件</param>
        /// <param name="controls">获取到的控件列表</param>
        /// <param name="includeNoneTopLevelForm">是否包含非顶级窗体</param>
        /// <param name="predicate">过滤方法</param>
        private static void GetAllControls(this Control ctrl, bool includeSelf, bool includeChildren,
            bool includeDescends, bool includeDisposed, bool includeNoneTopLevelForm,
            ref List<Control> controls, Func<Control /*ctrl*/, bool /*isTarget*/> predicate = null)
        {
            if (null == ctrl)
            {
                return;
            }

            if (!includeDisposed && ctrl.IsDisposed)
            {
                return;
            }

            //排除非顶级窗体
            if (!includeNoneTopLevelForm && (ctrl is Form f) && !f.TopLevel)
            {
                return;
            }

            if (null == controls)
            {
                controls = new List<Control>();
            }

            if (includeSelf && (predicate?.Invoke(ctrl) ?? true))
            {
                controls.Add(ctrl);
            }

            if (!includeChildren || ctrl.Controls.Count <= 0)
            {
                return;
            }

            foreach (Control c in ctrl.Controls)
            {
                c.GetAllControls(true, includeDescends, includeDescends,
                    includeDisposed, includeNoneTopLevelForm, ref controls, predicate);
            }

            //controls = controls.Distinct().ToList();
        }

        /// <summary>
        /// 创建TCtrl的新实例并替换控件集合中的特定控件
        /// </summary>
        /// <typeparam name="TCtrl"></typeparam>
        /// <param name="controls"></param>
        /// <param name="target">要替换掉的目标控件</param>
        /// <param name="args">构造函数参数</param>
        /// <returns></returns>
        public static TCtrl ReplaceControl<TCtrl>(this Control.ControlCollection controls, Control target, params object[] args)
            where TCtrl : Control
        {
            try
            {
                //创建新的实例
                var newCtrl = Activator.CreateInstance(typeof(TCtrl), args) as TCtrl;
                newCtrl.Dock = target.Dock;
                newCtrl.Location = target.Location;

                //替换掉旧的控件
                controls.ReplaceControl(target, newCtrl);

                //删除并释放旧的控件资源
                target.Hide();
                //target.Dispose();

                return newCtrl;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 替换控件集合中的特定控件
        /// </summary>
        /// <param name="controls"></param>
        /// <param name="target">要替换掉的目标控件</param>
        /// <param name="replace">要替换目标控件的源控件</param>
        public static void ReplaceControl(this Control.ControlCollection controls, Control target, Control replace)
        {
            if (null == controls || controls.Count <= 0 || null == target || null == replace)
            {
                return;
            }

            List<Control> allCtrls = new List<Control>();

            //从后往前删除控件
            for (int i = controls.Count - 1; i >= 0; --i)
            {
                var ctrl = controls[i];
                if (ctrl == target)
                {
                    allCtrls.Add(replace);
                }
                else
                {
                    allCtrls.Add(ctrl);
                }

                controls.RemoveAt(i);
            }

            //从后往前添加控件
            for (int i = allCtrls.Count - 1; i >= 0; --i)
            {
                controls.Add(allCtrls[i]);
            }
        }

        /// <summary>
        /// 获取父窗体
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="retRootMost">是否返回最根级的窗体（否则返回查找过程中的第一个窗体）</param>
        /// <returns></returns>
        public static Form GetParentForm(this Control ctrl, bool retRootMost = false)
        {
            List<Form> foundForms = null;
            return ctrl.GetParentForm(ref foundForms, retRootMost);
        }

        /// <summary>
        /// 获取父窗体
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="foundForms">已找到的窗体列表</param>
        /// <param name="retRootMost">是否返回最根级的窗体（否则返回查找过程中的第一个窗体）</param>
        /// <returns></returns>
        public static Form GetParentForm(this Control ctrl, ref List<Form> foundForms, bool retRootMost = false)
        {
            if (null == ctrl)
            {
                return null;
            }

            if (ctrl.Parent is Form form)
            {
                if (null == foundForms)
                {
                    foundForms = new List<Form>();
                }

                if (!foundForms.Contains(form))
                {
                    foundForms.Add(form);
                }

                if (!retRootMost)
                {
                    return form;
                }
            }

            return ctrl.Parent.GetParentForm(ref foundForms, retRootMost);
        }

        /// <summary>
        /// 获取所有上级控件
        /// </summary>
        /// <param name="ctrl"></param>
        /// <returns></returns>
        public static List<Control> GetParentControls(this Control ctrl)
        {
            var ctrls = new List<Control>();

            if (null == ctrl || null == ctrl.Parent)
            {
                return ctrls;
            }

            ctrls.Add(ctrl.Parent);

            var parentCtrls = ctrl.Parent.GetParentControls();
            if (!parentCtrls.IsEmpty())
            {
                ctrls.AddRange(parentCtrls);
            }

            return ctrls;
        }

        /// <summary>
        /// 获取控件路径
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="separator">路径分隔符</param>
        /// <returns></returns>
        public static string GetControlPath(this Control ctrl, string separator = ".")
        {
            if (null == ctrl)
            {
                return string.Empty;
            }

            var sbPath = new StringBuilder();

            var parentCtrls = ctrl.GetParentControls();
            parentCtrls.Reverse();
            parentCtrls.ForEach(c =>
            {
                if (sbPath.Length > 0)
                {
                    sbPath.Append(separator);
                }
                sbPath.Append(c.Name.GetValue($"[{c.GetType().Name}]"));
            });

            if (sbPath.Length > 0)
            {
                sbPath.Append(separator);
            }
            sbPath.Append(ctrl.Name.GetValue($"[{ctrl.GetType().Name}]"));

            return sbPath.ToString();
        }

        /// <summary>
        /// 添加菜单项
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="text">菜单文字</param>
        /// <param name="onClick">点击事件处理器</param>
        /// <param name="tag">自定义数据</param>
        /// <param name="enabled"></param>
        /// <param name="name">菜单名字</param>
        public static void AddContextMenuItem(this Control ctrl, string text, EventHandler onClick,
            string name = null, object tag = null, Image image = null, bool enabled = true)
        {
            if (null == ctrl)
            {
                return;
            }

            if (null == ctrl.ContextMenuStrip)
            {
                ctrl.ContextMenuStrip = new ContextMenuStrip();
                ctrl.ContextMenuStrip.Closed += (sender, e) => { ctrl.ContextMenuStrip = null; };
            }

            var item = new ToolStripMenuItem(text)
            {
                Name = name.GetValue(),
                Tag = tag,
                Enabled = enabled,
                Image = image,
            };

            item.Click += onClick;

            ctrl.ContextMenuStrip.Items.Add(item);
        }

        /// <summary>
        /// 添加菜单分隔栏
        /// </summary>
        /// <param name="ctrl"></param>
        public static void AddContextMenuSeparator(this Control ctrl)
        {
            if (null == ctrl)
            {
                return;
            }

            if (null == ctrl.ContextMenuStrip)
            {
                ctrl.ContextMenuStrip = new ContextMenuStrip();
            }

            var item = new ToolStripSeparator();

            ctrl.ContextMenuStrip.Items.Add(item);
        }

        /// <summary>
        /// 使用Component的Tag上下文
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="ctrl"></param>
        /// <param name="useAction">使用动作（为空时返回当前Tag上下文）</param>
        /// <param name="readonlyUse">只读使用（使用后不更新）</param>
        /// <returns></returns>
        public static TContext UseTagContext<TContext>(this Control ctrl, Action<TContext> useAction = null, bool readonlyUse = false)
            where TContext : TagContext, new()
        {
            if (null == ctrl)
            {
                return null;
            }

            var tagCtx = (ctrl.Tag as TContext) ?? new TContext();

            if (null == useAction || readonlyUse)
            {
                tagCtx = (TContext)Activator.CreateInstance(typeof(TContext), tagCtx);
            }

            if (useAction != null)
            {
                useAction(tagCtx);

                if (!readonlyUse)
                {
                    ctrl.Tag = tagCtx;
                }
            }

            return tagCtx;
        }

        /// <summary>
        /// 清除一个对象的某个事件所挂钩的delegate
        /// </summary>
        /// <param name="ctrl">控件对象</param>
        /// <param name="eventName">事件名称，默认的</param>
        private static void ClearEvents(this Control ctrl, string eventName = "_EventAll")
        {
            if (null == ctrl)
            {
                return;
            }

            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Static;
            EventInfo[] events = ctrl.GetType().GetEvents(bindingFlags);
            if (events == null || events.Length < 1) return;

            for (int i = 0; i < events.Length; i++)
            {
                try
                {
                    EventInfo ei = events[i];
                    //只删除指定的方法，默认是_EventAll，前面加_是为了和系统的区分，防以后雷同
                    if (eventName != "_EventAll" && ei.Name != eventName) continue;

                    /********************************************************
                     * class的每个event都对应了一个同名(变了，前面加了Event前缀)的private的delegate类
                     * 型成员变量（这点可以用Reflector证实）。因为private成
                     * 员变量无法在基类中进行修改，所以为了能够拿到base 
                     * class中声明的事件，要从EventInfo的DeclaringType来获取
                     * event对应的成员变量的FieldInfo并进行修改
                     ********************************************************/
                    FieldInfo fi = ei.DeclaringType.GetField("Event" + ei.Name, bindingFlags);
                    if (fi != null)
                    {
                        // 将event对应的字段设置成null即可清除所有挂钩在该event上的delegate
                        fi.SetValue(ctrl, null);
                    }
                }
                catch { }
            }
        }
    }
}