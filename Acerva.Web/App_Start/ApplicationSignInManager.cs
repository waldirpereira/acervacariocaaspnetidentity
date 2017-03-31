using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Acerva.Modelo;

namespace Acerva.Web
{
    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<Usuario, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public ApplicationSignInManager(UserManager<Usuario, string> userManager, IAuthenticationManager authenticationManager)
        : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(Usuario user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }


        public async Task<SignInStatus> SignInAsync(string userName, string password, bool rememberMe)
        {
            var user = await UserManager.FindByNameAsync(userName);

            if (user == null) return SignInStatus.Failure;

            if (await UserManager.IsLockedOutAsync(user.Id)) return SignInStatus.LockedOut;

            if (!await UserManager.CheckPasswordAsync(user, password))
            {
                await UserManager.AccessFailedAsync(user.Id);
                if (await UserManager.IsLockedOutAsync(user.Id))
                {
                    return SignInStatus.LockedOut;
                }

                return SignInStatus.Failure;
            }

            if (!await UserManager.IsEmailConfirmedAsync(user.Id))
            {
                return SignInStatus.RequiresVerification;
            }

            await base.SignInAsync(user, rememberMe, false);
            return SignInStatus.Success;
        }
    }
}
