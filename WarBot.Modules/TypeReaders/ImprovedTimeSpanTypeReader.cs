﻿using Discord.Commands;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace WarBot.Modules.TypeReaders
{
    /// <summary>
    /// To trick discord.net into using the custom type reader, instead of the default timespan parser.
    /// </summary>
    public class TimeSpanext
    {
        public TimeSpan Span { get; }
        public TimeSpanext(TimeSpan span)
        {
            this.Span = span;
        }
    }
    public class ImprovedTimeSpanTypeReader : TypeReader
    {
        private static readonly string[] _formats = new[]
        {
            "%d'd'%h'h'%m'm'%s's'", //4d3h2m1s
            "%d'd'%h'h'%m'm'",      //4d3h2m
            "%d'd'%h'h'%s's'",      //4d3h  1s
            "%d'd'%h'h'",           //4d3h
            "%d'd'%m'm'%s's'",      //4d  2m1s
            "%d'd'%m'm'",           //4d  2m
            "%d'd'%s's'",           //4d    1s
            "%d'd'",                //4d
            "%h'h'%m'm'%s's'",      //  3h2m1s
            "%h'h'%m'm'",           //  3h2m
            "%h'h'%s's'",           //  3h  1s
            "%h'h'",                //  3h
            "%m'm'%s's'",           //    2m1s
            "%m'm'",                //    2m
            "%s's'",                //      1s
        };



        public override Task<TypeReaderResult> Read(ICommandContext context, string input, IServiceProvider services)
        {
            //Store as variable, to prevent multiple calls to this method.
            string data = input.ToLowerInvariant();

            //First, try using the built-in TimeSpan.Parse. (Default Implementation)
            if (TimeSpan.TryParseExact(data, _formats, CultureInfo.InvariantCulture, out TimeSpan timeSpan))
                return Task.FromResult(TypeReaderResult.FromSuccess(new TimeSpanext(timeSpan)));
            //@XtremeOwnage - Try using the regular TimeSpan.TryPrase
            else if (TimeSpan.TryParse(data, out TimeSpan Time))
                return Task.FromResult(TypeReaderResult.FromSuccess(new TimeSpanext(Time)));
            else
            {
                return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Failed to parse TimeSpan"));
            }
        }
    }
}