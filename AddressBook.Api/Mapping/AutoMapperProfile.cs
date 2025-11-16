using AutoMapper;
using AddressBook.Core.DTOs;
using AddressBook.Core.Entities;

namespace AddressBook.Api.Mapping;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Contact, ContactDto>()
            .ForMember(d => d.JobName, opt => opt.MapFrom(s => s.Job != null ? s.Job.Name : null))
            .ForMember(d => d.DepartmentName, opt => opt.MapFrom(s => s.Department != null ? s.Department.Name : null))
            .ForMember(d => d.Age, opt => opt.MapFrom(s => s.Age));

        CreateMap<ContactCreateUpdateDto, Contact>();
    }
}
