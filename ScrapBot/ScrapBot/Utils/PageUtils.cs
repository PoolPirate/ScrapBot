using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Interactivity;
using ScrapTDWrapper.Entities;

namespace ScrapBot.Utils
{
    public static class PageUtils
    {
        public static PageBuilder LeaderBoardPage<T>(IEnumerable<T> values, int index, Func<T, string> nameSelector, Func<T, string> valueSelector,
                                                     string entityName, string valueName, Color color)
        {
            var descriptionBuilder = new StringBuilder();
            int topIndex = index;

            foreach (var value in values)
            {
                descriptionBuilder.Append($"#{topIndex + 1} {nameSelector(value)}: {valueSelector(value)}{valueName}\n");
                topIndex++;
            }

            return new PageBuilder()
                .WithTitle($"Top {index + 1} - {topIndex} {entityName}")
                .WithDescription(descriptionBuilder.ToString())
                .WithColor(color);
        }

        public static PageBuilder LeaderPage(Team team, Member leader)
            => new PageBuilder()
                .WithColor(Color.Magenta)
                .WithTitle($"{team.Name}'s Leader")
                .AddField("Name :bust_in_silhouette:", leader.Name)
                .AddField("Level <:crown:444526211574792192>", leader.Level)
                .AddField("Trophies :trophy:", leader.Trophies)
                .AddField("Wins This Season :muscle:", leader.SeasonWins);

        public static PageBuilder CoLeaderPage(Team team, Member[] coLeaders)
        {
            var descriptionBuilder = new StringBuilder();

            foreach (var coLeader in coLeaders)
            {
                descriptionBuilder.Append($"**{coLeader.Name}** - " +
                                          $"{coLeader.Level} <:crown:444526211574792192> {coLeader.Trophies} :trophy:\n");
            }

            return new PageBuilder()
                    .WithColor(Color.Purple)
                    .WithTitle($"{team.Name}'s CoLeaders")
                    .WithDescription(descriptionBuilder.ToString());
        }

        public static PageBuilder ElderPage(Team team, Member[] elders)
        {
            var descriptionBuilder = new StringBuilder();

            foreach (var elder in elders)
            {
                descriptionBuilder.Append($"**{elder.Name}** - " +
                                          $"{elder.Level} <:crown:444526211574792192> {elder.Trophies} :trophy:\n");
            }

            return new PageBuilder()
                    .WithColor(Color.DarkBlue)
                    .WithTitle($"{team.Name}'s Elders")
                    .WithDescription(descriptionBuilder.ToString());
        }

        public static PageBuilder MemberPage(Team team, Member[] members)
        {
            var descriptionBuilder = new StringBuilder();

            foreach (var member in members)
            {
                descriptionBuilder.Append($"**{member.Name}** - {member.Trophies} :trophy:\n");
            }

            return new PageBuilder()
                    .WithColor(Color.Blue)
                    .WithTitle($"{team.Name}'s Members")
                    .WithDescription(descriptionBuilder.ToString());
        }
    }
}
