using Refit;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Triopet.Web.Models;

namespace Triopet.Web
{
    public interface IApiService
    {
        [Get("/products")]
        Task<List<Product>> GetProductsAsync();

        [Post("/products")]
        Task<HttpResponseMessage> AddProductAsync([Body] Product product);

        [Put("/products")]
        Task<HttpResponseMessage> UpdateProduct([Body] Product product);

        [Delete("/products/{id}")]
        Task<HttpResponseMessage> DeleteProduct(int id);
    }
}
