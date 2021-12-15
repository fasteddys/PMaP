using Microsoft.Extensions.Configuration;
using PMaP.Models.Authenticate;
using PMaP.Models.DBModels;
using System.Net;
using System.Threading.Tasks;

namespace PMaP.Data
{
    public interface IAccountService
    {
        Task<AuthenticateResponse> Authenticate(AuthenticateRequest request);
        Task<AuthenticateResponse> GetById(int id);
    }

    public class AccountService : IAccountService
    {
        private IConfiguration _configuration { get; }
        private ILocalStorageService _localStorageService;
        private IHttpService _httpService;

        public AccountService(IConfiguration configuration, ILocalStorageService localStorageService, IHttpService httpService)
        {
            _configuration = configuration;
            _localStorageService = localStorageService;
            _httpService = httpService;
        }

        public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest request)
        {
            AuthenticateResponse authenticateResponse = await _httpService.Post<AuthenticateResponse>(_configuration.GetConnectionString("pmapApiUrl") + "/api/users/authenticate", request);
            if (authenticateResponse.RespCode == (int)HttpStatusCode.OK)
            {
                var profile = authenticateResponse.Profile;
                authenticateResponse.Profile = new Profile();
                await _localStorageService.SetItem("user", authenticateResponse);
                authenticateResponse.Profile = profile;
            }
            return authenticateResponse;
        }

        public async Task<AuthenticateResponse> GetById(int id)
        {
            AuthenticateResponse authenticateResponse = await _httpService.Get<AuthenticateResponse>(_configuration.GetConnectionString("pmapApiUrl") + "/api/users/" + id);
            return authenticateResponse;
        }
    }
}
