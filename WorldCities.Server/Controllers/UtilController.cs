using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Management;
using WorldCities.Server.Data;
using WorldCities.Server.Data.Models;

namespace WorldCities.Server.Controllers {
    //[Authorize(Roles = "Administrator")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UtilController : ControllerBase {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;
        public UtilController(
        ApplicationDbContext context ,
        RoleManager<IdentityRole> roleManager ,
        UserManager<ApplicationUser> userManager ,
        IWebHostEnvironment env ,
        IConfiguration configuration) {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _env = env;
            _configuration = configuration;
        }
        [HttpGet]
        public async Task<ActionResult> Mgmt() {

            ManagementObjectSearcher objOSDetails = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
            ManagementObjectCollection osDetailsCollection = objOSDetails.Get();

            var columns = new Dictionary<string , string> { { "==ManagementObjectSearcher==" , "==Details====================" } };
            int i = 1;
            foreach (ManagementObject mo in osDetailsCollection) {
                int j = 0;
                foreach (PropertyData prop in mo.Properties) {
                    var name = $"item[{i}x{++j}]({prop.Name})";
                    var value = $"{prop.Value}";
                    columns.Add(name , value);
                }
                i++;
            }
            columns.Add("==ManagementClass========" , "==Details====================");
            ManagementClass mc = new ManagementClass("win32_processor");
            ManagementObjectCollection moc = mc.GetInstances();

            foreach (ManagementObject mo in moc) {
                int j = 0;
                foreach (PropertyData prop in mo.Properties) {
                    var name = $"item[{i}x{++j}]({prop.Name})";
                    var value = $"{prop.Value}";
                    columns.Add(name , value);
                }
                i++;
            }


            return new JsonResult(columns);

        }
        [HttpGet]
        public async Task<ActionResult> Env() {
            return new JsonResult(new {
                MachineName = System.Environment.MachineName ,
                UserName = System.Environment.UserName ,
                ProcessPath = System.Environment.ProcessPath ,
                CurrentDirectory = System.Environment.CurrentDirectory ,
                Is64BitProcess = System.Environment.Is64BitProcess ,
            });
        }

    }
}

