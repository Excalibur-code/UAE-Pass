using UAE_Pass_Poc.Models;

namespace UAE_Pass_Poc.Services
{
    public interface IDidResolutionService
    {
        /// <summary>
        /// Fetches the DID document for a given DID.
        /// </summary>
        /// <param name="did">The Decentralized Identifier (DID) to resolve.</param>
        /// <returns>The DID Document if found, otherwise null.</returns>
        Task<DidDocument?> ResolveDid(string did);
    }
}