using p5rpc.qol.customizablengplus.Configuration;
using p5rpc.qol.customizablengplus.Template;
using Reloaded.Mod.Interfaces;
using Reloaded.Memory;
using Reloaded.Memory.Pointers;
using Reloaded.Memory.Interfaces;
using Reloaded.Memory.Sigscan.Definitions;
using Reloaded.Memory.SigScan.ReloadedII.Interfaces;
using System.Diagnostics;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Enums;
using System.Net;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using Reloaded.Hooks.Definitions.X64;
using System.Xml.Linq;
using p5rpc.lib.interfaces;

namespace p5rpc.qol.customizablengplus
{
    /// <summary>
    /// Your mod logic goes here.
    /// </summary>
    public class Mod : ModBase // <= Do not Remove.
    {
        /// <summary>
        /// Provides access to the mod loader API.
        /// </summary>
        private readonly IModLoader _modLoader;

        /// <summary>
        /// Provides access to the Reloaded.Hooks API.
        /// </summary>
        /// <remarks>This is null if you remove dependency on Reloaded.SharedLib.Hooks in your mod.</remarks>
        private readonly IReloadedHooks? _hooks;

        /// <summary>
        /// Provides access to the Reloaded logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Entry point into the mod, instance that created this class.
        /// </summary>
        private readonly IMod _owner;

        /// <summary>
        /// Provides access to this mod's configuration.
        /// </summary>
        private Config _configuration;

        /// <summary>
        /// The configuration of the currently executing mod.
        /// </summary>
        private readonly IModConfig _modConfig;

        internal static nint BaseAddress { get; private set; }

        private IHook<ClearInheritanceData>? _clearInheritanceDataHook;

        private nint ChallengeBattleData = 0;

        [Function(CallingConventions.Microsoft)]
        private unsafe delegate void ClearInheritanceData();

