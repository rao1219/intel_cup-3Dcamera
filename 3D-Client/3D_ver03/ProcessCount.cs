using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _3D_ver03
{

    /// <summary>
    /// 实现倒计时功能的类
    /// </summary>
    public class ProcessCount
    {
        public Int32 TotalSecond { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ProcessCount(Int32 totalSecond)
        {
            TotalSecond = totalSecond;
        }

        /// <summary>
        /// 减秒
        /// </summary>
        /// <returns></returns>
        public bool ProcessCountDown()
        {
            if (TotalSecond == 0)
                return false;
            else
            {
                TotalSecond--;
                return true;
            }
        }

        /// <summary>
        /// 获取小时显示值
        /// </summary>
        /// <returns></returns>
        public string GetHour()
        {
            return String.Format("{0:D2}", (TotalSecond / 3600));
        }

        /// <summary>
        /// 获取分钟显示值
        /// </summary>
        /// <returns></returns>
        public string GetMinute()
        {
            return String.Format("{0:D2}", (TotalSecond % 3600) / 60);
        }

        /// <summary>
        /// 获取秒显示值
        /// </summary>
        /// <returns></returns>
        public string GetSecond()
        {
            return String.Format("{0:D2}", TotalSecond % 60);
        }

    }
}