using Microsoft.EntityFrameworkCore;
using TaskKing.Api.Data;
using TaskKing.Api.Models;

namespace TaskKing.Api.Services;

public class CommentService
{
    private readonly TaskKingDbContext _context;

    public CommentService(TaskKingDbContext context)
    {
        _context = context;
    }

    public async Task<Comment> AddComment(int taskId, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content required");

        var taskExists = await _context.TaskItems.AnyAsync(t => t.Id == taskId);
        if (!taskExists)
            throw new ArgumentException("Task not found");

        var comment = new Comment
        {
            TaskItemId = taskId,
            Content = content
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        return comment;
    }

    public async Task<List<Comment>> GetCommentsForTask(int taskId)
    {
        return await _context.Comments
            .Where(c => c.TaskItemId == taskId)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();
    }
}