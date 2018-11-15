using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using Jinher.AMP.BTP.UI.Models;
using Jinher.AMP.BTP.Common;
public class FileUploadHelper
{
    /// <summary>
    /// 上传到图片服务器
    /// </summary>
    /// <param name="realPath">文件全路径</param>
    /// <returns></returns>
    public static string UploadFile(string realPath)
    {
        var imageUrl = string.Empty;
        using (FileStream stream = new FileStream(realPath, FileMode.Open))
        {
            int fileLength = Convert.ToInt32(stream.Length);
            Jinher.JAP.BaseApp.FileServer.Deploy.CustomDTO.FileDTO file = new Jinher.JAP.BaseApp.FileServer.Deploy.CustomDTO.FileDTO();
            file.UploadFileName = Path.GetFileName(realPath);
            byte[] fileData = new byte[fileLength];
            stream.Read(fileData, 0, fileLength);
            file.FileData = fileData;
            file.FileSize = fileData.Length;
            file.StartPosition = 0;
            file.IsClient = false;
            //上传文件获得url
            imageUrl = Jinher.AMP.BTP.TPS.BTPFileSV.Instance.UploadFile(file);
            if (!string.IsNullOrEmpty(imageUrl))//上传成功
            {
                imageUrl = CustomConfig.FileServerUrl + imageUrl;
            }

        }
        return imageUrl;
    }

    /// <summary>
    /// 上传到图片服务器
    /// </summary>
    /// <param name="stream">文件流</param>
    /// <param name="fileName">图片名称</param>
    /// <returns></returns>
    public static string UploadFile(Stream stream, string fileName)
    {
        var imageUrl = string.Empty;
        try
        {
            int fileLength = Convert.ToInt32(stream.Length);
            Jinher.JAP.BaseApp.FileServer.Deploy.CustomDTO.FileDTO file = new Jinher.JAP.BaseApp.FileServer.Deploy.CustomDTO.FileDTO();
            file.UploadFileName = fileName;
            byte[] fileData = new byte[fileLength];
            stream.Read(fileData, 0, fileLength);
            file.FileData = fileData;
            file.FileSize = fileData.Length;
            file.StartPosition = 0;
            file.IsClient = false;
            //上传文件获得url
            imageUrl = Jinher.AMP.BTP.TPS.BTPFileSV.Instance.UploadFile(file);
            if (!string.IsNullOrEmpty(imageUrl))//上传成功
            {
                imageUrl = CustomConfig.FileServerUrl + imageUrl;
            }
        }
        catch (Exception ex) { }
        finally
        {
            if (stream != null)
            {
                stream.Close();
                stream.Dispose();
            }
        }

        return imageUrl;
    }

    /// <summary>
    /// 上传到图片服务器
    /// </summary>
    /// <param name="fileData">文件byte</param>
    /// <param name="fileName">图片名称</param>
    /// <returns></returns>
    public static string UploadFile(byte[] fileData, string fileName)
    {
        var imageUrl = string.Empty;
        Jinher.JAP.BaseApp.FileServer.Deploy.CustomDTO.FileDTO file = new Jinher.JAP.BaseApp.FileServer.Deploy.CustomDTO.FileDTO();
        file.UploadFileName = fileName;
        file.FileData = fileData;
        file.FileSize = fileData.Length;
        file.StartPosition = 0;
        file.IsClient = false;
        //上传文件获得url
        imageUrl = Jinher.AMP.BTP.TPS.BTPFileSV.Instance.UploadFile(file);
        if (!string.IsNullOrEmpty(imageUrl))//上传成功
        {
            imageUrl = CustomConfig.FileServerUrl + imageUrl;
        }
        return imageUrl;
    }

}
