using Customers.Api.Contracts.Requests;
using Customers.Api.Mapping;
using Customers.Api.Repositories;
using Customers.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Customers.Api.Controllers;

[ApiController]
public class CustomerController : ControllerBase
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerController(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    [HttpPost("customers")]
    public async Task<IActionResult> Create([FromBody] CustomerRequest request)
    {
        var customer = request.ToCustomer();
        var customerDto = DomainToDtoMapper.ToCustomerDto(customer);

        await _customerRepository.CreateAsync(customerDto);

        var customerResponse = customer.ToCustomerResponse();

        return CreatedAtAction("Get", new { customerResponse.Id }, customerResponse);
    }

    // [HttpGet("customers/{id:guid}")]
    [HttpGet("customers/{idOrEmail}")]
    public async Task<IActionResult> Get([FromRoute] string idOrEmail)
    {
        var isGuid = Guid.TryParse(idOrEmail, out var id);
        
        var customer = isGuid ? await _customerRepository.GetAsync(id) :
                await _customerRepository.GetByEmailAsync(idOrEmail);

        if (customer is null)
        {
            return NotFound();
        }

        var customerResponse = customer;
        return Ok(customerResponse);
    }
    
    [HttpGet("customers")]
    public async Task<IActionResult> GetAll()
    {
        var customers = await _customerRepository.GetAllAsync();
        var customersResponse = customers; 
            //customers.ToCustomersResponse();
        return Ok(customersResponse);
    }
    
    [HttpPut("customers/{id:guid}")]
    public async Task<IActionResult> Update(
        [FromMultiSource] UpdateCustomerRequest request)
    {
        var existingCustomer = await _customerRepository.GetAsync(request.Id);

        if (existingCustomer is null)
        {
            return NotFound();
        }

        var customer = request.ToCustomer();
        var customerDto = DomainToDtoMapper.ToCustomerDto(customer);
        
        await _customerRepository.UpdateAsync(customerDto);

        var customerResponse = customer.ToCustomerResponse();
        return Ok(customerResponse);
    }
    
    [HttpDelete("customers/{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var deleted = await _customerRepository.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return Ok();
    }
}
