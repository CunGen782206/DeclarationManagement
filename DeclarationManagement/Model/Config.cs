using System.ComponentModel.DataAnnotations.Schema;

namespace DeclarationManagement.Model;

public class Config
{
    /// <summary> 用户ID </summary>
    public int ConfigID { get; set; }


    /// <summary>
    /// 起始的日期
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime ApplicationStartDate { get; set; }


    /// <summary>
    /// 终点的日期
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime ApplicationEndDate { get; set; }

}