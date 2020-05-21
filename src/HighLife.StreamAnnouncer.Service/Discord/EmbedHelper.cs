using System;
using System.Collections.Generic;
using System.Linq;
using Discord;
using HighLife.StreamAnnouncer.Domain.Entities;
using TwitchLib.Api.Helix.Models.Streams;
using TwitchLib.Api.Helix.Models.Users;

namespace HighLife.StreamAnnouncer.Service.Discord
{
    public static class EmbedHelper
    {
        public static Embed LiveMessageEmbedBuilder(Streamer streamer, User user, Stream stream)
        {
            var builder = new EmbedBuilder()
                .WithDescription(streamer.TagLine)
                .WithColor(new Color(streamer.HexColor))
                .WithThumbnailUrl(user.ProfileImageUrl)
                .WithAuthor(author =>
                {
                    author
                        .WithName(user.DisplayName)
                        .WithUrl($"https://twitch.tv/{user.Login}");
                })
                .AddField("Title", stream.Title);

            return builder.Build();
        }

        public static Embed PinnedMessageEmbedBuilder(List<Streamer> liveStreamers,
            List<Streamer> offlineStreamers)
        {
            var builder = new EmbedBuilder()
                .WithTitle("Official Content Creators")
                .WithColor(Color.Blue)
                .WithFooter(new EmbedFooterBuilder().WithText(
                    $"Last Update: {DateTime.UtcNow.ToShortTimeString()} UTC"));

            var onlineValue = liveStreamers.Any() ? string.Empty : "No Streamers Online";
            var offlineValue = offlineStreamers.Any() ? string.Empty : "No Streamers Offline";

            onlineValue = liveStreamers.Aggregate(onlineValue,
                (current, liveStreamer) => current + GetPinnedMessageString(liveStreamer));

            offlineValue = offlineStreamers.Aggregate(offlineValue,
                (current, offlineStreamer) => current + GetPinnedMessageString(offlineStreamer));
            builder.AddField(":green_circle:  Online Streamers", onlineValue);
            builder.AddField(":red_circle:  Offline Streamers", offlineValue);

            return builder.Build();
        }

        private static string GetPinnedMessageString(Streamer streamer)
        {
            return
                $"[{streamer.Username}](https://www.twitch.tv/{streamer.Username})";
        }
    }
}