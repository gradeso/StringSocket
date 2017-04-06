﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
// using System.Net.Http;
using System.Security.Cryptography;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using static System.Net.HttpStatusCode;
// using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Configuration;

namespace Boggle
{
    public class BoggleService // : IBoggleService
    {
        // connection string to db like in todolist
        private static string BoggleDB;

        static BoggleService()
        {
            BoggleDB = ConfigurationManager.ConnectionStrings["BoggleDB"].ConnectionString;
        }

        /// <summary>
        /// a game object for holding a pending game
        /// </summary>
        private static Game pendingGame = null;

        /// <summary>
        /// an object to use for locking
        /// </summary>
        private static readonly object sync = new object();

        /// <summary>
        /// a char array to use in generating tokens
        /// code found at http://stackoverflow.com/questions/19298801/generating-random-string-using-rngcryptoserviceprovider
        /// </summary>
        static readonly char[] AvailableCharacters = {
    'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
    'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
    'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
    'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
    '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-'
        };

        /// <summary>
        /// a token size constant for easy maintainability if we want to change the size of tokens
        /// </summary>
        private const int NEWTOKENSIZE = 40;

        /// <summary>
        /// The most recent call to SetStatus determines the response code used when
        /// an http response is sent.
        /// </summary>
        /// <param name="status"></param>
        private static void SetStatus(HttpStatusCode status)
        {
            WebOperationContext.Current.OutgoingResponse.StatusCode = status;
        }

        public UserID CreateUser(UserName username)
        {
            // validate user
            if (username == null)
            {
                SetStatus(Forbidden);
                return null;
            }
            //if the name is null or nothing after trimming
            if (string.IsNullOrWhiteSpace(username.Nickname))
            {
                SetStatus(Forbidden);
                return null;
            }

            // opens connection to database
            // using guarentees connection will drop after leaving code
            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                conn.Open();
                // transaction that contains all commands
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    using (SqlCommand command = new SqlCommand("insert into Users (UserID, Nickname) values (@UserID, @Nickname)", conn, trans))
                    {

                    }
                }
            }
        }

        /// <summary>
        /// helper method that generates a token string to use for user and
        /// game id's
        /// technique idea found at 
        /// http://stackoverflow.com/questions/19298801/generating-random-string-using-rngcryptoserviceprovider
        /// </summary>
        /// <returns></returns>
        private string GenerateTokenString(int length)
        {
            char[] identifier = new char[length];
            byte[] randomData = new byte[length];

            //randomize
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomData);
            }

            //use randomized data to create token
            for (int idx = 0; idx < identifier.Length; idx++)
            {
                int pos = randomData[idx] % AvailableCharacters.Length;
                identifier[idx] = AvailableCharacters[pos];
            }

            //return token
            return new string(identifier);
        }
    }
}