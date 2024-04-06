﻿using Dramalord.Data;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;

namespace Dramalord.Actions
{
    internal static class HeroFlirtAction
    {
        internal static void Apply(Hero hero, Hero target)
        {
            if (Info.ValidateHeroMemory(hero, target))
            {
                int score = Info.GetTraitscoreToHero(hero, target);
                if (score != 0)
                {
                    Info.ChangeHeroHornyBy(target, (score > 0) ? score : 0);
                    Info.ChangeEmotionToHeroBy(target, hero, score);

                    Info.ChangeHeroHornyBy(hero, (score > 0) ? score : 0);
                    Info.ChangeEmotionToHeroBy(hero, target, score);
                }

                Info.SetLastDaySeen(hero, target, CampaignTime.Now.ToDays);
                Info.IncreaseFlirtCountWithHero(hero, target);

                float emotionHero = Info.GetEmotionToHero(hero, target);
                float emotionTarget = Info.GetEmotionToHero(target, hero);

                if (hero != Hero.MainHero && !Info.GetIsCoupleWithHero(hero, target) && emotionHero >= DramalordMCM.Get.MinEmotionForDating && emotionTarget >= DramalordMCM.Get.MinEmotionForDating && (!Info.IsCloseRelativeTo(hero, target) || !DramalordMCM.Get.ProtectFamily))
                {
                    Info.SetIsCoupleWithHero(hero, target, true);
                    LogEntry.AddLogEntry(new EncyclopediaLogStartAffair(hero, target));
                    DramalordEvents.OnHeroesFlirt(hero, target, true);
                }
                else
                {
                    if (DramalordMCM.Get.FlirtOutput)
                        LogEntry.AddLogEntry(new LogFlirt(hero, target));
                    DramalordEvents.OnHeroesFlirt(hero, target, false);
                }
            }
        }
    }  
}
