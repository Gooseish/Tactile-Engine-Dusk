using System.IO;

namespace Tactile.State
{
    class Game_Skills_State : Game_State_Component
    {
        internal Game_Dance_State Dance_State { get; private set; }
        internal Game_Sacrifice_State Sacrifice_State { get; private set; }
        internal Game_Steal_State Steal_State { get; private set; }
        internal Game_Skill_Flash_State Skill_Flash_State { get; private set; }
        internal Game_Swap_State Swap_State { get; private set; }
        internal Game_Teleport_State Teleport_State { get; private set; }
        internal Game_Transform_State Transform_State { get; private set; }
        internal Game_Summon_State Summon_State { get; private set; }
        internal Game_Mass_Slow_State Mass_Slow_State { get; private set; }

        internal override void write(BinaryWriter writer)
        {
            Dance_State.write(writer);
            Sacrifice_State.write(writer);
            Steal_State.write(writer);
            Skill_Flash_State.write(writer);
            Swap_State.write(writer);
            Teleport_State.write(writer);
            Transform_State.write(writer);
            Summon_State.write(writer);
            Mass_Slow_State.write(writer);
        }

        internal override void read(BinaryReader reader)
        {
            Dance_State.read(reader);
            Sacrifice_State.read(reader);
            Steal_State.read(reader);
            Skill_Flash_State.read(reader);
            Swap_State.read(reader);
            Teleport_State.read(reader);
            Transform_State.read(reader);
            Summon_State.read(reader);
            Mass_Slow_State.read(reader);
        }

        internal Game_Skills_State()
        {
            Dance_State = new Game_Dance_State();
            Sacrifice_State = new Game_Sacrifice_State();
            Steal_State = new Game_Steal_State();
            Skill_Flash_State = new Game_Skill_Flash_State();
            Swap_State = new Game_Swap_State();
            Teleport_State = new Game_Teleport_State();
            Transform_State = new Game_Transform_State();
            Summon_State = new Game_Summon_State();
            Mass_Slow_State = new Game_Mass_Slow_State();
        }

        internal override void update()
        {
            Dance_State.update();
            Sacrifice_State.update();
            Steal_State.update();
            Skill_Flash_State.update();
            Swap_State.update();
            Teleport_State.update();
            Transform_State.update();
            Summon_State.update();
            Mass_Slow_State.update();
        }

        internal bool is_skill_ready()
        {
            if (dance_active) return false;
            if (sacrifice_active) return false;
            if (steal_active) return false;
            if (skill_flash_active) return false;
            if (swap_active) return false;
            if (teleport_active) return false;
            if (transform_active) return false;
            if (summon_active) return false;
            if (mass_slow_active) return false;
            return true;
        }

        public void skip_battle_scene()
        {
            Dance_State.skip_battle_scene();
        }

        internal bool dance_active { get { return Dance_State.dance_calling || Dance_State.in_dance; } }
        internal bool sacrifice_active { get { return Sacrifice_State.sacrifice_calling || Sacrifice_State.in_sacrifice; } }
        internal bool steal_active { get { return Steal_State.steal_calling || Steal_State.in_steal; } }
        internal bool skill_flash_active { get { return Skill_Flash_State.skill_flash_calling || Skill_Flash_State.in_skill_flash; } }
        internal bool swap_active { get { return Swap_State.swap_calling || Swap_State.in_swap; } }
        internal bool teleport_active { get { return Teleport_State.teleport_calling || Teleport_State.in_teleport; } }
        internal bool transform_active { get { return Transform_State.transform_calling || Transform_State.in_transform; } }
        internal bool summon_active { get { return Summon_State.summon_calling || Summon_State.in_summon; } }
        internal bool mass_slow_active { get { return Mass_Slow_State.mass_slow_calling || Mass_Slow_State.in_mass_slow; } }


    }
}
