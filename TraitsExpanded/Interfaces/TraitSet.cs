using System;
using System.Collections.Generic;
using System.Text.Json;
using TaleWorlds.CampaignSystem;

namespace TraitsExpanded
{
    public abstract class TraitSet
    {
        public abstract Guid Id { get; }

        public abstract string Name { get; }

        public abstract int Version { get; }

        public virtual List<ITrait> Traits { get; set; } = new List<ITrait>();

        public virtual int CurrentTraitIndex { get; set; } = 0;

        public virtual NPCRestrictionEnum NPCRestriction { get; set; } = NPCRestrictionEnum.NONE;

        public virtual bool IsActive { get; set; } = false;

        public virtual CharacterObject CurrentCharacter { get; set; }

        public virtual void SerializeCustomState(Utf8JsonWriter writer) { }

        public virtual void Init(CharacterObject character)
        {
            CurrentCharacter = character;
            Util.LogMessage($"{Id} initialized");
        }

        public virtual bool Activate()
        {
            var succeeded = false;

            if (Traits.Count > 0 && CurrentCharacter != null)
            {
                succeeded = Traits[CurrentTraitIndex].Activate(CurrentCharacter);
                IsActive = true;
            }
            else
            {
                IsActive = false;
                succeeded = true;
            }

            return succeeded;
        }

        public virtual bool Deactivate()
        {
            var succeeded = false;

            if (Traits.Count > 0 && CurrentCharacter != null)
            {
                succeeded = Traits[CurrentTraitIndex].Deactivate(CurrentCharacter);
                IsActive = false;
            }

            return succeeded;
        }

        public virtual bool Upgrade()
        {
            var succeeded = false;

            if (CurrentTraitIndex <= Traits.Count - 1)
            {
                if (CurrentTraitIndex > 0)
                {
                    Traits[CurrentTraitIndex].Deactivate(CurrentCharacter);
                    CurrentTraitIndex++;
                    Traits[CurrentTraitIndex].Activate(CurrentCharacter);
                }
                else
                {
                    succeeded = Activate();
                }

                succeeded = true;
            }

            return succeeded;
        }

        public virtual bool Downgrade()
        {
            var succeeded = false;

            if (CurrentTraitIndex > 0)
            {
                Traits[CurrentTraitIndex].Deactivate(CurrentCharacter);
                CurrentTraitIndex--;
                Traits[CurrentTraitIndex].Activate(CurrentCharacter);
                succeeded = true;
            }
            else
            {
                succeeded = Deactivate();
            }

            return succeeded;
        }

        public virtual void MissionTick(MissionStatusInfo statusInfo)
        {
        }

        public virtual void CampaignTick(CampaignStatusInfo statusInfo)
        {
        }

        public virtual void GameTick(GameStatusInfo statusInfo)
        {
        }
    }
}
