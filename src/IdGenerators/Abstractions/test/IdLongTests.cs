﻿namespace ClickView.GoodStuff.IdGenerators.Abstractions.Tests;

using System;
using Xunit;

public class IdLongTests
{
    [Fact]
    public void ToString_Returns()
    {
        var id = new IdLong(123);

        Assert.Equal("_123", id.ToString());
    }

    [Fact]
    public void Parse_Returns()
    {
        var id = IdLong.Parse("_1234");

        Assert.Equal(1234, (long)id);
    }

    [Theory]
    [InlineData("1234")]
    [InlineData("")]
    public void Parse_MalformedId_ThrowsFormatException(string str)
    {
        Assert.Throws<FormatException>(() => IdLong.Parse(str));
    }

    [Fact]
    public void TryParse_ReturnsTrue()
    {
        Assert.True(IdLong.TryParse("_1234", out var value));
        Assert.Equal(1234, (long)value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("1234")]
    [InlineData("")]
    public void TryParse_MalformedId_ReturnsFalse(string? str)
    {
        Assert.False(IdLong.TryParse(str, out _));
    }

    [Fact]
    public void Compare_SameValue_Equal()
    {
        Assert.Equal(new IdLong(555), new IdLong(555));
    }

    [Fact]
    public void Compare_DifferentValue_NotEqual()
    {
        Assert.NotEqual(new IdLong(455), new IdLong(555));
    }

    [Fact]
    public void Value_Returns()
    {
        var id = new IdLong(666);
        Assert.Equal(666, id.Value);
    }
}
