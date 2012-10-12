using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinkDao
{
    /// <summary>
    /// General message actions used by multiple messages
    /// </summary>
    public enum PinkoMessageAction
    {
        InvalidMessageAction = -1,  // Usually string cannot be converted
        
        // Snapshot request
        UserSnapshot,

        // User triggered subscriptions
        UserSubscription,
        Unsubscribe,

        /// Subscription Manager sent this request
        ManagerSubscription,
        ManagerUnsubscribe,

        // Do not remove and must be always last
        MaxActions
    }

    /// <summary>
    /// PinkoMessageActionExtensions
    /// </summary>
    public static class PinkoMessageActionExtensions
    {
        /// <summary>
        /// PinkoMessageAction
        /// </summary>
        public static PinkoMessageAction ToPinkoMessageAction(this string str)
        {
            var iParsedVal = -1;

            if (!int.TryParse(str, out iParsedVal) || iParsedVal < _invalidMessageActionInt || iParsedVal >= _maxActions)
                return PinkoMessageAction.InvalidMessageAction;

            return (PinkoMessageAction)iParsedVal;
        }

        /// <summary>
        /// Convert enum to string numeric value
        /// </summary>
        public static string ToSerialString(this PinkoMessageAction action)
        {
            return ((int) action).ToString();
        }

        public const int _invalidMessageActionInt = (int)PinkoMessageAction.InvalidMessageAction;
        public const int _maxActions = (int) PinkoMessageAction.MaxActions;
    }

}
