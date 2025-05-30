using Microsoft.AspNetCore.Mvc;
using OrderManagement.Models;
using OrderManagement.Services.Interfaces;
using System.Data;

namespace OrderManagement.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsApiController : ControllerBase
    {
        private readonly IDbConnection _dbConnection;
        private readonly IProductService _productService;

        public ProductsApiController(IDbConnection dbConnection, IProductService productService)
        {
            _dbConnection = dbConnection;
            _productService = productService;
        }

        // GET: api/products?name={name}
        [HttpGet]
        public async Task<ActionResult> GetProducts(string name = "")
        {
            try
            {
                var products = await _productService.GetProductsByName(name);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro ao recuperar os produtos: {ex.Message}" });
            }
        }

        // GET: api/products/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult> GetProduct(int id)
        {
            try
            {
                var product = await _productService.GetProductById(id);
                if (product == null)
                    return NotFound(new { message = $"Produto com ID {id} não encontrado." });

                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro ao recuperar o produto: {ex.Message}" });
            }
        }

        // PUT: api/products/
        [HttpPut]
        public async Task<ActionResult> UpdateProduct(Product product)
        {
            try
            {
                var updatedProduct = await _productService.UpdateProduct(product);
                if (updatedProduct == null)
                    return NotFound(new { message = $"Produto com ID {product.Id} não encontrado." });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro ao atualizar o produto: {ex.Message}" });
            }
        }

        // POST: api/products/
        [HttpPost]
        public async Task<ActionResult> CreateProduct(Product product)
        {

            if (!ModelState.IsValid)
                return BadRequest(new { message = "Os dados do produto são inválidos." });

            try
            {
                await _productService.CreateProduct(product);
                return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro ao cadastrar o produto: {ex.Message}" });
            }
        }

        // DELETE: api/products/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            try
            {
                await _productService.DeleteProduct(id);
                return NoContent(); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro ao excluir o produto: {ex.Message}" });
            }
        }
    }
}
