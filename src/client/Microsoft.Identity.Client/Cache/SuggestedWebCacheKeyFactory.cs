﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Identity.Client.Internal.Requests;

namespace Microsoft.Identity.Client.Cache
{
    internal static class SuggestedWebCacheKeyFactory
    {
        public static string GetKeyFromRequest(AuthenticationRequestParameters requestParameters)
        {
            if (GetOboOrAppKey(requestParameters, out string key))
            {
                return key;
            }

            if (requestParameters.ApiId == TelemetryCore.Internal.Events.ApiEvent.ApiIds.AcquireTokenSilent)
            {
                return requestParameters.Account?.HomeAccountId?.Identifier == null ? 
                    null :
                    requestParameters.ClientId + "." + requestParameters.Account?.HomeAccountId?.Identifier;
            }

            if (requestParameters.ApiId == TelemetryCore.Internal.Events.ApiEvent.ApiIds.GetAccountById)
            {
                return requestParameters.HomeAccountId == null ? 
                    null : 
                    requestParameters.ClientId + "." + requestParameters.HomeAccountId; 
            }

            return null;
        }

        public static string GetKeyFromResponse(AuthenticationRequestParameters requestParameters, string homeAccountIdFromResponse)
        {
            if (GetOboOrAppKey(requestParameters, out string key))
            {
                return key;
            }

            if (requestParameters.IsConfidentialClient || 
                requestParameters.ApiId == TelemetryCore.Internal.Events.ApiEvent.ApiIds.AcquireTokenSilent)
            {
                
                return homeAccountIdFromResponse == null ? 
                    null :
                    requestParameters.ClientId + "." + homeAccountIdFromResponse;
            }

            return null;
        }

        private static bool GetOboOrAppKey(AuthenticationRequestParameters requestParameters, out string key)
        {
            if (requestParameters.ApiId == TelemetryCore.Internal.Events.ApiEvent.ApiIds.AcquireTokenOnBehalfOf)
            {
                key = requestParameters.UserAssertion.AssertionHash;
                return true;
            }

            if (requestParameters.ApiId == TelemetryCore.Internal.Events.ApiEvent.ApiIds.AcquireTokenForClient)
            {
                key = requestParameters.ClientId + requestParameters.Authority.TenantId ?? "" + "_AppTokenCache";
                return true;
            }

            key = null;
            return false;
        }
    }
}
