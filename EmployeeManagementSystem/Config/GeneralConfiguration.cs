using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagementSystem.Config
{
	public class GeneralConfiguration: IGeneralConfiguration
    {
        public string APIToken { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = string.Empty; 
    }
}
