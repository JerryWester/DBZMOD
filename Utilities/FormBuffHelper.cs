﻿using System.Collections.Generic;
using System.Linq;
using DBZMOD.Buffs;
using DBZMOD.Config;
using DBZMOD.Enums;
using DBZMOD.Transformations;
using Microsoft.Xna.Framework;

namespace DBZMOD.Utilities
{
    // class for helping out with all the buff integers, lists of buffs in order, presence of buffs/abstraction
    public static class FormBuffHelper
    {
        public const int ABSURDLY_LONG_BUFF_DURATION = 666666;

        /*internal static TransformationDefinition[]
            transformationDefinitionList =
            {
                TransformationDefinitionManager.KaiokenDefinition,
                TransformationDefinitionManager.SuperKaiokenDefinition,
                TransformationDefinitionManager.KaiokenFatigueDefinition,

                TransformationDefinitionManager.SSJ1Definition,
                TransformationDefinitionManager.ASSJDefinition,
                TransformationDefinitionManager.USSJDefinition,

                TransformationDefinitionManager.SSJ2Definition,
                TransformationDefinitionManager.SSJ3Definition,
                TransformationDefinitionManager.SSJGDefinition,

                TransformationDefinitionManager.LSSJDefinition,

                TransformationDefinitionManager.TransformationExhaustionDefinition,
            },

            // returns a list of transformation steps specific to non-legendary SSJ players
            ssjBuffs =
            {
                TransformationDefinitionManager.SSJ1Definition,
                TransformationDefinitionManager.SSJ2Definition,
                TransformationDefinitionManager.SSJ3Definition,
                TransformationDefinitionManager.SSJGDefinition
            },

            // a list of transformation steps from SSJ1 through ascended SSJ forms
            ascensionBuffs =
            {
                TransformationDefinitionManager.SSJ1Definition,
                TransformationDefinitionManager.ASSJDefinition,
                TransformationDefinitionManager.USSJDefinition
            },

            // a list of transformation steps specific to legendary SSJ players
            legendaryBuffs =
            {
                TransformationDefinitionManager.SSJ1Definition,
                TransformationDefinitionManager.LSSJDefinition
            };*/

        public static void Initialize()
        {

        }

        public static string GetASSJNamePreference()
        {
            return ConfigModel.isSaiyanGradeNames ? "Super Saiyan Grade 2" : "Ascended Super Saiyan";
        }

        public static string GetUSSJNamePreference()
        {
            return ConfigModel.isSaiyanGradeNames ? "Super Saiyan Grade 3" : "Ultra Super Saiyan";
        }

        public static TransformationDefinitionManager TransformationDefinitionManager => DBZMOD.Instance.TransformationDefinitionManager;
    }
}
