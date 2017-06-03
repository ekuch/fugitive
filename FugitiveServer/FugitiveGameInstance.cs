using FugitiveModel;
using System;
using System.Collections.Generic;

namespace FugitiveServer
{
    public class FugitiveGameInstance
    {
        private Dictionary<CardType, LinkedList<HideoutCard>> _DrawCards;
        private BoardState _BoardState;
        private Dictionary<String, PlayerInfo> _PlayerInfo;
        private int _InitialFugitiveDraws;
        private int _InitialFugitiveHideouts;

        private readonly Random random = new Random();

        private struct ShuffleNode<T>
        {
            public ShuffleNode(Random random, T data)
            {
                this.data = data;
                this.order = random.Next();
            }
            public T data;
            public int order;
        }

        private LinkedList<HideoutCard> CreateDrawCards(int start, int totalCount)
        {
            List<ShuffleNode<HideoutCard>> shuffleDeck = new List<ShuffleNode<HideoutCard>>();
            int pastIndexMax = start + totalCount;
            for (int index = start; index < pastIndexMax; ++index)
            {
                shuffleDeck.Add(new ShuffleNode<HideoutCard>(random, new HideoutCard(index)));
            }
            shuffleDeck.Sort((x, y) => x.order.CompareTo(y.order));

            LinkedList<HideoutCard> drawCards = new LinkedList<HideoutCard>();
            foreach(ShuffleNode<HideoutCard> shuffledCard in shuffleDeck)
            {
                drawCards.AddLast(shuffledCard.data);
            }
            return drawCards;
                
        }

        private PlayerInfo ValidateState(string playerId, TurnStage requiredStage)
        {
            PlayerInfo playerInfo = PlayerInfo(playerId);

            if (_BoardState.PlayersTurn != playerInfo.Role)
            {
                throw new Exception("It is not your turn");
            }
            if (_BoardState.TurnStage != requiredStage)
            {
                throw new Exception("Action is not valid");
            }
            return playerInfo;
        }

        internal void GuessHideouts(string playerId, List<int> ids)
        {
            lock (_PlayerInfo)
            {
                PlayerInfo playerInfo = ValidateState(playerId, TurnStage.Guess);

                HashSet<int> hideouts = new HashSet<int>();
                foreach (HideoutStage hideoutStage in _BoardState.HideoutPath)
                {
                    if (!hideoutStage.IsVisible)
                    {
                        hideouts.Add(hideoutStage.Hideout.Hideout);
                    }
                }

                foreach(int guess in ids)
                {
                    if (!hideouts.Contains(guess))
                    {
                        EndTurn(playerId, true);
                        throw new Exception("Incorrect Guess");
                    }
                }

                foreach (HideoutStage hideoutStage in _BoardState.HideoutPath)
                {
                    if (ids.Contains(hideoutStage.Hideout.Hideout))
                    {
                        hideoutStage.IsVisible = true;
                    }
                }
                
                EndTurn(playerId);
            }
        }

        internal void AddHideout(string playerId, HideoutStage possiblyIllegalStage)
        {
            lock (_PlayerInfo)
            {
                PlayerInfo playerInfo = ValidateState(playerId, TurnStage.AddHideout);

                HideoutStage stage = possiblyIllegalStage.NormalizeStage(false); // we want this to be hidden, and we want correct values for steps taken.
                if (!stage.AreCardsUnique())
                {
                    throw new Exception("Illegal Hideout");
                }
                HashSet<int> stageIdCards = stage.getIds();
                if (!playerInfo.GetHandIds().IsSupersetOf(stageIdCards))
                {
                    throw new Exception("Illegal Hideout");
                }

                // Check that it is valid location
                HideoutCard latestHideout = LatestHideout();
                if (!stage.isValid(latestHideout.Hideout))
                {
                    throw new Exception("Illegal Hideout");
                }

                List<HideoutCard> cardsToRemoveFromHand = new List<HideoutCard>();
                foreach (HideoutCard card in playerInfo.Hand)
                {
                    if (stageIdCards.Contains(card.Hideout))
                    {
                        cardsToRemoveFromHand.Add(card);
                    }
                }
                foreach (HideoutCard card in cardsToRemoveFromHand)
                {
                    playerInfo.Hand.Remove(card);
                }
                _BoardState.HideoutPath.AddLast(stage);

                EndTurn(playerId);
            }
        }

