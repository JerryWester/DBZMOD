﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria.ModLoader;

namespace DBZMOD.Util
{
    /// <summary>
    ///     WIP stuff, this was Xeno considering switching from weak refs to reflection on player vars. Not sure if it's gonna work yet, haven't put in enough time.
    /// </summary>
    public static class CompatibilityUtil
    {
        private const string ENIGMA_MOD_NAME = "Laugicality";
        private const string ENIGMA_PLAYER_CLASS_NAME = "LaugicalityPlayer";
        private static bool _isEnigmaInitialized;
        private static Mod _enigma;
        public static Mod Enigma
        {
            get
            {
                if (_enigma == null && _isEnigmaInitialized)
                {
                    _enigma = ModLoader.GetMod(ENIGMA_MOD_NAME);
                    _isEnigmaInitialized = true;
                }
                return _enigma;
            }
        }
    }
}
