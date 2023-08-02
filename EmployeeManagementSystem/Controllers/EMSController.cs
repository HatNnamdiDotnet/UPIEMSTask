using CsvHelper;
using CsvHelper.Configuration;
using EmployeeManagementSystem.Config;
using EmployeeManagementSystem.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text;

namespace EmployeeManagementSystem.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EMSController : ControllerBase
    {
        private readonly IGeneralConfiguration _generalConfiguration;

        public EMSController(IGeneralConfiguration generalConfiguration)
        {
            _generalConfiguration = generalConfiguration;
        }

        private HttpClient GetHttpClient()
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _generalConfiguration.APIToken);
            return httpClient;
        }

        [HttpGet]
        [Route("GetAllEmployees")]
        public async Task<IActionResult> GetAllEmployeesAsync()
        {
            using (var httpClient = GetHttpClient())
            {
                var response = await httpClient.GetAsync($"{_generalConfiguration.BaseUrl}users");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var employees = JsonConvert.DeserializeObject<List<Employee>>(content);
                if(employees != null)
                {
                    return Ok(employees);
                }
                return BadRequest("Error occured");
            }
        }

        [HttpGet]
        [Route("GetEmployeesById")]
        public async Task<IActionResult> GetEmployeesByIdAsync(int id)
        {
            if(id > 0)
            {
                using (var httpClient = GetHttpClient())
                {
                    var response = await httpClient.GetAsync($"{_generalConfiguration.BaseUrl}users/" + id);
                    if(response.ReasonPhrase == "Not Found")
                    {
                        return BadRequest("Not found");
                    }
                    var res = response.EnsureSuccessStatusCode();

                    var content = await response.Content.ReadAsStringAsync();
                    var employees = JsonConvert.DeserializeObject<Employee>(content);
                    if (employees != null)
                    {
                        return Ok(employees);
                    }
                    return BadRequest("Not found");
                }
            }
            
            return BadRequest("Enter a valid employee Id");
        }

        [HttpGet]
        [Route("GetEmployeesByFirstName")]
        public async Task<IActionResult> GetEmployeesByFirstNameAsync(string firstName)
        {
            if(firstName != null)
            {
                using (var httpClient = GetHttpClient())
                {
                    var response = await httpClient.GetAsync($"{_generalConfiguration.BaseUrl}users?name=" + firstName);
                    response.EnsureSuccessStatusCode();

                    var content = await response.Content.ReadAsStringAsync();
                    var employees = JsonConvert.DeserializeObject<List<Employee>>(content);
                    if (employees != null)
                    {
                        return Ok(employees);
                    }
                    return BadRequest("Not found");
                }
            }
            return BadRequest("Error occured");
        }

        [HttpDelete]
        [Route("DeleteEmployeesById")]
        public async Task<IActionResult> DeleteEmployeesByIdAsync(int id)
        {
            if (id > 0)
            {
                using (var httpClient = GetHttpClient())
                {
                    var response = await httpClient.DeleteAsync($"{_generalConfiguration.BaseUrl}users/" + id);
                    if (response.IsSuccessStatusCode)
                    {
                        return Ok("Succesful");
                    }
                    return BadRequest(response.ReasonPhrase);
                }
            }
            return BadRequest("Enter employee Id");
        }

        [HttpPut]
        [Route("UpdateEmployeesById")]
        public async Task<IActionResult> UpdateEmployeesByIdAsync(int id, [FromBody] Employee employee)
        {
            if (id > 0 && employee != null)
            {
                using (var httpClient = GetHttpClient())
                {
                    var dataT = JsonConvert.SerializeObject(employee);
                    var contents = new StringContent(dataT, Encoding.Default, "application/json");
                    var response = await httpClient.PutAsync($"{_generalConfiguration.BaseUrl}users/" + id, contents);
                    if (response.ReasonPhrase == "Not Found")
                    {
                        return BadRequest("Not found");
                    }
                    var res = response.EnsureSuccessStatusCode();

                    var content = await response.Content.ReadAsStringAsync();
                    var employees = JsonConvert.DeserializeObject<Employee>(content);
                    if (employees != null)
                    {
                        return Ok(employees);
                    }
                    return BadRequest("Not found");
                }
            }
            return BadRequest("Enter employee Id");
        }
        
        [HttpPost]
        [Route("CreateEmployee")]
        public async Task<IActionResult> CreateEmployeedAsync([FromBody] Employee employee)
        {
            if (employee != null)
            {
                using (var httpClient = GetHttpClient())
                {
                    var dataT = JsonConvert.SerializeObject(employee);
                    var contents = new StringContent(dataT, Encoding.Default, "application/json");
                    var response = await httpClient.PostAsync($"{_generalConfiguration.BaseUrl}users", contents);
                    if (response.ReasonPhrase == "Not Found")
                    {
                        return BadRequest("Not found");
                    }
                    var res = response.EnsureSuccessStatusCode();

                    var content = await response.Content.ReadAsStringAsync();
                    var employees = JsonConvert.DeserializeObject<Employee>(content);
                    if (employees != null)
                    {
                        return Ok(employees);
                    }
                    return BadRequest("Not found");
                }
            }
            return BadRequest("Enter employee Id");
        }

        [HttpGet]
        [Route("ExportEmployeeList")]
        public async Task<IActionResult> ExportToCsv()
        {
            using (var httpClient = GetHttpClient())
            {
                var response = await httpClient.GetAsync($"{_generalConfiguration.BaseUrl}users");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var employees = JsonConvert.DeserializeObject<List<Employee>>(content);
                if (employees != null)
                {
                    List<Employee> data = employees;

                    string fileName = "exported_data.csv";

                    // Get the Downloads folder path
                    string downloadsFolderPath = GetDownloadsFolderPath();

                    // Combine the Downloads folder path with the file name to get the full file path
                    string filePath = Path.Combine(downloadsFolderPath, fileName);

                    // For tab-separated format, change the delimiter to '\t'
                    var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        Delimiter = ","
                    };

                    using (var writer = new StreamWriter(filePath))
                    using (var csv = new CsvWriter(writer, csvConfig))
                    {
                        csv.WriteRecords(data);
                    }
                    return Ok("Exported Successfully");
                }
                return BadRequest("Error occured");
            }
        }

        private string GetDownloadsFolderPath()
        {
            string downloadsFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            // Add the "Downloads" folder name to the path
            downloadsFolderPath = Path.Combine(downloadsFolderPath, "Downloads");

            // Create the "Downloads" folder if it doesn't exist
            if (!Directory.Exists(downloadsFolderPath))
            {
                Directory.CreateDirectory(downloadsFolderPath);
            }

            return downloadsFolderPath;
        }
    }
}
