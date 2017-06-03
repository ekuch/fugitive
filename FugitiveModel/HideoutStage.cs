using System.Collections.Generic;

namespace FugitiveModel
{
    public class HideoutStage
    {
        public HideoutStage(HideoutCard hideout, LinkedList<HideoutCard> extraSteps)
        {
            Hideout = hideout;
            ExtraSteps = extraSteps;
            IsVisible = false;
        }

        public HideoutCard Hideout { get; set; }
        public LinkedList<HideoutCard> ExtraSteps { get; set; }
        public bool IsVisible { get; set; }

        public HideoutStage ToVisibleInstance()
        {
            if (IsVisible)
            {
                return this;
            }

            LinkedList<HideoutCard> extraSteps = new LinkedList<HideoutCard>();
            for (int n = 0; n < ExtraSteps.Count; ++n)
            {
                extraSteps.AddLast(HideoutCard.InvisibleCard);
            }
            HideoutStage hiddenStage = new HideoutStage(HideoutCard.InvisibleCard, extraSteps);
            hiddenStage.IsVisible = false;
            return hiddenStage;
        }

        public HideoutStage NormalizeStage(bool isVisible)
        {
            LinkedList<HideoutCard> extraSteps = new LinkedList<HideoutCard>();
            foreach (HideoutCard extraStepCard in ExtraSteps)
            {
                extraSteps.AddLast(new HideoutCard(extraStepCard.Hideout));
            }
            HideoutStage hiddenStage = new HideoutStage(new HideoutCard(Hideout.Hideout), extraSteps);
            hiddenStage.IsVisible = isVisible;
            return hiddenStage;
        }

        public bool AreCardsUnique()
        {
            HashSet<int> ids = new HashSet<int>();
            ids.Add(Hideout.Hideout);

            foreach (HideoutCard extraStepCard in ExtraSteps)
            {
                if (ids.Contains(extraStepCard.Hideout))
                {
                    return false;
                }
                ids.Add(extraStepCard.Hideout);
            }
            return true;
        }

        public HashSet<int> getIds()
        {
            HashSet<int> ids = new HashSet<int>();
            ids.Add(Hideout.Hideout);

            foreach (HideoutCard extraStepCard in ExtraSteps)
            {
                ids.Add(extraStepCard.Hideout);
            }
            return ids;
        }

        public bool isValid(int previousHideout)
        {
            if (previousHideout >= Hideout.Hideout)
            {
                return false;
            }

            int extraSteps = 0;
            foreach(HideoutCard card in ExtraSteps)
            {
                extraSteps += card.Hideout;
            }

            if (previousHideout + extraSteps + 3 <= Hideout.Hideout)
            {
                return true;
            }
            return false;
        }
    }
}
