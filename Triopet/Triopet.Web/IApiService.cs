using Microsoft.AspNetCore.Mvc;
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

        [Delete("/products/{id}")]
        Task<HttpResponseMessage> DeleteProduct(int id);
       
        [Get("/categories")]
        Task<List<CategoryDto>> GetCategoriesAsync();

        [Get("/animaltype")]
        Task<List<AnimalTypeDto>> GetAnimalTypesAsync();

        [Get("/products/{id}")]
        Task<ProductDto> GetProductById(int id);

        [Delete("/deleteimage/{id}")]
        Task<HttpResponseMessage> DeleteImage(int id);

        [Get("/exits")]
        Task<List<ExitDto>> GetExits();

        [Get("/exits/{id}")]
        Task<ExitDto> GetExitById(int id);

        [Delete("/exits/{id}")]
        Task<HttpResponseMessage> DeleteExits(int id);
    }
}