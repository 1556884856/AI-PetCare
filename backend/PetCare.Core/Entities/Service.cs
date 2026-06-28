namespace PetCare.Core.Entities;

/// <summary>
/// 服务项目实体，定义宠物洗护服务的内容和价格。
/// 支持按类别（洗浴/美容/SPA/基础护理）和宠物类型筛选。
/// </summary>
public class Service
{
    /// <summary>服务ID</summary>
    public int Id { get; set; }
    
    /// <summary>服务名称</summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>服务描述</summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>服务类别（Bath=洗浴, Grooming=美容, Spa=SPA, Basic=基础护理）</summary>
    public string Category { get; set; } = "Bath";
    
    /// <summary>适用宠物类型（Dog/ Cat/ All）</summary>
    public string PetType { get; set; } = "All";
    
    /// <summary>服务价格</summary>
    public decimal Price { get; set; }
    
    /// <summary>服务时长（分钟）</summary>
    public int DurationMinutes { get; set; }
    
    /// <summary>服务图片URL</summary>
    public string ImageUrl { get; set; } = string.Empty;
    
    /// <summary>排序序号，数值越小越靠前</summary>
    public int SortOrder { get; set; }
    
    /// <summary>是否启用（可下架服务）</summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>创建时间</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>关联的预约记录</summary>
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
