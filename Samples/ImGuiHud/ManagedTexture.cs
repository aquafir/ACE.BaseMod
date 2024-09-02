////using ACE.DatLoader.FileTypes;
//using System;
//using System.Collections.Generic;
//using System.Drawing;
////using Microsoft.DirectX.Direct3D;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Vortice.Direct3D11;
//using Vortice.DXGI;
//using System.Drawing.Imaging;
//using Vortice;

//public class ManagedTexture : IDisposable
//{
//    //
//    // Summary:
//    //     The bitmap this texture is using.
//    public Bitmap Bitmap { get; set; }

//    //
//    // Summary:
//    //     The DirectX texture
//    public Texture Texture { get; set; }

//    //
//    // Summary:
//    //     Pointer to the unmanaged texture
//    public unsafe IntPtr TexturePtr
//    {
//        get
//        {
//            if (!(Texture == (Texture)null))
//            {
//                return (IntPtr)Texture.UnmanagedComPointer;
//            }

//            return IntPtr.Zero;
//        }
//    }


//    public ManagedTexture()
//    {
//        // Assume you already have a D3D11 device created
//        ID3D11Device device = GraphicsManager.Device;

//        // Lock the bitmap data
//        Bitmap bitmap = new Bitmap("path_to_your_image.png");
//        BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

//        // Describe the texture
//        Texture2DDescription textureDesc = new Texture2DDescription
//        {
//            Width = bitmap.Width,
//            Height = bitmap.Height,
//            MipLevels = 1,
//            ArraySize = 1,
//            Format = Format.B8G8R8A8_UNorm,
//            SampleDescription = new SampleDescription(1, 0),
//            Usage = ResourceUsage.Default,
//            BindFlags = BindFlags.ShaderResource,
//            CPUAccessFlags = CpuAccessFlags.None,
//            MiscFlags = ResourceOptionFlags.None,
//        };

//        // Initialize the texture data
//        //DataBox dataBox = new DataBox(bitmapData.Scan0, bitmapData.Stride, 0);
//        var dataRectangle = new DataRectangle[] {
//        new(
//            bitmapData.Scan0,    // Pointer to the raw data
//            bitmapData.Stride    // Number of bytes per row
//        )
//        };

//        // Create the texture
//        ID3D11Texture2D texture = device.CreateTexture2D(textureDesc, dataRectangle);

//        // Unlock the bitmap data
//        bitmap.UnlockBits(bitmapData);

//        // Create a shader resource view for the texture
//        ID3D11ShaderResourceView textureView = device.CreateShaderResourceView(texture);
//    }

//    //
//    // Summary:
//    //     Create a new managed texture from a DX Texture.
//    //
//    // Parameters:
//    //   texture:
//    //     The texture.
//    //public ManagedTexture(Texture texture)
//    //{
//    //    Texture = texture;
//    //    UBService.Huds.AddManagedTexture(this);
//    //}

//    //
//    // Summary:
//    //     Create a new managed texture from a bitmap file path.
//    //
//    // Parameters:
//    //   file:
//    //     The bitmap file path for the texture.
//    public ManagedTexture(string file)
//    {
//        Bitmap = new Bitmap(file);
//        CreateTexture();
//        //UBService.Huds.AddManagedTexture(this);
//    }

//    //
//    // Summary:
//    //     Create a new managed texture from a bitmap. This copies your bitmap data immediately
//    //     so you can dispose the passed bitmap immediately.
//    //
//    // Parameters:
//    //   bitmap:
//    //     The bitmap source for the texture.
//    public ManagedTexture(Bitmap bitmap)
//    {
//        Bitmap = new Bitmap(bitmap);
//        CreateTexture();
//        //UBService.Huds.AddManagedTexture(this);
//    }

//    //
//    // Summary:
//    //     Create a new managed texture from a bitmap stream
//    //
//    // Parameters:
//    //   stream:
//    //     The bitmap stream source
//    public ManagedTexture(Stream stream)
//    {
//        Bitmap = new Bitmap(stream);
//        CreateTexture();
//        //UBService.Huds.AddManagedTexture(this);
//    }

//    internal void CreateTexture()
//    {
//        if (!(Texture != (Texture)null) && Bitmap != null)
//        {
//            Texture = new Texture(GraphicsManager.Device, Bitmap, (Usage)512, (Pool)0);
//        }
//    }

//    internal void ReleaseTexture()
//    {
//        Texture texture = Texture;
//        if (texture != null)
//        {
//            texture.Dispose();
//        }

//        Texture = null;
//    }

//    //
//    // Summary:
//    //     Release this texture
//    public void Dispose()
//    {
//        ReleaseTexture();
//        Bitmap?.Dispose();
//        Bitmap = null;
//    }
//}