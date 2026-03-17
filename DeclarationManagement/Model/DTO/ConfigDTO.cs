using System.ComponentModel.DataAnnotations.Schema;

namespace DeclarationManagement.Model.DTO;

public class ConfigDTO
{
    /// <summary> 用户ID </summary>
    public int ConfigID { get; set; }


    /// <summary>
    /// 起始的日期
    /// </summary>
    public DateTime ApplicationStartDate { get; set; }


    /// <summary>
    /// 终点的日期
    /// </summary>
    public DateTime ApplicationEndDate { get; set; }

}