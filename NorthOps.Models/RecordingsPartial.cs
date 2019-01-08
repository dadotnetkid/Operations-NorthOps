using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NorthOps.Models.ViewModels;

namespace NorthOps.Models
{
    public partial class Recordings
    {
        private AcknowledgeTokenViewModel _acknowledgeTokenViewModel;

        public AcknowledgeTokenViewModel AcknowledgeTokenViewModel
        {
            get
            {
                if (_acknowledgeTokenViewModel == null)
                    _acknowledgeTokenViewModel = new AcknowledgeTokenViewModel();
                return _acknowledgeTokenViewModel;
            }
            set => _acknowledgeTokenViewModel = value;
        }
    }
}
