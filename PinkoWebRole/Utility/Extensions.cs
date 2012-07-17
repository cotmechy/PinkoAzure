using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PinkoWorkerCommon.Extensions;
using SignalR;

namespace PinkoWebRole.Utility
{
    static public class Extensions
    {
        /// <summary>
        /// Object Identity
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Verbose(this IRequest obj)
        {
            return string.Format("{0}: Name: {1} - Authen: {2}"
                                            , obj.VerboseIdentity()
                                            , obj.User.Identity.Name
                                            , obj.User.Identity.IsAuthenticated
                                            );
        }
    }
}