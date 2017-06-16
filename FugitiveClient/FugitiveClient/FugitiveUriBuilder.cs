using System;
using System.Collections.Generic;
using System.Text;
using FugitiveModel;

namespace FugitiveClient
{
    class FugitiveUriBuilder
    {
        public static String GetBoardState()
        {
            return string.Format("{0}/api/Fugitive/board/state", ServerAddress());
        }

        internal static string DrawCard(String playerId, CardType cardType)
        {
            return string.Format("{0}/api/Fugitive/player/{1}/draw/{2}", ServerAddress(), playerId, cardType.ToString());
        }

        internal static string JoinGame() // TODO: Game ID
        {
            return string.Format("{0}/api/Fugitive/join", ServerAddress());
        }

        private static string ServerAddress()
        {
            return "http://localhost:51982";
        }

        internal static string GetPlayerInfo(string playerId)
        {
            return string.Format("{0}/api/Fugitive/player/{1}/info", ServerAddress(), playerId);
        }

        internal static string PassTurn(string playerId)
        {
            return string.Format("{0}/api/Fugitive/player/{1}/turn/end", ServerAddress(), playerId);
        }

        internal static string AddHideout(string playerId)
        {
            return string.Format("{0}/api/Fugitive/player/{1}/hideout/add", ServerAddress(), playerId);
        }
    }
}
