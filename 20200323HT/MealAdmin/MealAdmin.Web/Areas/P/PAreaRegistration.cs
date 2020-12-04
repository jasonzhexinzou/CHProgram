using System.Web.Mvc;

namespace MealAdmin.Web.Areas.P
{
    public class PAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "P";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "P_default",
                "P/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new string[] { "MealAdmin.Web.Areas.P.Controllers" }
            );
        }
    }
}