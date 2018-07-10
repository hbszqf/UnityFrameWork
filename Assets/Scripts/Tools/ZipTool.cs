using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ZipTool
{
     /// <summary>
    /// 压缩单个文件
    /// </summary>
    /// <param name="toZipFolder">要进行压缩的文件目录</param>
    /// <param name="saveFolder">压缩后文件的保存目录</param>
    /// <param name="compressionLevel">压缩等级 (0-9)</param>
    /// <param name="blockSize">每次写入大小</param>
    public static void ZipEachFileInFolder(string toZipFolder, string saveFolder, int compressionLevel, int blockSize)
    {
        if (!Directory.Exists(toZipFolder))
        {
            throw new System.IO.DirectoryNotFoundException("指定要压缩的文件夹: " + toZipFolder + " 不存在!");
        }
        Log.Wsy("toZipFolder=" + toZipFolder);
        FileInfo[] files = GetDirectoryFiles(toZipFolder);
        for (int i = 0, iMax = files.Length; i < 2; i++)
        {
            FileInfo fileInfo = files[i];
            string filePath = fileInfo.FullName.Replace("\\", "/");
            string folerPath = fileInfo.DirectoryName.Replace("\\", "/");
            if (!string.IsNullOrEmpty(fileInfo.Extension))
            {
                string str = folerPath.Replace(toZipFolder, "");
                Log.Wsy(filePath+"\n"+str + "\n" + Path.Combine(saveFolder, str));
                ZipFile(filePath, Path.Combine(saveFolder, str), compressionLevel, blockSize);
            }
        }
    }

    /// <summary>
    /// 压缩单个文件
    /// </summary>
    /// <param name="toZipfile">要进行压缩的文件全路径</param>
    /// <param name="saveFolder">压缩后文件的保存目录</param>
    /// <param name="compressionLevel">压缩等级 (0-9)</param>
    /// <param name="blockSize">每次写入大小</param>
    public static void ZipFile(string toZipfile, string saveFolder, int compressionLevel, int blockSize)
    {
        //如果文件没有找到，则报错
        if (!System.IO.File.Exists(toZipfile))
        {
            throw new System.IO.FileNotFoundException("指定要压缩的文件: " + toZipfile + " 不存在!");
        }
        if (!Directory.Exists(saveFolder))
            Directory.CreateDirectory(saveFolder);
        using (System.IO.FileStream saveFile = System.IO.File.Create(saveFolder + Path.DirectorySeparatorChar + Path.GetFileName(toZipfile) + ".zip"))
        {
            using (ZipOutputStream zipStream = new ZipOutputStream(saveFile))
            {
                using (System.IO.FileStream fileStream = new System.IO.FileStream(toZipfile, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    string fileName = toZipfile.Substring(toZipfile.LastIndexOf("\\") + 1);
                    ZipEntry ZipEntry = new ZipEntry(fileName);
                    zipStream.PutNextEntry(ZipEntry);
                    zipStream.SetLevel(Mathf.Clamp(compressionLevel,0,9));

                    byte[] buffer = new byte[blockSize];
                    int sizeRead = 0;

                    try
                    {
                        do
                        {
                            sizeRead = fileStream.Read(buffer, 0, buffer.Length);
                            zipStream.Write(buffer, 0, sizeRead);
                        }
                        while (sizeRead > 0);
                    }
                    catch (System.Exception ex)
                    {
                        throw ex;
                    }
                    fileStream.Close();
                }
                zipStream.Finish();
                zipStream.Close();
            }
            saveFile.Close();
        }
    }

    /// <summary>
    /// 压缩单个文件
    /// </summary>
    /// <param name="toZipfile">要进行压缩的文件全路径</param>
    /// <param name="saveFolder">压缩后文件保存目录</param>
    public static void ZipFile(string toZipfile, string saveFolder)
    {
        //如果文件没有找到，则报错
        if (!File.Exists(toZipfile))
        {
            throw new System.IO.FileNotFoundException("指定要压缩的文件: " + toZipfile + " 不存在!");
        }
        if (!Directory.Exists(saveFolder))
            Directory.CreateDirectory(saveFolder);
        using (FileStream fs = File.OpenRead(toZipfile))
        {
            byte[] buffer = new byte[fs.Length];
            fs.Read(buffer, 0, buffer.Length);
            fs.Close();

            using (FileStream saveFile = File.Create(saveFolder + Path.DirectorySeparatorChar + Path.GetFileName(toZipfile) + ".zip"))
            {
                using (ZipOutputStream ZipStream = new ZipOutputStream(saveFile))
                {
                    string fileName = toZipfile.Substring(toZipfile.LastIndexOf("\\") + 1);
                    ZipEntry ZipEntry = new ZipEntry(fileName);
                    ZipStream.PutNextEntry(ZipEntry);
                    ZipStream.SetLevel(5);

                    ZipStream.Write(buffer, 0, buffer.Length);
                    ZipStream.Finish();
                    ZipStream.Close();
                }
            }
        }
    }

    /// <summary>
    /// 压缩多层目录
    /// </summary>
    /// <param name="toZipFolder">要进行压缩的文件目录</param>
    /// <param name="savePath">压缩后文件保存全路径(包含文件名+扩展名)</param>
    /// <param name="compressionLevel">压缩等级 (0-9)</param>
    public static void ZipFolder(string toZipFolder, string savePath, int compressionLevel = 5)
    {
        using (System.IO.FileStream saveFile = System.IO.File.Create(savePath))
        {
            using (ZipOutputStream s = new ZipOutputStream(saveFile))
            {
                s.SetLevel(Mathf.Clamp(compressionLevel, 0, 9));
                ZipSetp(toZipFolder, s, "");
            }
        }
    }

    /// <summary>
    /// 递归遍历目录
    /// </summary>
    /// <param name="strDirectory">The directory.</param>
    /// <param name="s">The ZipOutputStream Object.</param>
    /// <param name="parentPath">The parent path.</param>
    private static void ZipSetp(string strDirectory, ZipOutputStream s, string parentPath)
    {
        if (strDirectory[strDirectory.Length - 1] != Path.DirectorySeparatorChar)
        {
            strDirectory += Path.DirectorySeparatorChar;
        }
        Crc32 crc = new Crc32();

        string[] filenames = Directory.GetFileSystemEntries(strDirectory);

        for (int i = 0, iMax = filenames.Length; i < iMax;i++ )// 遍历所有的文件和目录
        {
            string file = filenames[i];
            if (Directory.Exists(file))// 先当作目录处理如果存在这个目录就递归Copy该目录下面的文件
            {
                string pPath = parentPath;
                pPath += file.Substring(file.LastIndexOf("\\") + 1) + Path.DirectorySeparatorChar;
                ZipSetp(file, s, pPath);
            }
            else // 否则直接压缩文件
            {
                //打开压缩文件
                using (FileStream fs = File.OpenRead(file))
                {
                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);

                    string fileName = parentPath + file.Substring(file.LastIndexOf("\\") + 1);
                    ZipEntry entry = new ZipEntry(fileName);
                    entry.DateTime = DateTime.Now;
                    entry.Size = fs.Length;
                    fs.Close();

                    crc.Reset();
                    crc.Update(buffer);
                    entry.Crc = crc.Value;

                    s.PutNextEntry(entry);
                    s.Write(buffer, 0, buffer.Length);
                }
            }
        }
    }

    /// <summary>
    /// 解压缩一个 zip 文件。
    /// </summary>
    /// <param name="zipFilePath">需要解压的zip文件全路径.</param>
    /// <param name="saveFolder">解压文件存放目录.</param>
    /// <param name="password">zip 文件的密码。</param>
    /// <param name="overWrite">是否覆盖已存在的文件。</param>
    /// <param name="folderPrifix">只解压内部文件夹的文件 类似 "Win/bundles/" 前面没有/ 后面有/ 可以为空 表示全部解压 </param>
    public static bool UnZip(string zipFilePath, string saveFolder = "", string password = "", string folderPrifix = "")
    {
        ZipConstants.DefaultCodePage = 0;
        bool result = false;
        try
        {
            if (!Directory.Exists(saveFolder))
                Directory.CreateDirectory(saveFolder);

            ZipInputStream s = new ZipInputStream(File.OpenRead(zipFilePath));
            s.Password = password;
            ZipEntry theEntry;
            while ((theEntry = s.GetNextEntry()) != null)
            {
                string entryName = theEntry.Name.Replace("\\", "/");
                Log.Print(entryName);
                if (folderPrifix != "")
                {
                    if (!entryName.StartsWith(folderPrifix))
                        continue;//不解压
                    entryName = entryName.Substring(folderPrifix.Length);
                }
				string directoryName = Path.GetDirectoryName(entryName);
				string fileName = Path.GetFileName(entryName);
                Log.Print(directoryName);
                if (!string.IsNullOrEmpty(directoryName))
                    Directory.CreateDirectory(saveFolder +Path .DirectorySeparatorChar+ directoryName);
				if (!string.IsNullOrEmpty(fileName))
                {
                    /**** add by xyy 如果有先删除****/
                    string filename = saveFolder + Path.DirectorySeparatorChar + entryName;
                    if (File.Exists(filename))
                    {
                        File.Delete(filename);
                    }
                    /***end***/

                    FileStream streamWriter = File.Create(filename);
					int size = 2048;
                    byte[] data = new byte[2048];
                    while (true)
                    {
                        size = s.Read(data, 0, data.Length);
                        if (size > 0)
                        {
                            streamWriter.Write(data, 0, size);
                        }
                        else
                        {
                            break;
                        }
                    }
                    streamWriter.Close();
                }
            }
            s.Close();
            result = true;
        }
        catch (Exception ex)
        {
            Debug.Log("UnZipError:"+ex.ToString());
        }
        return result;
    }



    /**** add by xzl 解压文件并且显示解压进度****/
    public delegate void ShowProcessDelegate(float process, string strContent, string title);

    public static bool UnZipAndShowProgress(string zipFilePath, string saveFolder = "", string password = "", string folderPrifix = "", ShowProcessDelegate showProgress = null)
    {
        ZipConstants.DefaultCodePage = 0;
        bool result = false;
        try
        {
            if (!Directory.Exists(saveFolder))
                Directory.CreateDirectory(saveFolder);

            int fileNumber = 0;
            ZipInputStream s2 = new ZipInputStream(File.OpenRead(zipFilePath));   
            ZipEntry theEntry2;
            while ((theEntry2 = s2.GetNextEntry()) != null)
            {
                string entryName = theEntry2.Name.Replace("\\", "/");
                if (folderPrifix != "")
                {
                    if (!entryName.StartsWith(folderPrifix))
                        continue;//不解压
                }
                fileNumber += 1;
            }
            s2.Close();


            ZipInputStream s = new ZipInputStream(File.OpenRead(zipFilePath));
            s.Password = password;
            ZipEntry theEntry;

            int fileNumberUnpack = 0;
            while ((theEntry = s.GetNextEntry()) != null)
            {
                string entryName = theEntry.Name.Replace("\\", "/");

                if (folderPrifix != "")
                {
                    if (!entryName.StartsWith(folderPrifix))
                        continue;//不解压
                    entryName = entryName.Substring(folderPrifix.Length);
                }
                string directoryName = Path.GetDirectoryName(entryName);
                string fileName = Path.GetFileName(entryName);

                if (!string.IsNullOrEmpty(directoryName))
                    Directory.CreateDirectory(saveFolder + Path.DirectorySeparatorChar + directoryName);
                if (!string.IsNullOrEmpty(fileName))
                {
                    /**** add by xyy 如果有先删除****/
                    string filename = saveFolder + Path.DirectorySeparatorChar + entryName;
                    if (File.Exists(filename))
                    {
                        File.Delete(filename);
                    }
                    /***end***/

                    FileStream streamWriter = File.Create(filename);
                    int size = 2048;
                    byte[] data = new byte[2048];
                    while (true)
                    {
                        size = s.Read(data, 0, data.Length);
                        if (size > 0)
                        {
                            streamWriter.Write(data, 0, size);
                        }
                        else
                        {
                            break;
                        }
                    }
                    streamWriter.Close();
                }
                fileNumberUnpack += 1;

                if (showProgress != null)
                {
                    showProgress(((float)fileNumberUnpack / fileNumber), "解压" + entryName, "解压到" + saveFolder);
                }
                //Debug.LogError("文件数量:" + fileNumber + "进度" + fileNumberUnpack + ((float)fileNumberUnpack / fileNumber));
            }
            s.Close();


            result = true;
        }
        catch (Exception ex)
        {
            Debug.Log("UnZipError:" + ex.ToString());
        }
        return result;
    }

      static FileInfo[] GetDirectoryFiles(string directoryPath) 
    {
        List<FileInfo> fileList = new List<FileInfo>();
        if (!Directory.Exists(directoryPath)) 
        {
            return fileList.ToArray();
        }
        DirectoryInfo tDir = new DirectoryInfo(directoryPath);
        FileInfo[] files = tDir.GetFiles();
        for (int i = 0; i < files.Length; i++)
        {               
            if (files[i].Extension !=".meta"&&files[i].Extension !=".DS_Store")
            {
                //Console.WriteLine("File : "+files[i].Name);
                fileList.Add(files[i]);
            }
        }
        DirectoryInfo[] directorys = tDir.GetDirectories();
        for (int i = 0; i < directorys.Length; i++)
        {
            //Console.WriteLine("Folder : "+directorys[i].Name);
            if (directorys[i].Name!=".svn")
            {
                fileList.AddRange(GetDirectoryFiles(directorys[i].FullName));
            }
                
        }
        return fileList.ToArray();
    }
}
