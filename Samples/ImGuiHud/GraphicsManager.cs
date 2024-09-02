using Vortice.Direct3D11;
using Vortice.Direct3D;

public class GraphicsManager
{
    private static readonly Lazy<ID3D11Device> _device = new Lazy<ID3D11Device>(CreateDevice);

    public static ID3D11Device Device => _device.Value;

    private static ID3D11Device CreateDevice()
    {
        // Define the flags for creating the device
        DeviceCreationFlags creationFlags = DeviceCreationFlags.BgraSupport;

        // Create the device and return it
        ID3D11Device device;
        ID3D11DeviceContext context;
        D3D11.D3D11CreateDevice(
            null,
            DriverType.Hardware,
            creationFlags,
            null,
            out device,
            out context);

        return device;
    }
}