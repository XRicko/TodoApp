using System.Text.Json;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

using ToDoList.Extensions;

using Xunit;

namespace Extensions.Tests
{
    public class DistributedCacheExtensionsTests
    {
        private readonly IDistributedCache cache;

        public DistributedCacheExtensionsTests()
        {
            var opts = Options.Create(new MemoryDistributedCacheOptions());
            cache = new MemoryDistributedCache(opts);
        }

        [Fact]
        public async Task SetRecordAsync_SetsRecord()
        {
            // Arrange
            string key = "ints";
            int[] expected = new[] { 13, 4567, 13591, 129737 };

            // Act
            await cache.SetRecordAsync(key, expected);
            int[] cached = JsonSerializer.Deserialize<int[]>(cache.GetString(key));

            // Arrange
            cached.Should().Equal(expected);
        }

        [Fact]
        public async Task GetRecordAsync_ReturnsCachedRecord()
        {
            // Arrange
            string key = "doubles";
            double[] expected = new[] { 12.31, 315.89, 1.12839 };

            cache.SetString(key, JsonSerializer.Serialize(expected));

            // Act
            double[] cached = await cache.GetRecordAsync<double[]>(key);

            // Arrange
            cached.Should().Equal(expected);
        }

        [Fact]
        public async Task GetRecordAsync_ReturnsDefaultForTheType()
        {
            // Arrange
            string key = "value";

            // Act
            double cached = await cache.GetRecordAsync<double>(key);

            // Assert
            cached.Should().Be(default);
        }
    }
}
