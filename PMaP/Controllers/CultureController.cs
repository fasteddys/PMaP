using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace PMaP.Controllers
{
    [Route("[controller]/[action]")]
    public class CultureController : Controller
    {
        public IActionResult SetCulture(string culture, string redirectionUri)
        {
            if (!string.IsNullOrEmpty(culture))
            {
                HttpContext.Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture, culture)),
                    new CookieOptions { Expires = System.DateTimeOffset.UtcNow.AddMonths(1) }
                );
            }

            if (string.IsNullOrEmpty(redirectionUri)) redirectionUri = "/";

            return LocalRedirect(redirectionUri);
        }
    }
}
