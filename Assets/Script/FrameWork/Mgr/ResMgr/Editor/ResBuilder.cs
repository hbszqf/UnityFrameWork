using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace FW
{
    /// <summary>
    /// 构建资源
    /// </summary>
    public class ResBuilder 
    {
        //打包配置
        ResBuilderConfig config;

        //ab名字缓存
        static Dictionary<string, string> cacheABNameDict = new Dictionary<string, string>();

        //打包路径名
        List<AssetBundleBuild> assetBundleBuildList;

        public ResBuilder(ResBuilderConfig config) {
            this.config = config;
        }
        
        /// <summary>
        /// 准备
        /// </summary>
        public void Prepare()
        {
            //标记AB
            MarkAB();
        }
        
        /// <summary>
        /// BuildRes
        /// </summary>
        /// <param name="target"></param>
        public void BuildRes(BuildTarget target)
        {
            //切换平台
            UnityEditor.EditorUserBuildSettings.SwitchActiveBuildTarget(BuildPipeline.GetBuildTargetGroup(target), target);

            //编译AB
            {
                string abFolder = ResHelper.GetABFolder(target);
                Directory.CreateDirectory(abFolder);

                //编译AB
                var manifest = BuildPipeline.BuildAssetBundles(abFolder, assetBundleBuildList.ToArray(), BuildAssetBundleOptions.ChunkBasedCompression, target);

                //所有的
                string[] abNameArray = manifest.GetAllAssetBundles();

                //删除无用的AB
                HashSet<string> abNameSet = new HashSet<string>(abNameArray);
                string[] files = Directory.GetFiles(abFolder);
                foreach (var item in files)
                {
                    string fileName = Path.GetFileName(item);
                    fileName = fileName.Replace(".manifest", "");
                    if (!abNameSet.Contains(fileName) && fileName != "AB")
                    {
                        Log.Print("删除AB:" + item);
                        File.Delete(item);
                    }
                }
            }

            //复制不打成AB的文件
            FileUtil.CopyDirectory("*", ResHelper.GetAssetsResWichIsNotABFolder(), ResHelper.GetResFolder(target), false);

            //复制
            foreach (var copyFolder in this.config.copyFolderList)
            {
                string org = ResHelper.GetAssetsResFolder() + copyFolder.folder;
                string dst = ResHelper.GetResFolder(target) + copyFolder.folder;
                string pattern = "*";
                if (copyFolder.pattern != null)
                {
                    pattern = copyFolder.pattern;
                }
                FileUtil.CopyDirectory(pattern, org, dst);
            }

            //复制Lua
            {
                string dst = ResHelper.GetLuaFolder();
                if (Directory.Exists(dst))
                {
                    Directory.Delete(dst, true);
                }
                FileUtil.CopyDirectory("*.lua", LuaConst.luaDir, dst, false);
                FileUtil.CopyDirectory("*.lua", LuaConst.toluaDir, dst, false);

            }

                
            //生成sbf
            CreateSBFAdnSave(ResHelper.GetResFolder(target));
        }

        /// <summary>
        /// 打包
        /// </summary>
        /// <param name="target"></param>
        /// <param name="path"></param>
        /// <param name="dev"></param>
        public void BuildPack(BuildTarget target, string path, bool dev)
        {
            //copy res 到
            string org = ResHelper.GetResFolder(target);
            string dst = Application.streamingAssetsPath + "/Res/";
            FileUtil.CopyDirectory("*", org, dst);

            //刷新一下
            AssetDatabase.Refresh();

            // 若干设置
            PlayerSettings.defaultScreenHeight = 960;
            PlayerSettings.defaultScreenWidth = 540;
            PlayerSettings.displayResolutionDialog = ResolutionDialogSetting.Disabled;
            PlayerSettings.defaultIsFullScreen = false;

            //BuildOptions
            BuildOptions buildOtions = BuildOptions.None;
            if (dev == true)
            {
                buildOtions = BuildOptions.Development;
            }

            //打包
            string[] BUILDSCENE = { "Assets/_Scenes/Start.unity", "Assets/_Scenes/Load.unity" };

            string ret = BuildPipeline.BuildPlayer(BUILDSCENE, path, target, buildOtions);

            if (ret.Length > 0)
            {
                UnityEngine.Debug.LogError("打包失败！" + ret);
                return;
            }
        }

        #region 标记AB名字
        private void MarkAB()
        {
            var abFolderList = config.abFolderList;
            foreach (var abFolder in abFolderList)
            {
                MarkABFolder(abFolder);
            }

            //
            Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();

            //BuildPipeline.BuildAssetBundles(folder)

            foreach (var item in cacheABNameDict)
            {
                string assetPath = item.Key;
                string bundleName = item.Value;
                if (!dict.ContainsKey(bundleName))
                {
                    dict.Add(bundleName, new List<string>());
                }
                dict[bundleName].Add(assetPath);
            }

            List<AssetBundleBuild> list = new List<AssetBundleBuild>();
            foreach (var item in dict)
            {
                var build = new AssetBundleBuild();
                build.assetBundleName = item.Key;
                build.addressableNames = item.Value.ToArray();
                build.assetNames = item.Value.ToArray();

                list.Add(build);
            }

            this.assetBundleBuildList = list;
        }




        private void MarkABFolder(ResBuilderConfig.ABFolder abFolder)
        {
            string folder = abFolder.folder.Replace(@"\", "/");
            string folder2 = ResHelper.RES_PATH + abFolder.folder;
            if (!folder.EndsWith("/"))
            {
                folder2 += "/";
            }

            var assetPaths = AssetDatabase.GetAllAssetPaths().Where((path) =>
            {
                if (!path.StartsWith(folder2))
                {
                    return false;
                }
                if (Directory.Exists(path))
                {
                    return false;
                }
                if (path.EndsWith(".cs") || path.EndsWith(".asset"))
                {
                    return false;
                }

                //过滤失败
                if (abFolder.filter != null) {
                    if (!abFolder.filter(path)) {
                        return false;
                    }
                }

                if (config.CheckIsExport != null) {
                    if (!config.CheckIsExport(path)) {
                        return false;
                    }
                }

                return true;
            });


            //assetPaths
            foreach(var assetPath in assetPaths)
            {
                MarkABAsset(assetPath, abFolder.renamer);
            }
        }

        private void MarkABAsset(string assetPath, System.Func<string, string> renamer)
        {
            string assetPath2 = assetPath;
            if (renamer != null)
            {
                assetPath2 = renamer(assetPath2);
            }
            SetCacheABName(assetPath, ResHelper.GetABName(assetPath2));
        }

        static void SetCacheABName(string assetPath, string abName)
        {
            cacheABNameDict[assetPath] = abName;
        }

        static void ClearCacheABName()
        {
            cacheABNameDict.Clear();
        }
        #endregion


        #region SBF相关
        /// <summary>
        /// 计算文件的MD5值，并out 文件大小，单位（M）
        /// </summary>
        private static string GetFileMd5(string file, out float size)
        {
            try
            {
                FileStream fs = new FileStream(file, FileMode.Open);
                size = fs.Length / 1024f / 1024f;
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(fs);
                fs.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("md5file() fail, error:" + ex.Message);
            }
        }
        static string Combine(string path1, string path2)
        {
            return Path.Combine(path1, path2).Replace("\\", "/"); ;
        }
        static SBF CreateSBF(string folder, string path)
        {
            string realPath = Combine(folder, path);
            float size = 0;
            string md5 = GetFileMd5(realPath, out size);
 
            return new SBF(md5, size);
        }

        //生成SBF
        static void CreateSBFAdnSave(string folder)
        {
            //清理SBF
            ClearSBF();

            //遍历文件
            string[] files = Directory.GetFiles(folder, "*", SearchOption.AllDirectories);
            foreach (var item in files)
            {
                string path = item.Replace("\\", "/");
                path = path.Replace(folder, "");
                if (path.StartsWith("/"))
                {
                    path = path.Substring(1);
                }
                if (path.EndsWith(".meta") || path.EndsWith(".manifest"))
                {
                    continue;
                }
                SBF sbf = CreateSBF(folder, path);
                AddSBF(path, sbf);
            }

            //保存SBF
            SaveSBF(Combine(folder, ResHelper.SBF_NAME));
            ClearSBF();
        }

        struct SBF
        {
            public string hash;
            public float size;
            public SBF(string hash, float size)
            {
                this.hash = hash;
                this.size = size;
            }
        }

        static Dictionary<string, SBF> sbfDict = new Dictionary<string, SBF>();

        static void ClearSBF()
        {
            sbfDict.Clear();
        }

        static void AddSBF(string file, SBF sbf)
        {
            sbfDict.Add(file, sbf);
        }

        static void SaveSBF(string path)
        {
            if (File.Exists(path)) File.Delete(path);

            FileStream fs = new FileStream(path, FileMode.CreateNew);
            StreamWriter sw = new StreamWriter(fs);

            List<string> strList = new List<string>();
            foreach (var item in sbfDict)
            {
                string file = item.Key;
                SBF sbf = item.Value;
                string value = string.Format("{0}|{1}|{2}", file, sbf.hash, sbf.size);
                if (value.IndexOf("/") != -1)
                {
                    strList.Insert(0, value);
                }
                else
                {
                    strList.Add(value);
                }
            }
            sw.Write(string.Join("\n", strList.ToArray()));

            sw.Close();
            fs.Close();
        }


        #endregion
    }
}

