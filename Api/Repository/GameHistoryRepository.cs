namespace Api.Repository
{
    public class GameHistoryRepository : IGameHistoryRepository
    {
        private readonly Context _context;
        private readonly IMapper _mapper;

        public GameHistoryRepository(Context context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task CreateGameHistoryAsync(GameHistoryDto model)
        {
            var blue1 = await _context.Users.Include(x => x.UserProfile).Where(x => x.PlayerName.ToLower() == model.Blue1.ToLower()).FirstOrDefaultAsync();
            var red1 = await _context.Users.Include(x => x.UserProfile).Where(x => x.PlayerName.ToLower() == model.Red1.ToLower()).FirstOrDefaultAsync();
            var blue2 = await _context.Users.Include(x => x.UserProfile).Where(x => x.PlayerName.ToLower() == model.Blue2.ToLower()).FirstOrDefaultAsync();
            var red2 = await _context.Users.Include(x => x.UserProfile).Where(x => x.PlayerName.ToLower() == model.Red2.ToLower()).FirstOrDefaultAsync();

            if (blue1 != null && red1 != null && blue2 != null && red2 != null)
            {
                if (model.Status == SD.Completed)
                {
                    if (model.Winner == SD.Blue)
                    {
                        if (model.GameType == SD.Hokm)
                        {
                            blue1.UserProfile.HokmScore += model.TargetScore;
                            blue2.UserProfile.HokmScore += model.TargetScore;
                        }
                        else
                        {
                            blue1.UserProfile.ShelemScore += model.TargetScore;
                            blue2.UserProfile.ShelemScore += model.TargetScore;
                        }
                    }

                    if (model.Winner == SD.Red)
                    {
                        if (model.GameType == SD.Hokm)
                        {
                            red1.UserProfile.HokmScore += model.TargetScore;
                            red2.UserProfile.HokmScore += model.TargetScore;
                        }
                        else
                        {
                            red1.UserProfile.ShelemScore += model.TargetScore;
                            red2.UserProfile.ShelemScore += model.TargetScore;
                        }
                    }
                }

                //if (model.Status == SD.Left)
                //{
                //    if (model.GameType == SD.Hokm)
                //    {
                //        if (model.LeftBy.ToLower() == blue1.PlayerName.ToLower())
                //        {
                //            blue1.UserProfile.HokmScore -= 10;
                //        }
                //        else if (model.LeftBy.ToLower() == red1.PlayerName.ToLower())
                //        {
                //            red1.UserProfile.HokmScore -= 10;
                //        }
                //        else if (model.LeftBy.ToLower() == blue2.PlayerName.ToLower())
                //        {
                //            blue2.UserProfile.HokmScore -= 10;
                //        }
                //        else if (model.LeftBy.ToLower() == red2.PlayerName.ToLower())
                //        {
                //            red2.UserProfile.HokmScore -= 10;
                //        }
                //    }
                //    else
                //    {
                //        if (model.LeftBy.ToLower() == blue1.PlayerName.ToLower())
                //        {
                //            blue1.UserProfile.ShelemScore -= 1000;
                //        }
                //        else if (model.LeftBy.ToLower() == red1.PlayerName.ToLower())
                //        {
                //            red1.UserProfile.ShelemScore -= 1000;
                //        }
                //        else if (model.LeftBy.ToLower() == blue2.PlayerName.ToLower())
                //        {
                //            blue2.UserProfile.ShelemScore -= 1000;
                //        }
                //        else if (model.LeftBy.ToLower() == red2.PlayerName.ToLower())
                //        {
                //            red2.UserProfile.ShelemScore -= 1000;
                //        }
                //    }
                //}
            }

            _context.GameHistory.Add(_mapper.Map<GameHistory>(model));
        }

        public async Task<List<GameHistoryDto>> GetAllGameHistories()
        {
            return _mapper.Map<List<GameHistoryDto>>(await _context.GameHistory.ToListAsync());
        }
    }
}
