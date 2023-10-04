using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using LDtk;
using LDtk.Renderer;
using LDtkTypes;
using TheLostHope.Engine.ContentManagement;
using TheLostHope.GameCode;
using TheLostHope.GameCode.GameStates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace TheLostHope.GameCode.LDtkExtensions
{
    public class BackgroundData
    {
        public Vector2 Position;
        public Texture2D Texture;
        public float ParallaxMult;

        public BackgroundData(Vector2 pos, Texture2D texture, float parallaxMult)
        {
            Position = pos;
            Texture = texture;
            ParallaxMult = parallaxMult;
        }
    }

    //
    // Summary:
    //     Renderer for the ldtkWorld, ldtkLevel, intgrids and entities. This can all be
    //     done in your own class if you want to reimplement it and customize it differently
    //     this one is mostly here to get you up and running quickly.
    public class ModifiedLDtkRenderer
    {
        private static RenderTarget2D RenderTarget;
        private static List<BackgroundData> ParallaxBackgrounds;
        private static List<BackgroundData> ParallaxForegrounds;

        private GraphicsDevice graphicsDevice;

        //
        // Summary:
        //     The spritebatch used for rendering with this Renderer
        public SpriteBatch SpriteBatch { get; set; }

        //
        // Summary:
        //     The levels identifier to layers Dictionary
        // protected Dictionary<string, RenderedLevel> PrerenderedLevels { get; set; } = new Dictionary<string, RenderedLevel>();


        //
        // Summary:
        //     This is used to intizialize the renderer for use with direct file loading
        public ModifiedLDtkRenderer(SpriteBatch spriteBatch)
        {
            SpriteBatch = spriteBatch;
            graphicsDevice = spriteBatch.GraphicsDevice;
            RenderTarget?.Dispose();
        }

        //
        // Summary:
        //     Prerender out the level to textures to optimize the rendering process
        //
        // Parameters:
        //   level:
        //     The level to prerender
        //
        // Exceptions:
        //   T:System.Exception:
        //     The level already has been prerendered
        public void PrerenderLevel(LDtkLevel level)
        {
            RenderTarget = new RenderTarget2D(graphicsDevice, level.PxWid, level.PxHei, mipMap: false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            graphicsDevice.SetRenderTarget(RenderTarget);
            SpriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
            RenderLayers(level);
            SpriteBatch.End();
            graphicsDevice.SetRenderTarget(null);
        }

        private void RenderLayers(LDtkLevel level)
        {
            for (int num = level.LayerInstances.Length - 1; num >= 0; num--)
            {
                LayerInstance layer = level.LayerInstances[num];
                if (layer._TilesetRelPath != null && layer._Type != 0)
                {
                    Texture2D texture = GetTexture(level, layer._TilesetRelPath);
                    int width = layer._CWid * layer._GridSize;
                    int height = layer._CHei * layer._GridSize;

                    graphicsDevice.Clear(ClearOptions.Target, Color.Transparent, 0f, 0);

                    switch (layer._Type)
                    {
                        case LayerType.Tiles:
                            foreach (TileInstance item in layer.GridTiles.Where((tile) => layer._TilesetDefUid.HasValue))
                            {
                                Vector2 position2 = new Vector2(item.Px.X + layer._PxTotalOffsetX, item.Px.Y + layer._PxTotalOffsetY);
                                Rectangle value2 = new Rectangle(item.Src.X, item.Src.Y, layer._GridSize, layer._GridSize);
                                SpriteEffects f2 = (SpriteEffects)item.F;
                                SpriteBatch.Draw(texture, position2, value2, Color.White, 0f, Vector2.Zero, 1f, f2, 0f);
                            }

                            break;
                        case LayerType.IntGrid:
                        case LayerType.AutoLayer:
                            if (layer.AutoLayerTiles.Length == 0)
                            {
                                break;
                            }

                            foreach (TileInstance item2 in layer.AutoLayerTiles.Where((tile) => layer._TilesetDefUid.HasValue))
                            {
                                Vector2 position = new Vector2(item2.Px.X + layer._PxTotalOffsetX, item2.Px.Y + layer._PxTotalOffsetY);
                                Rectangle value = new Rectangle(item2.Src.X, item2.Src.Y, layer._GridSize, layer._GridSize);
                                SpriteEffects f = (SpriteEffects)item2.F;
                                SpriteBatch.Draw(texture, position, value, Color.White, 0f, Vector2.Zero, 1f, f, 0f);
                            }

                            break;
                    }
                }
            }


            GetAndPrerenderBackgrounds(level);
        }

        private void GetAndPrerenderBackgrounds(LDtkLevel level)
        {
            //if (level.BgRelPath != null)
            //{
            //    Texture2D texture = GetTexture(level, level.BgRelPath);
            //    LevelBackgroundPosition bgPos = level._BgPos;
            //    Vector2 position = bgPos.TopLeftPx.ToVector2();
            //    SpriteBatch.Draw(texture, position, new Rectangle((int)bgPos.CropRect[0], (int)bgPos.CropRect[1], (int)bgPos.CropRect[2], (int)bgPos.CropRect[3]), Color.White, 0f, Vector2.Zero, bgPos.Scale, SpriteEffects.None, 0f);
            //}

            var backgrounds = level.GetCustomFields<LDtkLevelData>();
            if (backgrounds != null)
            {
                ParallaxBackgrounds = new List<BackgroundData>();
                if (backgrounds.Backgrounds != null)
                {
                    foreach (var b in backgrounds.Backgrounds)
                    {
                        string[] parts = b.Split('|');
                        string path = parts[0];
                        float paralalx = float.Parse(parts[1]);

                        ContentLoader.LoadTexture(path, path);

                        BackgroundData d = new(Vector2.Zero, ContentLoader.GetTexture(path), paralalx);
                        ParallaxBackgrounds.Add(d);
                    }
                }

                ParallaxForegrounds = new List<BackgroundData>();
                if (backgrounds.Foregrounds != null)
                {
                    foreach (var f in backgrounds.Foregrounds)
                    {
                        string[] parts = f.Split('|');
                        string path = parts[0];
                        float paralalx = float.Parse(parts[1]);

                        ContentLoader.LoadTexture(path, path);

                        BackgroundData d = new(Vector2.Zero, ContentLoader.GetTexture(path), paralalx);
                        d.ParallaxMult = paralalx;

                        ParallaxForegrounds.Add(d);
                    }
                }
            }
        }

        private Texture2D GetTexture(LDtkLevel level, string path)
        {
            string assetName = Path.ChangeExtension(path, null);
            ContentLoader.LoadTexture(assetName, assetName);
            return ContentLoader.GetTexture(assetName);
        }

        //
        // Summary:
        //     Render the prerendered level you created from PrerenderLevel()
        public void RenderPrerenderedLevel(LDtkLevel level)
        {
            SpriteBatch.Draw(RenderTarget, Vector2.Zero, Color.White);
        }
        public void RenderBackgrounds()
        {
            foreach (var bg in ParallaxBackgrounds)
            {
                Vector2 size = GameplayManager.Instance.GameCamera.Size;
                float scale = (float)Math.Ceiling(MathHelper.Max(size.Y / bg.Texture.Height,
                    size.X / bg.Texture.Width));

                SpriteBatch.Draw(bg.Texture, bg.Position, null, Color.White,
                    0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
        }
        public void RenderForegrounds()
        {
            foreach (var fr in ParallaxForegrounds)
            {
                float scale = (float)Math.Ceiling(MathHelper.Max(GameplayManager.Instance.GameCamera.Size.Y / fr.Texture.Height,
                    GameplayManager.Instance.GameCamera.Size.X / fr.Texture.Width));

                SpriteBatch.Draw(fr.Texture, fr.Position, null, Color.White,
                    0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
        }

        public void UpdateParallaxBackgrounds(GameTime gameTime, Vector2 currentCameraPos, Vector2 previousCameraPos)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            foreach (var bg in ParallaxBackgrounds)
            {
                bg.Position += (currentCameraPos - previousCameraPos) * (1 - bg.ParallaxMult);
            }
            foreach (var fr in ParallaxForegrounds)
            {
                fr.Position -= (currentCameraPos - previousCameraPos) * (1 - fr.ParallaxMult);
            }
        }

        //
        // Summary:
        //     Render the level directly without prerendering the layers alot slower than prerendering
        //public void RenderLevel(LDtkLevel level)
        //{
        //    Texture2D[] array = RenderLayers(level);
        //    for (int i = 0; i < array.Length; i++)
        //    {
        //        SpriteBatch.Draw(array[i], Vector2.Zero, Color.White);
        //    }
        //}

        //
        // Summary:
        //     Render intgrids by displaying the intgrid as solidcolor squares
        //public void RenderIntGrid(LDtkIntGrid intGrid)
        //{
        //    for (int i = 0; i < intGrid.GridSize.X; i++)
        //    {
        //        for (int j = 0; j < intGrid.GridSize.Y; j++)
        //        {
        //            if (intGrid.Values[j * intGrid.GridSize.X + i] != 0)
        //            {
        //                int num = intGrid.WorldPosition.X + i * intGrid.TileSize;
        //                int num2 = intGrid.WorldPosition.Y + j * intGrid.TileSize;
        //                SpriteBatch.Draw(pixel, new Vector2(num, num2), null, Color.Pink, 0f, Vector2.Zero, new Vector2(intGrid.TileSize), SpriteEffects.None, 0f);
        //            }
        //        }
        //    }
        //}

        //
        // Summary:
        //     Renders the entity with the tile it includes
        //
        // Parameters:
        //   entity:
        //     The entity you want to render
        //
        //   texture:
        //     The spritesheet/texture for rendering the entity
        public void RenderEntity<T>(T entity, Texture2D texture) where T : ILDtkEntity
        {
            SpriteBatch.Draw(texture, entity.Position, entity.Tile, Color.White, 0f, entity.Pivot * entity.Size, 1f, SpriteEffects.None, 0f);
        }

        //
        // Summary:
        //     Renders the entity with the tile it includes
        //
        // Parameters:
        //   entity:
        //     The entity you want to render
        //
        //   texture:
        //     The spritesheet/texture for rendering the entity
        //
        //   flipDirection:
        //     The direction to flip the entity when rendering
        public void RenderEntity<T>(T entity, Texture2D texture, SpriteEffects flipDirection) where T : ILDtkEntity
        {
            SpriteBatch.Draw(texture, entity.Position, entity.Tile, Color.White, 0f, entity.Pivot * entity.Size, 1f, flipDirection, 0f);
        }

        //
        // Summary:
        //     Renders the entity with the tile it includes
        //
        // Parameters:
        //   entity:
        //     The entity you want to render
        //
        //   texture:
        //     The spritesheet/texture for rendering the entity
        //
        //   animationFrame:
        //     The current frame of animation. Is a very basic entity animation frames must
        //     be to the right of them and be the same size
        public void RenderEntity<T>(T entity, Texture2D texture, int animationFrame) where T : ILDtkEntity
        {
            Rectangle tile = entity.Tile;
            tile.Offset(tile.Width * animationFrame, 0);
            SpriteBatch.Draw(texture, entity.Position, tile, Color.White, 0f, entity.Pivot * entity.Size, 1f, SpriteEffects.None, 0f);
        }

        //
        // Summary:
        //     Renders the entity with the tile it includes
        //
        // Parameters:
        //   entity:
        //     The entity you want to render
        //
        //   texture:
        //     The spritesheet/texture for rendering the entity
        //
        //   flipDirection:
        //     The direction to flip the entity when rendering
        //
        //   animationFrame:
        //     The current frame of animation. Is a very basic entity animation frames must
        //     be to the right of them and be the same size
        public void RenderEntity<T>(T entity, Texture2D texture, SpriteEffects flipDirection, int animationFrame) where T : ILDtkEntity
        {
            Rectangle tile = entity.Tile;
            tile.Offset(tile.Width * animationFrame, 0);
            SpriteBatch.Draw(texture, entity.Position, tile, Color.White, 0f, entity.Pivot * entity.Size, 1f, flipDirection, 0f);
        }
    }
}
