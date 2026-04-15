using Microsoft.EntityFrameworkCore;
using TaskKing.Api.Data;
using TaskKing.Api.Models;

namespace TaskKing.Api.Services;

public class CategoryService
{
    private readonly TaskKingDbContext _context;

    public CategoryService(TaskKingDbContext context)
    {
        _context = context;
    }

    public async Task<Category> Create(Category category)
    {
        ArgumentNullException.ThrowIfNull(category);

        if (string.IsNullOrWhiteSpace(category.Name))
            throw new ArgumentException("Name is required", nameof(category));

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return category;
    }
    
    public async Task<Category?> Update(int id, Category updated)
    {
        var category = await _context.Categories.FindAsync(id);

        if (category == null)
            return null;

        if (string.IsNullOrWhiteSpace(updated.Name))
            return null;

        category.Name = updated.Name;

        await _context.SaveChangesAsync();
        return category;
    }
    
    public async Task<bool> Delete(int id)
    {
        var category = await _context.Categories.FindAsync(id);

        if (category == null)
            return false;

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<Category>> GetAll()
        => await _context.Categories
            .OrderBy(c => c.Name)
            .ToListAsync();

    public async Task<Category?> GetById(int id)
        => await _context.Categories.FindAsync(id);
}