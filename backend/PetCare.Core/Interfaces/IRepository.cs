using System.Linq.Expressions;

namespace PetCare.Core.Interfaces;

/// <summary>
/// 通用仓储接口，提供实体的基本 CRUD 操作和条件查询能力。
/// 泛型参数 T 限定为引用类型（class）。
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>根据主键获取实体</summary>
    Task<T?> GetByIdAsync(int id, CancellationToken ct = default);
    
    /// <summary>获取所有实体</summary>
    Task<List<T>> GetAllAsync(CancellationToken ct = default);
    
    /// <summary>根据条件表达式查找实体集合</summary>
    Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    
    /// <summary>根据条件获取第一个匹配的实体</summary>
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    
    /// <summary>判断是否存在满足条件的实体</summary>
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    
    /// <summary>统计满足条件的实体数量</summary>
    Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    
    /// <summary>获取 IQueryable 用于构建复杂查询</summary>
    IQueryable<T> Query();
    
    /// <summary>添加新实体</summary>
    Task AddAsync(T entity, CancellationToken ct = default);
    
    /// <summary>更新实体（标记为已修改）</summary>
    void Update(T entity);
    
    /// <summary>删除实体</summary>
    void Delete(T entity);
    
    /// <summary>保存所有变更到数据库</summary>
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
