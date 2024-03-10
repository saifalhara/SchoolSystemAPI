using Contracts.DTO;
using Contracts.Requests;
using DataAccess.Repository;
using Microsoft.AspNetCore.Mvc;
using Model.Extention;
using Model.Models;
namespace SchoolSystemAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmployeeController(IEmployeeRepository employeeRepository) : ControllerBase
{

    [HttpPost("addEmployee")]
    public async Task<ActionResult> AddEmployee([FromForm] EmployeeRequest employeeRequest)
    {
        Employee employee = new Employee();
        employee.Phone = employeeRequest.Phone;
        employee.Name = employeeRequest.Name;    
        employee.Email = employeeRequest.Email;

        string path = ExtintionClass.SavePhoto(employeeRequest.formFile);
        if (path == null)
        {
            return BadRequest("An error occurred while uploading the file.");
        }
        employee.PhotoULR = path;
        employee.PhotoBase64 = ExtintionClass.ConvertToBase64(employeeRequest.formFile);
        await employeeRepository.AddEmployee(employee);

        return Created();
    }



    [HttpPut("AddRoleToEmployee/{id}/{roleId}")]
    public async Task<ActionResult> AddRoleToEmployee(int id, int roleId)
    {
        if (id == 0 || roleId == 0)
        {
            return BadRequest("Wrong Data! (Id/role) cannot be 0 ");
        }

        if (await employeeRepository.AddRoleToEmployee(id, roleId))
        {
            return NoContent();
        }
        return BadRequest("Role Does Not Changw");
    }


    [HttpGet("getAllEmployees/{index}/{number}")]
    public async Task<ActionResult> GetEmployees(int index, int number)
    {
        List<Employee> employees = await employeeRepository.GetAllEmployees(index, number);
        List<EmployeeDTO> employeeDTO = new List<EmployeeDTO>();

        foreach (Employee e in employees)
        {
            EmployeeDTO employeeDto = new EmployeeDTO();
            employeeDto.EmployeeId = e.EmployeeId;
            employeeDto.Name = e.Name;
            employeeDto.Email = e.Email;
            employeeDto.Phone = e.Phone;
            employeeDto.PhotoULR = e.PhotoULR;
            employeeDto.PhotoBase64 = e.PhotoBase64;
            employeeDto.RoleDTO = e.Roles.Select(r => new RoleDTO
            {
                RoleId = r.RoleId,
                Description = r.Description,
                RoleName = r.RoleName,
                IsDeleted = r.IsDeleted
            }).ToList();
            employeeDTO.Add(employeeDto);
        }
        return Ok(employeeDTO);
    }


    [HttpDelete("deleteEmployee/{id}")]
    public async Task<ActionResult> DeleteEmployee(int id)
    {
        try
        {
            bool isDelete = await employeeRepository.DeleteEmployee(id);
            if (isDelete)
            {
                return Ok("Employee deleted successfully!");
            }
            return BadRequest("This employee does not exist!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("getEmployeeById/{id}")]
    public async Task<ActionResult> GetEmployee(int id)
    {
        var employee = await employeeRepository.GetEmployee(id);
        if (employee == null)
        {
            return BadRequest("Employee does not found!");
        }
        EmployeeDTO employeeDto = new EmployeeDTO();
        employeeDto.Name = employee.Name;
        employeeDto.Email = employee.Email;
        employeeDto.Phone = employee.Phone;
        employeeDto.PhotoULR = employee.PhotoULR;
        employeeDto.PhotoBase64 = employee.PhotoBase64;
        employeeDto.RoleDTO = employee.Roles.Select(r => new RoleDTO
        {
            RoleId = r.RoleId,
            Description = r.Description,
            RoleName = r.RoleName,
            IsDeleted = r.IsDeleted
        }).ToList();
        return Ok(employeeDto);
    }
}
