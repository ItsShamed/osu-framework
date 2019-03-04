﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Extensions.PolygonExtensions;
using osu.Framework.Graphics.OpenGL;
using osu.Framework.Graphics.OpenGL.Vertices;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Shaders;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osuTK.Graphics.ES30;

namespace osu.Framework.Graphics.Shapes
{
    /// <summary>
    /// A simple rectangular box. Can be colored using the <see cref="Drawable.Colour"/> property.
    /// </summary>
    public class Box : Sprite
    {
        public Box()
        {
            Texture = Texture.WhitePixel;
        }

        protected override DrawNode CreateDrawNode() => new BoxDrawNode();

        protected class BoxDrawNode : SpriteDrawNode
        {
            public override void DrawHull(Action<TexturedVertex2D> vertexAction, ref float vertexDepth)
            {
                base.DrawHull(vertexAction, ref vertexDepth);

                if (Texture?.Available != true)
                    return;

                if (DrawColourInfo.Colour.MinAlpha != 1 || DrawColourInfo.Blending.RGBEquation != BlendEquationMode.FuncAdd)
                    return;

                TextureShader.Bind();
                Texture.TextureGL.WrapMode = WrapTexture ? TextureWrapMode.Repeat : TextureWrapMode.ClampToEdge;

                var conservativeScreenSpaceQuad = Quad.FromRectangle(DrawRectangle) * DrawInfo.Matrix;

                if (GLWrapper.IsMaskingActive)
                {
                    var polygon = conservativeScreenSpaceQuad.ClipTo(GLWrapper.CurrentMaskingInfo.ConservativeScreenSpaceQuad);

                    foreach (var tri in polygon.Triangulate())
                        Texture.DrawTriangle(tri, Depth, DrawColourInfo.Colour);
                }
                else
                    Blit(conservativeScreenSpaceQuad, vertexAction);

                vertexDepth -= 0.0001f;
            }

            public override void Draw(Action<TexturedVertex2D> vertexAction)
            {
                //if (DrawColourInfo.Colour.MinAlpha == 1 && DrawColourInfo.Blending.RGBEquation == BlendEquationMode.FuncAdd
                //                                        && (!GLWrapper.IsMaskingActive || GLWrapper.CurrentMaskingInfo.CornerRadius == 0))
                //{
                //    return;
                //}

                base.Draw(vertexAction);
            }
        }
    }
}
