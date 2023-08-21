using AutoMapper;
using WebChatApp.Data.Entities;
using WebChatApp.Models;

namespace WebChatApp.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {

            CreateMap<ManageUser, UserViewModel>()
                .ForMember(dst => dst.UserName, opt => opt.MapFrom(x => x.UserName));
            CreateMap<UserViewModel, ManageUser>();
        }
    }
}
