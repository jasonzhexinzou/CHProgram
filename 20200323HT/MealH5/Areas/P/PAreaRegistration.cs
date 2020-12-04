using System.Web.Mvc;

namespace MealH5.Areas.P
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
                new string[] { "MealH5.Areas.P.Controllers" }
            );
        }
    }
}