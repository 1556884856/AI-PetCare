namespace PetCare.Core.Entities;

/// <summary>
/// 宠物实体，记录用户的宠物信息。
/// 使用软删除（IsDeleted）而非物理删除，通过全局查询过滤器隐藏已删除宠物。
/// </summary>
public class Pet
{
    /// <summary>宠物ID</summary>
    public int Id { get; set; }
    
    /// <summary>所属用户ID</summary>
    public int UserId { get; set; }
    
    /// <summary>宠物名字</summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>宠物类型（Dog=狗狗, Cat=猫咪）</summary>
    public string Type { get; set; } = "Dog";
    
    /// <summary>品种（如：金毛、英短）</summary>
    public string Breed { get; set; } = string.Empty;
    
    /// <summary>年龄（月）</summary>
    public int Age { get; set; }
    
    /// <summary>体重（kg）</summary>
    public decimal Weight { get; set; }
    
    /// <summary>备注信息（如特殊需求）</summary>
    public string Notes { get; set; } = string.Empty;
    
    /// <summary>是否已被软删除</summary>
    public bool IsDeleted { get; set; } = false;
    
    /// <summary>创建时间</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>所属用户导航属性</summary>
    public User User { get; set; } = null!;
    
    /// <summary>关联的预约记录</summary>
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
