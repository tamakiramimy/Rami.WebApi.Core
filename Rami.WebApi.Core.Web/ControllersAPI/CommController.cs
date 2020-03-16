using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Rami.WebApi.Core.Domain;
using Rami.WebApi.Core.Framework;
using Rami.WebApi.Core.Web.Code;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Rami.WebApi.Core.Web.ControllersAPI
{
    /// <summary>
    /// 通用API
    /// </summary>
    [ApiAuthorize(AuthenticationSchemes = AuthHelper.JwtAuthScheme, Policy = AuthConst.ApiAuthSimple)]
    public class CommController : BaseController
    {
        /// <summary>
        /// 需要旋转的图片后缀
        /// </summary>
        private static readonly List<string> LstImgFormat = new List<string> { ".jpg", ".jpeg", ".icon", ".png", ".bmp" };

        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogHelper<CommController> logHelper;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="logHelper"></param>
        public CommController(ILogHelper<CommController> logHelper)
        {
            this.logHelper = logHelper;
        }

        #region 文件上传

        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="upType"></param>
        /// <returns></returns>
        [HttpPost("Upload")]
        public async Task<Result<List<UploadRes>>> Upload([FromQuery]string upType = "files")
        {
            try
            {
                var lstUploadRes = new List<UploadRes> { };

                // 获取上传目录的路径
                var upRootInfo = GetUploadPath(upType);
                var files = Request.Form.Files;
                foreach (var file in files)
                {
                    // 处理图片旋转
                    var finalStream = await GetFileFinStream(file);
                    // 文件保存的路径
                    var fileUpInfo = GetFilePath(upRootInfo, file.FileName);
                    // 保存文件
                    using (FileStream fs = new FileStream(fileUpInfo.PhyPath, FileMode.OpenOrCreate))
                    {
                        finalStream.Seek(0, SeekOrigin.Begin);
                        await finalStream.CopyToAsync(fs);
                        await fs.FlushAsync();
                    }

                    lstUploadRes.Add(fileUpInfo);
                }

                return new Result<List<UploadRes>> { IsSucc = true, Message = "上传成功！", Data = lstUploadRes };
            }
            catch (Exception ex)
            {
                logHelper.Error($"Upload:上传文件失败：{ex.Message}", ex);
            }

            return new Result<List<UploadRes>> { Message = "上传失败！" };
        }

        /// <summary>
        /// 获取上传的目录
        /// </summary>
        /// <returns></returns>
        private UploadRes GetUploadPath(string upType)
        {
            // 默认路径
            var virPath = "/Upload/Files/";

            // 上传相对路径
            upType = upType.ToLower();
            var dicUpPath = UpPathHelper.DicUpPath;
            if (dicUpPath.ContainsKey(upType))
            {
                virPath = dicUpPath[upType];
            }

            // 增加日期目录
            var dtNow = DateTime.Now;
            // 相对路径
            virPath = Path.Combine(virPath, dtNow.ToString("yyyyMMdd"));
            // 物理路径
            var phyPath = ComHelper.GetPhyWWWRoot(virPath);
            // 绝对路径
            var absPath = ComHelper.GetAbsPath(virPath);

            // 创建目录
            var dirInfo = new DirectoryInfo(phyPath);
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }

            return new UploadRes { VirPath = virPath, PhyPath = phyPath, AbsPath = absPath };
        }

        /// <summary>
        /// 获取上传的文件路径
        /// </summary>
        /// <param name="upRoot"></param>
        /// <param name="orgFileName"></param>
        /// <returns></returns>
        private UploadRes GetFilePath(UploadRes upRoot, string orgFileName)
        {
            var fileName = GetFilePathByExt(orgFileName);

            var virPath = Path.Combine(upRoot.VirPath, fileName);
            var absPath = Path.Combine(upRoot.AbsPath, fileName);
            var phyPath = Path.Combine(upRoot.PhyPath, fileName);

            return new UploadRes { OrgFileName = orgFileName, VirPath = virPath, AbsPath = absPath, PhyPath = phyPath };
        }

        /// <summary>
        /// 根据文件类型获取文件路径
        /// </summary>
        /// <param name="orgFileName"></param>
        /// <returns></returns>
        private string GetFilePathByExt(string orgFileName)
        {
            var dtNow = DateTime.Now;
            var guid = Math.Abs(Guid.NewGuid().GetHashCode());
            var extension = Path.GetExtension(orgFileName);
            var fileName = $"{dtNow.ToString("yyyyMMddHHmmssffff")}_{guid}{extension}";
            return fileName;
        }

        /// <summary>
        /// 获取文件流（图片旋转）
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private async Task<MemoryStream> GetFileFinStream(IFormFile file)
        {
            // 最终的文件流
            var finalStream = new MemoryStream();
            await file.CopyToAsync(finalStream);
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!LstImgFormat.Contains(extension))
            {
                return finalStream;
            }

            // 旋转图片
            var routeStream = RouteImg(finalStream, extension);
            return routeStream;
        }

        /// <summary>
        /// 旋转图片
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        private MemoryStream RouteImg(Stream stream, string extension)
        {
            Image img = Image.FromStream(stream);
            PropertyItem[] exif = img.PropertyItems;
            byte orientation = 0;
            foreach (PropertyItem prop in exif)
            {
                if (prop.Id == 274)
                {
                    orientation = prop.Value[0];
                    prop.Value[0] = 1;
                    img.SetPropertyItem(prop);
                }
            }

            // 旋转图片
            switch (orientation)
            {
                case 2:
                    img.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    break;
                case 3:
                    img.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    break;
                case 4:
                    img.RotateFlip(RotateFlipType.RotateNoneFlipY);
                    break;
                case 5:
                    img.RotateFlip(RotateFlipType.Rotate90FlipX);
                    break;
                case 6:
                    img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
                case 7:
                    img.RotateFlip(RotateFlipType.Rotate270FlipX);
                    break;
                case 8:
                    img.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    break;
                default:
                    break;
            }

            foreach (PropertyItem prop in exif)
            {
                if (prop.Id == 40962)
                {
                    prop.Value = BitConverter.GetBytes(img.Width);
                }
                else if (prop.Id == 40963)
                {
                    prop.Value = BitConverter.GetBytes(img.Height);
                }
            }

            var ms = new MemoryStream();
            if (extension == "png")
            {
                img.Save(ms, ImageFormat.Png);
            }
            else
            {
                img.Save(ms, ImageFormat.Jpeg);
            }

            return ms;
        }

        #endregion
    }
}