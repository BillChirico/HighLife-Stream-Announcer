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

        public static Embed PinnedMessageEmbedBuilder(IEnumerable<Streamer> liveStreamers,
            IEnumerable<Streamer> offlineStreamers)
        {
            var builder = new EmbedBuilder()
                .WithTitle("Content Creators")
                .WithColor(Color.Blue)
                .WithFooter(new EmbedFooterBuilder().WithText(
                    $"Last Update: {DateTime.UtcNow.ToShortTimeString()} UTC"));

            // Online
            var streamDescriptions = liveStreamers.Select(liveStreamer => $"{liveStreamer.Username} | :green_circle:")
                .ToList();

            // Offline
            streamDescriptions.AddRange(offlineStreamers.Select(liveStreamer =>
                $"{liveStreamer.Username} | :red_circle:"));

            builder.WithDescription(string.Join(Environment.NewLine, streamDescriptions));

            return builder.Build();
        }
    }
}