using FugitiveModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace FugitiveClient.ViewModel
{
    class HideoutCardViewModel
    {
        private HideoutCard _HideoutCard;

        HideoutCardViewModel(HideoutCard card)
        {
            _HideoutCard = card;
        }

        public int Hideout { get { return _HideoutCard.Hideout; } }

        public int Steps { get { return _HideoutCard.Steps; } }
    }
}
