﻿using Dramalord.Data;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;

namespace Dramalord.Actions
{
    internal static class HeroIntercourseAction
    {
        internal static void Apply(Hero hero, Hero target, bool byForce)
        {
            if(Info.ValidateHeroMemory(hero, target) && Info.ValidateHeroInfo(hero) && Info.ValidateHeroInfo(target))
            {
                Info.ChangeIntercourseSkillBy(hero, 1);
                Info.ChangeIntercourseSkillBy(target, (byForce) ? 0 : 1);

                int score = Info.GetTraitscoreToHero(hero, target);

                Info.ChangeEmotionToHeroBy(target, hero, (byForce) ? -100 : score + Info.GetIntercourseSkill(hero));

                Info.ChangeHeroHornyBy(hero, DramalordMCM.Get.HornyLossIntercourse * -1);
                Info.ChangeHeroHornyBy(target, DramalordMCM.Get.HornyLossIntercourse * -1);

                if (target == Hero.MainHero || hero == Hero.MainHero)
                {
                    Hero otherHero = (hero == Hero.MainHero) ? target : hero;
                    if (byForce)
                    {
                        TextObject banner = new TextObject("{=Dramalord140}{HERO.LINK} violated you while being their prisoner.");
                        StringHelpers.SetCharacterProperties("HERO", otherHero.CharacterObject, banner);
                        MBInformationManager.AddQuickInformation(banner, 1000, hero.CharacterObject, "event:/ui/notification/relation");
                    }
                    else
                    {
                        TextObject banner = new TextObject("{=Dramalord141}You were intimate with {HERO.LINK}.");
                        StringHelpers.SetCharacterProperties("HERO", otherHero.CharacterObject, banner);
                        MBInformationManager.AddQuickInformation(banner, 1000, otherHero.CharacterObject, "event:/ui/notification/relation");
                    }

                }

                Hero mother = hero.IsFemale ? hero : target;
                Hero father = target.IsFemale ? hero : target;

                if (mother != father && mother.Spouse == father && mother.IsPregnant)
                {
                    HeroOffspringData? offspring = Info.GetHeroOffspring(mother);
                    if (offspring != null && offspring.Father != father && CampaignTime.Now.ToDays - offspring.Conceived < DramalordMCM.Get.DaysUntilPregnancyVisible)
                    {
                        Info.ChangeOffspringFather(mother, father);
                        TextObject banner = new TextObject("{=Dramalord142}{HERO.LINK} tricked {SPOUSE.LINK} being the cause of her pregancy.");
                        StringHelpers.SetCharacterProperties("HERO", mother.CharacterObject, banner);
                        StringHelpers.SetCharacterProperties("SPOUSE", father.CharacterObject, banner);
                        MBInformationManager.AddQuickInformation(banner, 1000, hero.CharacterObject, "event:/ui/notification/relation");
                    }
                }

                if(mother.Spouse != null && mother.Spouse != father && mother.GetHeroTraits().Honor > 0)
                {
                    mother.SetTraitLevel(DefaultTraits.Honor, mother.GetHeroTraits().Honor - 1);
                }

                if (father.Spouse != null && father.Spouse != mother && father.GetHeroTraits().Honor > 0)
                {
                    father.SetTraitLevel(DefaultTraits.Honor, father.GetHeroTraits().Honor - 1);
                }

                if (DramalordMCM.Get.AffairOutput)
                {
                    LogEntry.AddLogEntry(new LogIntercourse(hero, target, byForce));
                }

                DramalordEvents.OnHeroesIntercourse(hero, target, byForce);
            }
        }
    }
}
