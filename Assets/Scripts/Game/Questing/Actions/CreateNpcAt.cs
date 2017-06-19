﻿// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;

namespace DaggerfallWorkshop.Game.Questing.Actions
{
    /// <summary>
    /// Tipton calls this "create npc" but its true function seems to reserve a quest site
    /// before linking resources. "create npc" is usually followed by "place npc"
    /// but can also be followed by "place item" or "create foe" for example.
    /// This action likely initiates some book-keeping in Daggerfall's quest system.
    /// In Daggerfall Unity this creates a SiteLink in QuestMachine.
    /// </summary>
    public class CreateNpcAt : ActionTemplate
    {
        Symbol placeSymbol;

        public override string Pattern
        {
            get { return @"create npc at (?<aPlace>\w+)"; }
        }

        public CreateNpcAt(Quest parentQuest)
            : base(parentQuest)
        {
        }

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            base.CreateNew(source, parentQuest);

            // Source must match pattern
            Match match = Test(source);
            if (!match.Success)
                return null;

            // Factory new action
            CreateNpcAt action = new CreateNpcAt(parentQuest);
            action.placeSymbol = new Symbol(match.Groups["aPlace"].Value);

            return action;
        }

        public override void Update(Task caller)
        {
            base.Update(caller);

            // Reserve SiteLink for Place resource
            // This will also be done automatically by "place npc", "place foe", etc.
            if (!QuestMachine.HasSiteLink(ParentQuest, placeSymbol))
                QuestMachine.CreateSiteLink(ParentQuest, placeSymbol);

            // Possible historical note:
            //  Many quests are very strict about first reserving a site using "create npc at"
            //  before placing a quest resource like an NPC, Foe, Item at that site.
            //  More developed quests (e.g. guild quests) don't seem to have this requirement.
            //  One likely reason is that quest writers would commonly forget to reserve the site first,
            //  leading to developers making this a more automated process down the track.
            //  Daggerfall Unity will create a SiteLink either way to support both formats.

            SetComplete();
        }
    }
}