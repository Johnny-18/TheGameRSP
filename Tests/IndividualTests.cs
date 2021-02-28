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

        [Fact]
        public void ChangeOnlineTime_MustBeAddedTime()
        {
            var onlineTime = new TimeSpan(1, 1, 1, 1);
            var gamerInfo = new GamerInfo();
            
            _service.ChangeOnlineTime(gamerInfo, onlineTime);
            
            Assert.Equal(onlineTime, gamerInfo.OnlineTime);
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