        public Mod(ModContext context)
        {
            _modLoader = context.ModLoader;
            _hooks = context.Hooks;
            _logger = context.Logger;
            _owner = context.Owner;
            _configuration = context.Configuration;
            _modConfig = context.ModConfig;

            using var thisProcess = Process.GetCurrentProcess();
            BaseAddress = thisProcess.MainModule!.BaseAddress;

            var startupScannerController = _modLoader.GetController<IStartupScanner>();
            if (startupScannerController == null || !startupScannerController.TryGetTarget(out var startupScanner))
            {
                throw new Exception("Failed to get IStartupScanner Controller");
            }

            var reloadedHooksController = _modLoader.GetController<IReloadedHooks>();
            if (reloadedHooksController == null || !reloadedHooksController.TryGetTarget(out var reloadedHooks))
            {
                throw new Exception("Failed to get IStartupScanner Controller");
            }

            var p5rLibController = _modLoader.GetController<IP5RLib>();
            if (p5rLibController == null || !p5rLibController.TryGetTarget(out var p5rLib))
            {
                throw new Exception("Failed to get IP5RLib Controller");
            }

            void SigScan(string pattern, string name, Action<nint> action)
            {
                startupScanner.AddMainModuleScan(pattern, result =>
                {
                    if (result.Found)
                    {
                        action(result.Offset + BaseAddress);
                        _logger.WriteLine($"Customizable NG+: {name} Signature Found: {result.Offset + BaseAddress:X}");
                    }
                    else
                    {
                        _logger.WriteLine($"Customizable NG+: {name}: Signature Not Found ");
                    }
                });
            }

            void ScanForData(string name, string pattern, int instructionLength, int instructionOffset, int extraOffset, Action<nint> action)
            {
                startupScanner.AddMainModuleScan(pattern, result =>
                {
                    unsafe
                    {
                        if (result.Found)
                        {
                            var offsetAddress = result.Offset + BaseAddress + instructionOffset;
                            var offsetAddressPointer = (int*)offsetAddress;
                            var offset = *offsetAddressPointer;

                            action(result.Offset + BaseAddress + instructionLength + offset + extraOffset);
                            _logger.WriteLine($"Customizable NG+: {name} Signature Found: {result.Offset + BaseAddress:X}");
                        }
                        else
                        {
                            _logger.WriteLine($"Customizable NG+: {name}: Signature Not Found ");
                        }
                    }
                });
            }

            void GetFunctionHook<T>(string name, string pattern, T function, Action<IHook<T>> action)
            {
                startupScanner.AddMainModuleScan(pattern, result =>
                {
                    if (result.Found)
                    {
                        action(reloadedHooks.CreateHook(function, BaseAddress + result.Offset).Activate());
                        _logger.WriteLine($"Customizable NG+: {name} Signature Found: {result.Offset + BaseAddress:X}");
                    }
                    else
                    {
                        _logger.WriteLine($"Customizable NG+: {name}: Signature Not Found ");
                    }
                });
            }

            var memory = Memory.Instance;

            if (_configuration.Enemyanalysis)
            {
                SigScan(                                                   "BA 0C 0D 00 00 48 8D 0D ?? ?? ?? ?? E8", "ENEMY_ANALYSIS_SEGMENT_1", address =>
                {
                    memory.SafeWrite((nuint)address, Convert.FromHexString("90 90 90 90 90 90 90 90 90 90 90 90".Replace(" ", "")));
                });

                SigScan(                                                   "E8 ?? ?? ?? ?? BA 00 57 00 00", "ENEMY_ANALYSIS_SEGMENT_2", address =>
                {
                    memory.SafeWrite((nuint)address, Convert.FromHexString("90 90 90 90 90".Replace(" ", "")));
                });
            }

            if (_configuration.Compendium)
            {
                SigScan(                                                   "BA 00 57 00 00 48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 4C 8D 2D", "COMPENDIUM_SEGMENT_1", address =>
                {
                    memory.SafeWrite((nuint)address, Convert.FromHexString("90 90 90 90 90 90 90 90 90 90 90 90".Replace(" ", "")));
                });

                SigScan(                                                   "E8 ?? ?? ?? ?? 4C 8D 2D ?? ?? ?? ?? 41 8B C4 49 8B CD 81 39 42 00 00 40", "COMPENDIUM_SEGMENT_2", address =>
                {
                    memory.SafeWrite((nuint)address, Convert.FromHexString("90 90 90 90 90".Replace(" ", "")));
                });
            }

            if (_configuration.SocialStats)
            {
                SigScan(                                                   "66 45 89 A4 ?? ?? ?? ?? ?? 83 F9 05", "SOCIAL_STATS", address =>
                {
                    memory.SafeWrite((nuint)address, Convert.FromHexString("90 90 90 90 90 90 90 90 90".Replace(" ", "")));
                });
            }

            if (_configuration.Ranged)
            {
                SigScan(                                                   "46 88 A4 ?? ?? ?? ?? ?? 81 F9 00 01 00 00", "RANGED_WEAPONS_CUSTOMIZATION", address =>
                {
                    memory.SafeWrite((nuint)address, Convert.FromHexString("90 90 90 90 90 90 90 90".Replace(" ", "")));
                });
            }

            if (_configuration.Money)
            {
                SigScan(                                                   "C7 05 ?? ?? ?? ?? D0 07 00 00 BA 01 00 00 00", "MONEY", address =>
                {
                    memory.SafeWrite((nuint)address, Convert.FromHexString("90 90 90 90 90 90 90 90 90 90".Replace(" ", "")));
                });
            }

            if (_configuration.HPandSP)
            {
                SigScan(                                                   "48 69 C8 A0 02 00 00 46 89 A4", "HP&SP_INCREASES_SEGMENT_1", address =>
                {
                    memory.SafeWrite((nuint)address, Convert.FromHexString("90 90 90 90 90 90 90".Replace(" ", "")));
                });

                SigScan(                                                   "46 89 A4 ?? ?? ?? ?? ?? 83 FA 0B", "HP&SP_INCREASES_SEGMENT_2", address =>
                {
                    memory.SafeWrite((nuint)address, Convert.FromHexString("90 90 90 90 90 90 90 90".Replace(" ", "")));
                });
            }

            if (_configuration.Melee)
            {
                SigScan(                                                   "E8 ?? ?? ?? ?? 8B C3 41 B0 ?? 48 C1 E8 0C 66 23 DE 33 D2 0F B7 CB FF 54 ?? ?? FF C7 48 8D 05", "MELEE_WEAPONS", address =>
                {
                    memory.SafeWrite((nuint)address, Convert.FromHexString("90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90".Replace(" ", "")));
                });
            }

            if (_configuration.Protectors)
            {
                SigScan(                                                   "E8 ?? ?? ?? ?? 0F B7 C3 41 B0 ?? 48 C1 E8 0C 66 23 DE 33 D2 0F B7 CB FF 54 ?? ?? FF C7 B8 00 10 00 00", "PROTECTORS", address =>
                {
                    memory.SafeWrite((nuint)address, Convert.FromHexString("90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90".Replace(" ", "")));
                });
            }

            if (_configuration.Accessories)
            {
                SigScan(                                                   "E8 ?? ?? ?? ?? 0F B7 C3 41 B0 ?? 48 C1 E8 0C 66 23 DE 33 D2 0F B7 CB FF 54 ?? ?? FF C7 B8 00 20 00 00", "ACCESSORIES", address =>
                {
                    memory.SafeWrite((nuint)address, Convert.FromHexString("90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90".Replace(" ", "")));
                });
            }

            if (_configuration.SkillCards)
            {
                SigScan(                                                   "E8 ?? ?? ?? ?? 0F B7 C3 41 B0 ?? 48 C1 E8 0C 66 23 DE 33 D2 0F B7 CB FF 54 ?? ?? FF C7 B8 00 60 00 00", "SKILL_CARDS", address =>
                {
                    memory.SafeWrite((nuint)address, Convert.FromHexString("90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90".Replace(" ", "")));
                });
            }

            if (_configuration.Ranged)
            {
                SigScan(                                                   "E8 ?? ?? ?? ?? 0F B7 C3 41 B0 ?? 48 C1 E8 0C 66 23 DE 33 D2 0F B7 CB FF 54 ?? ?? FF C7 B8 00 80 00 00", "RANGED_WEAPONS", address =>
                {
                    memory.SafeWrite((nuint)address, Convert.FromHexString("90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90".Replace(" ", "")));
                });
            }

            if (_configuration.Miscellaneous)
            {
                SigScan(                                                   "E8 ?? ?? ?? ?? 8B C3 41 B0 ?? 48 C1 E8 0C 66 23 DE 33 D2 0F B7 CB FF 54 ?? ?? FF C7 48 8D 0D", "MISCELLANEOUS", address =>
                {
                    memory.SafeWrite((nuint)address, Convert.FromHexString("90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90".Replace(" ", "")));
                });
            }

            if (_configuration.Keyitems)
            {
                SigScan(                                                   "E8 ?? ?? ?? ?? 8B C3 41 B0 ?? 48 C1 E8 0C 66 23 DE 33 D2 0F B7 CB FF 54 ?? ?? FF C7 83 FF 29", "KEY_ITEMS", address =>
                {
                    memory.SafeWrite((nuint)address, Convert.FromHexString("90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90".Replace(" ", "")));
                });

                SigScan(                                                   "41 21 00 4C 8D 05", "CONFIDANT_ULTIMATE_PERSONA_UNLOCKS", address =>
                {
                    memory.SafeWrite((nuint)address, Convert.FromHexString("90 90 90".Replace(" ", "")));
                });
            }

            if (_configuration.Jazzstats)
            {
                SigScan(                                                   "44 88 64 ?? ?? 83 F8 05", "JAZZ_PERSONA_STATS", address =>
                {
                    memory.SafeWrite((nuint)address, Convert.FromHexString("90 90 90 90 90".Replace(" ", "")));
                });
            }

            if (_configuration.SkillCards)
            {
                SigScan(                                                   "BA 80 00 00 00 48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 41 8B CC", "YUSUKE_SKILL_CARDS_DUPLICATION_DATA", address =>
                {
                    memory.SafeWrite((nuint)address, Convert.FromHexString("90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90".Replace(" ", "")));
                });
            }

            if (_configuration.Stamps)
            {
                SigScan(                                                   "44 89 A4 ?? ?? ?? ?? ?? 44 88 A4", "MEMENTOS_PLATFORM_STAMPS", address =>
                {
                    memory.SafeWrite((nuint)address, Convert.FromHexString("90 90 90 90 90 90 90 90".Replace(" ", "")));
                });

                SigScan(                                                   "44 88 A4 ?? ?? ?? ?? ?? 83 F9 0A", "MEMENTOS_RANDOM_STAMPS", address =>
                {
                    memory.SafeWrite((nuint)address, Convert.FromHexString("90 90 90 90 90 90 90 90".Replace(" ", "")));
                });
            }

            if (_configuration.Jazzskills)
            {
                SigScan(                                                   "BA 30 00 00 00 48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 4C 8D 9C 24", "JAZZ_SKILLS_SEGMENT_1", address =>
                {
                    memory.SafeWrite((nuint)address, Convert.FromHexString("90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90".Replace(" ", "")));
                });

                SigScan(                                                   "4C 89 25 ?? ?? ?? ?? 49 8B 5B ?? 49 8B 73 ?? 49 8B 7B ?? 4C 89 25", "JAZZ_SKILLS_SEGMENT_2", address =>
                {
                    memory.SafeWrite((nuint)address, Convert.FromHexString("90 90 90 90 90 90 90".Replace(" ", "")));
                });

                SigScan(                                                   "4C 89 25 ?? ?? ?? ?? 4C 89 25 ?? ?? ?? ?? 4C 89 25 ?? ?? ?? ?? 4C 89 25 ?? ?? ?? ?? 44 89 25 ?? ?? ?? ?? 49 8B E3 41 5F 41 5E 41 5D 41 5C 5D C3 85 C0", "JAZZ_SKILLS_SEGMENT_3", address =>
                {
                    memory.SafeWrite((nuint)address, Convert.FromHexString("90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90".Replace(" ", "")));
                });
            }

            void ClearInheritanceData_Custom()
            {
                _clearInheritanceDataHook!.OriginalFunction();

                // Resets Challenge Battle Scores & Rewards
                for (int i = 0; i < 13; i++)
                {
                    memory.SafeWrite((nuint)(ChallengeBattleData + i * 8), new byte[8]);
                }

                // Challenge Battles Unlock Flags
                p5rLib.FlowCaller.BIT_OFF(11592);
                p5rLib.FlowCaller.BIT_OFF(11593);
                p5rLib.FlowCaller.BIT_OFF(11594);
                p5rLib.FlowCaller.BIT_OFF(11595);
                p5rLib.FlowCaller.BIT_OFF(11596);
                p5rLib.FlowCaller.BIT_OFF(11597);
                // Challenge Battles Unlock Notification Flags
                p5rLib.FlowCaller.BIT_OFF(12773);
                p5rLib.FlowCaller.BIT_OFF(12774);
                p5rLib.FlowCaller.BIT_OFF(12775);
                p5rLib.FlowCaller.BIT_OFF(12776);
                p5rLib.FlowCaller.BIT_OFF(12777);

                return;
            }

            if (_configuration.Challenge)
            {
                ScanForData("Challenge Battle Data Reference", "48 8D 0D ?? ?? ?? ?? F3 44 0F 10 1D ?? ?? ?? ?? 49 83 C7 0C", 7, 3, 2, address =>
                {
                    ChallengeBattleData = address;
                });

                GetFunctionHook<ClearInheritanceData>("ClearInheritanceData", "48 89 5C 24 ?? 48 89 74 24 ?? 48 89 7C 24 ?? 55 41 54 41 55 41 56 41 57 48 8D 6C 24 ?? 48 81 EC 30 01 00 00 45 33 E4",
                        ClearInheritanceData_Custom, hook => _clearInheritanceDataHook = hook);
            }
            // For more information about this template, please see
            // https://reloaded-project.github.io/Reloaded-II/ModTemplate/

            // If you want to implement e.g. unload support in your mod,
            // and some other neat features, override the methods in ModBase.

            // TODO: Implement some mod logic
        }

        #region Standard Overrides
        public override void ConfigurationUpdated(Config configuration)
        {
            // Apply settings from configuration.
            // ... your code here.
            _configuration = configuration;
            _logger.WriteLine($"[{_modConfig.ModId}] Config Updated: Applying");
        }
        #endregion

        #region For Exports, Serialization etc.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Mod() { }
#pragma warning restore CS8618
        #endregion
    }
}