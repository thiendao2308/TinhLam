using AutoMapper;
using TinhLam.Data;
using TinhLam.ViewModels;

namespace TinhLam.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<RegisterVM, User>();
                
        }
    }
}
