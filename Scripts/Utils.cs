using da.Scripts.Interfaces;
using da.Scripts.Objects;
using Godot;
using System.Collections.Generic;
using System.Reflection;

namespace da.Scripts
{
    public static class Utils
    {

        /// <summary>
        /// 利用反射来判断对象是否包含某个属性
        /// </summary>
        /// <param name="instance">object</param>
        /// <param name="propertyName">需要判断的属性</param>
        /// <returns>是否包含</returns>
        public static bool ContainProperty(this object instance, string propertyName)
        {
            if (instance != null && !string.IsNullOrEmpty(propertyName))
            {
                PropertyInfo _findedPropertyInfo = instance.GetType().GetProperty(propertyName);
                return (_findedPropertyInfo != null);
            }
            return false;
        }

        /// <summary>
        /// 检查给定的节点是否已经被保存，如果是，则释放它
        /// </summary>
        /// <param name="node"></param>
        public static void CheckSaveAndFree(Node node)
        {
            if (node == null || Node.IsInstanceValid(node) == false) return;
            var map = node.FindParent("*Map*");
            if (map != null)
            {
                var mapsave = GameGlobal.Instance.save.MapSaveData;
                if (mapsave.ContainsKey(map.Name))
                {
                    var dict = mapsave[map.Name];
                    if (dict.ContainsKey(node.Name))
                    {
                        node.QueueFree();
                    }
                }
            }
        }

        /// <summary>
        /// 读取一个已保存的IOpenable节点的状态，并设置它
        /// 如果没有保存的状态，则设为默认状态
        /// </summary>
        /// <param name="node"></param>
        public static void LoadIOpenableNode(Node node)
        {
            if (node != null && Node.IsInstanceValid(node) && node is IOpenable openable)
            {
                var map = node.FindParent("*Map*");
                if (map != null)
                {
                    var mapsave = GameGlobal.Instance.save.MapSaveData;
                    if (mapsave.ContainsKey(map.Name))
                    {
                        var dict = mapsave[map.Name];
                        if (dict.ContainsKey(node.Name))
                        {
                            var boolean = dict[node.Name].AsBool();
                            if (boolean)
                            {
                                openable.SetToOpenedState();
                            }
                            else
                            {
                                openable.SetToClosedState();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 将节点保存到地图存储数据中
        /// </summary>
        /// <param name="node"></param>
        public static void SaveToMapData(Node node, bool state = true)
        {
            if (node != null && Node.IsInstanceValid(node))
            {
                var map = node.FindParent("*Map*");
                if (map != null)
                {
                    var mapsave = GameGlobal.Instance.save.MapSaveData;
                    if (mapsave.ContainsKey(map.Name))
                    {
                        var dict = mapsave[map.Name];
                        dict.TryAdd(node.Name, state);
                    }
                }
            }
        }

        /// <summary>
        /// 将数据保存到地图存储数据中
        /// </summary>
        /// <param name="map">地图节点</param>
        /// <param name="key">保存的键名</param>
        /// <param name="data">保存的数据</param>
        public static void SaveDataToMapData(Node map, string key, Variant data)
        {
            if (map != null && Node.IsInstanceValid(map))
            {
                var mapsave = GameGlobal.Instance.save.MapSaveData;
                if (!mapsave.ContainsKey(map.Name)) mapsave.Add(map.Name, new());
                if (!mapsave[map.Name].TryAdd(key, data))
                {
                    mapsave[map.Name][key] = data;
                }
            }
        }

        public static bool CheckSaved(Node node)
        {
            if (node != null && Node.IsInstanceValid(node))
            {
                var map = node.FindParent("*Map*");
                if (map != null)
                {
                    var mapsave = GameGlobal.Instance.save.MapSaveData;
                    return mapsave.ContainsKey(map.Name) && mapsave[map.Name].ContainsKey(node.Name);
                }
            }
            return false;
        }

        public static void SaveToGlobalData(string key, bool value = true)
        {
            if (GameGlobal.Instance.save.GlobalSaveData.TryAdd(key, value))
            {
                GameGlobal.Instance.save.GlobalSaveData[key] = value;
            }
        }

        public static bool CheckGlobalData(string key)
        {
            return GameGlobal.Instance.save.GlobalSaveData.ContainsKey(key) && GameGlobal.Instance.save.GlobalSaveData[key].AsBool();
        }
    }
}
