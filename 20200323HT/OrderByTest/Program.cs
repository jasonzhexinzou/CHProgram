using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OrderByTest
{
    class Program
    {
        static void Main(string[] args)
        {


            Thread.CurrentThread.CurrentCulture = new CultureInfo("zh-cn");
            string[] temArr = { "7点自造DIY烘焙馆(西直门店)1", "二比格比萨(新街口店)2", "凯丝恩贝(阜成门店)3", "嘉和一品(交大店)4", "五五源素5", "合利屋快餐(西直门店)6", "拿渡麻辣香锅(万通新世界店)7", "德克士(西直门店)8", "正一味(新华百货店)9", "丽华快餐(阜外店)10", "厨来厨往(东光胡同店)11", "一品三笑(星街坊店)12", "宋记餐厅13", "必胜宅急送14", "日昌餐馆(文慧桥店)15", "俏舌16" };

            var resName = new List<Restaurant>();
            var i = 0;
            foreach(var item in temArr)
            {
                var res=new Restaurant
                {
                    id = i.ToString(),
                    name = item
                };
                i++;
                resName.Add(res);
            }


            foreach(var item in resName)
            {
                var s = "";
                for (i = 0; i < item.name.Length; i++)
                {
                    s += string.Format("{0:D10}", (int)item.name[i] ) + " ";
                }
                s += " : " + item.name;
                Console.WriteLine(s);
            }

            resName = resName.OrderBy(a => a.name).ToList();

            Console.WriteLine("----------------");

            foreach (var item in resName)
            {
                var s = "";
                for (i = 0; i < item.name.Length; i++)
                {
                    s += string.Format("{0:D10}", (int)item.name[i]) + " ";
                }
                s += " : " + item.name;
                Console.WriteLine(s);
            }
            Console.ReadLine();
        }
    }

    public class Restaurant
    {
        public string id { get;set;}
        public string name { get;set;}
    }
}
