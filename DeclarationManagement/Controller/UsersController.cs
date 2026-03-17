using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DeclarationManagement.Model;
using DeclarationManagement.Model.DTO;

//非暴露接口不能使用 public
namespace DeclarationManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UsersController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        /// <summary>
        /// 获得所有的用户
        /// </summary>
        /// <returns></returns>
        [HttpGet("getUsers")]
        public async Task<ActionResult> GetUsers([FromQuery] string name = "")
        {
            if (string.IsNullOrEmpty(name))
            {
                var users = await _context.Users.ToListAsync();
                // 使用 AutoMapper 映射为 UserManageDTO 列表
                var userDTOs = _mapper.Map<List<UserManageDTO>>(users);
                return Ok(userDTOs);
            }
            else
            {
                var users = await _context.Users.Where(u => u.Name == name).ToListAsync();
                // 使用 AutoMapper 映射为 UserManageDTO 列表
                var userDTOs = _mapper.Map<List<UserManageDTO>>(users);
                return Ok(userDTOs);
            }
        }


        /// <summary>
        /// 管理用户
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        [HttpPost("userManage")]
        public async Task<ActionResult> UserManage([FromBody] UserManageDTO userDto)
        {
            if (userDto.UserID == 0)
            {
                return await CreateUser(userDto);
            }
            else
            {
                return await ModifyUser(userDto);
            }
        }

        private async Task<ActionResult> CreateUser(UserManageDTO userDto)
        {
            //查找当前工号是否已经有存在的数据
            var userInDb = await _context.Users.FirstOrDefaultAsync(u => u.JobNumber == userDto.JobNumber);

            if (userInDb != null)
            {
                return Conflict("当前工号已存在");
            }

            if (userDto.Power == nameof(Power.审核用户))
            {
                var userPower = await JudgeUserPower(userDto); //找到对应的审核用户，然后提醒是否替换。
                if (userPower != null)
                {
                    var userDTO = _mapper.Map<UserManageDTO>(userPower); //转换为DTO进行返回
                    return Ok(userDTO); //返回UserDTO
                }
            }

            return await CreateUserConfirm(userDto);
        }

        /// <summary>
        /// 确认创建(强制替换)
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        [HttpPost("userManageConfirm")]
        public async Task<ActionResult> UserManageConfirm([FromBody] UserManageDTO userDto)
        {
            if (userDto.UserID == 0)
            {
                return await CreateUserConfirm(userDto);
            }
            else
            {
                return await ModifyUserConfirm(userDto);
            }
        }

        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        private async Task<ActionResult> CreateUserConfirm(UserManageDTO userDto)
        {
            if (userDto.Power == nameof(Power.审核用户))
            {
                var userPower = await JudgeUserPower(userDto); //找到对应的审核用户，然后替换。
                userPower.Power = nameof(Power.普通用户);
                await _context.SaveChangesAsync();
            }
            // 将 DTO 映射为实体
            var user = _mapper.Map<User>(userDto);
            user.Username = userDto.JobNumber;
            user.Password = ConfigManage.password;
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return Ok();
        }




        /// <summary>
        /// 判断用户的权限是否已经存在
        /// </summary>
        /// <returns></returns>
        private async Task<User> JudgeUserPower(UserManageDTO userDto)
        {
            var userInDb =
                await _context.Users.FirstOrDefaultAsync(u => u.Role == userDto.Role && u.Power == userDto.Power);
            return userInDb;
        }


        // POST: api/Users
        [HttpPost("ResetPassword")]
        public async Task<ActionResult> ResetPassword([FromBody] UserManageDTO userDto)
        {
            var userInDb = await _context.Users.FindAsync(userDto.UserID);
            if (userInDb == null)
            {
                return NotFound("用户不存在");
            }

            userInDb.Password = ConfigManage.password;
            await _context.SaveChangesAsync();

            return Ok("重置密码成功");
        }


        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        private async Task<ActionResult> ModifyUser(UserManageDTO userDto)
        {
            var user = await _context.Users.FindAsync(userDto.UserID);
            if (user == null)
            {
                return NotFound("未找到当前用户");
            }

            var userInDb =
                await _context.Users.FirstOrDefaultAsync(u =>
                    u.JobNumber == userDto.JobNumber && u.UserID != userDto.UserID);
            if (userInDb != null)
            {
                return Conflict("当前工号已存在");
            }

            if (user.Power != nameof(Power.审核用户) && userDto.Power == nameof(Power.审核用户))
            {
                var userPower = await JudgeUserPower(userDto); //找到对应的审核用户，然后提醒是否替换。
                if (userPower != null)
                {
                    var userDTO = _mapper.Map<UserManageDTO>(userPower); //转换为DTO进行返回
                    return Ok(userDTO); //返回UserDTO
                }
            }


            _mapper.Map(userDto, user);
            await _context.SaveChangesAsync();
            // try
            // {
            //     await _context.SaveChangesAsync();
            // }
            // catch (DbUpdateConcurrencyException)
            // {
            //     if (!_context.Users.Any(e => e.JobNumber == userDto.JobNumber))
            //     {
            //         return BadRequest("当前工号已存在");
            //     }
            //     else
            //     {
            //         throw;
            //     }
            // }
            return Ok();
        }

        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        private async Task<ActionResult> ModifyUserConfirm(UserManageDTO userDto)
        {
            var user = await _context.Users.FindAsync(userDto.UserID);
            if (user.Power != nameof(Power.审核用户) && userDto.Power == nameof(Power.审核用户))
            {
                var userPower = await JudgeUserPower(userDto); //找到对应的审核用户，然后提醒是否替换。
                userPower.Power = nameof(Power.普通用户);
                await _context.SaveChangesAsync();
            }

            _mapper.Map(userDto, user);
            await _context.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        [HttpPost("deleteUser")]
        public async Task<ActionResult> DeleteUser([FromBody] UserManageDTO userDto)
        {
            var user = await _context.Users.FindAsync(userDto.UserID);
            if (user == null)
            {
                return NotFound("未找到当前用户");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok("删除用户成功");
        }
    }
}