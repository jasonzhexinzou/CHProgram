using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealApiEleme.Entity
{
    /// <summary>
    /// 基础实体类
    /// </summary>
    public class BaseEntity
    {
        /// <summary>
        /// 
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string request_id { get; set; }
    }
}
