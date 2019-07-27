using System;
using System.Collections.Concurrent;
using System.Net;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using CcNet.Utils.Extensions;

namespace CcNet.Utils.Helpers
{
    public delegate Image LoadImageDelegate(string url, out string error);

    public static class ImageHelper
    {
        private static ConcurrentDictionary<string, Image> Images
            = new ConcurrentDictionary<string, Image>();

        /// <summary>
        /// 删除图片
        /// </summary>
        /// <param name="url"></param>
        /// <param name="error">错误原因</param>
        public static void DeleteImage(string url, out string error)
        {
            error = string.Empty;

            try
            {
                if (!url.IsValid())
                {
                    error = "无效的URL地址。";
                }

                var lowerUrl = url.LowerCase();

                if (Images.ContainsKey(lowerUrl) &&
                    !Images.TryRemove(lowerUrl, out Image img))
                {
                    error = "删除图片失败";
                    return;
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
        }

        /// <summary>
        /// 获取图片
        /// </summary>
        /// <param name="url"></param>
        /// <param name="useCache">是否使用缓存</param>
        /// <param name="error">错误原因</param>
        /// <param name="loadImage">加载图片的方法</param>
        /// <returns></returns>
        public static Image GetImage(string url, bool useCache, out string error, LoadImageDelegate loadImage = null)
        {
            error = string.Empty;

            try
            {
                if (!url.IsValid())
                {
                    error = "无效的URL地址。";
                    return null;
                }

                var lowerUrl = url.LowerCase();

                if (useCache && Images.ContainsKey(lowerUrl))
                {
                    return Images[lowerUrl];
                }

                Image image = null;

                if (loadImage != null)
                {
                    image = loadImage(url, out error);
                }
                else
                {
                    image = LoadImage(url, out error);
                }

                if (image != null)
                {
                    Images.AddOrUpdate(lowerUrl, image, (k, v) => v = image);
                }

                return image;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 获取网络图片
        /// </summary>
        /// <param name="url">图片URL</param>
        /// <param name="useCache">是否使用缓存</param>
        /// <param name="error">错误信息</param>
        /// <param name="width">图片控件的宽度</param>
        /// <param name="height">图片控件的高度</param>
        /// <returns></returns>
        public static Bitmap GetBitmap(string url, bool useCache, out string error, int? width = null, int? height = null)
        {
            var bitmap = GetBitmap(url, useCache, out error, width, height, LoadImage);
            return bitmap ?? GetErrorImage(width, height);
        }

        /// <summary>
        /// 获取流图片
        /// </summary>
        /// <param name="name">图片名称</param>
        /// <param name="base64">base64加密过的图片内容</param>
        /// <param name="useCache">是否使用缓存</param>
        /// <param name="error">错误信息</param>
        /// <param name="width">图片控件的宽度</param>
        /// <param name="height">图片控件的高度</param>
        /// <returns></returns>
        public static Bitmap GetBitmap(string name, string base64, bool useCache, out string error, int? width = null, int? height = null)
        {
            error = null;

            var bitmap = GetBitmap(name, useCache, out error, width, height,
                (string url, out string err) => ParseImage(base64, out err));

            return bitmap ?? GetErrorImage(width, height);
        }

        /// <summary>
        /// 获取图片
        /// </summary>
        /// <param name="key"></param>
        /// <param name="useCache">是否使用缓存</param>
        /// <param name="error">错误原因</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="loadImage">加载图片的方法</param>
        /// <returns></returns>
        private static Bitmap GetBitmap(string key, bool useCache, out string error, int? width, int? height, LoadImageDelegate loadImage = null)
        {
            try
            {
                var image = GetImage(key, useCache, out error, loadImage);
                if (null == image)
                {
                    return null;
                }

                var bmp = new Bitmap(image);

                if ((!width.HasValue || width.Value == bmp.Width) &&
                    (!height.HasValue || height.Value == bmp.Height))
                {
                    return bmp;
                }

                return ResizeImage(bmp, width ?? bmp.Width, height ?? bmp.Height);
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 获取错误图片
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private static Bitmap GetErrorImage(int? width, int? height)
        {
            var img = Properties.Resource.img_error;
            return ResizeImage(img, width, height);
        }

        /// <summary>
        /// Resize图片
        /// </summary>
        /// <param name="bmp">原始Bitmap</param>
        /// <param name="width">新的宽度</param>
        /// <param name="height"> 新的高度</param>
        /// <returns>处理以后的图片</returns>
        private static Bitmap ResizeImage(Bitmap bmp, int? width, int? height)
        {
            try
            {
                if ((!width.HasValue || width.Value == bmp.Width) &&
                    (!height.HasValue || height.Value == bmp.Height))
                {
                    return bmp;
                }

                Bitmap b = new Bitmap(width ?? bmp.Width, height ?? bmp.Height);
                Graphics g = Graphics.FromImage(b);

                // 插值算法的质量
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(bmp, new Rectangle(0, 0, width ?? bmp.Width, height ?? bmp.Height),
                    new Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
                g.Dispose();
                return b;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 解析内存图片
        /// </summary>
        /// <param name="base64">BASE64加密的图片流</param>
        /// <param name="error"></param>
        /// <returns></returns>
        private static Image ParseImage(string base64, out string error)
        {
            Image img = null;
            error = string.Empty;

            try
            {
                var decode = Convert.FromBase64String(base64);
                var stream = new MemoryStream(decode);

                img = Image.FromStream(stream);
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            return img;
        }

        /// <summary>
        /// 加载网络图片
        /// </summary>
        /// <param name="url"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        private static Image LoadImage(string url, out string error)
        {
            Image img = null;
            error = string.Empty;

            try
            {
#if true
                var request = WebRequest.Create(url);
                if (null == request)
                {
                    error = "初始化请求失败。";
                    return null;
                }

                var response = request.GetResponse();
                if (null == response)
                {
                    error = "请求URL失败。";
                    return null;
                }

                var stream = response.GetResponseStream();
                if (null == response)
                {
                    error = "请求URL失败。";
                    return null;
                }


                img = Image.FromStream(stream);
#else
                var response = HttpHelper.SendGet(url);
                if (null == response || null == response.Content)
                {
                    error = "请求URL失败。";
                    return null;
                }

                if (!response.IsSuccessStatusCode)
                {
                    error = response.ReasonPhrase;
                    return null;
                }

                using (var stream = response.Content.ReadAsStreamAsync().Result)
                {
                    img = Image.FromStream(stream);
                }
#endif
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            return img;
        }
    }
}