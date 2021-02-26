using System.Threading.Tasks;
using RSPGame.Services.Statistics;
using RSPGame.Storage;

namespace RSPGame.Services.Game
{
    public class SeriesService
    {
        private readonly IIndividualStatService _individualStatService;

        private readonly IRspStorage _storage;

        public SeriesService(IIndividualStatService individualStatService, IRspStorage storage)
        {
            _individualStatService = individualStatService;
            _storage = storage;
        }
        
        public async Task SaveWork(SeriesRepository seriesRepository)
        {
            foreach (var round in seriesRepository.GetRounds())
            {
                await _individualStatService.ChangeGamerInfoAfterRound(round.Gamer1, round.UserAction1,
                    round.RoundResultForGamer1);
                
                await _individualStatService.ChangeGamerInfoAfterRound(round.Gamer2, round.UserAction2,
                    round.RoundResultForGamer2);
            }

            await _storage.SaveToFile();
        }
    }
}