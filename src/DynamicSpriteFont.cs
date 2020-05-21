using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteFontPlus.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SpriteFontPlus
{
	public class DynamicSpriteFont
	{
		internal struct TextureEnumerator : IEnumerable<Texture2D>
		{
			private readonly FontSystem _font;

			public TextureEnumerator(FontSystem font)
			{
				_font = font;
			}

			public IEnumerator<Texture2D> GetEnumerator()
			{
				foreach (var atlas in _font.Atlases)
				{
					yield return atlas.Texture;
				}
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}

		private readonly FontSystem _fontSystem;

		public IEnumerable<Texture2D> Textures
		{
			get { return new TextureEnumerator(_fontSystem); }
		}

		public int Size
		{
			get
			{
				return _fontSystem.FontSize;
			}
			set
			{
				_fontSystem.FontSize = value;
			}
		}

		public float Spacing
		{
			get
			{
				return _fontSystem.Spacing;
			}
			set
			{
				_fontSystem.Spacing = value;
			}
		}

		public bool UseKernings
		{
			get
			{
				return _fontSystem.UseKernings;
			}

			set
			{
				_fontSystem.UseKernings = value;
			}
		}

		public int? DefaultCharacter
		{
			get
			{
				return _fontSystem.DefaultCharacter;
			}

			set
			{
				_fontSystem.DefaultCharacter = value;
			}
		}

		public event EventHandler CurrentAtlasFull
		{
			add
			{
				_fontSystem.CurrentAtlasFull += value;
			}

			remove
			{
				_fontSystem.CurrentAtlasFull -= value;
			}
		}

		private DynamicSpriteFont(byte[] ttf, int defaultSize, int textureWidth, int textureHeight, int blur)
		{
			_fontSystem = new FontSystem(textureWidth, textureHeight, blur)
			{
				FontSize = defaultSize
			};

			_fontSystem.AddFontMem(ttf);
        }



        public float DrawString(
            SpriteBatch batch,
            CharSourceUnion charSource,
            Vector2 pos,
            ColorSourceUnion colorSource
        ) =>
            DrawString(batch: batch, charSource: charSource, pos: pos, colorSource: colorSource, scale: Vector2.One);
		
        public float DrawString(SpriteBatch batch, CharSourceUnion charSource, Vector2 pos, ColorSourceUnion colorSource, Vector2 scale, float depth = 0f)
        {
            _fontSystem.Scale = scale;

            float result;

            if (charSource.IsStringSource && colorSource.IsColor)
            {
                result = _fontSystem.DrawText(
                    batch: batch,
                    x: pos.X,
                    y: pos.Y,
                    str: charSource.String,
                    color: colorSource.Color,
                    depth: depth
                );
            }
			else if (charSource.IsStringSource && colorSource.IsGlyphColors)
            {
                result = _fontSystem.DrawText(
                    batch: batch,
                    x: pos.X,
                    y: pos.Y,
                    str: charSource.String,
                    glyphColors: colorSource.GlyphColors,
                    depth: depth
                );
            }
            else if (charSource.IsStringBuilderSource && colorSource.IsColor)
            {
				result = _fontSystem.DrawText(
                    batch: batch,
                    x: pos.X,
                    y: pos.Y,
                    str: charSource.StringBuilder,
                    color: colorSource.Color,
                    depth: depth
                );
			}
            else if (charSource.IsStringBuilderSource && colorSource.IsGlyphColors)
            {
                result = _fontSystem.DrawText(
                    batch: batch,
                    x: pos.X,
                    y: pos.Y,
                    str: charSource.StringBuilder,
                    glyphColors: colorSource.GlyphColors,
                    depth: depth
                );
			}
            else
            {
                throw new ArgumentException("char source is invalid state", "charSource");
            }

			_fontSystem.Scale = Vector2.One;

            return result;
        }

		public void AddTtf(byte[] ttf)
		{
			_fontSystem.AddFontMem(ttf);
		}

		public void AddTtf(Stream ttfStream)
		{
			AddTtf(ttfStream.ToByteArray());
		}

		public Vector2 MeasureString(string text)
		{
			Bounds bounds = new Bounds();
			_fontSystem.TextBounds(0, 0, text, ref bounds);

			return new Vector2(bounds.X2, bounds.Y2);
		}

		public Vector2 MeasureString(StringBuilder text)
		{
			Bounds bounds = new Bounds();
			_fontSystem.TextBounds(0, 0, text, ref bounds);

			return new Vector2(bounds.X2, bounds.Y2);
		}

		public Rectangle GetTextBounds(Vector2 position, string text)
		{
			Bounds bounds = new Bounds();
			_fontSystem.TextBounds(position.X, position.Y, text, ref bounds);

			return new Rectangle((int)bounds.X, (int)bounds.Y, (int)(bounds.X2 - bounds.X), (int)(bounds.Y2 - bounds.Y));
		}

		public Rectangle GetTextBounds(Vector2 position, StringBuilder text)
		{
			Bounds bounds = new Bounds();
			_fontSystem.TextBounds(position.X, position.Y, text, ref bounds);

			return new Rectangle((int)bounds.X, (int)bounds.Y, (int)(bounds.X2 - bounds.X), (int)(bounds.Y2 - bounds.Y));
		}

		public void Reset(int width, int height)
		{
			_fontSystem.Reset(width, height);
		}

		public void Reset()
		{
			_fontSystem.Reset();
		}

		public static DynamicSpriteFont FromTtf(byte[] ttf, int defaultSize, int textureWidth = 1024, int textureHeight = 1024, int blur = 0)
		{
			return new DynamicSpriteFont(ttf, defaultSize, textureWidth, textureHeight, blur);
		}

		public static DynamicSpriteFont FromTtf(Stream ttfStream, int defaultSize, int textureWidth = 1024, int textureHeight = 1024, int blur = 0)
		{
			return FromTtf(ttfStream.ToByteArray(), defaultSize, textureWidth, textureHeight, blur);
		}
	}
}
