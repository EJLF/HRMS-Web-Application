using HRMS_Web_Application.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace HRMS_Web_Application.Controllers
{
    public class EmployeeController : Controller
    {
        public static string baseUrl = "http://localhost:5237/api/ApplicationUser";
        HttpClient client = new HttpClient();
        public void SetupHttpRequestHeaders()
        {
            string accessToken = HttpContext.Session.GetString("JWToken");
            string apiKey = HttpContext.Session.GetString("ApiKey");
            client.DefaultRequestHeaders.Add("ApiKey", apiKey);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        public async Task SetupViewBag()
        {
            SetupHttpRequestHeaders();
            string jsonStrDepartment = await client.GetStringAsync("http://localhost:5237/api/Department");
            var result = JsonConvert.DeserializeObject<List<Department>>(jsonStrDepartment).ToList();
            List<SelectListItem> departmentList = result.Select(d => new SelectListItem
            {
                Value = d.DeptId.ToString(),
                Text = d.DeptName
            }).ToList();

            ViewBag.DepartmentNames = departmentList;

            string jsonStrPosition = await client.GetStringAsync("http://localhost:5237/api/Position");
            var resultpos = JsonConvert.DeserializeObject<List<Position>>(jsonStrPosition).ToList();
            List<SelectListItem> positionList = resultpos.Select(d => new SelectListItem
            {
                Value = d.PosId.ToString(),
                Text = d.positionName
            }).ToList();

            ViewBag.DepartmentList = departmentList;
            ViewBag.PositionList = positionList;
        }

        public async Task<IActionResult> List()
        {
            try
            {
                var result = await GetApplicationUser();
                return View(result);
            }
            catch (Exception ex)
            {
                TempData["HRMSAlert"] = "Error, Please Try Again!" + ex.Message;
                return RedirectToAction("Unauthorized", "Home");
            }
        }

        [HttpGet]
        public async Task<List<ApplicationUser>> GetApplicationUser()
        {
            SetupHttpRequestHeaders();
            string jsonStr = await client.GetStringAsync(baseUrl);
            var result = JsonConvert.DeserializeObject<List<ApplicationUser>>(jsonStr).ToList();

            return result;
        }

        public async Task<IActionResult> Create()
        {
            await SetupViewBag();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegisterEmployeeViewModel registerEmployee)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await SetupViewBag();

                    var stringContent = new StringContent(JsonConvert.SerializeObject(registerEmployee), Encoding.UTF8, "application/json");
                    //?FirstName=ff
                    //&MiddleName=ff
                    //&LastName=ff
                    //&Gender=Male
                    //&DateOfBirth=04%2F19%2F2323%2010%3A30%3A00
                    //&Phone=09121213341
                    //&Email=r%40r.com
                    //&DepartmenttId=1
                    //&PosistionId=1
                    //&EmployeeType=ff
                    //&Street=ff
                    //&Barangay=ff
                    //&City=ff
                    //&State=ff
                    //&PostalCode=1234
                    //&DateHired=04%2F19%2F2323%2010%3A30%3A00
                    //&activeStatus=true
                    //&deleteStatus=false
                    //&Password=123123123
                    //http://localhost:5237/api/ApplicationUser?FirstName=EarlJoseph&MiddleName=Litong&LastName=Ferran&Gender=Male&DateOfBirth=5/15/2023 12:00:00 AM&Phone=09123812739&Email=earljosephferran121999@gmail.com&DepartmenttId=1&PosistionId=1&EmployeeType=Regular&Street=1234&Barangay=San Mariano&City=Roxas&State=Oriental Mindoro&PostalCode=1234&DateHired=5/22/2023 12:00:00 AM&activeStatus=true&deleteStatus=false&Password=123123123
                    var url = string.Format(baseUrl + "?FirstName=" + registerEmployee.FirstName +
                                                      "&MiddleName=" + registerEmployee.MiddleName +
                                                      "&LastName=" + registerEmployee.LastName +
                                                      "&Gender=" + registerEmployee.Gender +
                                                      "&DateOfBirth=" + registerEmployee.DateOfBirth +
                                                      "&Phone=" + registerEmployee.Phone +
                                                      "&Email=" + registerEmployee.Email +
                                                      "&DepartmenttId=" + registerEmployee.DepartmentId +
                                                      "&PosistionId=" + registerEmployee.PositionId +
                                                      "&EmployeeType=" + registerEmployee.EmployeeType +
                                                      "&Street=" + registerEmployee.Street +
                                                      "&Barangay=" + registerEmployee.Barangay +
                                                      "&City=" + registerEmployee.City +
                                                      "&State=" + registerEmployee.State +
                                                      "&PostalCode=" + registerEmployee.PostalCode +
                                                      "&DateHired=" + registerEmployee.DateHired +
                                                      "&activeStatus=true" +
                                                      "&deleteStatus=false" +
                                                      "&Password=" + registerEmployee.Password);

                    var response = client.PostAsync(url, stringContent).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["ApplicationUserAlert"] = "Save Successfull!";
                        return RedirectToAction("List");
                    }
                    TempData["ApplicationUserAlert"] = "Failed to Save! " + response.StatusCode;
                    return View();
                }
                else
                {
                    await SetupViewBag();
                    return View(); 
                }
            }
            catch (Exception ex)
            {
                TempData["ApplicationUserAlert"] = "Error, Please Try Again! " + ex.Message;
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(string accountId)
        {
            try
            {
                //http://localhost:5237/api/ApplicationUser/1d779ffa-42d2-4ccb-a6a7-1599ae0325ed
                await SetupViewBag();
                //SetupHttpRequestHeaders();
                var response = client.GetAsync(baseUrl + "/" + accountId).Result;
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    EditEmployeeViewModel editEmployee = JsonConvert.DeserializeObject<EditEmployeeViewModel>(data);
                    return View(editEmployee);
                }
                return View();
            }
            catch (Exception ex)
            {
                TempData["ApplicationUserAlert"] = "Error, Please Try Again! " + ex.Message;
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Details(EditEmployeeViewModel editEmployee)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    SetupHttpRequestHeaders();
                    string data = JsonConvert.SerializeObject(editEmployee);
                    StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                    var url = string.Format(baseUrl + "/" + editEmployee.Id +
                                                      "?FirstName=" + editEmployee.FirstName +
                                                      "&MiddleName=" + editEmployee.MiddleName +
                                                      "&LastName=" + editEmployee.LastName +
                                                      "&Gender=" + editEmployee.Gender +
                                                      "&DateOfBirth=04%2F12%2F3043" +
                                                      "&Phone=" + editEmployee.Phone +
                                                      "&Email=d%40d.com" + editEmployee.Email +
                                                      "&EmployeeType=try" +
                                                      "&Street=" + editEmployee.Street +
                                                      "&Barangay=" + editEmployee.Barangay +
                                                      "&City=" + editEmployee.FirstName +
                                                      "&State=" + editEmployee.City +
                                                      "&PostalCode=" + editEmployee.PostalCode +
                                                      "&DateHired=04%2F12%2F3043" +
                                                      "&activeStatus=true" +
                                                      "&deleteStatus=false");

                    var response = client.PutAsync(url, content).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var responsecontent = response.Content.ReadAsStringAsync().Result;
                        TempData["ApplicationUserAlert"] = responsecontent;
                        return RedirectToAction("List");
                    }
                    TempData["ApplicationUserAlert"] = "Error, Please Try Again! " + response.StatusCode;
                    return RedirectToAction("List");
                }
                else
                {
                    TempData["ApplicationUserAlert"] = "Error, Please Try Again! ";
                    await SetupViewBag();
                    return View();
                }
            }
            catch (Exception ex)
            {
                TempData["ApplicationUserAlert"] = "Error, Please Try Again! " + ex.Message;
                return View();
            }
        }
    }
}
