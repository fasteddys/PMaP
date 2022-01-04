using Microsoft.Extensions.Configuration;
using PMaP.Models.Authenticate;
using System;
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
            try
            {
                string authenticateResponse = await _httpService.Get<string>(_configuration.GetConnectionString("pmapApiUrl") + "/api/Login/" + request.Username + "/" + request.Password);
                await _localStorageService.SetItem("user", new AuthenticateResponse { Token = authenticateResponse, Username = request.Username });
                return new AuthenticateResponse { RespCode = (int)HttpStatusCode.OK };
            }
            catch (Exception ex)
            {
                return new AuthenticateResponse { RespCode = (int)HttpStatusCode.InternalServerError, Message = ex.Message };
            }
        }
    }
}
