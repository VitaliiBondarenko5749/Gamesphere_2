using AutoMapper;
using CatalogOfGames.BAL.DTOs;
using Helpers;
using CatalogOfGames.DAL.Entities;
using CatalogOfGames.DAL.Repositories;

namespace CatalogOfGames.BAL.Services;

public interface IPublisherService
{
    Task<ServerResponse> AddAsync(AddPublisherDTO addPublisherDTO);
    Task<ServerResponse> DeleteAsync(Guid id);
    Task<PublisherInfoDTO[]?> GetTopTenByNameAsync(string name);
    Task<ServerResponse> UpdateDataInGameAsync(UpdatePublisherInGameDTO dto);
}

public class PublisherService : IPublisherService
{
    private readonly IUnitOfWork unitOfWork;

    public PublisherService(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task<ServerResponse> AddAsync(AddPublisherDTO addPublisherDTO)
    {
        Publisher? publisher = await unitOfWork.PublisherRepository.GetByNameAsync(addPublisherDTO.Name);

        if (publisher is not null)
        {
            return new ServerResponse
            {
                Message = $"Publisher {addPublisherDTO.Name} already exists!",
                IsSuccess = false
            };
        }

        publisher = new Publisher
        {
            Id = Guid.NewGuid(),
            Name = addPublisherDTO.Name
        };

        await unitOfWork.PublisherRepository.CreateAsync(publisher);

        await unitOfWork.SaveChangesAsync();

        return new ServerResponse
        {
            Message = $"Publisher {publisher.Name} has been added!",
            IsSuccess = true
        };
    }

    public async Task<ServerResponse> DeleteAsync(Guid id)
    {
        Publisher? publisher = await unitOfWork.PublisherRepository.GetByIdAsync(id);

        if (publisher is null)
        {
            return new ServerResponse
            {
                Message = "Publisher is not found!",
                IsSuccess = false
            };
        }

        await unitOfWork.PublisherRepository.DeleteAsync(id);

        await unitOfWork.SaveChangesAsync();

        return new ServerResponse
        {
            Message = $"Publisher {publisher.Name} has been removed!",
            IsSuccess = true
        };
    }

    public async Task<PublisherInfoDTO[]?> GetTopTenByNameAsync(string name)
    {
        Publisher[]? publishers = await unitOfWork.PublisherRepository.GetTopTenByNameAsync(name);

        if(publishers is null)
        {
            return null;
        }

        MapperConfiguration mapperConfiguration = new(config =>
        {
            config.CreateMap<Publisher, PublisherInfoDTO>();
        });

        IMapper mapper = mapperConfiguration.CreateMapper();

        return mapper.Map<PublisherInfoDTO[]>(publishers);
    }

    public async Task<ServerResponse> UpdateDataInGameAsync(UpdatePublisherInGameDTO dto)
    {
        Game? game = await unitOfWork.GameRepository.GetByIdAsync(dto.GameId);

        if(await unitOfWork.PublisherRepository.GetByIdAsync(dto.PublisherId) is not null && game is not null)
        {
            game.PublisherId = dto.PublisherId;
            
            unitOfWork.GameRepository.UpdateAsync(game);  

            await unitOfWork.SaveChangesAsync();

            return new ServerResponse { Message = "Game data have been updated!", IsSuccess = true };
        }

        return new ServerResponse { Message = "Game data have not been updated!", IsSuccess = false };
    }
}