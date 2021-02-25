using System;
using System.Collections.Generic;
using RSPGame.Models;
using RSPGame.Services.Statistics;
using Xunit;

namespace Tests
{
    public class IndividualTests
    {
        private readonly IIndividualStatService _service;

        public IndividualTests()
        {
            _service = new IndividualStatService();
        }

        [Theory]
        [MemberData(nameof(GamerInfoGameActionsRoundResultForWin))]
        public async void ChangeGamerInfoAfterRound_If_GamerIs_Win_MustChanger_GamerInfo(GamerInfo gamerInfo, GameActions action, RoundResult status)
        {
            await _service.ChangeGamerInfoAfterRound(gamerInfo, action, status);
            
            Assert.Equal(1, gamerInfo.CountWins);
        }
        
        [Theory]
        [MemberData(nameof(GamerInfoGameActionsRoundResultForLose))]
        public async void ChangeGamerInfoAfterRound_If_GamerIs_Lose_MustChanger_GamerInfo(GamerInfo gamerInfo, GameActions action, RoundResult status)
        {
            await _service.ChangeGamerInfoAfterRound(gamerInfo, action, status);
            
            Assert.Equal(1, gamerInfo.CountLoses);
        }
        
        [Theory]
        [MemberData(nameof(GamerInfoGameActionsRoundResultForDraw))]
        public async void ChangeGamerInfoAfterRound_If_GamerIs_Draw_MustChanger_GamerInfo(GamerInfo gamerInfo, GameActions action, RoundResult status)
        {
            await _service.ChangeGamerInfoAfterRound(gamerInfo, action, status);
            
            Assert.Equal(1, gamerInfo.CountDraws);
        }

        [Theory]
        [MemberData(nameof(GamerInfoGameActionsRoundResultForScissor))]
        public async void ChangeGamerInfoAfterRound_ChangeCounterScissors(GamerInfo gamerInfo, GameActions action, RoundResult status)
        {
            await _service.ChangeGamerInfoAfterRound(gamerInfo, action, status);
            
            Assert.Equal(1, gamerInfo.CountScissors);
        }
        
        [Theory]
        [MemberData(nameof(GamerInfoGameActionsRoundResultForPaper))]
        public async void ChangeGamerInfoAfterRound_ChangeCounterPapers(GamerInfo gamerInfo, GameActions action, RoundResult status)
        {
            await _service.ChangeGamerInfoAfterRound(gamerInfo, action, status);
            
            Assert.Equal(1, gamerInfo.CountPapers);
        }
        
        [Theory]
        [MemberData(nameof(GamerInfoGameActionsRoundResultForRock))]
        public async void ChangeGamerInfoAfterRound_ChangeCounterRocks(GamerInfo gamerInfo, GameActions action, RoundResult status)
        {
            await _service.ChangeGamerInfoAfterRound(gamerInfo, action, status);
            
            Assert.Equal(1, gamerInfo.CountRocks);
        }

        [Theory]
        [InlineData(GameActions.None, RoundResult.None)]
        [InlineData(GameActions.Paper, RoundResult.Win)]
        [InlineData(GameActions.Scissors, RoundResult.Lose)]
        [InlineData(GameActions.Rock, RoundResult.Draw)]
        public async System.Threading.Tasks.Task ChangeGamerInfoAfterRound_ThrowArgumentNullException_WhenGamerInfo_IsNull(GameActions action, RoundResult result)
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.ChangeGamerInfoAfterRound(null, action, result));
        }
        
        [Theory]
        [InlineData(RoundResult.Win)]
        [InlineData(RoundResult.Lose)]
        [InlineData(RoundResult.Draw)]
        public async System.Threading.Tasks.Task ChangeGamerInfoAfterRound_ThrowArgumentException_WhenGameActions_IsNone(RoundResult result)
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _service.ChangeGamerInfoAfterRound(new GamerInfo(), GameActions.None, result));
        }
        
        [Theory]
        [InlineData(GameActions.Paper)]
        [InlineData(GameActions.Scissors)]
        [InlineData(GameActions.Rock)]
        public async System.Threading.Tasks.Task ChangeGamerInfoAfterRound_ThrowArgumentException_WhenRoundResult_IsNone(GameActions action)
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _service.ChangeGamerInfoAfterRound(new GamerInfo(), action, RoundResult.None));
        }

        [Fact]
        public void ChangeOnlineTime_MustBeAddedTime()
        {
            var onlineTime = new TimeSpan(1, 1, 1, 1);
            var gamerInfo = new GamerInfo();
            
            _service.ChangeOnlineTime(gamerInfo, onlineTime);
            
            Assert.Equal(onlineTime, gamerInfo.OnlineTime);
        }
        
        [Fact]
        public void ChangeOnlineTime_ThrowArgumentNullException_If_GamerInfo_IsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _service.ChangeOnlineTime(null, TimeSpan.MaxValue));
        }
        
        [Fact]
        public void ChangeOnlineTime_ThrowArgumentException_If_TimeSpan_IsZero()
        {
            Assert.Throws<ArgumentException>(() => _service.ChangeOnlineTime(new GamerInfo(), TimeSpan.Zero));
        }

        public static IEnumerable<object[]> GamerInfoGameActionsRoundResultForWin()
        {
            yield return new object[] {new GamerInfo(), GameActions.Paper, RoundResult.Win};
            yield return new object[] {new GamerInfo(), GameActions.Rock, RoundResult.Win};
            yield return new object[] {new GamerInfo(), GameActions.Scissors, RoundResult.Win};
        }
        
        public static IEnumerable<object[]> GamerInfoGameActionsRoundResultForLose()
        {
            yield return new object[] {new GamerInfo(), GameActions.Paper, RoundResult.Lose};
            yield return new object[] {new GamerInfo(), GameActions.Rock, RoundResult.Lose};
            yield return new object[] {new GamerInfo(), GameActions.Scissors, RoundResult.Lose};
        }
        
        public static IEnumerable<object[]> GamerInfoGameActionsRoundResultForDraw()
        {
            yield return new object[] {new GamerInfo(), GameActions.Paper, RoundResult.Draw};
            yield return new object[] {new GamerInfo(), GameActions.Rock, RoundResult.Draw};
            yield return new object[] {new GamerInfo(), GameActions.Scissors, RoundResult.Draw};
        }
        
        public static IEnumerable<object[]> GamerInfoGameActionsRoundResultForScissor()
        {
            yield return new object[] {new GamerInfo(), GameActions.Scissors, RoundResult.Win};
            yield return new object[] {new GamerInfo(), GameActions.Scissors, RoundResult.Draw};
            yield return new object[] {new GamerInfo(), GameActions.Scissors, RoundResult.Lose};
        }
        
        public static IEnumerable<object[]> GamerInfoGameActionsRoundResultForPaper()
        {
            yield return new object[] {new GamerInfo(), GameActions.Paper, RoundResult.Win};
            yield return new object[] {new GamerInfo(), GameActions.Paper, RoundResult.Draw};
            yield return new object[] {new GamerInfo(), GameActions.Paper, RoundResult.Lose};
        }
        
        public static IEnumerable<object[]> GamerInfoGameActionsRoundResultForRock()
        {
            yield return new object[] {new GamerInfo(), GameActions.Rock, RoundResult.Win};
            yield return new object[] {new GamerInfo(), GameActions.Rock, RoundResult.Draw};
            yield return new object[] {new GamerInfo(), GameActions.Rock, RoundResult.Lose};
        }
    }
}