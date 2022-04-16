﻿using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using gESilk.engine.window;
using OpenTK.Graphics.OpenGL4;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace gESilk.engine.render.assets.textures;

[SuppressMessage("Interoperability", "CA1416", MessageId = "Validate platform compatibility")]
public class ImageTexture : Texture
{
    public ImageTexture(string path,
        Application application,
        PixelFormat format = PixelFormat.Format32bppArgb)
    {
        Format = PixelInternalFormat.Rgba;
        Id = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, Id);

        var bmp = new Bitmap(path);
        bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
        var bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, format);

        GL.TexImage2D(TextureTarget.Texture2D, 0, Format, bmp.Width, bmp.Height, 0,
            OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, bmpData.Scan0);
        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
            (int)TextureMinFilter.LinearMipmapLinear);
        if (application.HasExtension("GL_EXT_texture_filter_anisotropic"))
        {
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D,
                (TextureParameterName)ArbTextureFilterAnisotropic.TextureMaxAnisotropy, 4f);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureLodBias, 0f);
        }


        bmp.UnlockBits(bmpData);
        bmp.Dispose();
    }
}