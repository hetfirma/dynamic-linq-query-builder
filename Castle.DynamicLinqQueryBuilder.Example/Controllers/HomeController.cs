using Castle.DynamicLinqQueryBuilder.Example.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Castle.DynamicLinqQueryBuilder.Example.Sample;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Reflection;
using System.Runtime.Serialization;

namespace Castle.DynamicLinqQueryBuilder.Example.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public class LowercaseNamingPolicy : JsonNamingPolicy
        {
            public override string ConvertName(string name)
            {
                return name.ToLowerInvariant(); // Convert the property name to lowercase
            }
        }

        public IActionResult Index()
        {
            var options = new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                IncludeFields = true,
                WriteIndented = false,
                TypeInfoResolver = System.Text.Json.Serialization.Metadata.DataContractResolver.Default,
            };

            var definitions = typeof(PersonRecord).GetDefaultColumnDefinitionsForType(true);
            var people = PersonBuilder.GetPeople().Take(2);

            //Augment the definitions to show advanced scenarios not
            //handled by GetDefaultColumnDefinitionsForType(...)

            //Let's tweak the generated definition of FirstName to make it
            //a select element in jQuery QueryBuilder UI populated by
            //the possible values from our dataset
            var firstName = definitions.First(p => p.Field.ToLower() == "firstname");
            firstName.Values = people.Select(p => p.FirstName).Distinct().ToList();
            firstName.Input = "select";

            var birthday = definitions.First(p => p.Field.ToLower() == "birthday");
            birthday.Plugin = "datepicker";
            birthday.Plugin_config = new
            {
                format = "mm/dd/yyyy",
                todayBtn = "linked",
                todayHighlight = true,
                autoclose = true
            };
            var jsonRaw = JsonSerializer.Serialize(definitions, options);

            ViewBag.FilterDefinition = JsonSerializer.Serialize(definitions, options);

            ViewBag.Model = people;
            return View();
        }

        [HttpPost]
        public JsonResult Index([FromBody]QueryBuilderFilterRule obj)
        {
            var rules = obj.Rules;

            var people = PersonBuilder.GetPeople().BuildQuery(obj).ToList();
            return Json(people);
        }

        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


     
    }
}