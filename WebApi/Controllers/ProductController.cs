using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Repository;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository productRepository;

        public ProductController(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }

        // GET: api/product
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(productRepository.GetAllProducts());
        }

        // GET: api/product/1
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var product = productRepository.GetProductById(id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // GET: api/product/byname/Toyata
        [HttpGet("byname/{name}")]
        public IActionResult Get(string name)
        {
            var product = productRepository.GetProductByName(name);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // POST: api/product
        [HttpPost]
        public IActionResult Post([FromBody] Product product)
        {
            try
            {
                productRepository.AddProduct(product);
                return CreatedAtAction(nameof(Get), new { id = product.ProductId }, product);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to add product. {ex.Message}");
            }
        }

        // PUT: api/product/1
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Product product)
        {
            if (id != product.ProductId)
            {
                return BadRequest();
            }

            try
            {
                productRepository.UpdateProduct(product);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }

        /*
                 [
          {
            "op": "replace",
            "path": "/productQuantity",
            "value": 25
          }
        ],**/

        [HttpPatch("{id}")]
        public IActionResult Patch(int id, [FromBody] JsonPatchDocument<Product> patchDoc)
        {
            var product = productRepository.GetProductById(id);

            if (product == null)
            {
                return NotFound();
            }

            try
            {
                patchDoc.ApplyTo(product);
            }
            catch (Exception)
            {
                return BadRequest();
            }

            try
            {
                productRepository.UpdateProduct(product);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }


        // DELETE: api/product/1
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                productRepository.DeleteProduct(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
