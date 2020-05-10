using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace TraitsExpanded
{
    public abstract class TraitSet
    {
        public abstract string Name { get; }
        
        protected virtual List<ITrait> Traits { get; } = new List<ITrait>();
        
        public virtual string Id { get; }
        
        public virtual int CurrentTraitIndex { get; protected set; } = 0;
        
        
        public virtual NPCRestrictionEnum NPCRestriction { get; } = NPCRestrictionEnum.NONE;
        
        public virtual bool IsActive { get; protected set; } = false;
        
        public virtual CharacterObject CurrentCharacter { get; protected set; }
        
        public TraitSet(CharacterObject character)
        {
            CurrentCharacter = character;
            Util.LogMessage($"{Id} initialized");
            
            if (CurrentCharacter != null)
            {
                Util.LogMessage($"{Id} initialized for a given character! {character.Name} Activating!");
                Activate();
            }
        }

        protected TraitSet()
        {
            Util.LogMessage($"{Id} initialized");
        }

        public virtual bool Activate(CharacterObject character)
        {
            var succeeded = false;

            if (character != CurrentCharacter) CurrentCharacter = character;
            
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

        protected virtual bool Activate()
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

        public IEnumerable<ITrait> GetPositiveTraits() => Traits.Where(trait => trait.Cost >= 0).ToList();
        public IEnumerable<ITrait> GetNegativeTraits() => Traits.Where(trait => trait.Cost < 0).ToList();
        
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