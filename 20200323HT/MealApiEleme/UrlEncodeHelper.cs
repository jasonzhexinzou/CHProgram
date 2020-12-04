using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealApiEleme
{
    public static class UrlEncodeHelper
    {
        /// <summary>
        /// 将一个Url字符串进行Url编码，转码过的部分子串采用大写字母形式
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UrlEncodeToUpper(this string str)
        {
            str = System.Web.HttpUtility.UrlEncode(str, Encoding.UTF8);
            var array_str_chat = str.ToCharArray();
            for (var i = 0; i < array_str_chat.Length - 2; i++)
            {
                if (array_str_chat[i] == '%')
                {
                    array_str_chat[i + 1] = Char.ToUpper(array_str_chat[i + 1]);
                    array_str_chat[i + 2] = Char.ToUpper(array_str_chat[i + 2]);
                }
            }
            str = new string(array_str_chat);
            return str;
        }
    }
}
