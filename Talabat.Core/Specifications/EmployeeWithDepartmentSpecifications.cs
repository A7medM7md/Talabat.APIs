using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications
{
	public class EmployeeWithDepartmentSpecifications : BaseSpecification<Employee>
	{
		// This Constructor Is Used To "GetAllEmployees()"
		public EmployeeWithDepartmentSpecifications() 
		{
			Includes.Add(E => E.Department);
		}

		// This Constructor Is Used To "GetEmployeeById()"
		public EmployeeWithDepartmentSpecifications(int id) : base(E => E.Id == id)
		{
			Includes.Add(E => E.Department);
		}

	}
}
