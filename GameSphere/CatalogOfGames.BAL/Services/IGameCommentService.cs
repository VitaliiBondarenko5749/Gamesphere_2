using CatalogOfGames.BAL.DTOs;
using Helpers;
using CatalogOfGames.DAL.Entities;
using CatalogOfGames.DAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CatalogOfGames.BAL.Services;

#pragma warning disable

public interface IGameCommentService
{
    Task<ServerResponse> AddAsync(AddCommentDTO dto);
    Task<ServerResponse> DeleteAsync(Guid id);
    Task<PageResult<CommentInfoDTO>> GetByPaginationAsync(GetCommentsPaginationDTO dto);
    Task<ServerResponse> DoLikeOperationAsync(LikeCommentDTO dto);
    Task<ServerResponse> CheckExistenceAsync(LikeCommentDTO dto);
    Task<int> GetCountOfLikesAsync(Guid commentId);
}

public class GameCommentService : IGameCommentService
{
    private readonly IUnitOfWork unitOfWork;

    public GameCommentService(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task<ServerResponse> AddAsync(AddCommentDTO dto)
    {
        Game? game = await unitOfWork.GameRepository.GetByIdAsync(dto.GameId);

        if (game is null)
        {
            return new ServerResponse { Message = "Game is not found!", IsSuccess = false };
        }

        GameComment comment = new()
        {
            Id = Guid.NewGuid(),
            UserId = dto.UserId,
            GameId = dto.GameId,
            Content = dto.Content,
            CreatedAt = DateTime.Now
        };

        await unitOfWork.GameCommentRepository.CreateAsync(comment);

        await unitOfWork.SaveChangesAsync();

        return new ServerResponse { Message = "Comment has been added!", IsSuccess = true };
    }

    public async Task<ServerResponse> DeleteAsync(Guid id)
    {
        GameComment? gameComment = await unitOfWork.GameCommentRepository.GetByIdAsync(id);

        if (gameComment is null)
        {
            return new ServerResponse { Message = "Comment is not found!", IsSuccess = false };
        }

        await unitOfWork.GameCommentRepository.DeleteAsync(id);

        await unitOfWork.SaveChangesAsync();

        return new ServerResponse { Message = "Comment has been removed!", IsSuccess = true };
    }

    public async Task<PageResult<CommentInfoDTO>> GetByPaginationAsync(GetCommentsPaginationDTO dto)
    {
        IQueryable<GameComment>? query = await unitOfWork.GameCommentRepository.GetByGameIdAsync(dto.GameId);

        query = query.OrderByDescending(c => c.CreatedAt);

        int totalCount = query.Count();

        if (dto.Page.HasValue)
        {
            query = query.Skip((dto.Page.Value - 1) * dto.PageSize);
        }

        query = query.Take(dto.PageSize);

        List<CommentInfoDTO> comments = await query.Select(c => new CommentInfoDTO
        {
            Id = c.Id,
            Content = c.Content,
            CreatedAt = $"{c.CreatedAt.Day}.{c.CreatedAt.Month}.{c.CreatedAt.Year}, {c.CreatedAt.Hour}:{c.CreatedAt.Minute}",
            UserId = c.UserId
        }).ToListAsync();

        return new PageResult<CommentInfoDTO>
        {
            Count = totalCount,
            PageIndex = dto.Page ?? 1,
            PageSize = dto.PageSize,
            Items = comments
        };
    }

    public async Task<ServerResponse> DoLikeOperationAsync(LikeCommentDTO dto)
    {
        LikedComment? likedComment = await unitOfWork.LikedCommentRepository.CheckExistenceAsync(dto.CommentId, dto.UserId);

        if (likedComment is null)
        {
            likedComment = new()
            {
                Id = Guid.NewGuid(),
                CommentId = dto.CommentId,
                UserId = dto.UserId
            };

            await unitOfWork.LikedCommentRepository.CreateAsync(likedComment);
        }
        else
        {
            await unitOfWork.LikedCommentRepository.DeleteAsync(likedComment.Id);
        }

        await unitOfWork.SaveChangesAsync();

        return new ServerResponse { Message = "Like operation has finished successfully!", IsSuccess = true };
    }

    public async Task<ServerResponse> CheckExistenceAsync(LikeCommentDTO dto)
    {
        LikedComment? likedComment = await unitOfWork.LikedCommentRepository.CheckExistenceAsync(dto.CommentId, dto.UserId);

        return new ServerResponse
        {
            Message = (likedComment is null) ? "User didn't put like in the comment" : "User putted like in the comment",
            IsSuccess = likedComment is not null
        };
    }

    public async Task<int> GetCountOfLikesAsync(Guid commentId)
    {
        return await unitOfWork.LikedCommentRepository.GetCountOfLikesAsync(commentId);
    }
}