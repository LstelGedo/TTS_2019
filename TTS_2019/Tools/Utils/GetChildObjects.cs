using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace TTS_2019.Tools.Utils
{
    //获取元素的所有子元素
    public static class GetChildObjects
    {
        /// <summary>  
        /// 获得指定元素的所有子元素  
        /// </summary>  
        /// <typeparam name="T"></typeparam>  
        /// <param name="obj"></param>  
        /// <returns></returns>  
        public static List<T> GetChildObject<T>(DependencyObject obj) where T : FrameworkElement
        {
            DependencyObject child = null;
            List<T> childList = new List<T>();

            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);

                if (child is T)
                {
                    childList.Add((T)child);
                }
                childList.AddRange(GetChildObject<T>(child));
            }
            return childList;
        }
    }
}
