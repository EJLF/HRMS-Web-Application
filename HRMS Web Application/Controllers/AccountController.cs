using HRMS_Web_Application.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text;

namespace HRMS_Web_Application.Controllers
{
    public class AccountController : Controller
    {
        public string baseUrl = "http://localhost:5237/api/Account";
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    string apiKey = "123qwe";
                    httpClient.DefaultRequestHeaders.Add("ApiKey", apiKey);  
                    StringContent stringContent = new StringContent(JsonConvert.SerializeObject(loginModel), Encoding.UTF8, "application/json");
                                                     
                    using (var response = await httpClient.PostAsync(baseUrl+"?UserName=" +loginModel.UserName+"&Password="+loginModel.Password, stringContent))
                    {
                        string token = await response.Content.ReadAsStringAsync();
                        token = token.Replace("{\"token\":\"", "").Replace("\"}", "");

                        var jwtToken = new JwtSecurityToken(token);
                        var role = jwtToken.Claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
                        var username = jwtToken.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");

                        if (!response.IsSuccessStatusCode)
                        {
                            ViewBag.Message = "Invalid credentials";
                            return Redirect("~/Account/Login");
                        }
                        HttpContext.Session.SetString("JWToken", token);
                        HttpContext.Session.SetString("ApiKey", apiKey);
                    }
                }
                return RedirectToAction("List", "Employee");
                // return Redirect("~/Department/Index");
            }
            catch (Exception ex)
            {
                TempData["AccountAlert"] = "Error, Please Try Again!" + ex.Message;
                return Redirect("~/Account/Login");
            }
        }
    }
}
