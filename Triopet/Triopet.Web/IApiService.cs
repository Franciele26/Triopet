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
        #region Product
        [Get("/products")]
        Task<List<ProductDto>> GetProductsAsync();

        [Get("/products/{id}")]
        Task<ProductDto> GetProductById(int id);

        [Post("/products")]
        Task<HttpResponseMessage> AddProductAsync([Body] ProductDto product);

        [Put("/products")]
        Task<HttpResponseMessage> UpdateProduct([Body] ProductDto product);

        [Put("/products/{id}")]
        Task<HttpResponseMessage> DeleteProduct(int id);
        #endregion

        #region Categories
        [Get("/categories")]
        Task<List<CategoryDto>> GetCategoriesAsync();

        [Get("/motive")]
        Task<List<ReasonDto>> GetReasonsAsync();
        #endregion

        #region Animal Type
        [Get("/animaltype")]
        Task<List<AnimalTypeDto>> GetAnimalTypesAsync();
        #endregion

        #region Images
        [Delete("/deleteimage/{id}")]
        Task<HttpResponseMessage> DeleteImage(int id);
        #endregion

        #region Exits
        [Get("/exits")]
        Task<List<ExitDto>> GetExits();

        [Get("/exits/{id}")]
        Task<ExitDto> GetExitById(int id);

        [Delete("/exits/{id}")]
        Task<HttpResponseMessage> DeleteExits(int id);

        [Post("/exits/")]
        Task<HttpResponseMessage> CreateNewExit([Body] ExitDto exit);

        [Put("/exits/")]
        Task<HttpResponseMessage> UpdateExit([Body] ExitDto exit);
        #endregion

        #region Entries
        [Get("/entries/")]
        Task<List<EntryDto>> GetEntries();

        [Get("/entries/{id}")]
        Task<EntryDto> GetEntryById(int id);

        [Delete("/entries/{id}")]
        Task<HttpResponseMessage> DeleteEntry(int id);

        [Post("/entries/")]
        Task<HttpResponseMessage> CreateNewEntry([Body] ExitDto exit);

        [Put("/entries/")]
        Task<HttpResponseMessage> UpdateEntry([Body] ExitDto exit);
        #endregion



    }
}
