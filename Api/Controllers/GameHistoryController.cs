namespace Api.Controllers
{
    [Authorize]
    public class GameHistoryController : ApiCoreController
    {
        private ApiResult _response;
        public GameHistoryController()
        {
            _response = new ApiResult();
        }

        [HttpGet]
        public async Task<ActionResult<List<GameHistoryDto>>> GetAllHistories()
        {
            return Ok(await UnitOfWork.GameHistoryRepository.GetAllGameHistories());
        }

        [HttpPost]
        public async Task<ActionResult<ApiResult>> CreateGameHistory(GameHistoryDto model)
        {
            await UnitOfWork.GameHistoryRepository.CreateGameHistoryAsync(model);
            if (UnitOfWork.HasChanges())
            {
                if (await UnitOfWork.CompleteAsync())
                {
                    return _response;
                }
            }

            _response.IsSuccess = false;
            _response.Message = "Unable to create game history";

            return _response;
        }
    }
}
