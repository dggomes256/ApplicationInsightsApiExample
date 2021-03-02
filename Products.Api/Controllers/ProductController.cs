using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Products.Domain.Entities;
using Products.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Products.Api.Controllers
{
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private IProductRepository _productRepository;
        IConfiguration _configuration;
        private ILogger<ProductController> _logger;
        public ProductController(IProductRepository productRepository, ILogger<ProductController> logger, IConfiguration configuration)
        {
            _productRepository = productRepository;
            _logger = logger;
            _configuration = configuration;

        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        public async Task<IActionResult> Get()
        {

            return Ok(await _productRepository.Get());
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody]Product product)
        {
            try
            {
                await using (ServiceBusClient client = new ServiceBusClient(_configuration.GetSection("ProductsServiceBus:ConnectionString").Value))
                {
                    // create a sender for the queue 
                    ServiceBusSender sender = client.CreateSender(_configuration.GetSection("ProductsServiceBus:CreateProductQueueName").Value);

                    // create a message that we can send
                    ServiceBusMessage message = new ServiceBusMessage(JsonConvert.SerializeObject(product));

                    // send the message
                    await sender.SendMessageAsync(message);
                }
                return Ok("Solicitação de criação efetuada com sucesso");
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }
    }
}
