using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Entity
{
    public class P_MARKET
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string NameShow { get; set; }
        public string InvoiceTitle { get; set; }
        public string DutyParagraph { get; set; }
    }
    
    public class P_MARKET_INVOICE_OBJ
    {
        /// <summary>
        /// Rx(&CD-HA)	葛兰素史克（中国）投资有限公司
        /// </summary>
        public string Rx { get; set; }
        /// <summary>
        /// Vx	葛兰素史克（中国）投资有限公司
        /// </summary>
        public string Vx { get; set; }
        /// <summary>
        /// DDT	葛兰素史克日用保健品（中国）有限公司
        /// </summary>
        public string DDT { get; set; }
        /// <summary>
        /// TSKF	中美天津史克制药有限公司
        /// </summary>
        public string TSKF { get; set; }
        /// <summary>
        /// Rx(&CD-HA)	葛兰素史克（中国）投资有限公司 纳税人识别号
        /// </summary>
        public string RxDutyParagraph { get; set; }
        /// <summary>
        /// Vx	葛兰素史克（中国）投资有限公司 纳税人识别号
        /// </summary>
        public string VxDutyParagraph { get; set; }
        /// <summary>
        /// DDT	葛兰素史克日用保健品（中国）有限公司 纳税人识别号
        /// </summary>
        public string DDTDutyParagraph { get; set; }
        /// <summary>
        /// TSKF	中美天津史克制药有限公司 纳税人识别号
        /// </summary>
        public string TSKFDutyParagraph { get; set; }
        /// <summary>
        /// R&D	葛兰素史克（上海）医药研发有限公司
        /// </summary>
        public string RD { get; set; }
        /// <summary>
        /// R&D	葛兰素史克（上海）医药研发有限公司 纳税人识别号
        /// </summary>
        public string RDDutyParagraph { get; set; }
    }
}
