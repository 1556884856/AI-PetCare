using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetCare.Core.Entities;

/// <summary>
/// 所有实体的抽象基类，包含公共属性 Id 和创建时间。
/// </summary>
public abstract class BaseEntity
{
    /// <summary>实体唯一标识</summary>
    public int Id { get; set; }
    
    /// <summary>记录创建时间（UTC）</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 软删除接口，标记实体是否已被逻辑删除。
/// 实现此接口的实体在查询时可通过全局过滤器隐藏已删除记录。
/// </summary>
public interface ISoftDelete
{
    /// <summary>是否已被软删除</summary>
    bool IsDeleted { get; set; }
}
