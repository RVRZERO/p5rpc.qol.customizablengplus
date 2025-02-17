using p5rpc.qol.customizablengplus.Template.Configuration;
using Reloaded.Mod.Interfaces.Structs;
using System.ComponentModel;

namespace p5rpc.qol.customizablengplus.Configuration
{
    public class Config : Configurable<Config>
    {
        [Category("Life Sim")]
        [DisplayName("Money")]
        [Description("This is a bool.")]
        [DefaultValue(false)]
        public bool Money { get; set; } = true;

        [Category("Life Sim")]
        [DisplayName("Social Stats")]
        [Description("This is a bool.")]
        [DefaultValue(false)]
        public bool SocialStats { get; set; } = true;

        [Category("Metaverse")]
        [DisplayName("Persona Compendium")]
        [Description("This is a bool.")]
        [DefaultValue(false)]
        public bool Compendium { get; set; } = true;

        [Category("Metaverse")]
        [DisplayName("Mementos Stamps")]
        [Description("This is a bool.")]
        [DefaultValue(false)]
        public bool Stamps { get; set; } = true;

        [Category("Metaverse")]
        [DisplayName("HP and SP Increases")]
        [Description("This is a bool.")]
        [DefaultValue(false)]
        public bool HPandSP { get; set; } = true;

        [Category("Metaverse")]
        [DisplayName("Jazz Persona Stats Increases")]
        [Description("This is a bool.")]
        [DefaultValue(false)]
        public bool Jazzstats { get; set; } = true;

        [Category("Metaverse")]
        [DisplayName("Jazz Club Skills")]
        [Description("This is a bool.")]
        [DefaultValue(false)]
        public bool Jazzskills { get; set; } = true;

        [Category("Metaverse")]
        [DisplayName("Enemy Analysis")]
        [Description("This is a bool.")]
        [DefaultValue(false)]
        public bool Enemyanalysis { get; set; } = true;

        [Category("Inventory")]
        [DisplayName("Melee Weapons")]
        [Description("This is a bool.")]
        [DefaultValue(false)]
        public bool Melee { get; set; } = true;

        [Category("Inventory")]
        [DisplayName("Ranged Weapons")]
        [Description("This is a bool.")]
        [DefaultValue(false)]
        public bool Ranged { get; set; } = true;

        [Category("Inventory")]
        [DisplayName("Protectors")]
        [Description("This is a bool.")]
        [DefaultValue(false)]
        public bool Protectors { get; set; } = true;

        [Category("Inventory")]
        [DisplayName("Accessories")]
        [Description("This is a bool.")]
        [DefaultValue(false)]
        public bool Accessories { get; set; } = true;

        [Category("Inventory")]
        [DisplayName("Skill Cards")]
        [Description("This is a bool.")]
        [DefaultValue(false)]
        public bool SkillCards { get; set; } = true;

        [Category("Inventory")]
        [DisplayName("Key Items")]
        [Description("This is a bool.")]
        [DefaultValue(false)]
        public bool Keyitems { get; set; } = true;

        [Category("Inventory")]
        [DisplayName("Miscellaneous")]
        [Description("This is a bool.")]
        [DefaultValue(false)]
        public bool Miscellaneous { get; set; } = true;

        [Category("Fixes")]
        [DisplayName("Challenge Battle Score/Rewards Reset")]
        [Description("This is a bool.")]
        [DefaultValue(false)]
        public bool Challenge { get; set; } = true;
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
