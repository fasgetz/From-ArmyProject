﻿using ArmyClient.ViewModel.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmyClient.ViewModel.SocialNetworks.VK
{
    class VK_MainVM : MainVM
    {
        protected ArmyVkAPI.MyApiVK api;
        const string login = "89114876557";
        const string password = "Simplepass19";


        public VK_MainVM()
        {
            api = new ArmyVkAPI.MyApiVK();

            api.Authorization(login, password);
        }
    }
}
