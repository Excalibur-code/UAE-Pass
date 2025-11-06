using AutoMapper;
using UAE_Pass_Poc.Entities;
using UAE_Pass_Poc.Models.Request;

namespace UAE_Pass_Poc.MappingProfile
{
    public class Mappers : Profile
    {
        public Mappers()
        {
            // Request Presentation Model to Entity
            CreateMap<RequestPresentationModel, RequestPresentation>()
                .ForMember(dest => dest.Request, opt => opt.MapFrom(src => src.RequestId))
                .ForMember(dest => dest.RequestedVerifiedAttributes, opt => opt.MapFrom(src => string.Join(',', src.RequestedVerifiedAttributes)));
            CreateMap<DocumentInfo, Document>().ReverseMap();
            CreateMap<Models.Request.DocInstance, Entities.DocInstance>().ReverseMap();

            //Request Presentation Response Model to Entity
            CreateMap<Models.Response.RequestPresentationResponseModel, RequestPresentationResponseMapping>()
                .ForMember(dest => dest.ProofOfPresentationId, opt => opt.MapFrom(src => src.ProofOfPresentationRequestId));

            CreateMap<ReceivePresentationModel, ReceivePresentation>().ReverseMap();
        }
    }
}