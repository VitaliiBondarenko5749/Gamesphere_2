using AutoMapper;
using FluentValidation.Results;
using Forum.BAL.DTOs;
using Forum.BAL.Validators;
using Forum.DAL.Entities;
using Forum.DAL.Repositories;
using Helpers;
using Helpers.GeneralClasses.Forum.DTOs;

namespace Forum.BAL.Services;

public interface IReplyService 
{
    Task<PageResult<ReplyInfoDTO>> GetAllAsync(GetRepliesPaginationDTO dto);
    Task<ServerResponse> SendReplyAsync(ReplyToPostDTO dto);
    Task<ServerResponse> DeleteAsync(Guid replyId);
}

public class ReplyService : IReplyService 
{
    private readonly IUnitOfWork unitOfWork;

    public ReplyService(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task<PageResult<ReplyInfoDTO>> GetAllAsync(GetRepliesPaginationDTO dto)
    {
        IEnumerable<Reply>? query = await unitOfWork.ReplyRepository.GetAllAsync(dto);

        MapperConfiguration mapperConfiguration = new(config =>
        {
            config.CreateMap<Reply, ReplyInfoDTO>();
        });

        IMapper mapper = mapperConfiguration.CreateMapper();

        List<ReplyInfoDTO>? replies = mapper.Map<List<ReplyInfoDTO>>(query);

        int totalCount = await unitOfWork.ReplyRepository.GetCountOfPostReplies(dto.PostId);

        return new PageResult<ReplyInfoDTO>
        {
            Count = totalCount,
            PageIndex = dto.Page,
            PageSize = dto.PageSize,
            Items = replies
        };
    }

    public async Task<ServerResponse> SendReplyAsync(ReplyToPostDTO dto)
    {
        ReplyToPostValidator validator = new();
        ValidationResult validationResult = await validator.ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            string errors = validationResult.ToString("~");

            return new ServerResponse { Message = "Data are not valid!", IsSuccess = false, Errors = errors.Split('~') };
        }

        Reply reply = new() 
        {
            Id = Guid.NewGuid(),
            UserId = dto.UserId,
            PostId = dto.PostId,
            Content = dto.Content,
            CreatedAt = DateTime.Now,
            ReplyToId = dto.ReplyToId
        };

        await unitOfWork.ReplyRepository.AddAsync(reply);

        unitOfWork.Commit();

        return new ServerResponse { Message = "Reply has been added successfully!", IsSuccess = true };
    }

    public async Task<ServerResponse> DeleteAsync(Guid replyId)
    {
        await unitOfWork.ReplyRepository.DeleteByReplyToReplyId(replyId);

        await unitOfWork.ReplyRepository.DeleteAsync(replyId);

        unitOfWork.Commit();

        return new ServerResponse { Message = "Reply has been removed", IsSuccess = true };
    }
}