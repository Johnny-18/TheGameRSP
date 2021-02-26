using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using RSPGame.Models;
using RSPGame.Services.Statistics;
using RSPGame.Storage;
using Xunit;

namespace Tests
{
    public class GeneralStatTests
    {
        private readonly GeneralStatService _service;

        public GeneralStatTests()
        {
            _service = new GeneralStatService();
        }

        [Fact]
        public async void GetStatAsync_IncomeThreeObjectsGamerInfos_ButOneHaveLessThenTenGames_Return_TwoGamerInfos_WithGamesMoreThenTen()
        {
            var users = new List<User>
            {
                new User
                {
                    GamerInfo = new GamerInfo
                    {
                        CountWins = 12
                    }
                },
                new User
                {
                    GamerInfo = new GamerInfo
                    {
                        CountWins = 3
                    }
                },
                new User {
                    GamerInfo = new GamerInfo
                    {
                        CountWins = 20
                    }
                }
            };
            
            var mock = new Mock<IRspStorage>();

            mock.Setup(x => x.GetUsersAsync()).ReturnsAsync(() => users);

            var stats = (await _service.GetStatAsync(mock.Object)).ToList();
            
            Assert.Equal(2, stats.Count);
        }

        [Fact]
        public async Task GetStatAsync_ThrowArgumentNullException_IfStorageIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.GetStatAsync(null));
        }
    }
}