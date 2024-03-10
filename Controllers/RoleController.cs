using Contracts.DTO;
using Contracts.Requests;
using DataAccess.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.Models;

namespace SchoolSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleRepository RoleRepository;
        public RoleController(IRoleRepository roleRepository)
        {
            this.RoleRepository = roleRepository;
        }
        [HttpPost("addRole")]
        public async Task<ActionResult> AddRole([FromBody] RoleRequest RoleRequest)
        {
            if (RoleRequest == null)
            {
                return BadRequest("Wrong Data!");
            }
            var isAdd = await RoleRepository.AddRole(RoleRequest);
            if(isAdd)
            return Created();
            return BadRequest("Error in adding role"); 

        }

        [HttpGet("getAllRole")]
        public async Task<ActionResult> GetAllRole(int index, int number)
        {
            var role = await RoleRepository.GetRoles(index, number);
            List<RoleDTO> roleDTOs = new List<RoleDTO>();
            foreach (Role r in role)
            {
                RoleDTO roleDTO = new RoleDTO();
                roleDTO.IsDeleted = false;
                roleDTO.Description = r.Description;
                roleDTO.RoleName = r.RoleName;
                roleDTO.RoleId = r.RoleId;
                roleDTO.employeeDTOs = r.Employees.Select(e => new EmployeeDTO
                {
                    EmployeeId = e.EmployeeId,
                    Name = e.Name,
                    Email = e.Email,
                    Phone = e.Phone,
                    PhotoBase64 = e.PhotoBase64,
                    PhotoULR = e.PhotoULR
                }
                    ).ToList();
                roleDTOs.Add(roleDTO);
            }
            return Ok(roleDTOs);

        }

        [HttpDelete("deleteRole/{id}")]
        public async Task<ActionResult> DeleteRole(int id)
        {
            var role = RoleRepository.DeleteRole(id);
            if (role == false)
            {
                return BadRequest("This role does not found");
            }
            return Ok("The role Deleted successgfully");
        }

        [HttpPut("editRole")]
        public async Task<ActionResult> EditRole([FromBody]RoleRequest roleRequest)
        {
            var role = RoleRepository.EditRole(roleRequest);
            if (role == false)
                return BadRequest("there is error when edit role ");
            return Ok("Role Updated successfuly");
        }
    }
}