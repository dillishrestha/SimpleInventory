﻿using SimpleInventory.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleInventory.ViewModels
{
    class ManageItemsViewModel : BaseViewModel
    {
        public ManageItemsViewModel(IChangeViewModel viewModelChanger) : base(viewModelChanger)
        {
        }
    }
}