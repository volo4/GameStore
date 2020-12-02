using System.Web.Security;
using GameStore.WebUI.Infrastructure.Abstract;
using static System.Web.Security.FormsAuthentication;

namespace GameStore.WebUI.Infrastructure.Concrete
{
    public class FormAuthProvider : IAuthProvider
    {
        [System.Obsolete]
        public bool Authenticate(string username, string password)
        {
            var result = FormsAuthentication.Authenticate(username, password);
            if (result)
                SetAuthCookie(username, false);
            return result;
        }
    }
}