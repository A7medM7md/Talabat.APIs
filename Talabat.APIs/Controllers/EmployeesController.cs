using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;
using Talabat.Core.Specifications;

namespace Talabat.APIs.Controllers
{
	public class EmployeesController : ApiBaseController
	{
		private readonly IGenericRepository<Employee> _employeeRepo;

		public EmployeesController(IGenericRepository<Employee> employeeRepo)
		{
			_employeeRepo = employeeRepo;
		}

		[HttpGet] // GET : /api/employees
		public async Task<ActionResult<IReadOnlyList<Employee>>> GetEmployees()
		{
			var spec = new EmployeeWithDepartmentSpecifications();

			var employees = await _employeeRepo.GetAllWithSpecAsync(spec);

			return Ok(employees);
		}

		[HttpGet("{id}")] // GET : /api/employees/1
		public async Task<ActionResult<Employee>> GetEmployee(int id)
		{
			var spec = new EmployeeWithDepartmentSpecifications(id);

			var employee = await _employeeRepo.GetEntityWithSpecAsync(spec);

			return Ok(employee);
		}
	}
}
