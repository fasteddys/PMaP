using Microsoft.Extensions.Configuration;
using PMaP.Models.Authenticate;
using System.Net;
using System.Threading.Tasks;

namespace PMaP.Data
{
    public interface IAccountService
    {
        Task<AuthenticateResponse> Authenticate(AuthenticateRequest request);
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
                await _localStorageService.SetItem("user", authenticateResponse);
            }
            return authenticateResponse;
        }
    }
}
