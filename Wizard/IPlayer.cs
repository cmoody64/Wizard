﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wizard
{
    interface IPlayer
    {
        void TakeCard(Card card);
        Card PlayCard();
    }
}