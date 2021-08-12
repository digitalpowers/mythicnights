﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MythicNights.DataContext;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MythicNights.Users
{
    internal class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly NavigationManager navManager;
        private readonly IHttpClientFactory clientFactory;
        private readonly PlayerManager playerManager;

        public string token { get; set; }
        public ulong discordId { get; set; }

        public Player currentPlayer { get; set; }

        public CustomAuthStateProvider(NavigationManager navManager, IHttpClientFactory clientFactory, PlayerManager playerManager)
        {
            this.navManager = navManager;
            this.clientFactory = clientFactory;
            this.playerManager = playerManager;
        }

        public Task CheckAuth()
        {
            if (token == null)
            {
                navManager.NavigateTo("https://discord.com/api/oauth2/authorize?client_id=861771760642424852&redirect_uri=https%3A%2F%2Fmythicnights.com%2Fredirect&response_type=code&scope=identify%20guilds");
            }
            return Task.CompletedTask;
        }

        public async Task AuthFromCode(string code)
        {
            var webClient = clientFactory.CreateClient();
            var formContent = new FormUrlEncodedContent(new[]
            {
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("code", code),
                    new KeyValuePair<string, string>("client_id", "861771760642424852"),
                    new KeyValuePair<string, string>("client_secret", "AtDxeW2MjB5NDLj0ehgKzNY2W0dpC-9Z"),
                    new KeyValuePair<string, string>("redirect_uri", "https://mythicnights.com/redirect"),
                });
            var result = await webClient.PostAsync("https://discord.com/api/oauth2/token", formContent);

            var respPayload = await result.Content.ReadAsStringAsync();
            var tokenPayload = JsonConvert.DeserializeAnonymousType(respPayload, new { access_token = "", refresh_token = "" });
            token = tokenPayload.access_token;


            using var msg = new HttpRequestMessage(HttpMethod.Get, "https://discord.com/api/oauth2/@me");
            msg.Headers.Add("Authorization", $"Bearer {token}");
            using var resp = await webClient.SendAsync(msg);
            var me = await resp.Content.ReadAsStringAsync();
            var meObj = JsonConvert.DeserializeObject<MeObject>(me);
            discordId = ulong.Parse(meObj.user.id);
            // { "access_token": "2pkPMpXHqHe9VwuAUkNrbSa84Gr3gT", "expires_in": 604800, "refresh_token": "tlAuBCqIMfkGk9fQYH7GWSa6t60e4g", "scope": "identify guilds", "token_type": "Bearer"}
        }

        public override async  Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if (currentPlayer == null)
            {
                currentPlayer = await playerManager.GetPlayer(discordId);
            }
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, currentPlayer.Nickname),
            }, "discordauth");

            var user = new ClaimsPrincipal(identity);

            return new AuthenticationState(user);

        }

        public class MeObject
        {
            public Application application { get; set; }
            public string[] scopes { get; set; }
            public DateTime expires { get; set; }
            public User user { get; set; }
        }

        public class Application
        {
            public string id { get; set; }
            public string name { get; set; }
            public object icon { get; set; }
            public string description { get; set; }
            public string summary { get; set; }
            public bool hook { get; set; }
            public bool bot_public { get; set; }
            public bool bot_require_code_grant { get; set; }
            public string verify_key { get; set; }
        }

        public class User
        {
            public string id { get; set; }
            public string username { get; set; }
            public object avatar { get; set; }
            public string discriminator { get; set; }
            public int public_flags { get; set; }
        }

    }

   

}
