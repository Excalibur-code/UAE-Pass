using AutoMapper;
using UAE_Pass_Poc.Entities;
using UAE_Pass_Poc.Models.Request;

namespace UAE_Pass_Poc.MappingProfile
{
    public class Mappers : Profile
    {
        public Mappers()
        {
            // Add your mappings here
            CreateMap<RequestPresentationModel, RequestPresentation>().ReverseMap();
            CreateMap<DocumentInfo, Document>().ReverseMap();
            CreateMap<Models.Request.DocInstance, Entities.DocInstance>().ReverseMap();
        }
    }
}