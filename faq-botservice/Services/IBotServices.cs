﻿using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.AI.QnA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FAQBot
{
    public interface IBotServices
    {
        LuisRecognizer Dispatch { get; }
        QnAMaker FHBielefeldQnA { get; }
    }
}
