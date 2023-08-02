using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagementSystem.Config
{
	public interface IGeneralConfiguration
	{
        public string APIToken { get; set; }
        public string BaseUrl { get; set; }
    }
}
