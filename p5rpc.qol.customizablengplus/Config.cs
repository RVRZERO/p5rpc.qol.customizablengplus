using p5rpc.qol.customizablengplus.Template.Configuration;
using Reloaded.Mod.Interfaces.Structs;
using System.ComponentModel;

namespace p5rpc.qol.customizablengplus.Configuration
{
    public class Config : Configurable<Config>
    {
        [Category("Life Sim")]
        [DisplayName("Money")]
        [Description("Enable this to carry over Money to NG+.")]
        [DefaultValue(false)]
        public bool Money { get; set; } = false;

        [Category("Life Sim")]
        [DisplayName("Social Stats")]
        [Description("Enable this to carry over Social Stats to NG+.")]
        [DefaultValue(false)]
        public bool SocialStats { get; set; } = false;

        [Category("Metaverse")]
        [DisplayName("Persona Compendium")]
        [Description("Enable this to carry over the Persona Compendium to NG+.")]
        [DefaultValue(false)]
        public bool Compendium { get; set; } = false;

        [Category("Metaverse")]
        [DisplayName("Mementos Stamps")]
        [Description("Enable this to carry over Mementos Stamps to NG+.")]
        [DefaultValue(false)]
        public bool Stamps { get; set; } = false;

        [Category("Metaverse")]
        [DisplayName("HP & SP Increases")]
        [Description("Enable this to carry over HP & SP Increases to NG+.")]
        [DefaultValue(false)]
        public bool HPandSP { get; set; } = false;

        [Category("Metaverse")]
        [DisplayName("Jazz Club Stats Increases")]
        [Description("Enable this to carry over Jazz Club Stats Increases to NG+.")]
        [DefaultValue(false)]
        public bool JazzStats { get; set; } = false;

        [Category("Metaverse")]
        [DisplayName("Jazz Club Skills")]
        [Description("Enable this to carry over Jazz Club Skills to NG+.")]
        [DefaultValue(false)]
        public bool JazzSkills { get; set; } = false;

        [Category("Metaverse")]
        [DisplayName("Enemy Analysis Data")]
        [Description("Enable this to carry over Enemy Analysis Data to NG+.")]
        [DefaultValue(false)]
        public bool EnemyAnalysisData { get; set; } = false;

        [Category("Inventory")]
        [DisplayName("Melee Weapons")]
        [Description("Enable this to carry over Melee Weapons to NG+.")]
        [DefaultValue(false)]
        public bool Melee { get; set; } = false;

        [Category("Inventory")]
        [DisplayName("Ranged Weapons")]
        [Description("Enable this to carry over Ranged Weapons to NG+.")]
        [DefaultValue(false)]
        public bool Ranged { get; set; } = false;

        [Category("Inventory")]
        [DisplayName("Protectors")]
        [Description("Enable this to carry over Protectors to NG+.")]
        [DefaultValue(false)]
        public bool Protectors { get; set; } = false;

        [Category("Inventory")]
        [DisplayName("Accessories")]
        [Description("Enable this to carry over Accessories to NG+.")]
        [DefaultValue(false)]
        public bool Accessories { get; set; } = false;

        [Category("Inventory")]
        [DisplayName("Skill Cards")]
        [Description("Enable this to carry over Skill Cards to NG+.")]
        [DefaultValue(false)]
        public bool SkillCards { get; set; } = false;

        [Category("Inventory")]
        [DisplayName("Key Items")]
        [Description("Enable this to carry over Key Items to NG+.")]
        [DefaultValue(false)]
        public bool KeyItems { get; set; } = false;

        [Category("Inventory")]
        [DisplayName("Miscellaneous")]
        [Description("Enable this to carry over the Perma-Pick, SP Recovery Gifts from Confidants, Fishing Pools, Dart Set, and Jump Cue to NG+.")]
        [DefaultValue(false)]
        public bool Miscellaneous { get; set; } = false;

        [Category("Additions")]
        [DisplayName("Challenge Battle\nScores Reset")]
        [Description("Enable this to reset Challenge Battle Scores and allow you to win the rewards again in NG+.")]
        [DefaultValue(false)]
        public bool ResetChallengeBattles { get; set; } = false;
    }

    /// <summary>
    /// Allows you to override certain aspects of the configuration creation process (e.g. create multiple configurations).
    /// Override elements in <see cref="ConfiguratorMixinBase"/> for finer control.
    /// </summary>
    public class ConfiguratorMixin : ConfiguratorMixinBase
    {
        // 
    }
}
