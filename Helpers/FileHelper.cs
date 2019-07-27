using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using CcNet.Utils.Extensions;

namespace CcNet.Utils.Helpers
{
    /// <summary>
    /// 预定的文件后缀
    /// </summary>
    public enum PredefinedExt : uint
    {
        [Description("全部文件")]
        All = 0x10000000,

        [Description("图片文件")]
        Image = 0x00001000,

        [Description("ZIP文件")]
        Zip = 0x00010000,

        [Description("RAR文件")]
        Rar = 0x00020000,

        [Description("文档文件")]
        Word = 0x0000000F,

        [Description("Excel 2003文件")]
        Word_2003 = 0x00000001,

        [Description("Excel 2007文件")]
        Word_2007 = 0x00000002,

        [Description("表格文件")]
        Excel = 0x000000F0,

        [Description("Excel 2003文件")]
        Excel_2003 = 0x00000010,

        [Description("Excel 2007文件")]
        Excel_2007 = 0x00000020,

        [Description("演示文件")]
        PowerPoint = 0x00000F00,

        [Description("PowerPoint 2003文件")]
        PowerPoint_2003 = 0x00000100,

        [Description("PowerPoint 2007文件")]
        PowerPoint_2007 = 0x00000200,
    }

    /// <summary>
    /// PredefinedExt扩展类
    /// </summary>
    public static class PredefinedExtExtension
    {
        /// <summary>
        /// 检查当前标志中是否包含目标标志组
        /// </summary>
        /// <param name="self"></param>
        /// <param name="flags">目标标志组</param>
        /// <param name="checkAll">检查全部或任意</param>
        /// <returns></returns>
        public static bool Contains(this PredefinedExt self, PredefinedExt flags, bool checkAll = true)
        {
            if (checkAll)
            {
                return ((uint)self).ContainsAll((uint)flags);
            }
            else
            {
                return ((uint)self).ContainsAny((uint)flags);
            }
        }
    }

    /// <summary>
    /// 目录帮助类
    /// </summary>
    public static class FileHelper
    {
        /// <summary>
        /// 设置文件后缀
        /// </summary>
        /// <param name="extType">预定义的文件后缀类型</param>
        /// <returns></returns>
        public static List<FileExtFilter> SetFileExts(PredefinedExt extType)
        {
            var extFilters = new List<FileExtFilter>();

            extFilters.CheckAdd(extType, PredefinedExt.Image);
            extFilters.CheckAdd(extType, PredefinedExt.Zip);
            extFilters.CheckAdd(extType, PredefinedExt.Rar);
            extFilters.CheckAdd(extType, PredefinedExt.Word);
            extFilters.CheckAdd(extType, PredefinedExt.Word_2003);
            extFilters.CheckAdd(extType, PredefinedExt.Word_2007);
            extFilters.CheckAdd(extType, PredefinedExt.Excel);
            extFilters.CheckAdd(extType, PredefinedExt.Excel_2003);
            extFilters.CheckAdd(extType, PredefinedExt.Excel_2007);
            extFilters.CheckAdd(extType, PredefinedExt.PowerPoint);
            extFilters.CheckAdd(extType, PredefinedExt.PowerPoint_2003);
            extFilters.CheckAdd(extType, PredefinedExt.PowerPoint_2007);

            //“全部”放到最后添加
            extFilters.CheckAdd(extType, PredefinedExt.All);

            return extFilters;
        }

        /// <summary>
        /// 选择文件
        /// </summary>
        /// <param name="extFilters"></param>
        /// <param name="title">窗口标题</param>
        /// <param name="initialDirectory">初始化目录路径</param>
        /// <returns></returns>
        public static string SelectFile(this List<FileExtFilter> extFilters, string title = "选择文件", string initialDirectory = null)
            => extFilters.SelectFiles(false, title, initialDirectory)?.FirstOrDefault();

        /// <summary>
        /// 选择文件
        /// </summary>
        /// <param name="extFilter"></param>
        /// <param name="title">窗口标题</param>
        /// <param name="initialDirectory">初始化目录路径</param>
        /// <returns></returns>
        private static string SelectFile(this FileExtFilter extFilter, string title = "选择文件", string initialDirectory = null)
            => extFilter.SelectFiles(false, title, initialDirectory)?.FirstOrDefault();

