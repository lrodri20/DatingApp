using System.Linq;
using AutoMapper;
using DatingApp.API.Dtos;
using DatingApp.API.Models;

namespace DatingApp.API.helpers
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles(){
            CreateMap<User,UserForListDto>()
            .ForMember(dest => dest.PhotoUrl, opt =>{
                opt.MapFrom(src => src.Photos.FirstOrDefault(p =>p.IsMain).URL);
            })
            .ForMember(dest => dest.Age, opt =>{
                opt.ResolveUsing(d => d.DateOfBirth.CalculateAge());
            });
            CreateMap<User,UserForDetailedDto>().ForMember(dest => dest.PhotoUrl, opt =>{
                opt.MapFrom(src => src.Photos.FirstOrDefault(p =>p.IsMain).URL);
                
            }).ForMember(dest => dest.Age, opt =>{
                opt.ResolveUsing(d => d.DateOfBirth.CalculateAge());
            });
            
            CreateMap<Photo,PhotosForDetailedDto>();
            CreateMap<UserForUpdateDto, User>();
            CreateMap<Photo, PhotoForReturnDto>();
            CreateMap<PhotoForCreationDto,Photo>();
            CreateMap<MessageForCreationDto, Message>();
            CreateMap<Message,MessageToReturnDto>()
            .ForMember(m => m.SenderPhotoUrl, opt => opt
                .MapFrom(u => u.Sender.Photos.FirstOrDefault(p => p.IsMain).URL))
            .ForMember(m => m.RecipientPhotoUrl, opt => opt
                .MapFrom(u => u.Recipient.Photos.FirstOrDefault(p => p.IsMain).URL));
        }
    }
} 