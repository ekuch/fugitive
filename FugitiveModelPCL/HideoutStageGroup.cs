using FugitiveModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FugitiveModel
{
    // For View Model
    public class HideoutStageGroup : List<HideoutCard>
    {
        public HideoutStageGroup(HideoutStage stage)
        {
            this.Hideout = stage.Hideout;
            foreach (HideoutCard steps in stage.ExtraSteps)
            {
                this.Add(steps);
            }
        }
        public HideoutCard Hideout { get; set; }
    }
}
