using Refit;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Triopet.Shared.Models;

namespace Triopet.Web
{
    public interface IApiService
    {
        [Get("/products")]
        Task<List<ProductDto>> GetProductsAsync();

        [Post("/products")]
        Task<HttpResponseMessage> AddProductAsync([Body] ProductDto product);

        [Put("/products")]
        Task<HttpResponseMessage> UpdateProduct([Body] ProductDto product);

        [Put("/products/{id}")]
        Task<HttpResponseMessage> DeleteProduct(int id);
       
        [Get("/categories")]
        Task<List<CategoryDto>> GetCategoriesAsync();

        [Get("/animaltype")]
        Task<List<AnimalTypeDto>> GetAnimalTypesAsync();

        [Get("/products/{id}")]
        Task<ProductDto> GetProductById(int id);


        [Get("/reasons")]
        Task<List<ReasonDto>> GetReasonsAsync();


        [Get("/exits/{id}")]
        Task<ExitDto> GetExitById(int id);

        [Get("/exits")]
        Task<List<ExitDto>> GetExitsAsync();

        [Post("/exits")]
        Task<HttpResponseMessage> AddExitAsync([Body] ExitDto exit);

        [Put("/exits")]
        Task<HttpResponseMessage> UpdateExit([Body] ExitDto exit);

        [Delete("/exits/{id}")]
        Task<HttpResponseMessage> DeleteExit(int id);
    }
}
