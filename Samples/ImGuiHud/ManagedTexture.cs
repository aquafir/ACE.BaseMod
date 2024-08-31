using ACE.DatLoader.FileTypes;
using System;
using System.Collections.Generic;
using System.Drawing;
//using Microsoft.DirectX.Direct3D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vortice.DXGI;
//using Microsoft.DirectX.Direct3D;
using Vortice.Direct3D;

public class ManagedTexture : IDisposable
{
    //
    // Summary:
    //     The bitmap this texture is using.
    public Bitmap Bitmap { get; set; }

    //
    // Summary:
    //     The DirectX texture
    public Vortice.Texture Texture { get; set; }

    //
    // Summary:
    //     Pointer to the unmanaged texture
    public unsafe IntPtr TexturePtr
    {
        get
        {
            if (!(Texture == (Texture)null))
            {
                return (IntPtr)Texture.UnmanagedComPointer;
            }

            return IntPtr.Zero;
        }
    }

    //
    // Summary:
    //     Create a new managed texture from a DX Texture.
    //
    // Parameters:
    //   texture:
    //     The texture.
    public ManagedTexture(Texture texture)
    {
        Texture = texture;
        UBService.Huds.AddManagedTexture(this);
    }

    //
    // Summary:
    //     Create a new managed texture from a bitmap file path.
    //
    // Parameters:
    //   file:
    //     The bitmap file path for the texture.
    public ManagedTexture(string file)
    {
        Bitmap = new Bitmap(file);
        CreateTexture();
        //UBService.Huds.AddManagedTexture(this);
    }

    //
    // Summary:
    //     Create a new managed texture from a bitmap. This copies your bitmap data immediately
    //     so you can dispose the passed bitmap immediately.
    //
    // Parameters:
    //   bitmap:
    //     The bitmap source for the texture.
    public ManagedTexture(Bitmap bitmap)
    {
        Bitmap = new Bitmap(bitmap);
        CreateTexture();
        //UBService.Huds.AddManagedTexture(this);
    }

    //
    // Summary:
    //     Create a new managed texture from a bitmap stream
    //
    // Parameters:
    //   stream:
    //     The bitmap stream source
    public ManagedTexture(Stream stream)
    {
        Bitmap = new Bitmap(stream);
        CreateTexture();
        //UBService.Huds.AddManagedTexture(this);
    }

    internal void CreateTexture()
    {
        if (!(Texture != (Texture)null) && Bitmap != null)
        {
            Texture = new Texture(UBService.Huds.D3Ddevice, Bitmap, (Usage)512, (Pool)0);
        }
    }

    internal void ReleaseTexture()
    {
        Texture texture = Texture;
        if (texture != null)
        {
            texture.Dispose();
        }

        Texture = null;
    }

    //
    // Summary:
    //     Release this texture
    public void Dispose()
    {
        ReleaseTexture();
        Bitmap?.Dispose();
        Bitmap = null;
    }
}