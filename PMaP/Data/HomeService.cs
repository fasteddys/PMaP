using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PMaP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PMaP.Data
{
    public class HomeService
    {
        public IConfiguration Configuration { get; }

        public HomeService(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public async Task<HomeModel> PortfolioComposition()
        {
            HomeModel model = new HomeModel();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Configuration.GetConnectionString("homeUri"));
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //HTTP GET
                var responseTask = await client.GetAsync("home");

                var result = responseTask;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = await result.Content.ReadAsStringAsync();
                    model = JsonConvert.DeserializeObject<HomeModel>(readTask);
                }
            }

            return model;
        }
    }
}
