using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FugitiveModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FugitiveServer.Controllers
{
    [Route("api/[controller]")]
    public class FugitiveController : Controller
    {
        private static FugitiveGameInstance instance = new FugitiveGameInstance();

        public FugitiveController()
        {
        }

        // GET api/values
        [HttpGet("join")]
        public string JoinGame()
        {
            return instance.JoinGame();
        }

        // GET api/values
        [HttpGet("board/state")]
        public BoardState GetBoardState()
        {
            return instance.GetVisibleBoardState();
        }

        // GET api/values/5
        [HttpPost("player/{playerId}/draw/{cardType}")]
        public void DrawCard(String playerId, CardType cardType)
        {
            instance.DrawCard(playerId, cardType);
        }

        [HttpGet("player/{playerId}/info")]
        public PlayerInfo GetPlayerInfo(String playerId)
        {
            return instance.PlayerInfo(playerId);
        }

        [HttpGet("player/{playerId}/hideout/latest")]
        public HideoutCard GetLatestHideout(String playerId)
        {
            return instance.LatestHideout(playerId);
        }

        [HttpPost("player/{playerId}/hideout/guess")]
        public void GuessHideouts(String playerId, [FromBody]List<int> ids)
        {
            instance.GuessHideouts(playerId, ids);
        }

        [HttpPost("player/{playerId}/hideout/add")]
        public void AddHideout(String playerId, [FromBody]HideoutStage stage)
        {
            instance.AddHideout(playerId, stage);
        }

        [HttpPost("player/{playerId}/turn/end")]
        public void EndTurn(String playerId)
        {
            instance.EndTurn(playerId);
        }
    }
}
