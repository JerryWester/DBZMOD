﻿using DBZMOD.Buffs;
using DBZMOD.Effects.Animations.Aura;
using DBZMOD.Utilities;
using Microsoft.Xna.Framework;

namespace DBZMOD.Transformations.SSJ.SSJ1.USSJ
{
    public sealed class USSJTransformation : TransformationDefinition
    {
        public USSJTransformation(params TransformationDefinition[] parents) : base(BuffKeyNames.ussj, FormBuffHelper.GetUSSJNamePreference(), TransformationDefinitionManager.defaultTransformationTextColor,
            1.9f, 0.9f, 6, 1.6f, 1.5f, 0.75f, 0f,
            new TransformationAppearanceDefinition(AuraAnimations.ussjAura, new ReadOnlyColor(SSJ1Transformation.LIGHTING_RED, SSJ1Transformation.LIGHTING_GREEN, SSJ1Transformation.LIGHTING_BLUE), new HairAppearance("Hairs/ASSJ/ASSJHair", new ReadOnlyColor(0f, 0f, 0f), 0), HairStyleAppearance.USSJHairStyle, Color.Turquoise),
            typeof(USSJBuff),
            canBeMastered: true, parents: parents)
        {
        }
    }
}