        /// <summary>
        /// 批量选择文件
        /// </summary>
        /// <param name="extFilters"></param>
        /// <param name="multiSelect">是否多选</param>
        /// <param name="title">窗口标题</param>
        /// <param name="initialDirectory">初始化目录路径</param>
        /// <returns></returns>
        public static List<string> SelectFiles(this List<FileExtFilter> extFilters, bool multiSelect, string title = "选择文件", string initialDirectory = null)
            => extFilters.Combine().SelectFiles(multiSelect, title, initialDirectory);

        /// <summary>
        /// 批量选择文件
        /// </summary>
        /// <param name="extFilter"></param>
        /// <param name="multiSelect">是否多选</param>
        /// <param name="title">窗口标题</param>
        /// <param name="initialDirectory">初始化目录路径</param>
        /// <returns></returns>
        private static List<string> SelectFiles(this FileExtFilter extFilter, bool multiSelect, string title = "选择文件", string initialDirectory = null)
        {
            if (null == extFilter || !extFilter.Filter.IsValid())
            {
                return null;
            }

            var ofd = new OpenFileDialog
            {
                Filter = extFilter.Filter,
                Title = title,
                InitialDirectory = initialDirectory.GetValue(Environment.GetFolderPath(Environment.SpecialFolder.Desktop)),
                RestoreDirectory = true,
                AddExtension = true,
                Multiselect = multiSelect,
                CheckFileExists = true,
                CheckPathExists = true,
            };

            if (ofd.ShowDialog() != DialogResult.OK)
            {
                return null;
            }

            var fileNames = new List<string>();
            if (ofd.FileNames.IsEmpty())
            {
                fileNames.Add(ofd.FileName);
            }
            else
            {
                fileNames.AddRange(ofd.FileNames);
            }

            return fileNames;
        }

        /// <summary>
        /// 选择保存路径
        /// </summary>
        /// <param name="extFilters"></param>
        /// <param name="fileName">要保存的文件名称</param>
        /// <param name="title">窗口标题</param>
        /// <param name="initialDirectory">初始化目录路径</param>
        /// <returns></returns>
        public static string SelectSavePath(this List<FileExtFilter> extFilters, string fileName, string title = "选择文件", string initialDirectory = null)
            => extFilters.Combine().SelectSavePath(fileName, title, initialDirectory);

        /// <summary>
        /// 选择保存路径
        /// </summary>
        /// <param name="extFilter"></param>
        /// <param name="fileName">要保存的文件名称</param>
        /// <param name="title">窗口标题</param>
        /// <param name="initialDirectory">初始化目录路径</param>
        /// <returns></returns>
        private static string SelectSavePath(this FileExtFilter extFilter, string fileName, string title = "选择文件", string initialDirectory = null)
        {
            var sfd = new SaveFileDialog
            {
                Filter = extFilter?.Filter,
                Title = title,
                FileName = fileName,
                InitialDirectory = initialDirectory.GetValue(Environment.GetFolderPath(Environment.SpecialFolder.Desktop)),
                RestoreDirectory = true,
                AddExtension = true,
                //CheckFileExists = true,
                //CheckPathExists = true,
            };

            if (sfd.ShowDialog() != DialogResult.OK)
            {
                return string.Empty;
            }

            if (!Path.HasExtension(sfd.FileName) &&
                extFilter != null && extFilter.DefaultExt.IsValid())
            {
                return $"{sfd.FileName}.{extFilter.DefaultExt.LowerCase()}";
            }

            return sfd.FileName;
        }

