using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;

namespace FW
{ 
    public class SBF
    {
        public class SBFItem
        {
            public string path; //路径
            public double size; //mb
            public string md5;

            public SBFItem(string line)
            {
                string[] infos = line.Split('|');
                this.path = infos[0];
                this.md5 = infos[1];
                this.size = Convert.ToDouble(infos[2]);
            }
        }

        //所有对象的集合
        Dictionary<string, SBFItem> itemDict = new Dictionary<string, SBFItem>();

        public SBF(string doc)
        {
            StringReader reader = new StringReader(doc);
            while (true)
            {
                string line = reader.ReadLine();
                if (line == null || line == "")
                {
                    break;
                }

                var item = new SBFItem(line.Trim());
                itemDict.Add(item.path, item);
            }

        }

        #region 静态函数
        /// <summary>
        /// 对比sbf
        /// </summary>
        /// <param name="sbf">对比的sbf</param>
        /// <param name="less">this比sbf少的</param>
        /// <param name="more">this比sbf多的</param>
        /// <param name="diff">this和sbf都拥有 但是md5不一样的</param>
        public void Compare(SBF sbf, out List<SBFItem> diff, out List<SBFItem> more, out List<SBFItem> less)
        {
            diff = new List<SBFItem>();
            more = new List<SBFItem>();
            less = new List<SBFItem>();

            foreach (var pairs in this.itemDict)
            {
                var key = pairs.Key;
                var value = pairs.Value;
                if (sbf.itemDict.ContainsKey(key))
                {
                    if (sbf.itemDict[key].md5 != value.md5)
                    {
                        diff.Add(value);
                    }
                }
                else
                {
                    more.Add(value);

                }
            }

            foreach (var pair in sbf.itemDict)
            {
                if (!this.itemDict.ContainsKey(pair.Key))
                {
                    less.Add(pair.Value);
                }
            }
        }
        #endregion
    }
}