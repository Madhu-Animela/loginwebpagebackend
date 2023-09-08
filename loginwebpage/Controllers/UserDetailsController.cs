using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace loginwebpage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserDetailsController : ControllerBase
    {
        private readonly LoginwebpageContext _context;
        public UserDetailsController(LoginwebpageContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("signup")]

        public async Task<IActionResult> signup(Customer userDetails)
        {
            var user=await _context.Customers.FirstOrDefaultAsync(x=>x.Email == userDetails.Email);
            if (user != null)
            {
                return BadRequest(userDetails);
            }
            else {
                await _context.Customers.AddAsync(userDetails);
                await _context.SaveChangesAsync();
                return Ok(userDetails);
            }
        }

        [HttpPost("forgot")]
        public async Task<IActionResult> newpassword(ForgotPassword userForgot)
        {
            var userDetails= await _context.Customers.FirstOrDefaultAsync(x=>x.Email == userForgot.Email);
            if(userDetails!=null)
            {
                return Ok(new {prop="Success"});
            }
            return NotFound(new {prop="user is not exits"});
        }

        [HttpPost("login")]
        public async Task<IActionResult> login([FromBody] LoginPage loginDetails)
        {
            var user = await _context.Customers.FirstOrDefaultAsync(x => x.Email == loginDetails.Email);
            if(user != null)
            {
                if(user.Password == loginDetails.Password)
                {
                    return Ok(new { prop = "Login Successful" });
                }
                return BadRequest(new {prop = "Incorrect Password"});
            }
            return NotFound(new { prop = "Invalid Email" });
        }

        [HttpPost("newpassword")]
        public async Task<IActionResult> newpassword([FromBody] NewPassword userNewPassword)
        {
            var useDetails=await _context.Customers.FirstOrDefaultAsync(x=>x.Email==userNewPassword.Email);
            if(useDetails!=null) {
                useDetails.Password = userNewPassword.Password;
                _context.Entry(useDetails).State=EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(new { prop = "Success" });
            }
            return NotFound(new {prop = "Invalid credentials"});
        }
    }
}

