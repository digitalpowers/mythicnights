using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Discord.OAuth2;
using Discord.WebSocket;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MythicNights.Data;
using MythicNights.DataContext;
using MythicNights.PartyBuilding;
using MythicNights.RaiderIO;
using MythicNights.Users;
using Plk.Blazor.DragDrop;
using System;

namespace MythicNights
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            var dbPath = $"{path}{System.IO.Path.DirectorySeparatorChar}mythicnights.db";
            Console.WriteLine($"DbPath is {dbPath}");

            services.AddDbContextFactory<MythicNightContext>(
                options => options.UseSqlite($"Data Source={dbPath}")
                );
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost ;
            });

            services.AddAuthentication(opt =>
            {
                opt.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                opt.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = DiscordDefaults.AuthenticationScheme;
            })
                .AddCookie()
                .AddDiscord(x =>
                {
                    x.AppId = Configuration["Discord:AppId"];
                    x.AppSecret = Configuration["Discord:AppSecret"];
                    x.SaveTokens = true;
                    x.Scope.Add("guilds");
                });

            services.AddRazorPages();
            services.AddServerSideBlazor();

            services.AddHttpClient();
            services.AddSingleton<DiscordSocketClient>(s => new DiscordSocketClient());
            services.AddSingleton<UserService>();
            services.AddSingleton<GuildManager>();
            services.AddSingleton<EventManager>();
            services.AddSingleton<RaiderIOClient>();
            services.AddSingleton<PartyBuilder>();
            services.AddSingleton<PlayerManager>();


            services.Scan(scan => scan.FromAssemblyOf<IScoreGroup>()
                .AddClasses(classes => classes.AssignableTo<IScoreGroup>())
                    .AsImplementedInterfaces()
                    .WithSingletonLifetime()
                .AddClasses(classes => classes.AssignableTo<ICommand>())
                    .AsImplementedInterfaces()
                    .WithSingletonLifetime());

            services.AddHttpContextAccessor();

            services.AddBlazorDragDrop();

            services.AddBlazorise(options =>
                    {
                        options.ChangeTextOnKeyPress = true;
                    })
                    .AddBootstrapProviders()
                    .AddFontAwesomeIcons();

            //services.AddHostedService<Test>();
            services.AddHostedService<BotService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            }
            app.UseForwardedHeaders();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapDefaultControllerRoute();
                endpoints.MapFallbackToPage("/_Host");
            });

        }
    }

    //internal class Test : IHostedService
    //{
    //    private readonly IDbContextFactory<MythicNightContext> contextFactory;
    //    private readonly RaiderIOClient rioClient;
    //    private readonly PartyBuilder partyBuilder;

    //    public Test(IDbContextFactory<MythicNightContext> contextFactory, RaiderIOClient rioClient, PartyBuilder partyBuilder)
    //    {
    //        this.contextFactory = contextFactory;
    //        this.rioClient = rioClient;
    //        this.partyBuilder = partyBuilder;
    //    }
    //    public async Task StartAsync(CancellationToken cancellationToken)
    //    {
    //        using var context = contextFactory.CreateDbContext();
    //        var players = await context.Players.Include(p => p.Toons).ToListAsync();
    //        //if(players.Count == 0)
    //        //{
    //        //    await context.Players.AddAsync(await AddPlayer(5, "digitalpowers", new (string, string)[] { ("kalefupanda", "eonar"), ("Kaleesi", "Eonar"), ("kalesosneaky", "Eonar") }));
    //        //    await context.Players.AddAsync(await AddPlayer(6, "raven", new (string, string)[] { ("Guilt", "Eonar") }));
    //        //    await context.Players.AddAsync(await AddPlayer(7, "sarah", new (string, string)[] { ("Sehmicor", "Skullcrusher"), ("Sehmi", "Skullcrusher"), ("Sehmaria", "Skullcrusher"), ("Elenia", "Skullcrusher"), ("Fiore", "Skullcrusher") }));
    //        //    await context.Players.AddAsync(await AddPlayer(8, "Bueller", new (string, string)[] { ("Lasagna", "Velen"), ("Pudytat", "Velen"), ("Fettucini", "Velen"), ("Imahoot", "Velen"), ("Rigatti", "Velen"), ("Capnsmacks", "Velen"), ("Pheona", "Velen"), ("Bruschetta", "Velen"), ("Focaccia", "Velen") }));
    //        //    await context.Players.AddAsync(await AddPlayer(9, "someday", new (string, string)[] { ("Someday", "Eonar") }));
    //        //    await context.Players.AddAsync(await AddPlayer(10, "aust", new (string, string)[] { ("Aust", "Velen"), ("Oakhallow", "Velen"), ("Stayne", "Velen"), ("Crauley", "Velen") }));
    //        //    await context.Players.AddAsync(await AddPlayer(11, "darth", new (string, string)[] { ("Darthballz", "Eonar"), ("Demonamidala", "Eonar") }));
    //        //    await context.Players.AddAsync(await AddPlayer(12, "jen", new (string, string)[] { ("Sassenachh", "Eonar"), ("Steviwonder", "Eonar"), ("Carolebaskin", "Eonar") }));
    //        //    await context.Players.AddAsync(await AddPlayer(13, "pizza", new (string, string)[] { ("Pizzahunt", "Eonar") }));
    //        //    await context.Players.AddAsync(await AddPlayer(14, "rap", new (string, string)[] { ("Rapstar", "Eonar") }));
    //        //    await context.Players.AddAsync(await AddPlayer(15, "brain", new (string, string)[] { ("Whitezombie", "Velen"), ("Shadowplay", "Velen") }));
    //        //    await context.Players.AddAsync(await AddPlayer(16, "lacho", new (string, string)[] { ("Lachonacho", "Velen"), ("Heshlon", "Velen") }));
    //        //    await context.Players.AddAsync(await AddPlayer(18, "ardbeg", new (string, string)[] { ("Ardbeg", "Velen"), ("Oligarch", "Velen"), ("Escaenor", "Velen"), ("Churchill", "Velen"), ("Potus", "Velen"), ("Cerberus", "Velen"), ("Edict", "Velen") }));
    //        //    await context.Players.AddAsync(await AddPlayer(19, "curiosity", new (string, string)[] { ("Curiosity", "Velen") }));
    //        //    await context.SaveChangesAsync();
    //        //    players = await context.Players.Include(p => p.Toons).ToListAsync();
    //        //}

    //        for (int i = 0; i < 10; i++)
    //        {
    //            var winningGroup = partyBuilder.BuildParty(players);
    //            foreach(var group in winningGroup)
    //            {
    //                Console.WriteLine(group.ToString());
    //            }
    //            Console.WriteLine();
    //            Console.WriteLine();
    //        }
    //        //var client = new WowClient(Region.US, Locale.en_US, "key");
    //        //var character = await client.GetCharacterAsync("Kalefupanda-Eonar", CharacterOptions.AllOptions);

    //        //var client = new RaiderIOClient(Region.US, "Aust", "Velen");
    //        ////var guild = await client.GetGuildRaidProgAsync(Region.US, "Velen", "Delusions of Grandeur");
    //        //var stats = await client.GetCharacterStatsAsync();
    //        //var affixes = await client.GetAffixesAsync(Region.US);
    //        //var stats = await rioClient.GetCharacterStats("Aust", "Velen");
    //    }

    //    private async Task<Player> AddPlayer(ulong userid, string discName, params (string,string)[] toons)
    //    {
    //        var toonStats = new List<Toon>();
    //        foreach(var (name, realm) in toons)
    //        {
    //            var stats = await rioClient.GetCharacterStats(name, realm);
    //            if(stats.mythic_plus_scores_by_season == null)
    //            {
    //                Console.WriteLine($"Unable to find scores for {name}-{realm}");
    //                continue;
    //            }
    //            var scores = new List<(Role, float)>
    //            {
    //                (Role.Dps, stats.mythic_plus_scores_by_season[0].scores.dps ),
    //                (Role.Tank, stats.mythic_plus_scores_by_season[0].scores.tank ),
    //                (Role.Healer, stats.mythic_plus_scores_by_season[0].scores.healer )
    //            };
    //            scores.Sort((x,y) => y.Item2.CompareTo(x.Item2));
    //            var preferedRole = scores[0].Item1;
    //            var toon = new Toon()
    //            {
    //                Name = stats.name,
    //                Realm = stats.realm,
    //                iLvl = stats.gear.item_level_equipped,
    //                PreferedRole = preferedRole,
    //                RaiderIO = stats.mythic_plus_scores_by_season[0].scores.all
    //            };
    //            if(scores.Count > 1 && scores[1].Item2 > 0)
    //            {
    //                toon.Offspec = scores[1].Item1;
    //            }
    //            toonStats.Add(toon);
    //        }
            
    //        var player = new Player()
    //        {
    //            DiscordUserId = userid,
    //            DiscordUsername = discName,
    //            Toons = toonStats
    //        };
    //        return player;
    //    }

    //    public Task StopAsync(CancellationToken cancellationToken)
    //    {
    //        return Task.FromResult(0);
    //    }
    //}

}