        internal void EndTurn(string playerId, bool badGuess = false)
        {
            if (_BoardState.TurnStage == TurnStage.AddHideout)
            {
                if (this._InitialFugitiveHideouts > 1)
                {
                    --_InitialFugitiveHideouts;
                    return;
                }
                _BoardState.PlayersTurn = Role.Marshal;
                _BoardState.TurnStage = TurnStage.Draw;
            }
            else if(_BoardState.TurnStage == TurnStage.Guess)
            {
                int remaningHideouts = _BoardState.NumberOfHiddenHideouts();
                if (remaningHideouts == 0)
                {
                    _BoardState.PlayersTurn = Role.Marshal;
                    _BoardState.TurnStage = TurnStage.GameOver;
                    return;
                }

                if (LatestHideout().Hideout == 42)
                {
                    if (badGuess)
                    {
                        _BoardState.PlayersTurn = Role.Fugitive;
                        _BoardState.TurnStage = TurnStage.GameOver;
                        return;
                    }
                    // Allowed to keep guessing.
                    return;
                }
                else
                {
                    _BoardState.PlayersTurn = Role.Fugitive;
                    _BoardState.TurnStage = TurnStage.Draw;
                }
            }
        }

        public FugitiveGameInstance()
        {
            _DrawCards = new Dictionary<CardType, LinkedList<HideoutCard>>();
            _DrawCards.Add(CardType.HIDEOUT_FOUR_TO_FOURTEEN, CreateDrawCards(4, 11));
            _DrawCards.Add(CardType.HIDEOUT_FIFTEEN_TO_TWENTYEIGHT, CreateDrawCards(15, 14));
            _DrawCards.Add(CardType.HIDEOUT_TWENTYNINE_TO_FOURTYONE, CreateDrawCards(29, 13));

            _BoardState = new BoardState();
            _BoardState.HideoutPath = new LinkedList<HideoutStage>();
            HideoutStage initialStage = new HideoutStage(new HideoutCard(1), new LinkedList<HideoutCard>());
            initialStage.IsVisible = true;
            _BoardState.HideoutPath.AddLast(initialStage);
            _BoardState.PlayersTurn = Role.Fugitive;
            _BoardState.TurnStage = TurnStage.WaitForPlayers;
            _BoardState.DrawCardsLeft = new Dictionary<CardType, int>();
            foreach (CardType cardType in _DrawCards.Keys)
            {
                _BoardState.DrawCardsLeft.Add(cardType, _DrawCards[cardType].Count);
            }


            _PlayerInfo = new Dictionary<string, PlayerInfo>();
            _InitialFugitiveDraws = 2;
            _InitialFugitiveHideouts = 2;

            // TODO Remove these
            JoinGame();
            JoinGame();
#if DEBUG
            _BoardState.Comment = "";
            foreach(String playerId in _PlayerInfo.Keys)
            {
                _BoardState.Comment += playerId + " : " + _PlayerInfo[playerId].Role + Environment.NewLine;
            }
#endif
        }

        public BoardState GetVisibleBoardState()
        {
            return _BoardState.ToVisibleInstance();
        }

        public String JoinGame()
        {
            lock (_PlayerInfo)
            {
                String player = Guid.NewGuid().ToString();

                if (_PlayerInfo.Count >= 2)
                {
                    throw new Exception("Game full");
                }

                if (_PlayerInfo.Count == 0)
                {
                    _PlayerInfo.Add(player, 
                        new PlayerInfo()
                        {
                            Role = Role.Fugitive,
                            Hand = new List<HideoutCard>()
                            {
                                new HideoutCard(42),
                                new HideoutCard(2),
                                new HideoutCard(3)
                            }
                        });
                }
                else if (_PlayerInfo.Count == 1)
                {
                    _PlayerInfo.Add(player,
                        new PlayerInfo()
                        {
                            Role = Role.Marshal,
                            Hand = new List<HideoutCard>()
                        });
                }

                if (_PlayerInfo.Count == 2)
                {
                    _BoardState.PlayersTurn = Role.Fugitive;
                    _BoardState.TurnStage = TurnStage.Draw;
                }

                return player;
            }
        }

        public void DrawCard(String playerId, CardType cardType)
        {
            lock(_BoardState)
            {
                PlayerInfo playerInfo = ValidateState(playerId, TurnStage.Draw);

                HideoutCard card = _DrawCards[cardType].Last.Value;
                _DrawCards[cardType].RemoveLast();

                playerInfo.Hand.Add(card);


                if (_BoardState.PlayersTurn == Role.Fugitive)
                {
                    if (_InitialFugitiveDraws > 1)
                    {
                        --_InitialFugitiveDraws;
                    }
                    else
                    {
                        _BoardState.TurnStage = TurnStage.AddHideout;
                    }
                }
                else
                {
                    _BoardState.TurnStage = TurnStage.Guess;
                }
                
            }
        }

        public PlayerInfo PlayerInfo(String playerId)
        {
            PlayerInfo playerInfo;
            if (!_PlayerInfo.TryGetValue(playerId, out playerInfo))
            {
                throw new Exception("Player does not exist");
            }
            return playerInfo;
        }

        public HideoutCard LatestHideout(String playerId)
        {
            if (PlayerInfo(playerId).Role == Role.Fugitive)
            {
                return _BoardState.HideoutPath.Last.Value.Hideout;
            }
            throw new Exception("You are not the fugitive");
        }

        private HideoutCard LatestHideout()
        {
            return _BoardState.HideoutPath.Last.Value.Hideout;
        }
    }
}
