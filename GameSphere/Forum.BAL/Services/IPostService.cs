using AutoMapper;
using FluentValidation.Results;
using Forum.BAL.DTOs;
using Forum.BAL.Extensions;
using Forum.BAL.Validators;
using Forum.DAL.Entities;
using Forum.DAL.Repositories;
using Helpers;
using Helpers.GeneralClasses.Forum.DTOs;

namespace Forum.BAL.Services;

public interface IPostService
{
    Task<PageResult<ShortPostInfoDTO>> GetAllAsync(GetTopicsPaginationDTO dto);
    Task<ServerResponse> AddAsync(AddPostDTO dto);
    Task<ServerResponse> DeleteAsync(Guid postId);
    Task<Post> GetByIdAsync(Guid id);
    Task<ServerResponse> AddToFavoriteAsync(SavePostDTO dto);
    Task<ServerResponse> DeleteFromFavoriteAsync(SavePostDTO dto);
    Task<ServerResponse> CheckExistenceInFavAsync(SavePostDTO dto);
    Task<List<ShortPostInfoDTO>> GetSimilarPostsAsync(GetSimilarPostsDTO dto);
    Task DeleteUserForumDataAsync(string userId);
}

public class PostService : IPostService
{
    private readonly IUnitOfWork unitOfWork;

    public PostService(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    // DONE.
    public async Task<PageResult<ShortPostInfoDTO>> GetAllAsync(GetTopicsPaginationDTO dto)
    {
        IEnumerable<Post>? query = await unitOfWork.PostRepository.GetAllAsync(dto);

        MapperConfiguration mapperConfiguration = new(config =>
        {
            config.CreateMap<Post, ShortPostInfoDTO>();
        });

        IMapper mapper = mapperConfiguration.CreateMapper();

        List<ShortPostInfoDTO>? posts = mapper.Map<List<ShortPostInfoDTO>>(query);

        if (posts is not null)
        {
            foreach (ShortPostInfoDTO post in posts)
            {
                post.NumberOfReplies = await unitOfWork.ReplyRepository.GetCountOfPostReplies(post.Id);
            }
        }

        int totalCount = await unitOfWork.PostRepository.GetCountOfPostsAsync(dto);

        return new PageResult<ShortPostInfoDTO>
        {
            Count = totalCount,
            PageIndex = dto.Page ?? 1,
            PageSize = dto.PageSize,
            Items = posts
        };
    }

    // DONE.
    public async Task<ServerResponse> AddAsync(AddPostDTO dto)
    {
        AddPostValidator validator = new();
        ValidationResult validationResult = await validator.ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            string errors = validationResult.ToString("~");

            return new ServerResponse { Message = "Data are not valid!", IsSuccess = false, Errors = errors.Split('~') };
        }

        Post post = new()
        {
            Id = Guid.NewGuid(),
            UserId = dto.UserId,
            GameId = dto.GameId,
            Topic = dto.Subject,
            Content = dto.Text,
            CreatedAt = DateTime.Now,
            Views = 0
        };

        await unitOfWork.PostRepository.AddAsync(post);

        unitOfWork.Commit();

        return new ServerResponse { Message = "Post has been added successfully!", IsSuccess = true };
    }

    public async Task<ServerResponse> DeleteAsync(Guid postId)
    {
        await unitOfWork.PostRepository.DeleteAsync(postId);
        unitOfWork.Commit();

        return new ServerResponse { Message = "Post has been removed successfully!", IsSuccess = true };
    }

    // DONE.
    public async Task<Post> GetByIdAsync(Guid id)
    {
        Post post = await unitOfWork.PostRepository.GetAsync(id);

        int rowsAffected = await unitOfWork.PostRepository.UpdateViewsAsync(post.Id, post.Views + 1);

        if (rowsAffected > 0)
        {
            unitOfWork.Commit();
            post.Views++;
        }

        return post;
    }

    public async Task<ServerResponse> AddToFavoriteAsync(SavePostDTO dto)
    {
        FavoritePost favoritePost = new()
        {
            Id = Guid.NewGuid(),
            PostId = dto.PostId,
            UserId = dto.UserId
        };

        await unitOfWork.FavoritePostRepository.AddAsync(favoritePost);
        unitOfWork.Commit();

        return new ServerResponse { Message = "Post has been added to saved!", IsSuccess = true };
    }

    public async Task<ServerResponse> DeleteFromFavoriteAsync(SavePostDTO dto)
    {
        int rowsAffected = await unitOfWork.FavoritePostRepository.DeleteByPostAndUserIdAsync(dto.PostId, dto.UserId);

        if (rowsAffected > 0)
        {
            unitOfWork.Commit();

            return new ServerResponse { Message = "Post has been removed from Favorites!", IsSuccess = true };
        }

        return new ServerResponse { Message = "Something went wrong during removing post...", IsSuccess = false };
    }

    public async Task<ServerResponse> CheckExistenceInFavAsync(SavePostDTO dto)
    {
        FavoritePost? favoritePost = await unitOfWork.FavoritePostRepository.GetByPostAndUserIdAsync(dto.PostId, dto.UserId);

        if (favoritePost is not null)
        {
            return new ServerResponse { Message = "Post exists!", IsSuccess = true };
        }

        return new ServerResponse { Message = "Post does not exist!", IsSuccess = false };
    }

    public async Task<List<ShortPostInfoDTO>> GetSimilarPostsAsync(GetSimilarPostsDTO dto)
    {
        IEnumerable<Post> posts = await unitOfWork.PostRepository.GetAllAsync();

        // Обчислюємо вектор для поточного поста
        Dictionary<string, int> currentPostVector = TextComparer.GetTermFrequencyVector(dto.CurrentPostText);

        // Створюємо словник для зберігання подібності кожного поста
        Dictionary<Post, double> similarityScores = new();

        foreach (Post post in posts)
        {
            string clearText = TextComparer.RemoveHtmlTags(post.Content);

            if (string.IsNullOrEmpty(clearText))
            {
                continue;
            }

            Dictionary<string, int> postVector = TextComparer.GetTermFrequencyVector(clearText);
            double similarity = TextComparer.CosineSimilarity(currentPostVector, postVector);

            similarityScores[post] = similarity;
        }

        List<ShortPostInfoDTO> dtos = similarityScores
            .Where(x => !x.Key.Id.Equals(dto.PostId))
            .OrderByDescending(x => x.Value)
            .Take(dto.Count)
            .Select(x => new ShortPostInfoDTO
           {
               Id = x.Key.Id,
               UserId = x.Key.UserId,
               Topic = x.Key.Topic,
               CreatedAt = x.Key.CreatedAt,
               Views = x.Key.Views
           })
            .ToList();

        foreach (ShortPostInfoDTO shortPost in dtos)
        {
            shortPost.NumberOfReplies = await unitOfWork.ReplyRepository.GetCountOfPostReplies(shortPost.Id);
        }

        return dtos;
    }

    public async Task DeleteUserForumDataAsync(string userId)
    {
        IEnumerable<Guid>? postIds = await unitOfWork.PostRepository.GetPostIdsByUserAsync(userId);

        if (postIds is not null)
        {
            foreach (Guid postId in postIds)
            {
                await unitOfWork.ReplyRepository.DeleteRepliesRecursivelyAsync(postId);
            }
        }

        unitOfWork.Commit();
    }
}