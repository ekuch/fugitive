using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FugitiveClient
{
	public partial class MainPage : TabbedPage
    {
		public MainPage()
		{
            BoardState = new BoardStateViewModel(this);
            this.BindingContext = this;

            InitializeComponent();
		}

        public BoardStateViewModel BoardState { get; private set; }
	}
}
