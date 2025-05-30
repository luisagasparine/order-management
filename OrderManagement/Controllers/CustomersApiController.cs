using Microsoft.AspNetCore.Mvc;
using OrderManagement.Models;
using OrderManagement.Services.Interfaces;
using System.Data;

namespace OrderManagement.Controllers
{
    [ApiController]
    [Route("api/customers")]
    public class CustomersApiController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersApiController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        // GET: api/customers?name={name}&email={email}
        [HttpGet]
        public async Task<ActionResult> GetCustomers(string name = "", string email = "")
        {
            try
            {
                var customers = await _customerService.GetCustomersByNameOrEmail(name, email);
                return Ok(customers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro ao recuperar os clientes: {ex.Message}" });
            }
        }

        // GET: api/customers/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult> GetCustomer(int id)
        {
            try
            {
                var customer = await _customerService.GetCustomerById(id);

                if (customer == null)
                    return NotFound(new { message = $"Cliente com ID {id} não encontrado." });

                return Ok(customer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro ao recuperar o cliente: {ex.Message}" });
            }
        }

        // PUT: api/customers/
        [HttpPut]
        public async Task<ActionResult> UpdateCustomer(Customer customer)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Os dados do cliente são inválidos." });
            
            try
            {
                var updatedCustomer = await _customerService.UpdateCustomer(customer);

                if (updatedCustomer == null)
                    return NotFound(new { message = $"Cliente com ID {customer.Id} não encontrado." });
                
                return NoContent(); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro ao atualizar o cliente: {ex.Message}" });
            }
        }

        // POST: api/customers/
        [HttpPost]
        public async Task<ActionResult> CreateCustomer(Customer customer)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Os dados do cliente são inválidos." });

            try
            {
                await _customerService.CreateCustomer(customer);
                return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro ao cadastrar o cliente: {ex.Message}" });
            }
        }

        // DELETE: api/customers/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCustomer(int id)
        {
            try
            {
                await _customerService.DeleteCustomer(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro ao excluir o cliente: {ex.Message}" });
            }
        }
    }
}
