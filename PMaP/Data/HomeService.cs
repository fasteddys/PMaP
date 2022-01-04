using Microsoft.Extensions.Configuration;
using PMaP.Models;
using PMaP.Models.DBModels;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace PMaP.Data
{
    public interface IHomeService
    {
        Task<HomeModel> PortfolioComposition();
    }

    public class HomeService : IHomeService
    {
        private IConfiguration _configuration;
        private IHttpService _httpService;

        public HomeService(IConfiguration configuration, IHttpService httpService)
        {
            _configuration = configuration;
            _httpService = httpService;
        }

        public async Task<HomeModel> PortfolioComposition()
        {
            try
            {
                var httpResponse = await _httpService.Get<IEnumerable<Home>>(_configuration.GetConnectionString("pmapApiUrl") + "/api/Home");
                return new HomeModel { Documents = httpResponse };
            }
            catch (Exception ex)
            {
                return new HomeModel { ResponseCode = (int)HttpStatusCode.InternalServerError, Message = ex.Message };
            }
        }
    }
}
