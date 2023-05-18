using HRMS_Web_Application.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace HRMS_Web_Application.Controllers
{
    public class PositionController : Controller
    {
        public static string baseUrl = "http://localhost:5237/api/Position";
        HttpClient client = new HttpClient();
        public void SetupHttpRequestHeaders()
        {
            string accessToken = HttpContext.Session.GetString("JWToken");
            string apiKey = HttpContext.Session.GetString("ApiKey");
            client.DefaultRequestHeaders.Add("ApiKey", apiKey);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Position position)
        {
            try
            {
                SetupHttpRequestHeaders();

                var stringContent = new StringContent(JsonConvert.SerializeObject(position), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(baseUrl + "?PositionName=" + position.positionName, stringContent);
                if (response.IsSuccessStatusCode)
                {
                    TempData["PositionAlert"] = "Save Successfull!";
                    return RedirectToAction("List");
                }
                TempData["PositionAlert"] = "Failed";
                return View();
            }
            catch (Exception ex)
            {
                TempData["PositionAlert"] = "Error, Please Try Again!\n\n" + ex.Message;
                return View();
            }
        }
        public async Task<IActionResult> List()
        {
            try
            {
                var pos = await GetPosition();
                return View(pos);
            }
            catch (Exception ex)
            {
                TempData["HRMSAlert"] = "Error, Please Try Again!" + ex.Message;
                return RedirectToAction("Unauthorized", "Home");
            }
        }
        [HttpGet]
        public async Task<List<Position>> GetPosition()
        {
            SetupHttpRequestHeaders();
            string jsonStr = await client.GetStringAsync(baseUrl);
            var result = JsonConvert.DeserializeObject<List<Position>>(jsonStr).ToList();
            return result;
        }

        [HttpGet]
        public async Task<IActionResult> Update(int posId)
        {
            try
            {
                SetupHttpRequestHeaders();
                var response = client.GetAsync(baseUrl + "/" + posId).Result;
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    Position dept = JsonConvert.DeserializeObject<Position>(data);
                    return View(dept);
                }
                return View();
            }
            catch (Exception ex)
            {
                TempData["PositionAlert"] = "Error, Please Try Again!" + ex.Message;
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update(Position position, int posId)
        {
            try
            {
                SetupHttpRequestHeaders();
                string data = JsonConvert.SerializeObject(position);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                var url = string.Format(baseUrl + "?id={0}&PositionName={1}", posId, position.positionName);
                var response = client.PutAsync(url, content).Result;
                if (response.IsSuccessStatusCode)
                {
                    var responsecontent = response.Content.ReadAsStringAsync().Result;
                    Position todo = JsonConvert.DeserializeObject<Position>(responsecontent);
                    TempData["PositionAlert"] = "Update Successfull!";
                    return RedirectToAction("List");
                }
                return View();
            }
            catch (Exception ex)
            {
                TempData["PositionAlert"] = "Error, Please Try Again!" + ex.Message;
                return View();
            }
        }

        public IActionResult Delete(int PosId)
        {
            try
            {
                SetupHttpRequestHeaders();
                var url = string.Format(baseUrl + "?id={0}", PosId);
                var response = client.DeleteAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["PositionAlert"] = "Delete Successfull!";
                    return RedirectToAction("List");
                }
                TempData["PositionAlert"] = "Error, Please Try Again!";
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                TempData["PositionAlert"] = "Error, Please Try Again!\n\n" + ex.Message;
                return RedirectToAction("List");
            }
        }
    }
}
