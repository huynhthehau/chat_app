using AutoMapper;
using WebChatApp.Data.Entities;
using WebChatApp.Models;

namespace WebChatApp.Mappings
{
    public class RoomProfile : Profile
    {
        public RoomProfile()
        {
            CreateMap<RoomViewModel, Room>();
            CreateMap<Room, RoomViewModel>();
        }
    }
}
