using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuartzComponent
{
    public class QuartzGroupLevel
    {
        /// <summary>
        /// 非常重要地组
        /// </summary>
        public const string EXTREMELY = "EXTREMELY";
        /// <summary>
        /// 重要地
        /// </summary>
        public const string IMPORTANTLY = "IMPORTANTLY";
        /// <summary>
        /// 一般地
        /// </summary>
        public const string GENERALLY = "GENERALLY";

        public static String GetGroupLevelName(GroupLevel groupLevel)
        {
            string ret = "GENERALLY";
            switch (groupLevel)
            {
                case GroupLevel.EXTREMELY:
                    ret = "EXTREMELY";
                    break;
                case GroupLevel.IMPORTANTLY:
                    ret = "IMPORTANTLY";
                    break;
                case GroupLevel.GENERALLY:
                    ret = "GENERALLY";
                    break;
            }
            return ret;
        }
    }

    public enum GroupLevel
    {
        EXTREMELY = 0,
        IMPORTANTLY = 1,
        GENERALLY = 2
    }
}
