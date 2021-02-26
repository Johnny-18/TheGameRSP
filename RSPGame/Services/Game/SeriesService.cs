using System.Threading.Tasks;
using RSPGame.Services.Statistics;
using RSPGame.Storage;

namespace RSPGame.Services.Game
{
    public class SeriesService
    {
        private readonly IIndividualStatService _individualStatService;

        private readonly IRspStorage _storage;

        public SeriesRepository SeriesRepository { get; set; }

        public SeriesService(IRspStorage storage)
        {
            SeriesRepository = new SeriesRepository();
            _individualStatService = new IndividualStatService();
            _storage = storage;
        }
        
        public async Task SaveWork()
        {
            foreach (var round in SeriesRepository.GetRounds())
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