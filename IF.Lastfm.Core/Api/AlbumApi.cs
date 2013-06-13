﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using IF.Lastfm.Core.Api.Enums;
using IF.Lastfm.Core.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IF.Lastfm.Core.Api
{
    public class AlbumApi : IAlbumApi
    {
        public IAuth Auth { get; private set; }

        public AlbumApi(IAuth auth)
        {
            Auth = auth;
        }

        public async Task<Album> GetAlbumInfoAsync(string artistname, string albumname, bool autocorrect = false)
        {
            const string apiMethod = "album.getInfo";

            var parameters = new Dictionary<string, string>
                {
                    {"artist", artistname},
                    {"album", albumname},
                    {"autocorrect", Convert.ToInt32(autocorrect).ToString()}
                };

            var httpClient = new HttpClient();
            var apiUrl = LastFm.FormatApiUrl(apiMethod, Auth.ApiKey, parameters);
            var lastResponse = await httpClient.GetAsync(apiUrl);
            var json = await lastResponse.Content.ReadAsStringAsync();

            LastFmApiError error;
            if (LastFm.IsResponseValid(json, out error) && lastResponse.IsSuccessStatusCode)
            {
                var jtoken = JsonConvert.DeserializeObject<JToken>(json);

                var album = Album.ParseJToken(jtoken.SelectToken("album"));

                return album;
            }
            else
            {
                LastFm.HandleError(error);
                return null; // an exception is gon' be thrown but the compiler needs this
            }
        }

        public async Task<Album> GetAlbumInfoWithMbidAsync(string mbid, bool autocorrect = false)
        {
            throw new System.NotImplementedException();
        }
    }
}