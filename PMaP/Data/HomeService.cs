using Microsoft.Extensions.Configuration;
using PMaP.Models;
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
            return await _httpService.Get<HomeModel>(_configuration.GetConnectionString("pmapApiUrl") + "/api/home") ?? new HomeModel();
        }
    }
}
