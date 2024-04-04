﻿using Dramalord.Data;
using Dramalord.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace Dramalord.Actions
{
    internal static class HeroLeaveClanAction
    {
        internal static void Apply(Hero hero, bool withChildren, Hero causedBy)
        {
            if (Info.ValidateHeroInfo(hero) && hero.Clan != null)
            {
                Clan oldClan = hero.Clan;

                Kingdom? kingdom = hero.MapFaction as Kingdom;
                if (kingdom != null && kingdom.RulingClan != null && kingdom.RulingClan.Leader == hero)
                {
                    Campaign.Current.KingdomManager.AbdicateTheThrone(kingdom);
                }

                if (hero.Clan.Leader == hero)
                {
                    ChangeClanLeaderAction.ApplyWithoutSelectedNewLeader(hero.Clan);
                }

                if (hero.GovernorOf != null)
                {
                    ChangeGovernorAction.RemoveGovernorOf(hero);
                }

                if (hero.PartyBelongedTo != null)
                {
                    MobileParty party = hero.PartyBelongedTo;
                    if (party.Army != null && party.Army.LeaderParty == party)
                    {
                        DisbandArmyAction.ApplyByUnknownReason(party.Army);
                    }
                    party.Army = null;

                    if (party.Party.IsActive && party.Party.LeaderHero == hero)
                    {
                        DisbandPartyAction.StartDisband(party);
                        party.Party.SetCustomOwner(null);
                        DestroyPartyAction.Apply(null, party); // test
                    }

                    hero.Clan = null;
                    hero.UpdateHomeSettlement();
                }

                if (withChildren)
                {
                    foreach (Hero child in hero.Children)
                    {
                        if (child.IsChild)
                        {
                            child.Clan = null;
                            child.UpdateHomeSettlement();
                        }
                    }
                }

                LogEntry.AddLogEntry(new EncyclopediaLogLeaveClan(hero, oldClan, causedBy));
                DramalordEvents.OnHeroesLeaveClan(hero, oldClan, causedBy);
            }
        }
    }
}
