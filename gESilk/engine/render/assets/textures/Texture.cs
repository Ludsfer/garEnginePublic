﻿using gESilk.engine.render.materialSystem.settings;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace gESilk.engine.render.assets.textures;

public abstract class Texture : Asset
{
    protected PixelInternalFormat Format;
    protected int Id;
    protected int Width;
    protected int Height;

    protected Texture()
    {
        AssetManager.Register(this);
    }

    public virtual int Use(int slot)
    {
        if (SlotManager.IsSlotSame(slot, Id)) return slot;
        SlotManager.SetSlot(slot, Id);
        GL.ActiveTexture(TextureUnit.Texture0 + slot);
        GL.BindTexture(TextureTarget.Texture2D, Id);
        return slot;
    }

    public virtual int Get()
    {
        return Id;
    }

    public override void Delete()
    {
        if (Id == -1) return;
        GL.DeleteTexture(Id);
        Id = -1;
    }


    public virtual Vector2 GetMipSize(int level)
    {
        var width = Width;
        var height = Height;
        while (level != 0)
        {
            width /= 2;
            height /= 2;
            level--;
        }

        return new Vector2(width, height);
    }

    public static int GetMipLevelCount(int width, int height)
    {
        return (int)Math.Floor(Math.Log2(Math.Min(width, height)));
    }

    public int GetMipsCount()
    {
        return (int)Math.Floor(Math.Log2(Math.Min(Width, Height)));
    }

    public virtual void Use(int slot, TextureAccess access, int level = 0)
    {
        GL.BindImageTexture(slot, Id, 0, false, 0, access, (SizedInternalFormat)Format);
    }

    public virtual void BindToBuffer(RenderBuffer buffer, FramebufferAttachment attachmentLevel,
        TextureTarget target = TextureTarget.Texture2D, int level = 0)
    {
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, buffer.Get());
        GL.BindTexture(TextureTarget.Texture2D, Id);
        GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, attachmentLevel, target, Id, 0);
    }

    public virtual void BindToBuffer(FrameBuffer buffer, FramebufferAttachment attachmentLevel, bool isShadow = false)
    {
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, buffer.Fbo);
        GL.BindTexture(TextureTarget.Texture2D, Id);
        if (isShadow)
        {
            GL.DrawBuffer(DrawBufferMode.None);
            GL.ReadBuffer(ReadBufferMode.None);
        }

        GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, attachmentLevel, TextureTarget.Texture2D, Id, 0);
    }
}