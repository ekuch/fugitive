using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace FugitiveModel
{
    public class BoardState
    {
        public static BoardState fromJson(string json)
        {
            return JsonConvert.DeserializeObject<BoardState>(json);
        }

        public LinkedList<HideoutStage> HideoutPath { get; set; }
        public Dictionary<CardType, int> DrawCardsLeft { get; set; }
        
        public Role PlayersTurn { get; set; }
        public TurnStage TurnStage { get; set; }

        public String Comment { get; set; }

        public BoardState ToVisibleInstance()
        {
            BoardState visibleBoard = new BoardState();
            visibleBoard.DrawCardsLeft = this.DrawCardsLeft;
            visibleBoard.PlayersTurn = this.PlayersTurn;
            visibleBoard.TurnStage = this.TurnStage;

            visibleBoard.HideoutPath = new LinkedList<HideoutStage>();
            foreach (HideoutStage hideout in HideoutPath)
            {
                visibleBoard.HideoutPath.AddLast(hideout.ToVisibleInstance());
            }
            visibleBoard.Comment = Comment;
            return visibleBoard;
        }

        public int NumberOfHiddenHideouts()
        {
            int count = 0;
            foreach(HideoutStage stage in HideoutPath)
            {
                if (!stage.IsVisible)
                {
                    ++count;
                }
            }
            return count;
        }
    }
}
