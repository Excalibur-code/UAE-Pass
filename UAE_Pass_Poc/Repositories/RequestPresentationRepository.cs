using UAE_Pass_Poc.DBContext;

namespace UAE_Pass_Poc.Repositories;

public class RequestPresentationRepository : Repository<Entities.RequestPresentation>, IRequestPresentationRepository
{
    public RequestPresentationRepository(UaePassDbContext context) : base(context)
    {
    }
}