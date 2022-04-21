﻿using BinanceAPI.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace BinanceAPI.Authentication
{
    /// <summary>
    /// Base class for authentication providers
    /// </summary>
    public class AuthenticationProvider
    {
        private readonly object signLock = new();
        private readonly HMACSHA256 encryptor;

        /// <summary>
        /// The provided credentials
        /// </summary>
        public ApiCredentials Credentials { get; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="credentials"></param>
        public AuthenticationProvider(ApiCredentials credentials)
        {
            if (credentials.Secret == null)
                throw new ArgumentException("No valid API credentials provided. Key/Secret needed.");

            encryptor = new HMACSHA256(Encoding.ASCII.GetBytes(credentials.Secret.GetString()));

            Credentials = credentials;
        }

        /// <summary>
        /// Add authentication to the parameter list based on the provided credentials
        /// </summary>
        /// <param name="uri">The uri the request is for</param>
        /// <param name="method">The HTTP method of the request</param>
        /// <param name="parameters">The provided parameters for the request</param>
        /// <param name="signed">Wether or not the request needs to be signed. If not typically the parameters list can just be returned</param>
        /// <param name="parameterPosition">Where parameters are placed, in the URI or in the request body</param>
        /// <param name="arraySerialization">How array parameters are serialized</param>
        /// <returns>Should return the original parameter list including any authentication parameters needed</returns>
        public virtual Dictionary<string, object> AddAuthenticationToParameters(string uri, HttpMethod method, Dictionary<string, object> parameters, bool signed, HttpMethodParameterPosition parameterPosition, ArrayParametersSerialization arraySerialization)
        {
            if (!signed)
                return parameters;

            string signData;
            if (parameterPosition == HttpMethodParameterPosition.InUri)
            {
                signData = parameters.CreateParamString(true, arraySerialization);
            }
            else
            {
                var formData = HttpUtility.ParseQueryString(string.Empty);
                foreach (var kvp in parameters.OrderBy(p => p.Key))
                {
                    if (kvp.Value.GetType().IsArray)
                    {
                        var array = (Array)kvp.Value;
                        foreach (var value in array)
                            formData.Add(kvp.Key, value.ToString());
                    }
                    else
                        formData.Add(kvp.Key, kvp.Value.ToString());
                }
                signData = formData.ToString();
            }

            lock (signLock)
                parameters.Add("signature", ByteToString(encryptor.ComputeHash(Encoding.UTF8.GetBytes(signData))));
            return parameters;
        }

        /// <summary>
        /// Add authentication to the header dictionary based on the provided credentials
        /// </summary>
        /// <param name="uri">The uri the request is for</param>
        /// <param name="method">The HTTP method of the request</param>
        /// <param name="parameters">The provided parameters for the request</param>
        /// <param name="signed">Wether or not the request needs to be signed. If not typically the parameters list can just be returned</param>
        /// <param name="parameterPosition">Where post parameters are placed, in the URI or in the request body</param>
        /// <param name="arraySerialization">How array parameters are serialized</param>
        /// <returns>Should return a dictionary containing any header key/value pairs needed for authenticating the request</returns>
        public virtual Dictionary<string, string> AddAuthenticationToHeaders(string uri, HttpMethod method, Dictionary<string, object> parameters, bool signed, HttpMethodParameterPosition parameterPosition, ArrayParametersSerialization arraySerialization)
        {
            if (Credentials.Key == null)
                throw new ArgumentException("No valid API credentials provided. Key/Secret needed.");

            return new Dictionary<string, string> { { "X-MBX-APIKEY", Credentials.Key.GetString() } };
        }

        /// <summary>
        /// Sign a string
        /// </summary>
        /// <param name="toSign"></param>
        /// <returns></returns>
        public virtual string Sign(string toSign)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sign a byte array
        /// </summary>
        /// <param name="toSign"></param>
        /// <returns></returns>
        public virtual byte[] Sign(byte[] toSign)
        {
            return toSign;
        }

        /// <summary>
        /// Convert byte array to hex
        /// </summary>
        /// <param name="buff"></param>
        /// <returns></returns>
        protected static string ByteToString(byte[] buff)
        {
            var result = string.Empty;
            foreach (var t in buff)
                result += t.ToString("X2"); /* hex format */
            return result;
        }
    }
}