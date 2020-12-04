using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Entity
{
    public class P_TACode
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string NameShow { get; set; }
        public string InvoiceTitle { get; set; }
        public string DutyParagraph { get; set; }
    }
}
