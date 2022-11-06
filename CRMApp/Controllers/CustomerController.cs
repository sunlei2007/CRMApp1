using CRMApp.Common;
using CRMAppEntity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace CRMApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        [HttpPost("GetCustomers")]
        public async Task<JsonTemplate<List<Customer>>> GetCustomers([FromBody] Customer customer)
        {
            List<Customer> result;
            using (DBContext ctx = new())
            {

               var exp = from e in ctx.CustomerSet
                         select e;
                customer.Name = customer.Name.Trim();
                customer.Address = customer.Address.Trim();
                if (customer.Name != "" && customer.Name.Length>0)
                   exp = exp.Where(e => e.Name.Contains(customer.Name));
                if (customer.Address != "" && customer.Address.Length > 0)
                   exp = exp.Where(e => e.Address.Contains(customer.Address));

                result = await exp.ToListAsync();
            }

            return new JsonTemplate<List<Customer>> { StatusCode = "200", Msg = "success", Content = result ?? new List<Customer>() };

        }
    }
}
