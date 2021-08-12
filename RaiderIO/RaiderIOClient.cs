using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MythicNights.RaiderIO
{
    public class RaiderIOClient
    {
        private readonly IHttpClientFactory clientFactory;

        public RaiderIOClient(IHttpClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        public async Task<CharacterStats> GetCharacterStats(string name, string server)
        {
            var url = $"https://raider.io/api/v1/characters/profile?region=us&realm={server}&name={name}&fields=mythic_plus_scores_by_season:current,gear";
            var result = await clientFactory.CreateClient().GetAsync(url);
            var content = await result.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<CharacterStats>(content);
        }
    }

    public class CharacterStats
    {
        public string name { get; set; }
        public string race { get; set; }
        public string _class { get; set; }
        public string active_spec_name { get; set; }
        public string active_spec_role { get; set; }
        public string gender { get; set; }
        public string faction { get; set; }
        public int achievement_points { get; set; }
        public int honorable_kills { get; set; }
        public string thumbnail_url { get; set; }
        public string region { get; set; }
        public string realm { get; set; }
        public DateTime last_crawled_at { get; set; }
        public string profile_url { get; set; }
        public string profile_banner { get; set; }
        public Mythic_Plus_Scores_By_Season[] mythic_plus_scores_by_season { get; set; }
        public Gear gear { get; set; }
    }

    public class Gear
    {
        public int item_level_equipped { get; set; }
        public int item_level_total { get; set; }
    }


    public class Mythic_Plus_Scores_By_Season
    {
        public string season { get; set; }
        public Scores scores { get; set; }
    }

    public class Scores
    {
        public float all { get; set; }
        public float dps { get; set; }
        public float healer { get; set; }
        public float tank { get; set; }
        public float spec_0 { get; set; }
        public float spec_1 { get; set; }
        public float spec_2 { get; set; }
        public float spec_3 { get; set; }
    }

}