        private static readonly Dictionary<PredefinedExt, string[]> PredefinedExtList
            = new Dictionary<PredefinedExt, string[]>
            {
                [PredefinedExt.All] = new string[] { "*" },

                [PredefinedExt.Image] = new string[] { "bmp", "jpg", "jpeg", "png", "gif" },

                [PredefinedExt.Zip] = new string[] { "zip" },
                [PredefinedExt.Rar] = new string[] { "rar" },

                [PredefinedExt.Word] = new string[] { "doc", "docx" },
                [PredefinedExt.Word_2003] = new string[] { "doc" },
                [PredefinedExt.Word_2007] = new string[] { "docx" },

                [PredefinedExt.Excel] = new string[] { "xls", "xlsx" },
                [PredefinedExt.Excel_2003] = new string[] { "xls" },
                [PredefinedExt.Excel_2007] = new string[] { "xlsx" },

                [PredefinedExt.PowerPoint] = new string[] { "ppt", "pptx" },
                [PredefinedExt.PowerPoint_2003] = new string[] { "ppt" },
                [PredefinedExt.PowerPoint_2007] = new string[] { "pptx" },
            };

        /// <summary>
        /// 包含时添加
        /// </summary>
        /// <param name="extFilters"></param>
        /// <param name="extType"></param>
        /// <param name="toCheckExtType"></param>
        private static void CheckAdd(this List<FileExtFilter> extFilters, PredefinedExt extType, PredefinedExt toCheckExtType)
        {
            if (!extType.Contains(toCheckExtType))
            {
                return;
            }

            var extFilter = toCheckExtType.ToFileExtFilter();
            if (null == extFilter)
            {
                return;
            }

            extFilters.Add(extFilter);
        }

        /// <summary>
        /// 将PredefinedExt转换成FileExtFilter
        /// </summary>
        /// <param name="extType"></param>
        /// <returns></returns>
        private static FileExtFilter ToFileExtFilter(this PredefinedExt extType)
        {
            if (!PredefinedExtList.ContainsKey(extType))
            {
                return null;
            }

            return new FileExtFilter(extType.GetDesc(), PredefinedExtList[extType]);
        }

        /// <summary>
        /// 合并过滤器，获取最终的过滤器，例如：
        /// 表格文件(*.xls;*.xlsx)|*.xls;*.xlsx|所有文件(*.*)|*.*
        /// </summary>
        /// <param name="extFilters"></param>
        /// <returns></returns>
        private static FileExtFilter Combine(this List<FileExtFilter> extFilters)
        {
            if (extFilters.IsEmpty())
            {
                return null;
            }

            var result = new FileExtFilter();
            var sbFilter = new StringBuilder();

            foreach (var extFilter in extFilters)
            {
                if (null == extFilter)
                {
                    continue;
                }

                if (!result.DefaultExt.IsValid())
                {
                    result.DefaultExt = extFilter.DefaultExt;
                }

                if (sbFilter.Length > 0)
                {
                    sbFilter.Append("|");
                }

                sbFilter.Append(extFilter.Filter);
            }

            result.Filter = sbFilter.ToString();

            return result;
        }

        public sealed class FileExtFilter
        {
            /// <summary>
            /// 类型过滤
            /// </summary>
            public string Filter { get; set; }

            /// <summary>
            /// 默认的后缀
            /// </summary>
            public string DefaultExt { get; set; }

            /// <summary>
            /// 默认构造函数
            /// </summary>
            public FileExtFilter() { }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="fileTypeDesc">文件类型描述</param>
            /// <param name="fileExts">文件后缀列表</param>
            public FileExtFilter(string fileTypeDesc, params string[] fileExts)
            {
                Filter = GetFilter(fileTypeDesc, fileExts);
                DefaultExt = fileExts?.FirstOrDefault();
            }

            /// <summary>
            /// 获取文件过滤，参考：
            /// 表格文件(*.xls;*.xlsx)|*.xls;*.xlsx|所有文件(*.*)|*.*
            /// </summary>
            /// <param name="fileTypeDesc">文件类型描述</param>
            /// <param name="fileExts">文件后缀列表</param>
            /// <returns></returns>
            private static string GetFilter(string fileTypeDesc, string[] fileExts)
            {
                if (fileExts.IsEmpty())
                {
                    return string.Empty;
                }

                var strExts = $"*.{string.Join(";*.", fileExts)}";
                return $"{fileTypeDesc.GetValue("文件")}({strExts})|{strExts}";
            }
        }
    }
}