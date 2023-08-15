using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LDtk;
using LDtk.Renderer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace LostHope.Engine.Rendering
{
    //
    // Summary:
    //     Renderer for the ldtkWorld, ldtkLevel, intgrids and entities. This can all be
    //     done in your own class if you want to reimplement it and customize it differently
    //     this one is mostly here to get you up and running quickly.
    public class ModifiedLDtkRenderer
    {
        private static Texture2D pixel;

        private GraphicsDevice graphicsDevice;

        private ContentManager content;

        //
        // Summary:
        //     The spritebatch used for rendering with this Renderer
        public SpriteBatch SpriteBatch { get; set; }

        //
        // Summary:
        //     The levels identifier to layers Dictionary
        protected Dictionary<string, RenderedLevel> PrerenderedLevels { get; set; } = new Dictionary<string, RenderedLevel>();


        //
        // Summary:
        //     This is used to intizialize the renderer for use with direct file loading
        public ModifiedLDtkRenderer(SpriteBatch spriteBatch, ContentManager content)
        {
            SpriteBatch = spriteBatch;
            this.content = content;
            graphicsDevice = spriteBatch.GraphicsDevice;
            if (pixel == null)
            {
                pixel = new Texture2D(graphicsDevice, 1, 1);
                pixel.SetData(new byte[4] { 255, 255, 255, 255 });
            }
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
            if (!PrerenderedLevels.ContainsKey(level.Identifier))
            {
                RenderedLevel value = default(RenderedLevel);
                SpriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
                value.Layers = RenderLayers(level);
                SpriteBatch.End();
                PrerenderedLevels.Add(level.Identifier, value);
                graphicsDevice.SetRenderTarget(null);
            }
        }

        private Texture2D[] RenderLayers(LDtkLevel level)
        {
            List<Texture2D> list = new List<Texture2D>();
            if (level.BgRelPath != null)
            {
                list.Add(RenderBackgroundToLayer(level));
            }

            for (int num = level.LayerInstances.Length - 1; num >= 0; num--)
            {
                LayerInstance layer = level.LayerInstances[num];
                if (layer._TilesetRelPath != null && layer._Type != 0)
                {
                    Texture2D texture = GetTexture(level, layer._TilesetRelPath);
                    int width = layer._CWid * layer._GridSize;
                    int height = layer._CHei * layer._GridSize;
                    RenderTarget2D renderTarget2D = new RenderTarget2D(graphicsDevice, width, height, mipMap: false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
                    graphicsDevice.SetRenderTarget(renderTarget2D);
                    list.Add(renderTarget2D);
                    switch (layer._Type)
                    {
                        case LayerType.Tiles:
                            foreach (TileInstance item in layer.GridTiles.Where((TileInstance tile) => layer._TilesetDefUid.HasValue))
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

                            foreach (TileInstance item2 in layer.AutoLayerTiles.Where((TileInstance tile) => layer._TilesetDefUid.HasValue))
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

            return list.ToArray();
        }

        private Texture2D RenderBackgroundToLayer(LDtkLevel level)
        {
            Texture2D texture = GetTexture(level, level.BgRelPath);
            RenderTarget2D renderTarget2D = new RenderTarget2D(graphicsDevice, level.PxWid, level.PxHei, mipMap: false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            graphicsDevice.SetRenderTarget(renderTarget2D);
            LevelBackgroundPosition bgPos = level._BgPos;
            Vector2 position = bgPos.TopLeftPx.ToVector2();
            SpriteBatch.Draw(texture, position, new Rectangle((int)bgPos.CropRect[0], (int)bgPos.CropRect[1], (int)bgPos.CropRect[2], (int)bgPos.CropRect[3]), Color.White, 0f, Vector2.Zero, bgPos.Scale, SpriteEffects.None, 0f);
            graphicsDevice.SetRenderTarget(null);
            return renderTarget2D;
        }

        private Texture2D GetTexture(LDtkLevel level, string path)
        {
            string assetName = Path.ChangeExtension(path, null);
            return content.Load<Texture2D>(assetName);
        }

        //
        // Summary:
        //     Render the prerendered level you created from PrerenderLevel()
        public void RenderPrerenderedLevel(LDtkLevel level)
        {
            if (PrerenderedLevels.TryGetValue(level.Identifier, out var value))
            {
                for (int i = 0; i < value.Layers.Length; i++)
                {
                    SpriteBatch.Draw(value.Layers[i], Vector2.Zero, Color.White);
                }

                return;
            }

            throw new LDtkException("No prerendered level with Identifier " + level.Identifier + " found.");
        }

        //
        // Summary:
        //     Render the level directly without prerendering the layers alot slower than prerendering
        public void RenderLevel(LDtkLevel level)
        {
            Texture2D[] array = RenderLayers(level);
            for (int i = 0; i < array.Length; i++)
            {
                SpriteBatch.Draw(array[i], level.Position.ToVector2(), Color.White);
            }
        }

        //
        // Summary:
        //     Render intgrids by displaying the intgrid as solidcolor squares
        public void RenderIntGrid(LDtkIntGrid intGrid)
        {
            for (int i = 0; i < intGrid.GridSize.X; i++)
            {
                for (int j = 0; j < intGrid.GridSize.Y; j++)
                {
                    if (intGrid.Values[j * intGrid.GridSize.X + i] != 0)
                    {
                        int num = intGrid.WorldPosition.X + i * intGrid.TileSize;
                        int num2 = intGrid.WorldPosition.Y + j * intGrid.TileSize;
                        SpriteBatch.Draw(pixel, new Vector2(num, num2), null, Color.Pink, 0f, Vector2.Zero, new Vector2(intGrid.TileSize), SpriteEffects.None, 0f);
                    }
                }
            }
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
