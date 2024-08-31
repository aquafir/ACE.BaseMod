

//public class TextureEnumModal : PickerModal<Textures>
//{
//    public TextureEnumModal(IComp picker) : base(picker) { }
//    public TextureEnumModal() : base(null)
//    {
//        Picker = new TexturedPicker<Textures>(x => TextureManager.GetOrCreateTexture(x), Enum.GetValues(typeof(Textures)).Cast<Textures>().ToArray());
//    }

//    public override void Init()
//    {
//        Picker = new TexturedPicker<Textures>(x => TextureManager.GetOrCreateTexture(x), Enum.GetValues(typeof(Textures)).Cast<Textures>().ToArray());

//        base.Init();
//    }
//}
