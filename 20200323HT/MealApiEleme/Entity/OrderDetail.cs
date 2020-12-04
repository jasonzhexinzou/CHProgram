using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealApiEleme.Entity
{
    public class OrderDetail
    {
        /// <summary>
        /// 
        /// </summary>
        public List<string> abandoned_extra { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<List<GroupItem>> group { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<CartExtraItem> extra { get; set; }
    }
}
