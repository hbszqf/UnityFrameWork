using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace FW
{
    public class ResBuilderConfig 
    {
        /// <summary>
        /// 一个需要打AB的目录
        /// </summary>
        public class ABFolder
        {
            public string folder; //需要打包AB目录 相对于 FW.Const.RES_PATH( Assets/Res )
            public System.Func<string, bool> filter; //过滤器
            public System.Func<string, string> renamer; //重命名
        }

        /// <summary>
        /// 一个需要复制的目录
        /// </summary>
        public class CopyFolder
        {
            public string folder; //需要复制的目录, 相对于FW.Const.RES_PATH(Assets/Res )
            public string pattern;//
            //public string dstFolder; //目标目录  
            //public System.Func<string, bool> filter;
        }

        //判断一个资源是否导出的函数
        public System.Func<string, bool> CheckIsExport = null;

        //判断一个资源是否是公用资源
        public System.Func<string, bool> CheckIsPublic = null;


        internal List<ABFolder> abFolderList = new List<ABFolder>();
        internal List<CopyFolder> copyFolderList = new List<CopyFolder>();

        public ResBuilderConfig()
        {
            AddABFolder(new ABFolder()
            {
                folder = "UI",
                renamer = (string assetPath) => {
                    string fileName = Path.GetFileNameWithoutExtension(assetPath);
                    string[] filename = fileName.Split('@');
                    string packageName = filename[0];
                    return packageName;
                },
            });
        }

        /// <summary>
        /// 添加一个打资源的目录
        /// </summary>
        public void AddABFolder(ABFolder abFolder)
        {
            abFolderList.Add(abFolder);
        }

        /// <summary>
        /// 添加一个copy目录
        /// </summary>
        public void AddCopyFolder(CopyFolder copyFolder)
        {
            copyFolderList.Add(copyFolder);
        }

    }

}
