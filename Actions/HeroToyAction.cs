﻿using Dramalord.Data;
using Dramalord.UI;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.SceneInformationPopupTypes;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Actions
{
    internal static class HeroToyAction
    {
        internal static void Apply(Hero hero)
        {
            if (Info.ValidateHeroInfo(hero))
            {
                bool broke = false;
                if (MBRandom.RandomInt(1, 100) < DramalordMCM.Get.ToyBreakChance)
                {
                    Info.SetHeroHasToy(hero, false);
                    TextObject textObject = new TextObject("{=Dramalord130}{HERO.LINK} played with their toy.");
                    StringHelpers.SetCharacterProperties("HERO", hero.CharacterObject, textObject);
                    MBInformationManager.AddQuickInformation(textObject, 1000, hero.CharacterObject, "event:/ui/notification/relation");

                    broke = true;
                }

                LogEntry.AddLogEntry(new LogUsedToy(hero));
                DramalordEvents.OnHeroesUsedToy(hero, broke);
            }
        }
    }
}
