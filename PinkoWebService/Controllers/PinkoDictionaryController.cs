using System.Collections.Generic;
using System.Web.Http;
using PinkDao;
using PinkoMocks;

namespace PinkoWebService.Controllers
{
    /// <summary>
    /// Controller for formula dictionaries
    /// </summary>
    public class PinkoDictionaryController : ApiController
    {

        /// <summary>
        /// Get public dictionary items
        /// </summary>
        /// <returns></returns>
        public IEnumerable<EntitytFormulaExpression> GetPublicDictionary()
        {
            return SampleMockData.GetEntitytFormulaExpressionMocks();
        }
    }
}
