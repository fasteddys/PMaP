using PMaP.Models.Authenticate;
using System.Threading.Tasks;

namespace PMaP.Data
{
    public interface IMainLayoutService
    {
        Task<AuthenticateResponse> GetUser();
    }

    public class MainLayoutService : IMainLayoutService
    {
        private ILocalStorageService _localStorageService;

        public MainLayoutService(ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
        }

        public async Task<AuthenticateResponse> GetUser()
        {
            AuthenticateResponse authenticateResponse = new AuthenticateResponse();

            try
            {
                authenticateResponse = await _localStorageService.GetItem<AuthenticateResponse>("user");
            }
            catch { }
            
            
            return authenticateResponse;
        }
    }
}
