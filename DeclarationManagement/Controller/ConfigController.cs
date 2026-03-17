using AutoMapper;
using DeclarationManagement.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DeclarationManagement.Model.DTO;

namespace DeclarationManagement.Controller;

[Route("api/[controller]")]
[ApiController]
public class ConfigController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public ConfigController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// 是否在申请时间段内
    /// </summary>
    /// <returns></returns>
    [HttpGet("GetIsApplicationPeriod")]
    public async Task<ActionResult> GetIsApplicationPeriod()
    {
        // 获取本机当前的年月日
        var currentDate = DateTime.Now.Date;
        // 从数据库中获取最后一条配置记录（假设 ConfigID 较大的记录为最新记录）
        var config = await _context.Configs
            .OrderByDescending(c => c.ConfigID)
            .FirstOrDefaultAsync();

        if (config == null)
        {
            return Ok(new { isApplicationPeriod = true });
        }

        // 使用 Date 属性来保证只比较日期部分
        bool isWithinPeriod =
            currentDate.Date >= config.ApplicationStartDate && currentDate.Date <= config.ApplicationEndDate;

        return Ok(isWithinPeriod);
    }

    /// <summary>
    /// 是否在申请时间段内
    /// </summary>
    /// <returns></returns>
    [HttpPost("SetApplicationPeriod")]
    public async Task<ActionResult> SetApplicationPeriod([FromBody] ConfigDTO configDto)
    {
        var config = _context.Configs.FirstOrDefault();
        if (config == null)
        {
            var config1 = _mapper.Map<Config>(configDto);
            await _context.Configs.AddAsync(config1);
        }
        else
        {
            configDto.ConfigID = 1;
            _mapper.Map(configDto, config);
        }
        await _context.SaveChangesAsync();

        return Ok("设置申请时间段成功");
    }

    /// <summary>
    /// 是否在申请时间段内
    /// </summary>
    /// <returns></returns>
    [HttpGet("GetApplicationPeriod")]
    public async Task<ActionResult> GetApplicationPeriod()
    {
        // 从数据库中获取最后一条配置记录（假设 ConfigID 较大的记录为最新记录）
        var config = await _context.Configs
            .OrderByDescending(c => c.ConfigID)
            .FirstOrDefaultAsync();

        if (config == null)
        {
            return Ok("暂未设置申请时间段");
        }

        return Ok(config);
    }
}
