using FugitiveModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FugitiveClient
{
    public class BoardStateViewModel : INotifyPropertyChanged
    {
        private BoardState _Model;
        private Page _Page;
        private PlayerInfo _PlayerInfo;
        private String _PlayerId = null;
        private HttpClient _HttpClient;
        public BoardStateViewModel(Page page)
        {
            _Page = page;
            _Model = new BoardState();
            _HttpClient = new HttpClient();
            Refresh = new Command(x => UpdateBoardState());
            JoinNameCommand = new Command(x => JoinGame(), (x) => { return _PlayerId == null; });
            DrawFourToFourteenCommand = new Command(x => Draw(CardType.HIDEOUT_FOUR_TO_FOURTEEN), (x) => { return CardsLeftFourToFourteen > 0 && CanDraw; });
            DrawFifteenToTwentyEightCommand = new Command(x => Draw(CardType.HIDEOUT_FIFTEEN_TO_TWENTYEIGHT), (x) => { return CardsLeftFifteenToTwentyEight > 0 && CanDraw; });
            DrawTwentyNineToFourtyOneCommand = new Command(x => Draw(CardType.HIDEOUT_TWENTYNINE_TO_FOURTYONE), (x) => { return CardsLeftTwentyNineToFourtyOne > 0 && CanDraw; });
            PassCommand = new Command(x => PassTurn(), (x) => { return _PlayerInfo != null && _Model != null && _PlayerInfo.Role == _Model.PlayersTurn && (_Model.TurnStage == TurnStage.AddHideout || _Model.TurnStage == TurnStage.Guess); });
            PlayHideoutCommand = new Command(x => PlayHideout(), (x) => true); // TODO fix condition
            HideoutPath = new ObservableCollection<HideoutStageGroup>();
            SetHideoutCommand = new Command(x => SetHideout(), x => SelectedHandCard != null);
            AddStepsCommand = new Command(x => AddSteps(), x => SelectedHandCard != null);
            RemoveStepsCommand = new Command(x => RemoveSteps(), x => SelectedStepCard != null);
            GuessHideoutCommand = new Command(x => GuessHideout(), x => SelectedHandCard != null);
            LinkedList<HideoutCard> cards = new LinkedList<HideoutCard>();
            StepCards = new ObservableCollection<HideoutCard>();
            Hand = new ObservableCollection<HideoutCard>();
            
            cards.AddLast(new HideoutCard(8));
            cards.AddLast(new HideoutCard(55));
            HideoutPath.Add(new HideoutStageGroup(new HideoutStage(new HideoutCard(12), new LinkedList<HideoutCard>())));
            HideoutPath.Add(new HideoutStageGroup(new HideoutStage(new HideoutCard(10), cards)));
            
            //TODO UpdateBoardState();
        }

        private void SetHideout()
        {
            HideoutCard currentHideout = Hideout;
            Hideout = SelectedHandCard;
            if (currentHideout != null)
            {
                Hand.Add(currentHideout);
            }
            SelectedHandCard = null;
            Hand.Remove(Hideout);
        }

        private void AddSteps()
        {
            StepCards.Add(SelectedHandCard);
            Hand.Remove(SelectedHandCard);
            SelectedHandCard = null;
        }

        private void RemoveSteps()
        {
            Hand.Add(SelectedStepCard);
            StepCards.Remove(SelectedStepCard);
            SelectedHandCard = null;
        }

        public Command Refresh { get; private set; }
        public Command JoinNameCommand { get; private set; }
        public Command DrawFourToFourteenCommand { get; private set; }
        public Command DrawFifteenToTwentyEightCommand { get; private set; }
        public Command DrawTwentyNineToFourtyOneCommand { get; private set; }
        public Command PlayHideoutCommand { get; private set; }
        public Command SetHideoutCommand { get; private set; }
        public Command AddStepsCommand { get; private set; }
        public Command RemoveStepsCommand { get; private set; }
        public Command PassCommand { get; private set; }
        public Command GuessHideoutCommand { get; private set; }


        private void UpdateCanDraw()
        {
            CanDraw = (_PlayerInfo.Role == _Model.PlayersTurn && _Model.TurnStage == TurnStage.Draw);
        }

        private HideoutCard _Hideout;
        public HideoutCard Hideout

        {
            get { return _Hideout; }
            set
            {
                if (value != _Hideout)
                {
                    _Hideout = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private ObservableCollection<HideoutCard> _Hand;
        public ObservableCollection<HideoutCard> Hand
        {
            get { return _Hand; }
            set
            {
                if (value != _Hand)
                {
                    _Hand = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private HideoutCard _SelectedHandCard;
        public HideoutCard SelectedHandCard
        {
            get { return _SelectedHandCard; }
            set
            {
                if (value != _SelectedHandCard)
                {
                    _SelectedHandCard = value;
                    NotifyPropertyChanged();
                    AddStepsCommand.ChangeCanExecute();
                    SetHideoutCommand.ChangeCanExecute();
                    GuessHideoutCommand.ChangeCanExecute();
                }
            }
        }

        private ObservableCollection<HideoutCard> _StepCards;
        public ObservableCollection<HideoutCard> StepCards
        {
            get { return _StepCards; }
            set
            {
                if (value != _StepCards)
                {
                    _StepCards = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private HideoutCard _SelectedStepCard;
        public HideoutCard SelectedStepCard
        {
            get { return _SelectedStepCard; }
            set
            {
                if (value != _SelectedStepCard)
                {
                    _SelectedStepCard = value;
                    NotifyPropertyChanged();
                    RemoveStepsCommand.ChangeCanExecute();
                }
            }
        }

        private bool _CanDraw;
        public bool CanDraw { get { return _CanDraw; }
            set
            {
                if (value != _CanDraw)
                {
                    _CanDraw = value;
                    NotifyPropertyChanged();
                    DrawFourToFourteenCommand.ChangeCanExecute();
                    DrawFifteenToTwentyEightCommand.ChangeCanExecute();
                    DrawTwentyNineToFourtyOneCommand.ChangeCanExecute();
                }
            }
        }

        private string _PlayersTurn;
        public string PlayersTurn {  get { return _PlayersTurn; }
            set {
                if (value != _PlayersTurn)
                {
                    _PlayersTurn = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _IsFugitive;
        public bool IsFugitive
        {
            get { return _IsFugitive; }
            set
            {
                if (value != _IsFugitive)
                {
                    _IsFugitive = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _IsMarshal;
        public bool IsMarshal
        {
            get { return _IsMarshal; }
            set
            {
                if (value != _IsMarshal)
                {
                    _IsMarshal = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _TurnAction;
        public string TurnAction
        {
            get { return _TurnAction; }
            set
            {
                if (value != _TurnAction)
                {
                    _TurnAction = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _CardsLeftFourToFourteen;
        public int CardsLeftFourToFourteen { get { return _CardsLeftFourToFourteen; }
            set
            {
                if (value != _CardsLeftFourToFourteen)
                {
                    _CardsLeftFourToFourteen = value;
                    NotifyPropertyChanged();
                    DrawFourToFourteenCommand.ChangeCanExecute();
                }
            }
        }

        private int _CardsLeftFifteenToTwentyEight;
        public int CardsLeftFifteenToTwentyEight
        {
            get { return _CardsLeftFifteenToTwentyEight; }
            set
            {
                if (value != _CardsLeftFifteenToTwentyEight)
                {
                    _CardsLeftFifteenToTwentyEight = value;
                    NotifyPropertyChanged();
                    DrawFifteenToTwentyEightCommand.ChangeCanExecute();
                }
            }
        }

        private int _CardsLeftTwentyNineToFourtyOne;
        public int CardsLeftTwentyNineToFourtyOne
        {
            get { return _CardsLeftTwentyNineToFourtyOne; }
            set
            {
                if (value != _CardsLeftTwentyNineToFourtyOne)
                {
                    _CardsLeftTwentyNineToFourtyOne = value;
                    NotifyPropertyChanged();
                    DrawTwentyNineToFourtyOneCommand.ChangeCanExecute();
                }
            }
        }

        public ObservableCollection<HideoutStageGroup> HideoutPath { get; private set; }

        private void UpdateBoardState()
        {
            HandleHttpExceptions(() =>
            {
                HttpResponseMessage message = _HttpClient.GetAsync(FugitiveUriBuilder.GetBoardState()).Result;
                if (message.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var jsonString = message.Content.ReadAsStringAsync().Result;
                    _Model = BoardState.fromJson(jsonString);
                    PlayersTurn = _Model.PlayersTurn.ToString();
                    TurnAction = _Model.TurnStage.ToString();
                    CardsLeftFourToFourteen = _Model.DrawCardsLeft[CardType.HIDEOUT_FOUR_TO_FOURTEEN];
                    CardsLeftFifteenToTwentyEight = _Model.DrawCardsLeft[CardType.HIDEOUT_FIFTEEN_TO_TWENTYEIGHT];
                    CardsLeftTwentyNineToFourtyOne = _Model.DrawCardsLeft[CardType.HIDEOUT_TWENTYNINE_TO_FOURTYONE];
                    HideoutPath.Clear();
                    foreach (HideoutStage stage in _Model.HideoutPath)
                    {
                        HideoutPath.Add(new HideoutStageGroup(stage));
                    }
                    GetPlayerInfo();                
                    UpdateCanDraw();
                    PassCommand.ChangeCanExecute();
                }
            });
        }

        private void HandleHttpExceptions(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch(Exception e)
            {
#if DEBUG
                _Page.DisplayAlert(e.GetType().ToString(), e.Message, "OK");
#else
                _Page.DisplayAlert("Server Error", "Try again later", "OK");
#endif
            }
        }

        private void JoinGame()
        {
            if (_PlayerId == null)
            {
                HandleHttpExceptions(() =>
                {
                    HttpResponseMessage message = _HttpClient.GetAsync(FugitiveUriBuilder.JoinGame()).Result;
                    if (message.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        _PlayerId = message.Content.ReadAsStringAsync().Result;
                        JoinNameCommand.ChangeCanExecute();
                        GetPlayerInfo();
                        UpdateCanDraw();
                    }
                });
            }
        }

        private void Draw(CardType cardType)
        {
            HandleHttpExceptions(() =>
            {
                HttpResponseMessage message = _HttpClient.PostAsync(FugitiveUriBuilder.DrawCard(_PlayerId, cardType), new StringContent("")).Result;
                if (message.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var jsonString = message.Content.ReadAsStringAsync().Result;
                    //TODO AddHideoutToHand(HideoutCard.fromJson(jsonString));
                }
            });
            UpdateBoardState();
        }

        private void GetPlayerInfo()
        {
            HandleHttpExceptions(() =>
            {
                HttpResponseMessage message = _HttpClient.GetAsync(FugitiveUriBuilder.GetPlayerInfo(_PlayerId)).Result;
                if (message.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var jsonString = message.Content.ReadAsStringAsync().Result;
                    _PlayerInfo = PlayerInfo.fromJson(jsonString);
                    IsMarshal = _PlayerInfo.Role == Role.Marshal;
                    IsFugitive = _PlayerInfo.Role == Role.Fugitive;
                    SelectedHandCard = null;
                    SelectedStepCard = null;
                    StepCards.Clear();
                    Hand.Clear();
                    foreach(HideoutCard handCard in _PlayerInfo.Hand)
                    {
                        Hand.Add(handCard);
                    }
                }
            });
        }


        private void PassTurn()
        {

            HandleHttpExceptions(() =>
            {
                HttpResponseMessage message = _HttpClient.PostAsync(FugitiveUriBuilder.PassTurn(_PlayerId), new StringContent("")).Result;
                // Is there anything for us to check?
            });
            UpdateBoardState();
        }

        private void PlayHideout()
        {
            HandleHttpExceptions(() =>
            {
                LinkedList<HideoutCard> stepCards = new LinkedList<HideoutCard>();
                foreach (HideoutCard stepCard in StepCards)
                {
                    stepCards.AddLast(stepCard);
                }
                HideoutStage stage = new HideoutStage(Hideout, stepCards);
                HttpResponseMessage message = _HttpClient.PostAsync(FugitiveUriBuilder.AddHideout(_PlayerId), new StringContent(stage.ToJson(), System.Text.Encoding.UTF8, "application/json")).Result;
                // Is there anything for us to check?
            });
        }



        #region INofiyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
#endregion
    }
}
